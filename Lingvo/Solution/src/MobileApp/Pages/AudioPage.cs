using Lingvo.Common.Entities;
using Lingvo.MobileApp.Forms;
using Lingvo.MobileApp.Templates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
	public class AudioPage : CarouselPage
	{
        public AudioPage(Workbook workbook, IPage selectedPage)
        {
            Title = workbook.Title;

            LingvoAudioPageTemplate.ButtonClickedEventHandlerHolder commandsHolder = new LingvoAudioPageTemplate.ButtonClickedEventHandlerHolder
            {
                ForwardHandler = (buttons, page, progress) => { },
                RewindHandler = (buttons, page, progress) => { },
                PlayPauseHandler = (buttons, page, progress) =>
                {
                    if(buttons.PlayPauseButton.Image.Equals(LingvoRoundImageButton.PlayImage))
                    {
                        buttons.PlayPauseButton.Image = LingvoRoundImageButton.PauseImage;
                        buttons.RecordStopButton.Image = LingvoRoundImageButton.StopImage;
                        buttons.ForwardButton.IsEnabled = buttons.RewindButton.IsEnabled = true;
                    } else
                    {
                        buttons.PlayPauseButton.Image = LingvoRoundImageButton.PlayImage;
                    }
                },
                RecordStopHandler = (buttons, page, progress) =>
                {
                    if(buttons.RecordStopButton.Image.Equals(LingvoRoundImageButton.StopImage))
                    {
                        buttons.PlayPauseButton.Image = LingvoRoundImageButton.PlayImage;
                        buttons.ForwardButton.IsEnabled = buttons.RewindButton.IsEnabled = false;
                        buttons.RecordStopButton.Image = LingvoRoundImageButton.RecordImage;
                    }
                }
            };

            ItemsSource = workbook.Pages;
            ItemTemplate = new LingvoAudioPageTemplate(commandsHolder, workbook);
            
            SelectedItem = selectedPage;
            
        }
	}
}
