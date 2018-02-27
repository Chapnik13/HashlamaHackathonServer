using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace Hashlamakaton
{
    public static class FindPedestrian
    {
        /// <summary>
        /// Find the pedestrian in the image
        /// </summary>
        /// <param name="image">The image</param>
        /// <param name="processingTime">The pedestrian detection time in milliseconds</param>
        /// <returns>The region where pedestrians are detected</returns>
        public static Rectangle[] Find(Mat image, bool tryUseCuda)
        {
            Stopwatch watch;
            System.Drawing.Rectangle[] regions;

            //check if there is a compatible Cuda device to run pedestrian detection
            if (tryUseCuda && CudaInvoke.HasCuda)
            {  //this is the Cuda version
                using (CudaHOG des = new CudaHOG(new Size(64, 128), new Size(16, 16), new Size(8, 8), new Size(8, 8)))
                {
                    var detector = des.GetDefaultPeopleDetector();
                    des.SetSVMDetector(detector);

                    using (GpuMat cudaBgr = new GpuMat(image))
                    using (GpuMat cudaBgra = new GpuMat())
                    using (VectorOfRect vr = new VectorOfRect())
                    {
                        CudaInvoke.CvtColor(cudaBgr, cudaBgra, ColorConversion.Bgr2Bgra);
                        des.DetectMultiScale(cudaBgra, vr);
                        regions = vr.ToArray();
                    }
                }
            }
            else
            {
                //this is the CPU/OpenCL version
                using (HOGDescriptor des = new HOGDescriptor())
                {
                    des.SetSVMDetector(HOGDescriptor.GetDefaultPeopleDetector());

                    //load the image to umat so it will automatically use opencl is available
                    UMat umat = image.ToUMat(AccessType.Read);

                    MCvObjectDetection[] results = des.DetectMultiScale(umat);
                    regions = new Rectangle[results.Length];
                    for (int i = 0; i < results.Length; i++)
                        regions[i] = results[i].Rect;
                }
            }

            return regions;
        }
    }
}