using Lingvo.Common.Adapters;
using Lingvo.Common.Entities;

using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
    public class AudioCarouselPage : CarouselPage
    {
        internal IRecorder Recorder
        {
            get; set;
        }

        internal IPlayer Player
        {
            get; set;
        }

        public AudioCarouselPage(Workbook workbook, IPage selectedPage)
        {
            Title = workbook.Title;

            workbook.Pages.ForEach((p) => Children.Add(new AudioPage(p, workbook.Pages.Count) { Recorder = this.Recorder, Player = this.Player }));

            SelectedItem = selectedPage;
        }
    }
}
