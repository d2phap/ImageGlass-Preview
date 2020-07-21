﻿using System;
using System.Collections.Generic;

using ImageGlass.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ImageGlass.Views
{
    // TODO WTS: Remove this sample page when/if it's not needed.
    // This page is an sample of how to launch a specific page in response to a protocol launch and pass it a value.
    // It is expected that you will delete this page once you have changed the handling of a protocol launch to meet
    // your needs and redirected to another of your pages.
    public sealed partial class SchemeActivationSamplePage : Page
    {
        public SchemeActivationSampleViewModel ViewModel { get; } = new SchemeActivationSampleViewModel();

        public SchemeActivationSamplePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var parameters = e.Parameter as Dictionary<string, string>;
            if (parameters != null)
            {
                ViewModel.Initialize(parameters);
            }
        }
    }
}
