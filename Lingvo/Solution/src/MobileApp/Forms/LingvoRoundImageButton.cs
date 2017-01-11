using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Forms
{
    public class LingvoRoundImageButton : View
    {
        public string Image
        {
            get; set;
        }

        public int Size
        {
            get; set;
        }

        public delegate void OnClickedEventHandler(object sender, EventArgs e);
        public event OnClickedEventHandler OnClicked;

        public void OnButtonClicked(object sender, EventArgs e)
        {
            OnClicked?.Invoke(this, e);
        }
    }
}
