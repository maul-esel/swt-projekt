using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
    public partial class MainPage : NavigationPage
    {
        public MainPage() : base(ContentPage())
        {
            BarTextColor = Color.White;
        }

        private static TabbedPage ContentPage()
        {
            TabbedPage contentPage = new TabbedPage() { Title = ((Span)App.Current.Resources["app_name"]).Text };
            contentPage.Children.Add(new LocalCollectionPage());
            contentPage.Children.Add(new DownloadPage());

            contentPage.ToolbarItems.Add(new ToolbarItem()
            {
                Icon = "ic_info_outline.png",
                Text = ((Span)App.Current.Resources["label_legal"]).Text,
                Command = new Command(() => App.Current.MainPage.Navigation.PushAsync(new LegalPage()))
            });

            TeacherMemosPage teacherMemosPage = new TeacherMemosPage(contentPage);
            contentPage.Children.Add(teacherMemosPage);

            return contentPage;
        }


    }
}
