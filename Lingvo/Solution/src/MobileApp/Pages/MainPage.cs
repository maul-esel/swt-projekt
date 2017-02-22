using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
    /// <summary>
    /// The application's main <c>NavigationPage</c>.
    /// </summary>
    public partial class MainPage : NavigationPage
    {
        /// <summary>
        /// A custom class that sets the Back button.
        /// Because our app is a tabbed application, the SetBackButtonTitle method must be invoked within the TabbedPage constructor
        /// </summary>
        private class CustomTabbedPage : TabbedPage
        {
            public CustomTabbedPage() : base()
            {
                NavigationPage.SetBackButtonTitle(this, "Zurück");
                Title = ((Span)App.Current.Resources["app_name"]).Text;
            }
        }
        public MainPage() : base(ContentPage())
        {
            BarTextColor = Color.White;
            SetBackButtonTitle(this, "Zurück");
        }

        /// <summary>
        /// A custom <c>TabbedPage</c> containing the <see cref="LocalCollectionPage"/>, <see cref="DownloadPage"/> and <see cref="TeacherMemosPage"/>.
        /// Additionally, it provides access to the <see cref="LegalPage"/> via a toolbar item.
        /// </summary>
        /// <returns>The <see cref="CustomTabbedPage"/> containing the content pages of the app.</returns>
        private static TabbedPage ContentPage()
        {
            TabbedPage contentPage = new CustomTabbedPage();
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