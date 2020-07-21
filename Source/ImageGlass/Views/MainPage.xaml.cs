using System;

using ImageGlass.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ImageGlass.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
