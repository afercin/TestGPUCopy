using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestGPUCopy
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GPU gpu;
        public MainWindow()
        {
            InitializeComponent();
            gpu = new GPU();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = ImageToBytes(Image.FromFile(@"D:/prueba.png"));
            byte[] copy = new byte[data.Length];

            gpu.GPUCopy(data, 0, copy, 0, data.Length);

            Img.Source = BytesToScreenImage(copy);
        }
        public static byte[] ImageToBytes(Image img)
        {
            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Jpeg);
                data = ms.ToArray();
            }
            return data;
        }


        public static BitmapImage BytesToScreenImage(byte[] imageBytes)
        {
            BitmapImage bitmap = new BitmapImage();
            using (MemoryStream stream = new MemoryStream(imageBytes))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }
            return bitmap;
        }
    }
}
