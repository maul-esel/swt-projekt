using Lingvo.Common.Entities;
using Lingvo.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Util
{
    public class AlertHelper
    {
        /// <summary>
        /// Displays a warning if not connected to wifi
        /// <returns>true if download should start, false otherwise</returns>
        /// </summary>
        public static async Task<bool> DisplayWarningIfNotWifiConnected()
        {
            INetworkService networkService = DependencyService.Get<INetworkService>();
            if (!networkService.IsWifiConnected())
            {
                string title = ((Span)App.Current.Resources["label_warning"]).Text;
                string desc = ((Span)App.Current.Resources["desc_notConnectedToWlan"]).Text;
                string accept = ((Span)App.Current.Resources["label_download"]).Text;
                string cancel = ((Span)App.Current.Resources["label_cancel"]).Text;

                return await App.Current.MainPage.DisplayAlert(title, desc, accept, cancel);
            }
            return true;
        }

        public static async Task<bool> DisplayWarningStudentTrackExists()
        {
            string title = ((Span)App.Current.Resources["label_warning"]).Text;
            string desc = ((Span)App.Current.Resources["desc_studentTrackAlreadyExists"]).Text;
            string accept = ((Span)App.Current.Resources["label_overwrite"]).Text;
            string cancel = ((Span)App.Current.Resources["label_cancel"]).Text;

            return await App.Current.MainPage.DisplayAlert(title, desc, accept, cancel);
        }

        public static async Task<bool> DisplayWarningTeacherMemoExists()
        {
            string title = ((Span)App.Current.Resources["label_warning"]).Text;
            string desc = ((Span)App.Current.Resources["desc_teacherMemoAlreadyExists"]).Text;
            string accept = ((Span)App.Current.Resources["label_overwrite"]).Text;
            string cancel = ((Span)App.Current.Resources["label_cancel"]).Text;

            return await App.Current.MainPage.DisplayAlert(title, desc, accept, cancel);
        }

        public static async Task<bool> DisplayWarningDeletePage(bool studentTrackExists)
        {
            string descRes = studentTrackExists ? "desc_deletePageWithStudentTrackQuestion" : "desc_deletePageQuestion";

            string title = ((Span)App.Current.Resources["label_warning"]).Text;
            string desc = ((Span)App.Current.Resources[descRes]).Text;

            string accept = ((Span)App.Current.Resources["label_delete"]).Text;
            string cancel = ((Span)App.Current.Resources["label_cancel"]).Text;

            return await App.Current.MainPage.DisplayAlert(title, desc, accept, cancel);
        }

        public static async Task<bool> DisplayWarningDeleteTeacherMemo(bool studentTrackExists)
        {
            string descRes = studentTrackExists ? "desc_deleteTeacherMemoWithStudentTrackQuestion" : "desc_deleteTeacherMemoQuestion";

            string title = ((Span)App.Current.Resources["label_warning"]).Text;
            string desc = ((Span)App.Current.Resources[descRes]).Text;
            string accept = ((Span)App.Current.Resources["label_delete"]).Text;
            string cancel = ((Span)App.Current.Resources["label_cancel"]).Text;

            return await App.Current.MainPage.DisplayAlert(title, desc, accept, cancel);
        }

        public static async Task<bool> DisplayWarningDeleteWorkbook()
        {
            string title = ((Span)App.Current.Resources["label_warning"]).Text;
            string desc = ((Span)App.Current.Resources["desc_deleteWorkbookQuestion"]).Text;
            string accept = ((Span)App.Current.Resources["label_delete"]).Text;
            string cancel = ((Span)App.Current.Resources["label_cancel"]).Text;

            return await App.Current.MainPage.DisplayAlert(title, desc, accept, cancel);
        }

        public static async Task<bool> DisplayWarningDeleteStudentTrack()
        {
            string title = ((Span)App.Current.Resources["label_warning"]).Text;
            string desc = ((Span)App.Current.Resources["desc_deleteStudentTrackQuestion"]).Text;
            string accept = ((Span)App.Current.Resources["label_delete"]).Text;
            string cancel = ((Span)App.Current.Resources["label_cancel"]).Text;

            return await App.Current.MainPage.DisplayAlert(title, desc, accept, cancel);
        }

        public static async Task DisplayInfoTeacherMemoNameExists()
        {
            string title = ((Span)App.Current.Resources["label_nameAlreadyExists"]).Text;
            string desc = ((Span)App.Current.Resources["desc_teacherTrackNameAlreadyExists"]).Text;
            string ok = ((Span)App.Current.Resources["label_ok"]).Text;
            await App.Current.MainPage.DisplayAlert(title, desc, ok);
        }

        public static async Task DisplaySyncError()
        {
            string title = ((Span)App.Current.Resources["label_error"]).Text;
            string desc = ((Span)App.Current.Resources["label_sync_error"]).Text;
            string ok = ((Span)App.Current.Resources["label_ok"]).Text;
            await App.Current.MainPage.DisplayAlert(title, desc, ok);
        }

        public static async Task DisplayFetchPageError()
        {
            string title = ((Span)App.Current.Resources["label_error"]).Text;
            string desc = ((Span)App.Current.Resources["label_fetchPageError"]).Text;
            string ok = ((Span)App.Current.Resources["label_ok"]).Text;
            await App.Current.MainPage.DisplayAlert(title, desc, ok);
        }

        public static async Task DisplayFetchWorkbookError()
        {
            string title = ((Span)App.Current.Resources["label_error"]).Text;
            string desc = ((Span)App.Current.Resources["label_fetchWorkbookError"]).Text;
            string ok = ((Span)App.Current.Resources["label_ok"]).Text;
            await App.Current.MainPage.DisplayAlert(title, desc, ok);
        }

        public static async Task DisplayAudioError()
        {
            string title = ((Span)App.Current.Resources["label_error"]).Text;
            string desc = ((Span)App.Current.Resources["desc_audioError"]).Text;
            string ok = ((Span)App.Current.Resources["label_ok"]).Text;
            await App.Current.MainPage.DisplayAlert(title, desc, ok);
        }


        public static async Task DisplayNoTeacherMemoNameError()
        {
            string title = ((Span)App.Current.Resources["label_error"]).Text;
            string desc = ((Span)App.Current.Resources["desc_noTeacherMemoName"]).Text;
            string ok = ((Span)App.Current.Resources["label_ok"]).Text;
            await App.Current.MainPage.DisplayAlert(title, desc, ok);
        }

        public static async Task DisplayNoTeacherMemoRecordingError()
        {
            string title = ((Span)App.Current.Resources["label_error"]).Text;
            string desc = ((Span)App.Current.Resources["desc_noTeacherMemoRecording"]).Text;
            string ok = ((Span)App.Current.Resources["label_ok"]).Text;
            await App.Current.MainPage.DisplayAlert(title, desc, ok);
        }
    }
}
