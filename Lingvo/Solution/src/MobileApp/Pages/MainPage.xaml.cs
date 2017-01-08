using Lingvo.MobileApp.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
	public partial class MainPage : TabbedPage
	{
		public MainPage()
		{
            InitializeComponent();

            this.Children.Add(new LocalCollectionPage() { Icon = "Icon.png", Title = "Meine Arbeitshefte" });
            
		}
	}
}
