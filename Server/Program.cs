using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Shared;
namespace Server
{
    internal class Program
    {
        static UdpClient server = new UdpClient(27001);
        static ImageHelper imageHelper = new ImageHelper();
        static async Task Main(string[] args)
        {
            Console.WriteLine("Server ayaqdadir...");
            IPEndPoint clientEp = await WaitForClient();
            if (clientEp == null)
            { 
                return;
            }
            Console.WriteLine($"Client qConnection: {clientEp}");

            while (true)
            {
                byte[] image = imageHelper.TakeScreenShotBytes();

                await SendSize(image, clientEp);
                await SendImage(image, clientEp);

                Console.WriteLine("Sekil gonderildi: " + image.Length);
                await Task.Delay(500);
            }
        }
        static async Task<IPEndPoint> WaitForClient()
        {
            var result = await server.ReceiveAsync();
            string message = Encoding.UTF8.GetString(result.Buffer);
            if (message == "start") 
            {
                return result.RemoteEndPoint;
            }
            Console.WriteLine("Error: Yanlis sekilde input verdiniz ");
            return null;
        }
        static async Task SendSize(byte[] image, IPEndPoint ep)
        {
            byte[] size = BitConverter.GetBytes(image.Length);
            await server.SendAsync(size, size.Length, ep);
        }
        static async Task SendImage(byte[] image, IPEndPoint ep)
        {
            int chunkSize = 60000;
            for (int i = 0; i < image.Length; i += chunkSize)
            {
                byte[] part = image.Skip(i).Take(chunkSize).ToArray();
                await server.SendAsync(part, part.Length, ep);
            }
        }
    }
}