﻿using Lingvo.Common.Entities;
using Lingvo.MobileApp.Entities;
using Lingvo.MobileApp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Templates
{
    /// <summary>
    /// The ViewCell for displaying information and providing context actions of a page.
    /// </summary>
    class LingvoWorkbookViewCell : ViewCell
    {
        private static readonly int DownloadButtonSize = Device.OnPlatform(iOS: 55, Android: 65, WinPhone: 110);

        /// <summary>
        /// The progress view showing the edited and total pages of the workbook or, if in <see cref="DownloadPage"/>, showing the download progress.
        /// </summary>
        internal LingvoAudioProgressView ProgressView
        {
            get; private set;
        }

        internal Label SubtitleLabel
        {
            get; private set;
        }

        /// <summary>
        /// The context menu item for deleting the workbook.
        /// </summary>
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

            SubtitleLabel = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                IsVisible = false,
                LineBreakMode = LineBreakMode.TailTruncation
            };

            SubtitleLabel.SetBinding(Label.TextProperty, "Subtitle");

            ProgressView = new LingvoAudioProgressView()
            {
                Size = Device.OnPlatform(iOS: 80, Android: 72, WinPhone: 240),
                LabelType = LingvoAudioProgressView.LabelTypeValue.NOfM,
                OuterProgressColor = (Color)App.Current.Resources["primaryColor"],
                MaxProgress = 0,
                Progress = 0,
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
                                            SubtitleLabel
                                        }
                                        }

                                }

            };

            grid.Children.Add(stackLayout, 0, 0);
            View = grid;
        }

        /// <summary>
        /// Called when the view cell appears on screen.
        /// Registers all important events.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LocalCollection.Instance.WorkbookChanged += Event_WorkbookChanged;
            LocalCollection.Instance.PageChanged += Event_PageChanged;
        }

        /// <summary>
        /// Called when the view cell disappears on screen.
        /// Unregisters the events registered in <see cref="OnAppearing"/>.
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            LocalCollection.Instance.WorkbookChanged -= Event_WorkbookChanged;
            LocalCollection.Instance.PageChanged -= Event_PageChanged;
        }

        /// <summary>
        /// Occurs when the delete workbook context menu item was clicked.
        /// Displays a warning dialog and deletes the workbook after positive result.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The clicked <c>EventArgs</c>.</param>
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

        /// <summary>
        /// Occurs when a page has changed.
        /// Refreshes the <c>BindingContext</c> of this view, if the changed page is part of the workbook.
        /// </summary>
        /// <param name="p">The page which has changed.</param>
        protected virtual void Event_PageChanged(IPage p)
        {
            Workbook workbook = (Workbook)BindingContext;
            if (p.workbookId.Equals(workbook.Id))
            {
                Workbook local = LocalCollection.Instance.Workbooks.FirstOrDefault(lwb => lwb.Id.Equals(p.workbookId));

                BindingContext = local != null ? local : p.Workbook;
            }
        }

        /// <summary>
        /// Occurs when a workbook has changed.
        /// Refreshes the <c>BindingContext</c> of this view, if the changed workbook is equal to it.
        /// </summary>
        /// <param name="w">The workbook which has changed.</param>
        protected virtual void Event_WorkbookChanged(Workbook w)
        {
            Workbook workbook = (Workbook)BindingContext;
            if (w.Id.Equals(workbook.Id))
            {
                Workbook local = LocalCollection.Instance.Workbooks.FirstOrDefault(lwb => lwb.Id.Equals(w.Id));

                BindingContext = local != null ? local : w;
            }
        }

        /// <summary>
        /// Binds the views in this view cell to the given workbook.
        /// Actually, it refreshes the progress view.
        /// </summary>
        /// <param name="workbook">The workbook to bind this view cell to.</param>
        protected virtual void BindViewCell(Workbook workbook)
        {
            int completed = 0;
            workbook.Pages.ForEach((p) => { if (p.StudentTrack != null) completed++; });
            ProgressView.MaxProgress = workbook.Pages.Count;
            ProgressView.Progress = completed;

            ProgressView.OuterProgressColor = (Color)App.Current.Resources["secondaryColor"];

            ProgressView.LabelType = LingvoAudioProgressView.LabelTypeValue.NOfM;
        }

        /// <summary>
        /// Occurs when the <c>BindingContext</c> has changed.
        /// Refreshes subtitle and calls <see cref="BindViewCell(Workbook)"/>.
        /// </summary>
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            Device.BeginInvokeOnMainThread(() =>
            {
                Workbook workbook = (Workbook)BindingContext;

                ProgressView.InnerProgressEnabled = false;
                SubtitleLabel.IsVisible = workbook.Subtitle?.Length > 0;

                BindViewCell(workbook);
            });
        }

    }
}
