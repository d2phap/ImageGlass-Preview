
using ImageGlass.Helpers;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;

namespace ImageGlass.ViewModels {
    public class MainViewModel: Observable {
        public MainViewModel() {
        }

        private string _titleText = "";

        public ApplicationView AppView { get; } = ApplicationView.GetForCurrentView();

        public string TitleText {
            get => _titleText = AppView.Title;
            set {
                AppView.Title = value;
                Set(ref _titleText, value);
            }
        }


        public ApplicationViewTitleBar TitleBar { get => AppView.TitleBar; }


        public double Dpi {
            get {
                var display = DisplayInformation.GetForCurrentView();
                return display.RawPixelsPerViewPixel;
            }
        }
    }
}
