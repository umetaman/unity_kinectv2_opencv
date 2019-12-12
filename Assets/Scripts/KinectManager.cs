using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;
using OpenCvSharp;

public class KinectManager : MonoBehaviour
{
    public KinectSensor KinectSensorRef
    {
        private set; get;
    }

    public ColorFrameReader ColorFrameReaderRef
    {
        private set; get;
    }

    public DepthFrameReader DepthFrameReaderRef
    {
        private set; get;
    }

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
 
    public ushort[] DepthData
    {
        private set; get;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        KinectSensorRef = KinectSensor.GetDefault();

        
    }

    public void OpenReader()
    {
        if (KinectSensorRef != null)
        {
            ColorFrameReaderRef = KinectSensorRef.ColorFrameSource.OpenReader();
            
            DepthFrameReaderRef = KinectSensorRef.DepthFrameSource.OpenReader();
            DepthData = new ushort[KinectSensorRef.DepthFrameSource.FrameDescription.LengthInPixels];        
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
