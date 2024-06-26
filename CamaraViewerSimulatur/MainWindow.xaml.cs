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
        private BitmapImage chosenImage;
        private WriteableBitmap loadedImage;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadImage_click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                chosenImage = new BitmapImage(new Uri(openFileDialog.FileName));
                loadedImage = new WriteableBitmap(chosenImage);
                ImageViewer.Source = loadedImage;
                calc_hist();
            }
        }

        private void calc_hist()
        {
            int[] hist = new int[256];
            int width = loadedImage.PixelWidth;
            int height = loadedImage.PixelHeight;
            if (loadedImage.Format.BitsPerPixel == 8) // Assuming image is 8-bit grayscale
            {
                int pixels_to_byte = width * ((loadedImage.Format.BitsPerPixel + 7) / 8); // round bytes to fit pixels number
                byte[] pixels = new byte[height * pixels_to_byte];
                loadedImage.CopyPixels(pixels, pixels_to_byte, 0);
                for (int i = 0; i < height; i++) // create histogram
                {
                    hist[pixels[i]]++; 
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
                int x = (int)((j / (double)255) * HistCanvas.Width);
                Line line = new Line
                {   
                    X1 = x,
                    Y1 = HistCanvas.Height,
                    X2 = x,
                    Y2 = HistCanvas.Height - hist[j],
                    Stroke = Brushes.MediumPurple,
                    StrokeThickness = 3
                };
                HistCanvas.Children.Add(line);
            }
        }
    }
}
