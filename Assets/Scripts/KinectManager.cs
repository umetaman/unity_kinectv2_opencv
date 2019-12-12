using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Windows.Kinect;
using OpenCvSharp;

public class KinectManager : MonoBehaviour
{
    //Kinect
    public KinectSensor KinectSensorRef
    {
        private set; get;
    }

    public MultiSourceFrameReader SourceReaderRef;

    public byte[] ColorData
    {
        private set; get;
    }

    public int ColorFrameWidth
    {
        private set; get;
    }

    public int ColorFrameHeight
    {
        private set; get;
    }

    //基本的に変化することはない
    public readonly int DepthFrameWidth = 512;
    public readonly int DepthFrameHeight = 424;
 
    //符号なし16Bit
    public ushort[] DepthData
    {
        private set; get;
    }

    public Body[] BodyData
    {
        private set; get;
    }

    //OpenCv
    public Mat ColorFrameMat
    {
        private set; get;
    }

    public Mat DepthFrameMat
    {
        private set; get;
    }

    // Start is called before the first frame update
    void Start()
    {
        //最初はnullを入れておく
        BodyData = null;

        KinectSensorRef = KinectSensor.GetDefault();
        OpenReader();
        SetupOpenCvMat();
    }

    public void OpenReader()
    {
        if (KinectSensorRef != null)
        {
            SourceReaderRef = KinectSensorRef.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth);

            //OpenCVで使うのでRGBA
            var colorFrameDesc = KinectSensorRef.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);
            ColorFrameWidth = colorFrameDesc.Width;
            ColorFrameHeight = colorFrameDesc.Height;
            
            //領域確保
            ColorData = new byte[colorFrameDesc.BytesPerPixel * colorFrameDesc.LengthInPixels];
            DepthData = new ushort[KinectSensorRef.DepthFrameSource.FrameDescription.LengthInPixels];

            if(KinectSensorRef.IsOpen == false)
            {
                KinectSensorRef.Open();
            }
        }
    }

    public void SetupOpenCvMat()
    {
        //BGRAのbyte[]
        ColorFrameMat = new Mat(ColorFrameHeight, ColorFrameWidth, MatType.CV_8UC4);
        //ushort[]
        DepthFrameMat = new Mat(DepthFrameHeight, DepthFrameWidth, MatType.CV_16UC1);
    }


    // Update is called once per frame
    void Update()
    {
        ReadFrame();

        if(ColorFrameMat.IsDisposed == false)
        {
            Cv2.ImShow("ColorFrameMat", ColorFrameMat);
        }

        Debug.Log("Update!");
    }


    public void ReadFrame()
    {
        if (SourceReaderRef != null)
        {
            //フレームの読み出し
            var frame = SourceReaderRef.AcquireLatestFrame();

            if (frame != null)
            {
                var colorFrame = frame.ColorFrameReference.AcquireFrame();
                if (colorFrame != null)
                {
                    var depthFrame = frame.DepthFrameReference.AcquireFrame();
                    if (depthFrame != null)
                    {
                        colorFrame.CopyConvertedFrameDataToArray(ColorData, ColorImageFormat.Bgra);
                        Marshal.Copy(ColorData, 0, ColorFrameMat.Data, ColorData.Length);

                        depthFrame.Dispose();
                        depthFrame = null;
                    }

                    colorFrame.Dispose();
                    colorFrame = null;
                }

                frame = null;
            }
        }
    }

    public void ReadDepthFrame()
    {

    }

    public void ReadBodyFrame()
    {

    }

    private void releaseKinectResources()
    {
        if(SourceReaderRef != null)
        {
            SourceReaderRef.Dispose();
            SourceReaderRef = null;
        }
        
        if (KinectSensorRef != null)
        {
            if (KinectSensorRef.IsOpen)
            {
                KinectSensorRef.Close();
            }

            KinectSensorRef = null;
        }
    }

    private void releaseOpenCvResources()
    {
        if(ColorFrameMat.IsDisposed == false)
        {
            ColorFrameMat.Dispose();
        }

        if(DepthFrameMat.IsDisposed == false)
        {
            DepthFrameMat.Dispose();
        }
    }

    private void OnApplicationQuit()
    {
        releaseKinectResources();
        releaseOpenCvResources();
    }
}
