using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Hashlamakaton
{
    class Program
    {
        static void Main(string[] args)
        {
            Camera camera1 = new Camera();
            Camera camera2 = new Camera();
            using (Sensors main = new Sensors())
            using (Client client = new Client("192.168.137.127", 2222))
            {
                while (true)
                {
                    if (main.IsAvilable)
                    {
                        client.SendJson(main.GetCurrent());
                    }
                    if (camera1.IsAvilable)
                    {
                        client.SendJson(camera1.GetCurrent());
                    }
                    if (camera2.IsAvilable)
                    {
                        client.SendJson(camera2.GetCurrent());
                    }
                }
            }
        }
    }
}