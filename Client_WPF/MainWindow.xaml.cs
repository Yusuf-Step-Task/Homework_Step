using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
namespace Client
{
    public partial class MainWindow : Window
    {
        private UdpClient client;
        private IPEndPoint serverEp;
        public MainWindow()
        {
            InitializeComponent();
            client = new UdpClient();
            serverEp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 27001);
        }
        private async void startButton_Click(object sender, RoutedEventArgs e)
        {
            startButton.IsEnabled = false;
            byte[] startMsg = Encoding.UTF8.GetBytes("start");
            await client.SendAsync(startMsg, startMsg.Length, serverEp);
            await Task.Run(ReceiveImages);
        }
        private async Task ReceiveImages()
        {
            while (true)
            {
                try
                {
                    var sizeResult = await client.ReceiveAsync();
                    int imageSize = BitConverter.ToInt32(sizeResult.Buffer, 0);
                    using MemoryStream ms = new MemoryStream();
                    while (ms.Length < imageSize)
                    {
                        var chunkResult = await client.ReceiveAsync();
                        ms.Write(chunkResult.Buffer, 0, chunkResult.Buffer.Length);
                    }
                    byte[] imageBytes = ms.ToArray();
                    Dispatcher.Invoke(() =>
                    {
                        screenImage.Source = ToBitmapImage(imageBytes);
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() => MessageBox.Show(ex.Message));
                    break;
                }
            }
        }
        private BitmapImage ToBitmapImage(byte[] bytes)
        {
            var image = new BitmapImage();
            using (var stream = new MemoryStream(bytes))
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
    }
}