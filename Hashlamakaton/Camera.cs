using Emgu.CV;
using System.Threading;
using System.Drawing;
using System.IO;
using Emgu.CV.Structure;

namespace Hashlamakaton
{
    public class Camera : ISensor
    {
        private Mat m_CurrentImage;

        private readonly object obj = new object();
        public Bitmap CurrentImage
        {
            get { lock (obj) { return m_CurrentImage.Bitmap; } }
        }

        public bool IsAvilable { get; private set; }

        private static int s_CamNum = 0;
        private readonly int r_CamNum;

        private Capture m_Capture;
        public Camera()
        {
            r_CamNum = s_CamNum++;
            m_Capture = new Capture(r_CamNum); //create a camera captue
            m_CurrentImage = m_Capture.QueryFrame();

            if (m_CurrentImage == null)
            {
                IsAvilable = false;
            }
            else
            {
                IsAvilable = true;
                ThreadStart updateFrameThreadStart = new ThreadStart(updateFrame);
                Thread updateFrameThread = new Thread(updateFrameThreadStart);
                updateFrameThread.Start();
            }
        }

        private void updateFrame()
        {
            while (true)
            {
                m_CurrentImage = m_Capture.QueryFrame();
            }
        }

        public object GetCurrent()
        {
            Rectangle[] results = FindPedestrian.Find(m_CurrentImage, true);

            foreach (Rectangle rect in results)
            {
                CvInvoke.Rectangle(m_CurrentImage, rect, new Bgr(Color.Red).MCvScalar);
            }

            MemoryStream ms = new MemoryStream();
            CurrentImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] bmpBytes = ms.GetBuffer();
            ms.Close();
            return new
            {
                Type = "Image",
                Id = r_CamNum,
                HumanDetected = results.Length,
                Bitmap = bmpBytes
        };
        }
    }
}
