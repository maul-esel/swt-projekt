using System;
using System.Linq;
using Xamarin.Forms;

namespace Lingvo.MobileApp
{
    public partial class MainPage : ContentPage
    {
        LingvoAudioProgressView lingvoAudioProgressView;
        LingvoSingleProgressView lingvoSingleProgressView;

        public MainPage()
        {
            InitializeComponent();

            Button button = new Button { Text = "Push me!" };

            lingvoAudioProgressView = new LingvoAudioProgressView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            lingvoSingleProgressView = new LingvoSingleProgressView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            button.Clicked += Button_Clicked;

            Picker picker = new Picker
            {
                Title = "Color",
                VerticalOptions = LayoutOptions.StartAndExpand
            };

            Enum.GetNames(typeof(LingvoSingleProgressView.LabelTypeValue)).All(i => { picker.Items.Add(i); return true; });

            picker.SelectedIndex = picker.Items.IndexOf(Enum.GetName(typeof(LingvoSingleProgressView.LabelTypeValue), LingvoSingleProgressView.LabelTypeValue.Percentual));

            picker.SelectedIndexChanged += Picker_SelectedIndexChanged;

            Content = new StackLayout
            {
                Children = {
                    button,
                    lingvoAudioProgressView,
                    lingvoSingleProgressView,
                    picker
                }
            };
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            lingvoSingleProgressView.LabelType = (LingvoSingleProgressView.LabelTypeValue)Enum.GetValues(typeof(LingvoSingleProgressView.LabelTypeValue)).GetValue(((Picker)sender).SelectedIndex);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            lingvoAudioProgressView.Progress = (lingvoAudioProgressView.Progress + 20) % 120;
            lingvoSingleProgressView.Progress = (lingvoSingleProgressView.Progress + 20) % 120;
        }
    }
}
