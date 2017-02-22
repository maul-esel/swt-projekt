using Lingvo.Common.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Lingvo.MobileApp.APIService;

namespace Lingvo.MobileApp.Util
{
    /// <summary>
    /// Base class for state object that gets passed around amongst async methods
    /// when doing async web request/response for data transfer.  We store basic
    /// things that track current state of a download, including # bytes transfered,
    /// as well as some async callbacks that will get invoked at various points.
    /// </summary>
    public class PageDownloadState
    {
        public int BytesRead;           // # bytes read during current transfer
        public long TotalBytes;            // Total bytes to read
        public double ProgIncrement;    // delta % for each buffer read
        public Stream StreamResponse;    // Stream to read from
        public byte[] BufferRead;        // Buffer to read data into
        public DateTime TransferStart;  // Used for tracking xfr rate

        public string FilePath
        {
            get; set;
        }

        public FileStream FileStream
        {
            get; set;
        }

        public TaskCompletionSource<bool> TaskSource
        {
            get; internal set;
        }

        public CancellationToken CancellationToken
        {
            get; set;
        }

        public WebRequest Request
        {
            get; set;
        }

        public WebResponse Response
        {
            get; set;
        }

        public IPage Page
        {
            get; set;
        }

        public PageDownloadState(int bufferSize)
        {
            TaskSource = new TaskCompletionSource<bool>();
            BytesRead = 0;
            BufferRead = new byte[bufferSize];
            StreamResponse = null;
        }
    }
}
