using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
    public partial class MainPage : NavigationPage
    {
        public MainPage() : base(ContentPage()) { }

        private static TabbedPage ContentPage()
        {
            TabbedPage contentPage = new TabbedPage() { Title = "Lingvo" };
            contentPage.Children.Add(new LocalCollectionPage());
            contentPage.Children.Add(new DownloadPage());
            
            TeacherMemosPage teacherMemosPage = new TeacherMemosPage(contentPage);
            contentPage.Children.Add(teacherMemosPage);

            return contentPage;
        }

        
    }
}
