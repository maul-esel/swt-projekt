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
        LingvoProgressView lingvoProgressView;

        public MainPage()
        {
            InitializeComponent();
            Button button = new Button { Text = "Push me!" };
            lingvoProgressView = new LingvoProgressView
            {
                StudentTrackColor = Color.Orange,
                TeacherTrackColor = Color.Aqua,
                Size = 600,
                StudentTrackProgress = 0,
                TeacherTrackProgress = 0,
                MaxProgress = 100,
                LabelType = LingvoProgressView.LabelTypeValue.Percentual
            };
            button.Clicked += Button_Clicked;
            Picker picker = new Picker
            {
                Title = "Color",
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            string[] items = Enum.GetNames(typeof(LingvoProgressView.LabelTypeValue));
            foreach(string item in items)
            {
                picker.Items.Add(item);
            }
            picker.SelectedIndex = picker.Items.IndexOf(Enum.GetName(typeof(LingvoProgressView.LabelTypeValue), LingvoProgressView.LabelTypeValue.Percentual));
            picker.SelectedIndexChanged += Picker_SelectedIndexChanged;
            Content = new StackLayout
            {
                Children = {
                    button
        ,
        lingvoProgressView,
        pick
                }
            };
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            lingvoProgressView.LabelType = (LingvoProgressView.LabelTypeValue) Enum.GetValues(typeof(LingvoProgressView.LabelTypeValue)).GetValue(((Picker)sender).SelectedIndex);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            lingvoProgressView.TeacherTrackProgress = (lingvoProgressView.TeacherTrackProgress + 20) % 120;
            lingvoProgressView.StudentTrackProgress = (lingvoProgressView.StudentTrackProgress + 20) % 120;
        }
    }
}
