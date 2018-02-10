using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Media.Capture;
using Windows.Storage;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EmotionalAPI
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        CameraCaptureUI captureUI = new CameraCaptureUI();
        StorageFile photo;
        IRandomAccessStream imageStream;

        const string APIKEY = "e2e56bec19ee46c78c675d316f53f74b";
        EmotionServiceClient emotionServiceClient = new EmotionServiceClient(APIKEY);
        Emotion[] emotionResult;

        public MainPage()
        {
            this.InitializeComponent();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);

        }

        private async void takePhoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
                if(photo == null)
                {
                    return;
                }
                else
                {
                    imageStream = await photo.OpenAsync(FileAccessMode.Read);
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream);
                    SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                    SoftwareBitmap softwareBitmapBGRB = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                    SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
                    await bitmapSource.SetBitmapAsync(softwareBitmapBGRB);

                    image.Source = bitmapSource;
                }
            }
            catch
            {
                output.Text = "Eroor taking photo";
            }
        }

        private async void getEmotion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                emotionResult = await emotionServiceClient.RecognizeAsync(imageStream.AsStream());
                if (emotionResult != null)
                {
                     
                    output.Text = "your emotions are: \n" +
                        "Happiness: " + emotionResult[0].Scores.Happiness + "\n" +
                        "Sadness: " + emotionResult[0].Scores.Sadness + "\n" +
                        "Surprise: " + emotionResult[0].Scores.Surprise + "\n" +
                        "Fear: " + emotionResult[0].Scores.Fear + "\n" +
                        "Anger: " + emotionResult[0].Scores.Anger + "\n" +
                        "Contempt: " + emotionResult[0].Scores.Contempt + "\n" +
                        "Disgust: " + emotionResult[0].Scores.Disgust + "\n" +
                        "Neutral: " + emotionResult[0].Scores.Neutral + "\n";

                }
            }
            catch
            {
                output.Text = "Error returning the emotion";
            }
        }
    }
}
