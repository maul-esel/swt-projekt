using Lingvo.Common.Entities;
using Lingvo.MobileApp.Controllers;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
    public class AudioCarouselPage : CarouselPage
    {
        public AudioCarouselPage(Workbook workbook, IPage selectedPage)
        {
            Title = workbook.Title;

            workbook.Pages.ForEach((p) => Children.Add(new AudioPage(p, workbook.Pages.Count)));
            
            SelectedItem = new List<ContentPage>(Children).Find((p) => ((AudioPage)p).Page.Equals(selectedPage));

            CurrentPageChanged += AudioCarouselPage_CurrentPageChanged;
			StudentPageController.Instance.SelectedPage = ((AudioPage)SelectedItem).Page;
        }

        private void AudioCarouselPage_CurrentPageChanged(object sender, System.EventArgs e)
        {
           StudentPageController.Instance.SelectedPage = ((AudioPage)SelectedItem).Page;
        }
    }
}
