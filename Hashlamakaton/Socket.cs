using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Hashlamakaton
{
    public class Client : IDisposable
    {
        private Socket socketClient;
        public Client(string i_IpAdress, int i_Port)
        {
            IPAddress ipAddress = IPAddress.Parse(i_IpAdress);

            IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, i_Port);
            socketClient = new Socket(ipAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            socketClient.Connect(remoteEndPoint);

        }

        public void Dispose()
        {
            socketClient.Dispose();
        }

        public void SendJpeg(Bitmap i_Bitmap)
        {
            MemoryStream ms = new MemoryStream();
            i_Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] bmpBytes = ms.GetBuffer();
            ms.Close();

            sendUdpPacket(bmpBytes);
        }

        public void SendText(string i_Text)
        {
            byte[] data = Encoding.ASCII.GetBytes(i_Text);
            sendUdpPacket(data);
        }

        public void SendJson(object i_Json)
        {
            string json = JsonConvert.SerializeObject(i_Json, Formatting.None);
            byte[] data = Encoding.ASCII.GetBytes(json);
            sendUdpPacket(data);
        }

        private void sendUdpPacket(byte[] i_Data)
        {
            try
            {
                socketClient.Send(i_Data);
            }
            catch
            {
                
            }
        }
    }
}
