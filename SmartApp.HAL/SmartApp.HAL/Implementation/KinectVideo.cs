﻿using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Extensions.Logging;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using SmartApp.HAL.Model;
using SmartApp.HAL.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SmartApp.HAL.Implementation
{
    internal class KinectVideo : IVideoSource
    {
        private KinectSensor _kinect = null;
        private readonly ILogger<KinectVideo> _logger;
        //face detection
        private FaceFrameSource[] _faceFrameSources;
        private FaceFrameReader[] _faceFrameReaders;
        private FaceFrameResult[] _faceFrameResults;
        //bodies detected (needed for face tracking)
        private Body[] _bodies;
        //reader for bodies and color camera
        private MultiSourceFrameReader _multiSourceFrameReader = null;

        public KinectVideo(ILogger<KinectVideo> logger)
        {
            _logger = logger;
            _logger.LogInformation("Kinect loaded.");

            _kinect = KinectSensor.GetDefault();
            //kinect availability callback
            _kinect.IsAvailableChanged += Sensor_IsAvailableChanged;
            //frame of color camera and bodies callback
            _multiSourceFrameReader = _kinect.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Body);
            
            //features needed of a face
            FaceFrameFeatures faceFrameFeatures = FaceFrameFeatures.BoundingBoxInColorSpace;
            //BodyCount == 6, we need arrays for detect up to 6 faces at time
            _faceFrameSources = new FaceFrameSource[_kinect.BodyFrameSource.BodyCount];
            _faceFrameReaders = new FaceFrameReader[_kinect.BodyFrameSource.BodyCount];
            _faceFrameResults = new FaceFrameResult[_kinect.BodyFrameSource.BodyCount];

            _bodies = new Body[_kinect.BodyFrameSource.BodyCount];

            for (int i = 0; i < _kinect.BodyFrameSource.BodyCount; i++)
            {
                _faceFrameSources[i] = new FaceFrameSource(_kinect, 0, faceFrameFeatures);
                _faceFrameReaders[i] = _faceFrameSources[i].OpenReader();
            }
            _kinect.Open();

            //DEBUG
            IsAvailable = true;
            Framerate = 10;
        }

        
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            IsAvailable = _kinect.IsAvailable;
            _logger.LogInformation("Kinect available = {0}", IsAvailable);
        }

        //Callback for a face arrived
        private void Face_FrameArrived(object sender, FaceFrameArrivedEventArgs e)
        {
            using (FaceFrame faceFrame = e.FrameReference.AcquireFrame())
            {
                if (faceFrame != null)
                {
                    //index of the source of this face
                    int index = GetFaceSourceIndex(faceFrame.FaceFrameSource);
                    if (ValidateFaceBoundingBox(faceFrame.FaceFrameResult))
                    {
                        //store result of the frame
                        _faceFrameResults[index] = faceFrame.FaceFrameResult;
                    }
                    else
                    {
                        _faceFrameResults[index] = null;
                    }
                }
            }
        }
        //Checks if a face has a balid bounding box
        private bool ValidateFaceBoundingBox(FaceFrameResult faceFrameResult)
        {
            bool isFaceValid = faceFrameResult != null;
            if (isFaceValid)
            {
                RectI boundingBox = faceFrameResult.FaceBoundingBoxInColorSpace;
                if (boundingBox != null)
                {
                    isFaceValid = (boundingBox.Right - boundingBox.Left) > 0 &&
                    (boundingBox.Bottom - boundingBox.Top) > 0 &&
                    boundingBox.Right <= _kinect.ColorFrameSource.FrameDescription.Width &&
                    boundingBox.Bottom <= _kinect.ColorFrameSource.FrameDescription.Height;
                }
            }
            return isFaceValid;
        }

        //Get the index of the sources given a face frame arrived
        private int GetFaceSourceIndex(FaceFrameSource faceFrameSource)
        {
            int index = -1;
            for (int i = 0; i < 6; i++)
            {
                if (_faceFrameSources[i] == faceFrameSource)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        //Callback for a frame of color camera or body arrived
        private void MultiSource_FrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            MultiSourceFrame multiSourceFrame = e.FrameReference.AcquireFrame();


            using (BodyFrame bodyFrame = multiSourceFrame.BodyFrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    //If it is a body frame
                    for (int i = 0; i < 6; i++)
                    {
                        bodyFrame.GetAndRefreshBodyData(_bodies);
                        if (!_faceFrameSources[i].IsTrackingIdValid)
                        {
                            if (_bodies[i].IsTracked)
                            {
                                _faceFrameSources[i].TrackingId = _bodies[i].TrackingId;
                            }
                        }
                    }
                }
            }

            using (ColorFrame colorFrame = multiSourceFrame.ColorFrameReference.AcquireFrame()) { 
                    if (colorFrame != null)
                {
                    FrameDescription colorFrameDescription = _kinect.ColorFrameSource.FrameDescription;
                    uint bytesPerpixel = 4;//colorFrameDescription.BytesPerPixel;
                    byte[] allBytes = new byte[(uint)(colorFrameDescription.Width * colorFrameDescription.Height * bytesPerpixel)];
                    colorFrame.CopyConvertedFrameDataToArray(allBytes, ColorImageFormat.Bgra);

                    Mat frame = new Mat(colorFrameDescription.Height, colorFrameDescription.Width, Emgu.CV.CvEnum.DepthType.Cv8U, (int)bytesPerpixel);
                    colorFrame.CopyConvertedFrameDataToIntPtr(frame.DataPointer, (uint)(colorFrameDescription.Width * colorFrameDescription.Height * bytesPerpixel), ColorImageFormat.Bgra);

                    List<Rectangle> rectangles = new List<Rectangle>();
                    bool faceFound = false;
                    for (int f = 0; f < 6; f++)
                    {
                        if (_faceFrameResults[f] != null)
                        {
                            faceFound = true;
                            RectI boundingBox = _faceFrameResults[f].FaceBoundingBoxInColorSpace;
                            int xCoord = boundingBox.Left;
                            int yCoord = boundingBox.Top;
                            int width = boundingBox.Right - boundingBox.Left;
                            int height = boundingBox.Bottom - boundingBox.Top;

                            rectangles.Add(new Rectangle(xCoord, yCoord, width, height));

                            //bytes del rettangolo con la faccia
                            /*
                            byte[] bytes = new byte[width * height * bytesPerpixel];

                            for (int i = 0; i < height; i++)
                            {
                                for (int j = 0; j < width * bytesPerpixel; j++)
                                {
                                    bytes[i * (width * bytesPerpixel) + j] = allBytes[(i + yCoord) * (colorFrameDescription.Width * bytesPerpixel) + (j + xCoord * bytesPerpixel)];
                                }
                            }
                            */
                        }
                    }
                    if (faceFound)
                    {
                        _logger.LogTrace("Kinect: Got frame. Found {0} faces.", rectangles.Count);

                        FrameReady?.Invoke(this, new VideoFrame(
                        DateTime.Now,
                        rectangles.Select(bounds => new VideoFrame.Face(bounds)).ToList(),
                        frame.ToImage<Bgr, byte>()
                        ));
                    }
                }
            }

        }

        public int Framerate { get ; set; }

        public bool IsAvailable { get; private set; }

        public event EventHandler<VideoFrame> FrameReady;

        public void Dispose()
        {
            if (_kinect != null)
            {
                _kinect.Close();
                _kinect = null;
            }

            if (_multiSourceFrameReader != null)
            {
                _multiSourceFrameReader.Dispose();
                _multiSourceFrameReader = null;
            }

            if (_faceFrameSources != null)
            {
                for (int i = 0; i < _faceFrameSources.Length; i++)
                {
                    if (_faceFrameSources[i] != null) _faceFrameSources[i].Dispose();
                }
            }
            if (_faceFrameReaders != null)
            {
                for (int i = 0; i < _faceFrameReaders.Length; i++)
                {
                    if (_faceFrameReaders[i] != null) _faceFrameReaders[i].Dispose();
                }
            }
        }

        public void Start()
        {
            if (_kinect != null && IsAvailable)
            {
                _multiSourceFrameReader.MultiSourceFrameArrived += MultiSource_FrameArrived;
                for (int i = 0; i < 6; i++)
                {
                    _faceFrameReaders[i].FrameArrived += Face_FrameArrived;
                }
                    _logger.LogInformation("Kinect started.");
                
                
            }
        }

        public void Stop()
        {

            _multiSourceFrameReader.MultiSourceFrameArrived -= MultiSource_FrameArrived;
            for (int i = 0; i < 6; i++)
            {
                _faceFrameReaders[i].FrameArrived -= Face_FrameArrived;
            }
            _logger.LogInformation("Kinect stopped.");


        }
    }
}
