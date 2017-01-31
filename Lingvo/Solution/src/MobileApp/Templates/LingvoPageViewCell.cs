using Lingvo.Common.Entities;
using Lingvo.MobileApp.Entities;
using Lingvo.MobileApp.Proxies;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Templates
{
    class LingvoPageViewCell : ViewCell
    {
        internal LingvoAudioProgressView ProgressView
        {
            get; private set;
        }

        private Label subtitleLabel;
        private MenuItem deleteStudentAction;

        public LingvoPageViewCell() :
            base()
        {
            Label titleLabel = new Label()
            {
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };

            string seite = ((Span)App.Current.Resources["text_seite"]).Text;
            titleLabel.SetBinding(Label.TextProperty, "Number", BindingMode.Default, null, seite + " {0}");

            subtitleLabel = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                IsVisible = false
            };

            subtitleLabel.SetBinding(Label.TextProperty, "Description");

            ProgressView = new LingvoAudioProgressView()
            {
                Size = Device.OnPlatform(iOS: 50, Android: 120, WinPhone: 240),
                LabelType = LingvoAudioProgressView.LabelTypeValue.None
            };

            LocalCollection.Instance.PageChanged += (p) =>
            {
                IPage page = (IPage)BindingContext;
                if (p.Id.Equals(page.Id))
                {
                    IPage local = new List<Workbook>(LocalCollection.Instance.Workbooks).Find(lwb => lwb.Id.Equals(p.workbookId)).Pages.Find(lp => lp.Id.Equals(page.Id));

                    BindingContext = local != null ? local : p;
                };
            };

            var deleteAction = new MenuItem
            {
                Text = ((Span)App.Current.Resources["label_delete"]).Text,
                Icon = "ic_delete.png",
                IsDestructive = true
            };

            deleteStudentAction = new MenuItem
            {
                Text = ((Span)App.Current.Resources["label_delete_studentTrack"]).Text,
                Icon = "ic_mic_off.png",
                IsDestructive = true
            };

            deleteAction.Clicked += (o, e) =>
            {
                IPage page = (IPage)BindingContext;
                Workbook workbook = new List<Workbook>(LocalCollection.Instance.Workbooks).Find(w => w.Id.Equals(page.workbookId));
                workbook.DeletePage(workbook.Pages.Find(p => p.Id.Equals(page.Id)));
            };

            deleteStudentAction.Clicked += (o, e) =>
            {
                IPage page = (IPage)BindingContext;
                page.DeleteStudentRecording();
            };

            ContextActions.Add(deleteAction);

            View = new StackLayout
            {
                Padding = new Thickness(5, 5),
                HeightRequest = Device.OnPlatform(iOS: 60, Android: 72, WinPhone: 260),
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

            IPage page = (IPage)BindingContext;

            ProgressView.OuterProgressColor = (Color)App.Current.Resources["primaryColor"];
            ProgressView.MaxProgress = 1;
            ProgressView.Progress = 1;
            ProgressView.LabelType = LingvoAudioProgressView.LabelTypeValue.None;
            ProgressView.InnerProgressColor = Color.Red;
            ProgressView.InnerProgressEnabled = page.StudentTrack != null;
            ProgressView.MuteEnabled = false;

            subtitleLabel.IsVisible = page.Description?.Length > 0;

            if (page.StudentTrack != null)
            {
                ContextActions.Add(deleteStudentAction);
            }
            else
            {
                ContextActions.Remove(deleteStudentAction);
            }
        }
    }
}
