using ImageGlass.Services;
using ImageGlass.Views;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace ImageGlass.Activation {
    internal class FileAssocActivationHandler: ActivationHandler<File​Activated​Event​Args> {

        protected override async Task HandleInternalAsync(File​Activated​Event​Args args) {
            var file = args.Files.FirstOrDefault();

            NavigationService.Navigate(typeof(MainPage), file);

            await Task.CompletedTask;
        }
    }
}
