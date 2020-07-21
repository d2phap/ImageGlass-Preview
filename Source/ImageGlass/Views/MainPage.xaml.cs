
using ImageGlass.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using Microsoft.Toolkit.Uwp.UI.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI;
using Windows.UI.Input.Preview.Injection;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using ColorHelper = Microsoft.Toolkit.Uwp.Helpers.ColorHelper;

namespace ImageGlass.Views {
    public sealed partial class MainPage: Page {
        public MainViewModel ViewModel { get; } = new MainViewModel();


        public MainPage() {
            InitializeComponent();

            // customize title bar
            SetWindowTitleBar();

            ListenToThemeEvents();
            ThemeListener_ThemeChanged(null);
        }


        #region Theme and colors

        private void ListenToThemeEvents() {
            var themeListener = new ThemeListener();
            themeListener.ThemeChanged += ThemeListener_ThemeChanged;
        }

        private void ThemeListener_ThemeChanged(ThemeListener sender) {
            var ui = new UISettings();

            var themedBgColor = ui.GetColorValue(UIColorType.Background);
            var themedForeColor = ui.GetColorValue(UIColorType.Foreground);

            // Page background
            var pageBgBrush = new AcrylicBrush() {
                BackgroundSource = AcrylicBackgroundSource.HostBackdrop,
                TintOpacity = 0,
                TintLuminosityOpacity = 0.5,
                FallbackColor = themedBgColor,
                TintColor = themedBgColor,
            };
            this.Background = pageBgBrush;


            // Title bar text
            var titleBarLeftBgBrush = new AcrylicBrush() {
                BackgroundSource = AcrylicBackgroundSource.Backdrop,
                TintOpacity = 0,
                TintLuminosityOpacity = 0.5,
                FallbackColor = themedBgColor,
                TintColor = themedBgColor,
            };
            pageTitleBar.Background = titleBarLeftBgBrush;
            var txt = titleBarLeftContent.FindChild<TextBlock>();
            txt.Foreground = new SolidColorBrush(themedForeColor);


            // Change color of Title bar buttons
            ViewModel.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            ViewModel.TitleBar.ButtonForegroundColor = themedForeColor;

            ViewModel.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            ViewModel.TitleBar.ButtonHoverBackgroundColor = ColorHelper.ToColor("#5fff");
            ViewModel.TitleBar.ButtonPressedBackgroundColor = ColorHelper.ToColor("#afff");

            // Toolbar
            toolBar.Background = titleBarLeftBgBrush;

            // Thumb bar
            thumbBar.Background = titleBarLeftBgBrush;
        }

        #endregion


        #region Title bar customization
        private void SetWindowTitleBar() {
            // Hide default title bar.
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);

            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(pageTitleBar);

            // Register a handler for when the size of the overlaid caption
            // control changes. For example,
            // when the app moves to a screen with a different DPI.
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            // Register a handler for when the title bar visibility changes.
            // For example, when the title bar is invoked in full screen mode.
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args) {
            UpdateTitleBarLayout(sender);
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar titleBar) {
            // Get the size of the caption controls area and back button 
            // (returned in logical pixels), and move your content around as necessary.
            titleBarLeftCln.Width = new GridLength(titleBar.SystemOverlayLeftInset);
            titleBarRightCln.Width = new GridLength(titleBar.SystemOverlayRightInset);
            btnOpen.Margin = new Thickness(0, 0, titleBar.SystemOverlayRightInset, 0);

            // Update title bar control size as needed to account for system size changes.
            pageTitleBar.Height = titleBar.Height;
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args) {
            if (sender.IsVisible) {
                pageTitleBar.Visibility = Visibility.Visible;
            }
            else {
                pageTitleBar.Visibility = Visibility.Collapsed;
            }
        }
        #endregion


        protected override async void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(1, 1));

            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1) {
                var filename = args.Last();

                picMain.Source = await LoadImage(filename);
            }
            else {
                picMain.Source = await LoadImage(@"C:\Users\d2pha\Desktop\_photo\manga.heic");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) {
            Window.Current.CoreWindow.PointerWheelChanged += CoreWindow_PointerWheelChanged;
        }

        private async void BtnOpen_Click(object sender, RoutedEventArgs e) {

            var openPicker = new FileOpenPicker {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            openPicker.FileTypeFilter.Add("*");
            var file = await openPicker.PickSingleFileAsync();

            //var appView = ApplicationView.GetForCurrentView();
            //appView.TitleBar.BackgroundColor = Colors.Transparent;

            if (file != null) {
                picMain.Source = await LoadImage(file.Path);
            }
        }



        private async Task<object> LoadImage(string path) {
            StorageFile file;

            try {
                file = await StorageFile.GetFileFromPathAsync(path);
            }
            catch (Exception) {
                // path is directory
                var dir = await StorageFolder.GetFolderFromPathAsync(path);
                var files = await dir.GetFilesAsync();

                file = files[0];
            }

            ImageSource bitmap = null;

            var ext = Path.GetExtension(file.Path).ToLower();


            try {
                using (var stream = await file.OpenAsync(FileAccessMode.Read)) {
                    if (ext == ".svg") {
                        var svg = new SvgImageSource();
                        await svg.SetSourceAsync(stream);

                        bitmap = svg;
                    }
                    else {
                        var bmp = new BitmapImage();
                        await bmp.SetSourceAsync(stream);

                        bitmap = bmp;
                    }
                }
            }
            catch (Exception ex) {
                // show error
                var x = new ContentDialog {
                    Content = ex.Message
                };
                await x.ShowAsync();
            }

            ViewModel.TitleText = $"ImageGlass - {file.Path}";

            return bitmap;
        }




        private void Image_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) {
            //picMainTransform.TranslateX = 0;
            //picMainTransform.TranslateY = 0;
        }

        private void Image_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e) {
            //picMainTransform.TranslateX += e.Delta.Translation.X / viewer.ZoomFactor;
            //picMainTransform.TranslateY += e.Delta.Translation.Y / viewer.ZoomFactor;

            // invert the sign of distance for correct panning direction
            var tranX = -1 * e.Delta.Translation.X;
            var tranY = -1 * e.Delta.Translation.Y;

            var offsetX = viewer.HorizontalOffset + tranX;
            var offsetY = viewer.VerticalOffset + tranY;

            viewer.ChangeView(offsetX, offsetY, null, true);

            //if (offsetX >= viewer.ScrollableWidth || offsetX <= 0) {
            //  picMainTransform.TranslateX += e.Delta.Translation.X / viewer.ZoomFactor;
            //}

            //if (offsetY >= viewer.ScrollableHeight || offsetY <= 0) {
            //  picMainTransform.TranslateY += e.Delta.Translation.Y / viewer.ZoomFactor;
            //}
        }


        private async void PicMain_ImageOpened(object sender, ImageExOpenedEventArgs e) {
            await Task.Delay(1);
            await AutoZoom(true);


            // center image
            var offsetX = viewer.ScrollableWidth / 2;
            var offsetY = viewer.ScrollableHeight / 2;

            viewer.ChangeView(offsetX, offsetY, null, true);
        }

        private void PicMain_ImageFailed(object sender, ImageExFailedEventArgs e) {

        }


        #region Zoom modes
        private async void ScaleToFit(bool disableAnimation = false) {
            var heightRatio = (viewer.ActualHeight - pageTitleBar.ActualHeight - toolBar.ActualHeight - thumbBar.ActualHeight) / picMain.ActualHeight;


            viewer.ChangeView(null, null, (float)heightRatio, disableAnimation);
            await Task.Delay(100);
        }

        private async Task ScaleToFill(bool disableAnimation = false) {
            var widthRatio = viewer.ActualWidth / picMain.ActualWidth;
            var heightRatio = (viewer.ActualHeight - pageTitleBar.ActualHeight - toolBar.ActualHeight - thumbBar.ActualHeight) / picMain.ActualHeight;
            var scale = Math.Max(widthRatio, heightRatio);

            viewer.ChangeView(null, null, (float)scale, disableAnimation);
            await Task.Delay(100);
        }

        private async Task AutoZoom(bool disableAnimation = false) {
            var widthRatio = viewer.ActualWidth / picMain.ActualWidth;
            var heightRatio = (viewer.ActualHeight - pageTitleBar.ActualHeight - toolBar.ActualHeight - thumbBar.ActualHeight) / picMain.ActualHeight;
            var minRatio = Math.Min(widthRatio, heightRatio);

            double scale = Math.Min(minRatio, 1);

            viewer.ChangeView(null, null, (float)scale, disableAnimation);
            await Task.Delay(100);
        }
        #endregion



        private void Viewer_DragOver(object sender, DragEventArgs e) {
            e.AcceptedOperation = DataPackageOperation.Copy;

            if (e.DragUIOverride != null) {
                e.DragUIOverride.Caption = "Open image";
                e.DragUIOverride.IsContentVisible = true;
            }
        }


        private async void Viewer_Drop(object sender, DragEventArgs e) {
            if (e.DataView.Contains(StandardDataFormats.StorageItems)) {
                var items = await e.DataView.GetStorageItemsAsync();

                if (items.Count > 0) {
                    picMain.Source = await LoadImage(items[0].Path);
                }
            }
        }


        private void HoldCtrlKey() {
            var inputInjector = InputInjector.TryCreate();

            var ctrlKey = new InjectedInputKeyboardInfo {
                VirtualKey = (ushort)VirtualKey.Control,
            };

            inputInjector.InjectKeyboardInput(new[] { ctrlKey });
            //viewer.ZoomMode = ZoomMode.Enabled;


            //var scale = viewer.ActualHeight / picMain.ActualHeight;
            viewer.HorizontalScrollMode = ScrollMode.Disabled;
            viewer.VerticalScrollMode = ScrollMode.Disabled;
        }

        private void ReleaseCtrlKey() {
            var inputInjector = InputInjector.TryCreate();

            var ctrlKey = new InjectedInputKeyboardInfo {
                VirtualKey = (ushort)VirtualKey.Control,
                KeyOptions = InjectedInputKeyOptions.KeyUp,
            };

            inputInjector.InjectKeyboardInput(new[] { ctrlKey });
            //viewer.ZoomMode = ZoomMode.Disabled;
            viewer.HorizontalScrollMode = ScrollMode.Enabled;
            viewer.VerticalScrollMode = ScrollMode.Enabled;

            viewer.HorizontalScrollMode = ScrollMode.Disabled;
            viewer.VerticalScrollMode = ScrollMode.Disabled;

        }

        private async void viewer_PointerWheelChanged(object sender, PointerRoutedEventArgs e) {
            //var delta = e.GetCurrentPoint(viewer).Properties.MouseWheelDelta;
            ////var scale = viewer.ActualHeight / picMain.ActualHeight;
            //if (delta <= 0) {
            //  viewer.ChangeView(null, null, (float?)(viewer.ZoomFactor - 0.000001), true);
            //}
        }

        private void viewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e) {
            //ViewModel.TitleText = e.FinalView.ZoomFactor.ToString();

        }


        private void viewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e) {
            // update the space viewer according to new zoom factor
            var top = (pageTitleBar.ActualHeight + toolBar.ActualHeight) / viewer.ZoomFactor;
            var bot = thumbBar.ActualHeight / viewer.ZoomFactor;

            viewerGrid.Margin = new Thickness(0, top, 0, bot);
        }

        private async void picMain_PointerWheelChanged(object sender, PointerRoutedEventArgs e) {

            //var delta = e.GetCurrentPoint(viewer).Properties.MouseWheelDelta;
            ////var scale = viewer.ActualHeight / picMain.ActualHeight;
            //if (delta <= 0) {
            //  viewer.ChangeView(null, null, (float?)(viewer.ZoomFactor - 0.000001), true);
            //}
        }


        private async void CoreWindow_PointerWheelChanged(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.PointerEventArgs e) {
            HoldCtrlKey();

            await Task.Delay(200);

            ReleaseCtrlKey();
        }


    }





}
