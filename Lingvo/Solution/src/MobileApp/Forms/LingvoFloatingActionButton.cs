using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Forms
{
    class LingvoFloatingActionButton : View
    {
        public delegate void FabClickedEventHandler(object sender, EventArgs e);

        public event FabClickedEventHandler FabClicked;
        public LingvoFloatingActionButton() : base() {
            WidthRequest = Device.OnPlatform(iOS: 56, Android: 56, WinPhone: 112);
            HeightRequest = Device.OnPlatform(iOS: 56, Android: 56, WinPhone: 112);
        }

        public void OnFabClicked(object sender, EventArgs e)
        {
            FabClicked?.Invoke(this, e);
        }
    }
}
