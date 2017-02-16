using Lingvo.Common.Entities;
using Lingvo.MobileApp.Entities;
using Lingvo.MobileApp.Proxies;
using Lingvo.MobileApp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private MenuItem deleteStudentAction, deleteAction;

        public LingvoPageViewCell() :
            base()
        {
            Label titleLabel = new Label()
            {
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                LineBreakMode = LineBreakMode.TailTruncation
            };

            string seite = ((Span)App.Current.Resources["text_seite"]).Text;
            titleLabel.SetBinding(Label.TextProperty, "Number", BindingMode.Default, null, seite + " {0}");

            subtitleLabel = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                IsVisible = false,
                LineBreakMode = LineBreakMode.TailTruncation
            };

            subtitleLabel.SetBinding(Label.TextProperty, "Description");

            ProgressView = new LingvoAudioProgressView()
            {
                Size = Device.OnPlatform(iOS: 50, Android: 120, WinPhone: 240),
                LabelType = LingvoAudioProgressView.LabelTypeValue.None,
                TextSize = 15
            };

            deleteAction = new MenuItem
            {
                Text = ((Span)App.Current.Resources["label_delete"]).Text,
                Icon = "ic_delete.png",
                IsDestructive = true
            };

            ContextActions.Add(deleteAction);

            deleteStudentAction = new MenuItem
            {
                Text = ((Span)App.Current.Resources["label_delete_studentTrack"]).Text,
                Icon = "ic_mic_off.png"
            };

            deleteAction.Clicked += DeleteAction_Clicked;

            deleteStudentAction.Clicked += DeleteStudentAction_Clicked;

            View = new StackLayout
            {
                Padding = new Thickness(5, 5),
                HeightRequest = Device.OnPlatform(iOS: 60, Android: 80, WinPhone: 260),
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LocalCollection.Instance.PageChanged += Event_PageChanged;

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            LocalCollection.Instance.PageChanged -= Event_PageChanged;
        }

        private async void DeleteStudentAction_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (await AlertHelper.DisplayWarningDeleteStudentTrack())
                {
                    Lingvo.Common.Entities.Page page = (Lingvo.Common.Entities.Page)BindingContext;
                    LocalCollection.Instance.DeleteStudentRecording(page);
                    ContextActions.Remove(deleteStudentAction);
                }
            }
            catch
            {
                Console.WriteLine("Context Actions null");
            }
        }

        private async void DeleteAction_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (await AlertHelper.DisplayWarningDeletePage(((IPage)BindingContext).StudentTrack != null))
                {
                    Lingvo.Common.Entities.Page page = (Lingvo.Common.Entities.Page)BindingContext;
                    Workbook workbook = LocalCollection.Instance.Workbooks.FirstOrDefault(w => w.Id.Equals(page.workbookId));
                    LocalCollection.Instance.DeletePage((Lingvo.Common.Entities.Page)workbook.Pages.Find(p => p.Id.Equals(page.Id)));
                    ContextActions.Remove(deleteAction);
                }
            }
            catch
            {
                Console.WriteLine("Context Actions null");
            }
        }

        protected virtual void Event_PageChanged(IPage p)
        {
            IPage page = (IPage)BindingContext;
            if (p.Id.Equals(page.Id))
            {
                Workbook localWorkbook = LocalCollection.Instance.Workbooks.FirstOrDefault(lwb => lwb.Id.Equals(p.workbookId));
                IPage local = localWorkbook?.Pages.Find(lp => lp.Id.Equals(page.Id));

                BindingContext = local != null ? local : p;
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            IPage page = (IPage)BindingContext;

            ProgressView.OuterProgressColor = (Color)App.Current.Resources["primaryColor"];
            ProgressView.InnerProgressEnabled = page.StudentTrack != null;

            ProgressView.InnerProgressColor = Color.Red;
            ProgressView.Progress = 1;
            ProgressView.MaxProgress = 1;
            ProgressView.LabelType = LingvoAudioProgressView.LabelTypeValue.None;
            ProgressView.MuteEnabled = false;

            try
            {
                if (page.StudentTrack != null && !ContextActions.Contains(deleteStudentAction))
                {
                    ContextActions.Add(deleteStudentAction);
                }
            }
            catch
            {
                Console.WriteLine("Context Actions null");
            }

            subtitleLabel.IsVisible = page.Description?.Length > 0;
        }
    }
}
