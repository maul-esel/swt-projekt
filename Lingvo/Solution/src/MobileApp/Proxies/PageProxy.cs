﻿using System;
using System.Threading.Tasks;
using Lingvo.Common.Entities;
using Lingvo.MobileApp.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace Lingvo.MobileApp.Proxies
{
    public class PageProxy : IPage
    {
        public int Id { get; set; }

		/// <summary>
		/// Called whenever a page is changed
		/// </summary>
        public delegate void OnPageChanged(int id);

        private Page original;

        public Workbook Workbook { get; set; }

        public int workbookId { get; set; }

		/// <summary>
		/// Gets or sets the creation time.
		/// </summary>
		/// <value>The creation time.</value>
        public DateTime CreationTime
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        /// <value>The number.</value>
        public int Number
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public String Description
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the teacher track if a real page exisits for this proxy
        /// </summary>
        /// <value>The teacher track.</value>
        public Recording TeacherTrack
        {
            get
            {
                if (original != null)
                {
                    return original.TeacherTrack;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Gets or sets the student track if a real page exisits for this proxy
        /// </summary>
        /// <value>The student track.</value>
        public Recording StudentTrack
        {
            get
            {
                if (original != null)
                {
                    return original.StudentTrack;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                throw new InvalidOperationException();
            }
        }


        public PageProxy()
        {
            LocalCollection.Instance.PageChanged += Instance_PageChanged;
        }

		/// <summary>
		/// Checks if the original page has changed and updates its reference if necessary.
		/// </summary>
		/// <param name="p">The changed page</param>
        private void Instance_PageChanged(IPage p)
        {
            if (p.Id.Equals(Id))
            {
                Workbook workbook = LocalCollection.Instance.Workbooks.FirstOrDefault(lwb => lwb.Id.Equals(p.workbookId));
                IPage local = workbook?.Pages.Find(lp => lp.Id.Equals(Id));
                if (local != null)
                {
                    original = (Page)local;
                }
                else
                {
                    original = null;
                }
            }
        }

        /// <summary>
        /// Load the real page object for this proxy
        /// </summary>
        /// <returns>The resolve.</returns>
        public async Task Resolve(CancellationTokenSource cancellationToken)
        {
            if (NewerVersionExists())
            {
                IPage localPage = LocalCollection.Instance.Workbooks.FirstOrDefault(w => w.Id.Equals(workbookId))?.Pages.Find(p => p.Id.Equals(Id));
                LocalCollection.Instance.DeletePage((Page)localPage);
                original = null;
            }

            if (original == null)
            {
                var p = App.Database.FindPage(this.Id);
                if (p != null)
                {
                    original = p;
                }
                else
                {
                    await DownloadPage(cancellationToken);
                }
            }

        }

		/// <summary>
		/// Downloads the page from the server.
		/// </summary>
		/// <returns>The page.</returns>
		/// <param name="cancellationToken">Token for cancelling the download</param>
        private async Task DownloadPage(CancellationTokenSource cancellationToken)
        {
            Page page = await CloudLibraryProxy.Instance.DownloadSinglePage(this, cancellationToken);

            if (App.Database.FindWorkbook(this.Workbook.Id) == null)
            {
                LocalCollection.Instance.AddWorkbook(Workbook);
            }

            original = page;
            CreationTime = page.TeacherTrack.CreationTime;

            App.Database.Save(original.TeacherTrack);
            App.Database.Save(original);
        }

        /// <summary>
        /// Deletes the student recording if a real page exisits for this proxy
        /// </summary>
        public void DeleteStudentRecording()
        {
            if (original != null)
            {
                original.DeleteStudentRecording();
            }
        }

		/// <summary>
		/// Checks if a newer version of this page exists 
		/// </summary>
		/// <returns><c>true</c>, if a newer version exists, <c>false</c> otherwise.</returns>
        public bool NewerVersionExists()
        {
            IPage localPage = LocalCollection.Instance.Workbooks.FirstOrDefault(w => w.Id.Equals(workbookId))?.Pages.Find(p => p.Id.Equals(Id));

            if (localPage != null)
            {
                return CreationTime.CompareTo(localPage.TeacherTrack.CreationTime) > 0;
            }

            return false;
        }
    }
}
