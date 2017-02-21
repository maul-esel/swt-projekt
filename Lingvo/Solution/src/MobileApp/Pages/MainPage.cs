using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
	
    public partial class MainPage : NavigationPage
    {
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
