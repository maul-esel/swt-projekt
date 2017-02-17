using Lingvo.Common.Entities;
using Lingvo.MobileApp.Entities;
using Lingvo.MobileApp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Templates
{
    class LingvoWorkbookViewCell : ViewCell
    {
        private static readonly int DownloadButtonSize = Device.OnPlatform(iOS: 55, Android: 65, WinPhone: 110);
        internal LingvoAudioProgressView ProgressView
        {
            get; private set;
        }

        private Label subtitleLabel;

        private MenuItem deleteAction;

        public LingvoWorkbookViewCell() :
            base()
        {
            Label titleLabel = new Label()
            {
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                LineBreakMode = LineBreakMode.TailTruncation
            };

            titleLabel.SetBinding(Label.TextProperty, "Title");

            subtitleLabel = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                IsVisible = false,
                LineBreakMode = LineBreakMode.TailTruncation
            };

            subtitleLabel.SetBinding(Label.TextProperty, "Subtitle");

            ProgressView = new LingvoAudioProgressView()
            {
                Size = Device.OnPlatform(iOS: 80, Android: 120, WinPhone: 240),
                LabelType = LingvoAudioProgressView.LabelTypeValue.NOfM,
                MuteEnabled = false,
                InnerProgressEnabled = false
            };

            deleteAction = new MenuItem
            {
                Text = ((Span)App.Current.Resources["label_delete"]).Text,
                Icon = "ic_delete.png",
                IsDestructive = true
            };

            deleteAction.Clicked += DeleteAction_Clicked;

            if (GetType().Equals(typeof(LingvoWorkbookViewCell)))
            {
                ContextActions.Add(deleteAction);
            }

            var grid = new Grid();

            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = DownloadButtonSize });

            var stackLayout = new StackLayout
            {
                Padding = new Thickness(5, 5),
                HeightRequest = Device.OnPlatform(iOS: 70, Android: 80, WinPhone: 260),
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

            grid.Children.Add(stackLayout, 0, 0);
            View = grid;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LocalCollection.Instance.WorkbookChanged += Event_WorkbookChanged;
            LocalCollection.Instance.PageChanged += Event_PageChanged;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            LocalCollection.Instance.WorkbookChanged -= Event_WorkbookChanged;
            LocalCollection.Instance.PageChanged -= Event_PageChanged;
        }

        private async void DeleteAction_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (await AlertHelper.DisplayWarningDeleteWorkbook())
                {
                    LocalCollection.Instance.DeleteWorkbook((Workbook)BindingContext);
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
            Workbook workbook = (Workbook)BindingContext;
            if (p.workbookId.Equals(workbook.Id))
            {
                Workbook local = LocalCollection.Instance.Workbooks.FirstOrDefault(lwb => lwb.Id.Equals(p.workbookId));

                BindingContext = local != null ? local : p.Workbook;
            }
        }

        protected virtual void Event_WorkbookChanged(Workbook w)
        {
            Workbook workbook = (Workbook)BindingContext;
            if (w.Id.Equals(workbook.Id))
            {
                Workbook local = LocalCollection.Instance.Workbooks.FirstOrDefault(lwb => lwb.Id.Equals(w.Id));

                BindingContext = local != null ? local : w;
            }
        }

        protected virtual void BindViewCell(Workbook workbook)
        {
            int completed = 0;
            workbook.Pages.ForEach((p) => { if (p.StudentTrack != null) completed++; });
            ProgressView.MaxProgress = workbook.Pages.Count;
            ProgressView.Progress = completed;

            ProgressView.OuterProgressColor = (Color)App.Current.Resources["secondaryColor"];

            ProgressView.LabelType = LingvoAudioProgressView.LabelTypeValue.NOfM;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            Device.BeginInvokeOnMainThread(() =>
            {
                Workbook workbook = (Workbook)BindingContext;

                ProgressView.InnerProgressEnabled = false;
                subtitleLabel.IsVisible = workbook.Subtitle?.Length > 0;

                BindViewCell(workbook);
            });
        }

    }
}
