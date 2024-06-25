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
            }
        }
    }
}