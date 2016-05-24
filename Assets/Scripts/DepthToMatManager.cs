﻿using UnityEngine;
using System.Collections;
using Windows.Kinect;
using OpenCVForUnity;



public class DepthToMatManager : MonoBehaviour
{
    private KinectSensor _Sensor;
    private CoordinateMapper _Mapper;
    private Mesh _Mesh;
    private Vector3[] _Vertices;
    private Vector2[] _UV;
    private int[] _Triangles;

    // Only works at 4 right now
    private const int _DownsampleSize = 4;
    private const double _DepthScale = 0.02f;
    private const int _Speed = 50;

    public MultiSourceManager _MultiManager;
    public ColorSourceManager _ColorManager;
    public  DepthSourceManager _DepthManager;


    //放深度mat
    Mat _Depth = new Mat();

    public Mat getDepthMat()
    {
        return _Depth;
    }
    public int getWidth()
    {
        return _Depth.width();
    }
    public int getheight()
    {
        return _Depth.height();
    } 
	// Use this for initialization
	void Start () {
        _Sensor = KinectSensor.GetDefault();
        if (_Sensor != null)
        {
            _Mapper = _Sensor.CoordinateMapper;
            var frameDesc = _Sensor.DepthFrameSource.FrameDescription;

            // Downsample to lower resolution
            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (_Sensor == null)
        {
            return;
        }

        if (_MultiManager == null)
        {
            return;
        }

        RefreshData(_MultiManager.GetDepthData(),
                    _MultiManager.ColorWidth,
                    _MultiManager.ColorHeight);

	}
    private void RefreshData(ushort[] depthData, int colorWidth, int colorHeight)
    {
        var frameDesc = _Sensor.DepthFrameSource.FrameDescription;

        ColorSpacePoint[] colorSpace = new ColorSpacePoint[depthData.Length];
        _Mapper.MapDepthFrameToColorSpace(depthData, colorSpace);

        //設定存放depth的Mat大小
        _Depth = new Mat(frameDesc.Height, frameDesc.Width,CvType.CV_8UC1);

        for (int y = 0; y < frameDesc.Height; y += _DownsampleSize)
        {
            for (int x = 0; x < frameDesc.Width; x += _DownsampleSize)
            {
                int indexX = x / _DownsampleSize;
                int indexY = y / _DownsampleSize;
                int smallIndex = (indexY * (frameDesc.Width / _DownsampleSize)) + indexX;

                double avg = GetAvg(depthData, x, y, frameDesc.Width, frameDesc.Height);

                avg = avg * _DepthScale;

                _Vertices[smallIndex].z = (float)avg;
                _Depth.put(x, y, avg);
                // Update UV mapping with CDRP
                //var colorSpacePoint = colorSpace[(y * frameDesc.Width) + x];
               // _UV[smallIndex] = new Vector2(colorSpacePoint.X / colorWidth, colorSpacePoint.Y / colorHeight);
            }
        }
    }
    private double GetAvg(ushort[] depthData, int x, int y, int width, int height)
    {
        double sum = 0.0;

        for (int y1 = y; y1 < y + _DownsampleSize; y1++)
        {
            for (int x1 = x; x1 < x + _DownsampleSize; x1++)
            {
                int fullIndex = (y1 * width) + x1;
                sum += depthData[fullIndex];
            }
        }
        return sum / _DownsampleSize * _DownsampleSize;
    }
}
