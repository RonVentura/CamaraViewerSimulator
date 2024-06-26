using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CamaraViewerSimulatur
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window

    {   
        // initialization
        private BitmapImage chosenImage;
        private WriteableBitmap loadedImage;
        private int width;
        private int height;
        private int pixels_to_byte;
        private byte[] pixels;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadImage_click(object sender, RoutedEventArgs e)
        {
            /// <summary>
            /// This function loading the image from the file system and adjust it to the main app window
            /// </summary>

            OpenFileDialog openFileDialog = new OpenFileDialog();
            Slider.Value = 10;
            if (openFileDialog.ShowDialog() == true)
            {
                chosenImage = new BitmapImage(new Uri(openFileDialog.FileName));
                loadedImage = new WriteableBitmap(chosenImage);
                ImageViewer.Source = loadedImage;
                width = loadedImage.PixelWidth;
                height = loadedImage.PixelHeight;
                pixels_to_byte = width * ((loadedImage.Format.BitsPerPixel + 7) / 8); // round bytes to fit pixels number
                pixels = new byte[height * pixels_to_byte];
                loadedImage.CopyPixels(pixels, pixels_to_byte, 0);
                calc_hist();
            }
        }

        private void SliderChange(object sender, RoutedPropertyChangedEventArgs<double> e) 
        {
            ///<summary>
            /// This function gets the slider value, scale it, and sent it to ChangeExposure()
            /// </summary>

            double exp = (Slider.Value)/ 10; // assuming slider min value 0, slider max value 20
            ChangeExposure(exp);
        }

        private void ChangeExposure(double exp)
        {   
            ///<summary>
            /// This function gets the scaled slider value, calc the new pixels value, and present the new image
            /// </summary>
            if (ImageViewer.Source == null) 
            {
                return;
            }

            if (loadedImage.Format.BitsPerPixel == 8) // Assuming image is 8-bit grayscale
            {
                byte[] new_pixels = new byte[height * pixels_to_byte];
                for (int i = 0; i < pixels.Length; i++) // change pixels vals
                {
                    new_pixels[i] = (byte)Math.Clamp((int)(pixels[i]*exp),0,255);
                }

                loadedImage.WritePixels(new Int32Rect(0, 0, width, height), new_pixels, pixels_to_byte, 0);
                ImageViewer.Source = loadedImage;
                calc_hist();
            }
        }

        private void calc_hist()
        {
            ///<summary>
            /// This function calculate and draw the histogram for the current presented image
            /// </summary>
            HistCanvas.Children.Clear();
            int[] hist = new int[256];
            if (loadedImage.Format.BitsPerPixel == 8) // Assuming image is 8-bit grayscale
            {
                byte[] curr_pixels = new byte[height * pixels_to_byte];
                loadedImage.CopyPixels(curr_pixels, pixels_to_byte, 0);
                for (int i = 0; i < curr_pixels.Length; i++) // create histogram
                {
                    hist[curr_pixels[i]]++; 
                }
            }

            // histogram normalization
            int max = hist.Max();
            if (max != 0)
            {
                for (int i = 0;i < hist.Length; i++)
                {
                    hist[i] = (int)((hist[i]/(double)max) * HistCanvas.Height);
                }
            }

            for (int j = 0; j < hist.Length; j++) // Draw hist
            {
                int x = (int)((j * HistCanvas.Width / (double)255)); //fit to canvas
                Line line = new Line
                {   
                    X1 = x,
                    Y1 = HistCanvas.Height,
                    X2 = x,
                    Y2 = HistCanvas.Height - hist[j],
                    Stroke = Brushes.MediumPurple,
                    StrokeThickness = HistCanvas.Width/256
                };
                HistCanvas.Children.Add(line);
            }
        }
    }
}
    