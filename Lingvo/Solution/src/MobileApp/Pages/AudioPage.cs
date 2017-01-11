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

            LingvoAudioPageTemplate.ButtonCommandsHolder commandsHolder = new LingvoAudioPageTemplate.ButtonCommandsHolder
            {
                ForwardCommand = new Command<IPage>((b) => Console.WriteLine("Forward" + b.Number)),
                RewindCommand = new Command<IPage>((b) => Console.WriteLine("Rewind" + b.Number)),
                PlayPauseCommand = new Command<IPage>((b) => Console.WriteLine("PlayPause" + b.Number)),
                RecordStopCommand = new Command<IPage>((b) => Console.WriteLine("RecordStop" + b.Number))
            };

            ItemsSource = workbook.Pages;
            ItemTemplate = new LingvoAudioPageTemplate(commandsHolder, workbook);
            
            SelectedItem = selectedPage;
            
        }
	}
}
