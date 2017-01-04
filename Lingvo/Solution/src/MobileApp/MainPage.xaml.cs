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
                InnerProgressEnabled = false,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            button.Clicked += Button_Clicked;

            Picker picker = new Picker
            {
                Title = "Color",
                VerticalOptions = LayoutOptions.StartAndExpand
            };

            Enum.GetNames(typeof(LingvoProgressView.LabelTypeValue)).All(i => { picker.Items.Add(i); return true; });

            picker.SelectedIndex = picker.Items.IndexOf(Enum.GetName(typeof(LingvoProgressView.LabelTypeValue), LingvoProgressView.LabelTypeValue.Percentual));

            picker.SelectedIndexChanged += Picker_SelectedIndexChanged;

            Content = new StackLayout
            {
                Children = {
                    button,
                    lingvoProgressView,
                    picker
                }
            };
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            lingvoProgressView.LabelType = (LingvoProgressView.LabelTypeValue)Enum.GetValues(typeof(LingvoProgressView.LabelTypeValue)).GetValue(((Picker)sender).SelectedIndex);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            lingvoProgressView.OuterProgress = (lingvoProgressView.OuterProgress + 20) % 120;
            lingvoProgressView.InnerProgress = (lingvoProgressView.InnerProgress + 20) % 120;
        }
    }
}
