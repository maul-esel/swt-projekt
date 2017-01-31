using Lingvo.Common.Entities;
using Lingvo.MobileApp.Entities;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Templates
{
    class LingvoWorkbookViewCell : ViewCell
    {
        internal LingvoAudioProgressView ProgressView
        {
            get; private set;
        }

        private Label subtitleLabel;
        public LingvoWorkbookViewCell() :
            base()
        {
            Label titleLabel = new Label()
            {
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };
            titleLabel.SetBinding(Label.TextProperty, "Title");

            subtitleLabel = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                IsVisible = false
            };

            subtitleLabel.SetBinding(Label.TextProperty, "Subtitle");

            ProgressView = new LingvoAudioProgressView()
            {
                Size = Device.OnPlatform(iOS: 80, Android: 120, WinPhone: 240),
                LabelType = LingvoAudioProgressView.LabelTypeValue.NOfM,
                MuteEnabled = false,
                InnerProgressEnabled = false
            };

            LocalCollection.Instance.WorkbookChanged += (w) =>
            {
                Workbook workbook = (Workbook)BindingContext;
                if (w.Id.Equals(workbook.Id))
                {
                    Workbook local = new List<Workbook>(LocalCollection.Instance.Workbooks).Find(lwb => lwb.Id.Equals(w.Id));

                    BindingContext = local != null ? local : w;
                }
            };

            LocalCollection.Instance.PageChanged += (p) =>
            {
                Workbook workbook = (Workbook)BindingContext;
                if (p.workbookId.Equals(workbook.Id))
                {
                    Workbook local = new List<Workbook>(LocalCollection.Instance.Workbooks).Find(lwb => lwb.Id.Equals(p.workbookId));

                    BindingContext = local != null ? local : p.Workbook;
                };
            };

            View = new StackLayout
            {
                Padding = new Thickness(5, 5),
                HeightRequest = Device.OnPlatform(iOS: 70, Android: 72, WinPhone: 260),
                Orientation = StackOrientation.Horizontal,
                Children =
                                {
                                    ProgressView,
                                    new StackLayout
                                    {
                                        HorizontalOptions = LayoutOptions.StartAndExpand,
                                        VerticalOptions = LayoutOptions.Center,
                                        Spacing = 0,
                                        Children =
                                        {
                                            titleLabel,
                                            subtitleLabel
                                        }
                                        }
                                }

            };
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            Workbook workbook = (Workbook)BindingContext;

            int completed = 0;
            workbook.Pages.ForEach((p) => { if (p.StudentTrack != null) completed++; });
            ProgressView.OuterProgressColor = (Color)App.Current.Resources["secondaryColor"];
            ProgressView.MaxProgress = workbook.Pages.Count;
            ProgressView.Progress = completed;
            ProgressView.InnerProgressEnabled = false;
            ProgressView.LabelType = LingvoAudioProgressView.LabelTypeValue.NOfM;

            subtitleLabel.IsVisible = workbook.Subtitle?.Length > 0;
        }

    }
}
