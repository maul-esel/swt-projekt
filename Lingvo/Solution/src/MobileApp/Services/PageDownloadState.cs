using Lingvo.Common.Entities;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Lingvo.MobileApp.Util
{
    /// <summary>
    /// Class for state object that gets passed around amongst async methods
    /// when doing async web request/response for downloading a page.
    /// </summary>
    public class PageDownloadState
    {
        public int BytesRead;           // # bytes read during current transfer
        public long TotalBytes;            // Total bytes to read
        public Stream StreamResponse;    // Stream to read from
        public byte[] BufferRead;        // Buffer to read data into
        
        /// <summary>
        /// The file path of the resulting file
        /// </summary>
        public string FilePath
        {
            get; set;
        }

        /// <summary>
        /// The <c>FileStream</c> of the resulting file
        /// </summary>
        public FileStream FileStream
        {
            get; set;
        }

        /// <summary>
        /// The <c>TaskCompletionSource</c> of the async download task
        /// </summary>
        public TaskCompletionSource<bool> TaskSource
        {
            get; internal set;
        }

        /// <summary>
        /// The <c>CancellationToken</c> of the async download task
        /// </summary>
        public CancellationToken CancellationToken
        {
            get; set;
        }

        /// <summary>
        /// The download request
        /// </summary>
        public WebRequest Request
        {
            get; set;
        }

        /// <summary>
        /// The server's response
        /// </summary>
        public WebResponse Response
        {
            get; set;
        }

        /// <summary>
        /// The <c>IPage</c> being downloaded
        /// </summary>
        public IPage Page
        {
            get; set;
        }

        /// <summary>
        /// Creates a new <c>PageDownloadState</c>.
        /// </summary>
        /// <param name="bufferSize">The size of the buffer for reading the response.</param>
        public PageDownloadState(int bufferSize)
        {
            TaskSource = new TaskCompletionSource<bool>();
            BytesRead = 0;
            BufferRead = new byte[bufferSize];
            StreamResponse = null;
        }
    }
}
