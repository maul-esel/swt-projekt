using Lingvo.MobileApp.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Util
{
	/// <summary>
	/// Helper class that displays text warnings and alerts to the user.
	/// </summary>
    public class AlertHelper
    {
		/// <summary>
		/// Displays a warning if not connected to wifi. The user can choose to download anyway or to abort.
		/// </summary>
		/// <returns>true if download should start, false otherwise</returns>
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

		/// <summary>
		/// Warns the user a student track for his current exercise already exists and he would overwrite it.
		/// </summary>
		/// <returns><c>true</c> if the user agrees to overwrite the existing recording, <c>false</c> if he aborts.</returns>
        public static async Task<bool> DisplayWarningStudentTrackExists()
        {
            string title = ((Span)App.Current.Resources["label_warning"]).Text;
            string desc = ((Span)App.Current.Resources["desc_studentTrackAlreadyExists"]).Text;
            string accept = ((Span)App.Current.Resources["label_overwrite"]).Text;
            string cancel = ((Span)App.Current.Resources["label_cancel"]).Text;

            return await App.Current.MainPage.DisplayAlert(title, desc, accept, cancel);
        }

		/// <summary>
		/// Warns the user he is about to overwite an existing teacher memo.
		/// </summary>
		/// <returns><c>true</c> if the user agrees to overwrite the memo, false otherwise.</returns>
        public static async Task<bool> DisplayWarningTeacherMemoExists()
        {
            string title = ((Span)App.Current.Resources["label_warning"]).Text;
            string desc = ((Span)App.Current.Resources["desc_teacherMemoAlreadyExists"]).Text;
            string accept = ((Span)App.Current.Resources["label_overwrite"]).Text;
            string cancel = ((Span)App.Current.Resources["label_cancel"]).Text;

            return await App.Current.MainPage.DisplayAlert(title, desc, accept, cancel);
        }

		/// <summary>
		/// Warns the user when he tries to delete a page.
		/// </summary>
		/// <param name="studentTrackExists"><c>true</c> if the page about to be deleted has a student track, <c>false</c> otherwise.
		/// The warning is adjusted accordingly.</param>
		/// <returns><c>true</c> if the user confirms the deletion, <c>false</c> otherwise</returns>
        public static async Task<bool> DisplayWarningDeletePage(bool studentTrackExists)
        {
            string descRes = studentTrackExists ? "desc_deletePageWithStudentTrackQuestion" : "desc_deletePageQuestion";

            string title = ((Span)App.Current.Resources["label_warning"]).Text;
            string desc = ((Span)App.Current.Resources[descRes]).Text;

            string accept = ((Span)App.Current.Resources["label_delete"]).Text;
            string cancel = ((Span)App.Current.Resources["label_cancel"]).Text;

            return await App.Current.MainPage.DisplayAlert(title, desc, accept, cancel);
        }

		/// <summary>
		/// Warns the user when he tries to delete a teacher memo.
		/// </summary>
		/// <param name="studentTrackExists"><c>true</c> if the memo about to be deleted has a student track, <c>false</c> otherwise.
		/// The warning is adjusted accordingly.</param>
		/// <returns><c>true</c> if the user confirms the deletion, <c>false</c> otherwise</returns>
		public static async Task<bool> DisplayWarningDeleteTeacherMemo(bool studentTrackExists)
        {
            string descRes = studentTrackExists ? "desc_deleteTeacherMemoWithStudentTrackQuestion" : "desc_deleteTeacherMemoQuestion";

            string title = ((Span)App.Current.Resources["label_warning"]).Text;
            string desc = ((Span)App.Current.Resources[descRes]).Text;
            string accept = ((Span)App.Current.Resources["label_delete"]).Text;
            string cancel = ((Span)App.Current.Resources["label_cancel"]).Text;

            return await App.Current.MainPage.DisplayAlert(title, desc, accept, cancel);
        }

		/// <summary>
		/// Warns the user when he tries to delete a workbook.
		/// </summary>
		/// <returns><c>true</c> if the user confirms the deletion, <c>false</c> otherwise.</returns>
        public static async Task<bool> DisplayWarningDeleteWorkbook()
        {
            string title = ((Span)App.Current.Resources["label_warning"]).Text;
            string desc = ((Span)App.Current.Resources["desc_deleteWorkbookQuestion"]).Text;
            string accept = ((Span)App.Current.Resources["label_delete"]).Text;
            string cancel = ((Span)App.Current.Resources["label_cancel"]).Text;

            return await App.Current.MainPage.DisplayAlert(title, desc, accept, cancel);
        }

		/// <summary>
		/// Warns the user when he tries to delete the student track of a page or teacher memo.
		/// </summary>
		/// <returns><c>true</c> if the user confirms the deletion, <c>false</c> otherwise</returns>
        public static async Task<bool> DisplayWarningDeleteStudentTrack()
        {
            string title = ((Span)App.Current.Resources["label_warning"]).Text;
            string desc = ((Span)App.Current.Resources["desc_deleteStudentTrackQuestion"]).Text;
            string accept = ((Span)App.Current.Resources["label_delete"]).Text;
            string cancel = ((Span)App.Current.Resources["label_cancel"]).Text;

            return await App.Current.MainPage.DisplayAlert(title, desc, accept, cancel);
        }

		/// <summary>
		/// Warns the user when he tries to create a teacher memo with a non-unique name.
		/// </summary>
        public static async Task DisplayInfoTeacherMemoNameExists()
        {
            string title = ((Span)App.Current.Resources["label_nameAlreadyExists"]).Text;
            string desc = ((Span)App.Current.Resources["desc_teacherTrackNameAlreadyExists"]).Text;
            string ok = ((Span)App.Current.Resources["label_ok"]).Text;
            await App.Current.MainPage.DisplayAlert(title, desc, ok);
        }

		/// <summary>
		/// Informs the user of an error communicating with the server.
		/// </summary>
        public static async Task DisplaySyncError()
        {
            string title = ((Span)App.Current.Resources["label_error"]).Text;
            string desc = ((Span)App.Current.Resources["label_sync_error"]).Text;
            string ok = ((Span)App.Current.Resources["label_ok"]).Text;
            await App.Current.MainPage.DisplayAlert(title, desc, ok);
        }

		/// <summary>
		/// Informs the user of an error downloading a page.
		/// </summary>
        public static async Task DisplayFetchPageError()
        {
            string title = ((Span)App.Current.Resources["label_error"]).Text;
            string desc = ((Span)App.Current.Resources["label_fetchPageError"]).Text;
            string ok = ((Span)App.Current.Resources["label_ok"]).Text;
            await App.Current.MainPage.DisplayAlert(title, desc, ok);
        }

		/// <summary>
		/// Informs the user of an error downloading a workbook.
		/// </summary>
        public static async Task DisplayFetchWorkbookError()
        {
            string title = ((Span)App.Current.Resources["label_error"]).Text;
            string desc = ((Span)App.Current.Resources["label_fetchWorkbookError"]).Text;
            string ok = ((Span)App.Current.Resources["label_ok"]).Text;
            await App.Current.MainPage.DisplayAlert(title, desc, ok);
        }

		/// <summary>
		/// Informs the user of an error with audio playback or recording.
		/// </summary>
        public static async Task DisplayAudioError()
        {
            string title = ((Span)App.Current.Resources["label_error"]).Text;
            string desc = ((Span)App.Current.Resources["desc_audioError"]).Text;
            string ok = ((Span)App.Current.Resources["label_ok"]).Text;
            await App.Current.MainPage.DisplayAlert(title, desc, ok);
        }

		/// <summary>
		/// Shown when the user tries to save a teacher memo without name.
		/// </summary>
        public static async Task DisplayNoTeacherMemoNameError()
        {
            string title = ((Span)App.Current.Resources["label_error"]).Text;
            string desc = ((Span)App.Current.Resources["desc_noTeacherMemoName"]).Text;
            string ok = ((Span)App.Current.Resources["label_ok"]).Text;
            await App.Current.MainPage.DisplayAlert(title, desc, ok);
        }

		/// <summary>
		/// Shown when the user tries to save a teacher memo without first recording it.
		/// </summary>
        public static async Task DisplayNoTeacherMemoRecordingError()
        {
            string title = ((Span)App.Current.Resources["label_error"]).Text;
            string desc = ((Span)App.Current.Resources["desc_noTeacherMemoRecording"]).Text;
            string ok = ((Span)App.Current.Resources["label_ok"]).Text;
            await App.Current.MainPage.DisplayAlert(title, desc, ok);
        }
    }
}
