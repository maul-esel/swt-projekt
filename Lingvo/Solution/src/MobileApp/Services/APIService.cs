﻿using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lingvo.Common.Services;

using Newtonsoft.Json;

using Lingvo.Common.Entities;
using Lingvo.MobileApp.Proxies;
using Lingvo.MobileApp.Util;
using System.Threading;
using Lingvo.MobileApp.Entities;
using Lingvo.MobileApp.Services;
using Lingvo.MobileApp.Services.Progress;
using Newtonsoft.Json.Converters;

namespace Lingvo.MobileApp
{
    /// <summary>
    /// Service for communication with the server
    /// </summary>
    public class APIService
    {
#if DEBUG
#if __ANDROID__
        // Android Simulator forwards development localhost to IP 10.0.2.2
        private const string URL = "http://10.0.2.2:5000/api/app/";
#elif __IOS__
                private const string URL = "http://localhost:5000/api/app/";
#endif
#else
        private const string URL = "https://lingvo.azurewebsites.net/api/app/";
#endif

        private static APIService instance;

        public static APIService Instance => instance ?? (instance = new APIService());

        private static readonly int BufferSize = 1500;

        private APIService()
        {
        }

        /// <summary>
        /// Downloads a teacher track from the given url.
        /// </summary>
        /// <returns>The from URL.</returns>
        /// <param name="url">URL.</param>
        /// <param name="progress">The progress delegate for progress reporting</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        private Task DownloadTeacherTrack(PageProxy proxy, string url, string filePath, CancellationToken cancellationToken)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            PageDownloadState reqState = new PageDownloadState(BufferSize);

            reqState.Request = req;
            reqState.Page = proxy;
            reqState.TransferStart = DateTime.Now;
            reqState.FilePath = filePath;
            reqState.CancellationToken = cancellationToken;

            // Start the asynchronous request.
            req.BeginGetResponse(new AsyncCallback(RespCallback), reqState);

            return reqState.TaskSource.Task;
        }

        /// <summary>
        /// Main response callback, invoked once we have first Response packet from
        /// server.  This is where we initiate the actual file transfer, reading from
        /// a stream.
        /// </summary>
        private static void RespCallback(IAsyncResult asyncResult)
        {
            PageDownloadState reqState = ((PageDownloadState)(asyncResult.AsyncState));
            WebRequest req = reqState.Request;

            WebResponse resp = req.EndGetResponse(asyncResult);
            reqState.Response = resp;
            reqState.TotalBytes = reqState.Response.ContentLength;

            // Set up a stream, for reading response data into it
            Stream responseStream = reqState.Response.GetResponseStream();
            reqState.StreamResponse = responseStream;

            reqState.FileStream = File.Create(reqState.FilePath);

            // Begin reading contents of the response data
            responseStream.BeginRead(reqState.BufferRead, 0, BufferSize, new AsyncCallback(ReadCallback), reqState);
        }

        /// <summary>
        /// Main callback invoked in response to the Stream.BeginRead method, when we have some data.
        /// </summary>
        private static void ReadCallback(IAsyncResult asyncResult)
        {

            // Will be either HttpWebRequestState or FtpWebRequestState
            PageDownloadState reqState = ((PageDownloadState)(asyncResult.AsyncState));

            Stream responseStream = reqState.StreamResponse;

            if (reqState.CancellationToken.IsCancellationRequested)
            {
                responseStream.Close();
                reqState.Response.Close();
                reqState.FileStream.Close();
                File.Delete(reqState.FilePath);

                ProgressHolder.Instance.GetPageProgress(reqState.Page.Id)?.Cancel();
            }

            reqState.CancellationToken.ThrowIfCancellationRequested();

            // Get results of read operation
            int bytesRead = responseStream.EndRead(asyncResult);

            // Got some data, need to read more
            if (bytesRead > 0)
            {
                int oldPercentage = (int)(((double)reqState.BytesRead / (double)reqState.TotalBytes) * 100.0f);

                // Report the progress
                reqState.BytesRead += bytesRead;
                double pctComplete = ((double)reqState.BytesRead / (double)reqState.TotalBytes) * 100.0f;

                if ((int)pctComplete > oldPercentage)
                {
                    ProgressHolder.Instance.GetPageProgress(reqState.Page.Id)?.Report(pctComplete);
                }

                //Write buffered chunk to result stream
                reqState.FileStream.Write(reqState.BufferRead, 0, bytesRead);

                // Kick off another read
                responseStream.BeginRead(reqState.BufferRead, 0, BufferSize, new AsyncCallback(ReadCallback), reqState);
                return;
            }

            // EndRead returned 0, so no more data to be read
            else
            {
                responseStream.Close();
                reqState.Response.Close();
                reqState.FileStream.Close();
                ProgressHolder.Instance.GetPageProgress(reqState.Page.Id)?.Report(100.0f);
                ProgressHolder.Instance.DeletePageProgress(reqState.Page);

                //Set positive result
                reqState.TaskSource.SetResult(true);
            }
        }

        /// <summary>
        /// Fetchs Data from URL.
        /// </summary>
        /// <returns>The from URL.</returns>
        /// <param name="url">URL.</param>
        /// <param name="convert">Function for data conversion</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        private async Task<T> FetchFromURLAsync<T>(string url, Func<WebResponse, Task<T>> convert)
        {
            var response = await WebRequest.Create(url).GetResponseAsync();
            var result = await convert(response);
            response.Close();
            return result;
        }


        private async Task<string> ReadTextAsync(WebResponse response)
        {
            var stream = response.GetResponseStream();
            var reader = new StreamReader(stream);
            var text = await reader.ReadToEndAsync();
            reader.Close();
            return text;
        }

        /// <summary>
        /// Fetchs data from the URL as text.
        /// </summary>
        /// <returns>The text from URL.</returns>
        /// <param name="url">URL.</param>
        private Task<string> FetchTextFromURLAsync(string url)
        {
            return FetchFromURLAsync(url, ReadTextAsync);
        }

        /// <summary>
        /// Fetchs the workbooks.
        /// </summary>
        /// <returns>The workbooks.</returns>
        public async Task<Workbook[]> FetchWorkbooks()
        {
            try
            {
                var responseFromServer = await FetchTextFromURLAsync(URL + "workbooks");
                return JsonConvert.DeserializeObject<Workbook[]>(responseFromServer);
            }
            catch
            {
                await AlertHelper.DisplaySyncError();
            }

            return null;
        }

        /// <summary>
        /// Fetchs a workbook.
        /// </summary>
        /// <returns>The workbook.</returns>
        /// <param name="workbookID">Workbook identifier.</param>
        public async Task<Workbook> FetchWorkbook(int workbookID, CancellationToken cancellationToken)
        {
            try
            {
                var responseFromServer = await FetchTextFromURLAsync(URL + "workbooks/" + workbookID);
                var workbook = JsonConvert.DeserializeObject<Workbook>(responseFromServer);

                await FetchPages(workbook);

                workbook.Pages.ForEach(p => ProgressHolder.Instance.CreatePageProgress(p));

                foreach (IPage page in workbook.Pages)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        ProgressHolder.Instance.GetWorkbookProgress(workbook.Id).Cancel();
                        break;
                    }

                    await ((PageProxy)page).Resolve(cancellationToken);
                }

                return workbook;
            }
            catch
            {
                await AlertHelper.DisplayFetchWorkbookError();
            }

            return null;
        }

        /// <summary>
        /// Fetchs a page.
        /// </summary>
        /// <returns>The page.</returns>
        /// <param name="proxy">Proxy.</param>
        public async Task<Page> FetchPage(PageProxy proxy, CancellationToken cancellationToken)
        {
            try
            {
                ProgressHolder.Instance.CreatePageProgress(proxy);

                Recording recording = await FetchTeacherTrack(proxy, "w" + proxy.Workbook.Id + "s" + proxy.Number + ".mp3", cancellationToken);

                Page page = new Page();
                page.Id = proxy.Id;
                page.Description = proxy.Description;
                page.Number = proxy.Number;
                page.Workbook = proxy.Workbook;
                page.workbookId = page.Workbook.Id;
                page.TeacherTrack = recording;
                page.teacherTrackId = page.TeacherTrack.Id;

                return page;
            }
            catch
            {
                await AlertHelper.DisplayFetchPageError();
            }

            return null;
        }

        /// <summary>
        /// Fetchs the pages for a workbook.
        /// </summary>
        /// <returns>Nothing, the pages are attached to the given workbook object</returns>
        /// <param name="workbook">Workbook.</param>
        public async Task FetchPages(Workbook workbook)
        {
            try
            {
                var responseFromServer = await FetchTextFromURLAsync($"{URL}workbooks/{workbook.Id}/pages");

                workbook.Pages.AddRange(JsonConvert.DeserializeObject<List<PageProxy>>(responseFromServer, new IsoDateTimeConverter()
                {
                    DateTimeFormat = "dd.MM.yyyy HH:mm:ss"
                }));


                foreach (var page in workbook.Pages)
                {
                    page.Workbook = workbook;
                }
            }
            catch
            {
                await AlertHelper.DisplaySyncError();
            }
        }

        /// <summary>
        /// Fetchs a teacher track.
        /// </summary>
        /// <returns>The teacher track.</returns>
        /// <param name="page">Page.</param>
        /// <param name="localPath">Local path.</param>
        private async Task<Recording> FetchTeacherTrack(PageProxy proxy, String localPath, CancellationToken cancellationToken)
        {
            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                await FetchTextFromURLAsync($"{URL}pages/{proxy.Id}")
            );

            await DownloadTeacherTrack(proxy, json["url"], FileUtil.getAbsolutePath(localPath), cancellationToken);

            return new Recording(
                 int.Parse(json["duration"]),
                 localPath,
                 DateTime.ParseExact(json["creationTime"], "dd.MM.yyyy HH:mm:ss", null)
             );
        }
    }
}
