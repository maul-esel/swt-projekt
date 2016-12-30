using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Lingvo.MobileApp
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
            Content = new StackLayout
            {
                Children = {
        new Button { Text = "Push me!" },
        new LingvoProgressView
        {
            StudentTrackProgress = 33,
            TeacherTrackProgress = 75,
            StudentTrackColor = Color.Orange,
           TeacherTrackColor = Color.Aqua
        }
      }
            };
        }
	}
}
