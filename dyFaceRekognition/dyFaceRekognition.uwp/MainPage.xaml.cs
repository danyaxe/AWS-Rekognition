//using dyFaceClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace dyFaceRekognition.uwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            //error handling
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();
            //handle cancel from picker

            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                BitmapImage bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(fileStream);
                SelectedImage.Source = bitmapImage;
            }
            /*
            //dangerous to specify it in production
            var rekognitionServices =
                new RekognitionServices("AKIARHH6DIMRTNVRA2S2", "pnz3ym+xyPBEpA623bzkff+tJLpeAOxE8t9ZFRgG");

            try
            {
                IsLoading.Visibility = Visibility.Visible;
                ResultTb.Text = await rekognitionServices.RegisterUser(file.Path, file.Name);
            }
            finally
            {
                IsLoading.Visibility = Visibility.Collapsed;
            }*/
        }

        private async void ValidateButton_Click(object sender, RoutedEventArgs e)
        {
            //error handling
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;

            StorageFile photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (photo == null)
            {
                // User cancelled photo capture
                return;
            }
            /*
            //dangerous to specify it in production
            var rekognitionServices =
                new RekognitionServices("AKIARHH6DIMRTNVRA2S2", "pnz3ym+xyPBEpA623bzkff+tJLpeAOxE8t9ZFRgG");

            try
            {
                IsLoading.Visibility = Visibility.Visible;
                if(await rekognitionServices.IsValidUser(photo.Path, photo.Name))
                {
                    ResultTb.Text = "Access granted!";
                }
                else
                {
                    ResultTb.Text = "Invalid user!";
                }
            }
            finally
            {
                IsLoading.Visibility = Visibility.Collapsed;
            }*/
        }
    }
}
