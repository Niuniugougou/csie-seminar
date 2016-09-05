﻿using System;
using UnityEngine;
using System.Collections;
using OpenCVForUnity;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading;

public class Match : MonoBehaviour {
    /*color match*/
    private Scalar _blockColor = new Scalar(255, 0, 0);
    private Scalar _backgroundColor = new Scalar(0, 255, 0);
    private Texture2D _matchTexture;
    private int _matchWidth;
    private int _matchHeight;
    private Mat hsvMat;
    private Mat thresholdMat;
    //colorObject
    ColorObject blue = new ColorObject("blue");
    ColorObject yellow = new ColorObject("yellow");
    ColorObject red = new ColorObject("red");
    ColorObject green = new ColorObject("green");
    /*kinect color */
    private int _colorWidth;
    private int _colorHeight;
    /*drawBlock*/
    public DrawBlock _drawBlock;
    private Mat blockMat;
    /*public data*/
    public int Width { get; private set; }
    public int Height { get; private set; }
    //public List<Point> MatchObjectPoint { get; private set; }
    DiceRecognition _dice = new DiceRecognition();

    //物體資訊
    public List<BaseObject> SensingResults = new List<BaseObject>();
    private int _clolrRange = 15;
    //是否可以儲存感測到的物件
    private bool isSave = new bool();


    public Mat src;
    public OpenCVForUnity.Rect DepthRect;
    public Mat Temp;

    public Texture2D GetMatchTexture()
    {
        return _matchTexture;
    }
    // Use this for initialization
    void Start()
    {
        _matchWidth = 800;
        _matchHeight = 450;

        _matchTexture = new Texture2D(_matchWidth, _matchHeight);

        isSave = false;
    }
	// Update is called once per frame
	void Update () {
        //確認已開啟攝影機
        if (_drawBlock.MatchHeight == 0 && _drawBlock.MatchWidth == 0) return;
        // ==========================
        // set public Width Height ==
        // ==========================
        Width = _drawBlock.MatchWidth;
        Height = _drawBlock.MatchHeight;
        //設定是否儲存特徵物體
        SetIsSave();
        //宣告存放深度與色彩影像
        Mat _ClolrMat = new Mat(Height, Width, CvType.CV_8UC3);
        Mat _DepthMat = new Mat(Height, Width, CvType.CV_8UC1);
        //取得影像資訊
        _ClolrMat = _drawBlock.GetBlockMat();
        _DepthMat = _drawBlock.GetBlockDepthMat();

        //宣告結果影像
        Mat BlackMat = new Mat(Height, Width, CvType.CV_8UC3);
        Mat BlackDepthMat = new Mat(Height, Width, CvType.CV_8UC1);

        getDepthContours(_DepthMat, BlackMat);
         //getContours(_DepthMat, BlackMat);


         //方法三 用特徵點抓物件
         //descriptorsORB(_NewTowMat, BlackMat, "queen");
         //descriptorsORB(BlackMat, BlackMat, "lena");


         //將結果影像轉換影像格式與大小設定
         Mat resizeMat = new Mat(_matchHeight, _matchWidth, CvType.CV_8UC3);
        Imgproc.resize(BlackMat, resizeMat, resizeMat.size());
        Utils.matToTexture2D(resizeMat, _matchTexture);
        _matchTexture.Apply();

        BlackMat.release();
        BlackDepthMat.release();
        BlackMat.release();
    }
    //深度影像處理
   public  bool getDepthContours(Mat _DepthMat,Mat BlackMat)
    {
        
        if (_DepthMat == null)
        {
            Debug.Log("_DepthMat Mat is Null");
            return false;
        }
        //設定Canny參數
        int threshold = 50;
        //宣告存放偵測結果資料
        Mat hierarchy = new Mat();
        List<MatOfPoint> contours = new List<MatOfPoint>();
        Mat cannyMat = new Mat();
        //做Canny輪廓化
        Imgproc.Canny(_DepthMat, cannyMat, threshold, threshold * 3);
        //找輪廓並編號
        Imgproc.findContours(cannyMat, contours, hierarchy, Imgproc.RETR_LIST, Imgproc.CHAIN_APPROX_SIMPLE);
        //取得輪廓數量
        int numObjects = contours.Count;
        //跑迴圈確認輪廓
        if (numObjects > 0)
        {
            Debug.Log("numObjects = " + numObjects);
            for (int index = 0; index < numObjects; index++)
            {
                //取得深度結果物體位置
                DepthRect = Imgproc.boundingRect(contours[index]);
                if (DepthRect.height > 40 && DepthRect.width > 40)
                {
                    //畫出輪廓結果
                    Imgproc.drawContours(BlackMat, contours, index, new Scalar(255, 255, 255), 5);
                    Point midPoint = new Point(DepthRect.x + (DepthRect.width / 2), DepthRect.y + (DepthRect.height / 2));
                    //Imgproc.line(BlackMat, midPoint, midPoint, new Scalar(255), 10);
                    Imgproc.putText(BlackMat, "O", midPoint, 1, 1, new Scalar(255, 0, 0), 20);

                }
            }
        }
        cannyMat.release();
        hierarchy.release();
        return true;
    }
    //找出特徵的顏色方法二
    public void getContours(Mat RGB, Mat cameraFeed)
    {
        Point cof_center = new Point(cameraFeed.cols() / 2.0, cameraFeed.rows() / 2.0);
        Mat cof_mat = Imgproc.getRotationMatrix2D(cof_center, 180, 1.0);
        Imgproc.warpAffine(cameraFeed, cameraFeed, cof_mat, cameraFeed.size());

        src = new Mat();
        RGB.copyTo(src);

        List<ColorObject> colorObjects = new List<ColorObject>();
        Temp = new Mat(RGB.height(),RGB.width(), CvType.CV_8UC3);
       // threshold.copyTo(temp);
        Mat hierarchy = new Mat();
        List<Point> ConsistP = new List<Point>();
        List<MatOfPoint> contours = new List<MatOfPoint>();

        Imgproc.blur(src, src, new Size(3, 3));
        Imgproc.Canny(src, Temp, 50, 150);
        morphOps(Temp);

        Imgproc.findContours(Temp, contours, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_NONE);
        
        int numObjects = contours.Count;
        List<Scalar> clickRGB = new List<Scalar>();
        for (int i = 0; i < numObjects; i++)
        {
            Imgproc.drawContours(Temp, contours, i, new Scalar(255, 255, 255),1);
        }
        double[] GetRGB = new double[10];
        if (numObjects > 0)
        {
            for (int index = 0; index < numObjects; index++)
            {

                OpenCVForUnity.Rect R0 = Imgproc.boundingRect(contours[index]);

                if (R0.height > 20 && R0.width > 20 && R0.height <_drawBlock.MatchHeight-10 && R0.width < _drawBlock.MatchWidth-10)
                {
                    ConsistP.Add(new Point(R0.x, R0.y));
                    ConsistP.Add(new Point(R0.x + R0.width, R0.y + R0.height));
                    clickRGB.Add(clickcolor(src, R0));
                }
            }

            for (int i = 0; i < ConsistP.Count; i += 2)
            {
                int ID = inRange(ConsistP[i], ConsistP[i + 1], clickRGB[i / 2]);
                if (ID != -1)
                {
                    Imgproc.rectangle(Temp, ConsistP[i], ConsistP[i + 1], new Scalar(255, 0, 255), 1);
                    Imgproc.putText(Temp, "ID=" + ID.ToString(), ConsistP[i], 1, 1, new Scalar(255, 0, 255), 1);
                }
            }
            // =================================
            // set public MatchObjectPoint =====
            // =================================
            //MatchObjectPoint = ConsistP;

            //ConsistP.Clear();
        }
        Temp.copyTo(cameraFeed);
        Imgproc.warpAffine(cameraFeed, cameraFeed, cof_mat, cameraFeed.size());
    }
    //侵蝕膨脹消除雜訊
    public void morphOps(Mat thresh)
    {
        //創造兩個矩陣做 侵蝕、膨脹(erode、dilate)
        //the element chosen here is a 3px by 3px rectangle
        Mat erodeElement = Imgproc.getStructuringElement(Imgproc.MORPH_RECT, new Size(3, 3));
        //dilate with larger element so make sure object is nicely visible
        Mat dilateElement = Imgproc.getStructuringElement(Imgproc.MORPH_RECT, new Size(3, 3));

        Imgproc.erode(thresh, thresh, erodeElement);
        Imgproc.erode(thresh, thresh, erodeElement);

        Imgproc.dilate(thresh, thresh, dilateElement);
        Imgproc.dilate(thresh, thresh, dilateElement);

    }
    //取得平均色彩
    public  Scalar clickcolor(Mat src, OpenCVForUnity.Rect R)
    {
        double average_R = 0, average_G = 0, average_B = 0;
        double[] _getrgb_Mid = src.get((int)R.y + R.height / 2, (int)R.x + R.width / 2);
        double[] _getrgb_Lift = src.get((int)R.y + R.height / 2, (int)R.x + R.width / 4);
        double[] _getrgb_Right = src.get((int)R.y + R.height / 2, (int)R.x + R.width / 4 * 3);
        double[] _getrgb_Top = src.get((int)R.y + R.height / 4, (int)R.x + R.width / 2);
        double[] _getrgb_Bot = src.get((int)R.y + R.height / 4 * 3, (int)R.x + R.width / 2);

        average_R = (_getrgb_Mid[0] + _getrgb_Lift[0] + _getrgb_Right[0] + _getrgb_Top[0] + _getrgb_Bot[0])/5;
        average_G = (_getrgb_Mid[1] + _getrgb_Lift[1] + _getrgb_Right[1] + _getrgb_Top[1] + _getrgb_Bot[1])/5;
        average_B = (_getrgb_Mid[2] + _getrgb_Lift[2] + _getrgb_Right[2] + _getrgb_Top[2] + _getrgb_Bot[2])/5;
        
        return new Scalar((int)average_R, (int)average_G, (int)average_B);
    }
    //當偵測到的物體平均色彩符合則比對成功
    public  int inRange(Point P1 , Point P2, Scalar src)
    {
        double[] _srcColor =src.val;
        for (int i = 0; i < SensingResults.Count; i++)
        {
            double[] _getrgb = SensingResults[i].getColor().val;
           if (_srcColor[0] < _getrgb[0] + _clolrRange &&
               _srcColor[0] > _getrgb[0] - _clolrRange &&
               _srcColor[1] < _getrgb[1] + _clolrRange &&
               _srcColor[1] > _getrgb[1] - _clolrRange &&
               _srcColor[2] < _getrgb[2] + _clolrRange &&
               _srcColor[2] > _getrgb[2] - _clolrRange)
           {
                SensingResults[i].SetPoint(P1, P2);
               return i;
           }
        }
       //判斷使否開啟特徵存檔
        if (isSave)
        {
            Debug.Log("Create" + SensingResults.Count);
            SensingResults.Add(new BaseObject(P1, P2, src));
            return SensingResults.Count;
        }
        else return -1;
    }
    //設定是否開啟儲存特徵物體的bool(預設快捷鍵為U)
    public void SetIsSave()
    {
        if (Input.GetKeyUp(KeyCode.U))
        {
            isSave = (isSave) ? false : true;
            Debug.Log((isSave) ? "isSave Set True" : "isSave Set false");
        }
    }
    //找出特徵的顏色方法三(ORB特徵點比對)
    public bool descriptorsORB(Mat RGB, Mat cameraFeed,string targetName)
    {
        if (RGB == null)
        {
            Debug.Log("RGB Mat is Null");
            return false;
        }
        //將傳入的RGB存入Src
        Mat SrcMat = new Mat();
        RGB.copyTo(SrcMat);
        //比對樣本載入
        Texture2D imgTexture = Resources.Load(targetName) as Texture2D;
        
        //Texture2D轉Mat
        Mat targetMat = new Mat(imgTexture.height, imgTexture.width, CvType.CV_8UC3);
        Utils.texture2DToMat(imgTexture, targetMat);

        //創建 ORB的特徵點裝置
        FeatureDetector detector = FeatureDetector.create(FeatureDetector.ORB);
        DescriptorExtractor extractor = DescriptorExtractor.create(DescriptorExtractor.ORB);

        //產生存放特徵點Mat
        MatOfKeyPoint keypointsTarget = new MatOfKeyPoint();
        Mat descriptorsTarget = new Mat();
        MatOfKeyPoint keypointsSrc = new MatOfKeyPoint();
        Mat descriptorsSrc = new Mat();

        //找特徵點圖Target
        detector.detect(targetMat, keypointsTarget);
        extractor.compute(targetMat, keypointsTarget, descriptorsTarget);

        //找特徵點圖Src
        detector.detect(SrcMat, keypointsSrc);
        extractor.compute(SrcMat, keypointsSrc, descriptorsSrc);

        //創建特徵點比對物件
        DescriptorMatcher matcher = DescriptorMatcher.create(DescriptorMatcher.BRUTEFORCE_HAMMINGLUT);
        MatOfDMatch matches = new MatOfDMatch();
        //丟入兩影像的特徵點
        matcher.match(descriptorsTarget, descriptorsSrc, matches);
        DMatch[] arrayDmatch = matches.toArray();

        //做篩選
        double max_dist = 0;
        double min_dist = 100;
        //-- Quick calculation of max and min distances between keypoints
        double dist = new double();
        for (int i = 0; i < matches.rows(); i++)
        {
            dist = arrayDmatch[i].distance;
            if (dist < min_dist) min_dist = dist;
            if (dist > max_dist) max_dist = dist;
        }
        Debug.Log("Max dist :" + max_dist);
        Debug.Log("Min dist :" + min_dist);

        List<DMatch> matchesGoodList = new List<DMatch>();
        
        MatOfDMatch matchesGood = new MatOfDMatch();
        matchesGood.fromList(matchesGoodList);
        
        //Draw Keypoints
        Features2d.drawKeypoints(SrcMat, keypointsSrc, SrcMat);

        List<Point> pTarget = new List<Point>();
        List<Point> pSrc = new List<Point>();

        Debug.Log("MatchCount"+matchesGoodList.Count);
        for (int i = 0; i < matchesGoodList.Count; i++)
        {
            pTarget.Add(new Point(keypointsTarget.toArray()[matchesGoodList[i].queryIdx].pt.x, keypointsTarget.toArray()[matchesGoodList[i].queryIdx].pt.y));
            pSrc.Add(new Point(keypointsSrc.toArray()[matchesGoodList[i].trainIdx].pt.x, keypointsSrc.toArray()[matchesGoodList[i].trainIdx].pt.y));
        }

        MatOfPoint2f p2fTarget = new MatOfPoint2f(pTarget.ToArray());
        MatOfPoint2f p2fSrc = new MatOfPoint2f(pSrc.ToArray());

        Mat matrixH = Calib3d.findHomography(p2fTarget, p2fSrc, Calib3d.RANSAC, 3);

        List<Point> srcPointCorners = new List<Point>();
        srcPointCorners.Add(new Point(0, 0));
        srcPointCorners.Add(new Point(targetMat.width(), 0));
        srcPointCorners.Add(new Point(targetMat.width(), targetMat.height()));
        srcPointCorners.Add(new Point(0, targetMat.height()));
        Mat originalRect = Converters.vector_Point2f_to_Mat(srcPointCorners);

        List<Point> srcPointCornersEnd = new List<Point>();
        srcPointCornersEnd.Add(new Point(0, targetMat.height()));
        srcPointCornersEnd.Add(new Point(0, 0));
        srcPointCornersEnd.Add(new Point(targetMat.width(), 0));
        srcPointCornersEnd.Add(new Point(targetMat.width(), targetMat.height()));
        Mat changeRect = Converters.vector_Point2f_to_Mat(srcPointCornersEnd);

        Core.perspectiveTransform(originalRect, changeRect, matrixH);
        List<Point> srcPointCornersSave = new List<Point>();

        Converters.Mat_to_vector_Point(changeRect, srcPointCornersSave);

        if ((srcPointCornersSave[2].x - srcPointCornersSave[0].x) < 5 || (srcPointCornersSave[2].y - srcPointCornersSave[0].y) < 5)
        {
            Debug.Log("Match Out Put image is to small");
            SrcMat.copyTo(cameraFeed);
            SrcMat.release();
            Imgproc.putText(cameraFeed,targetName, srcPointCornersSave[0], 0, 1, new Scalar(255, 255, 255), 2);
            return false;
        }
        //畫出框框
        Imgproc.line(SrcMat, srcPointCornersSave[0], srcPointCornersSave[1], new Scalar(255, 0, 0), 3);
        Imgproc.line(SrcMat, srcPointCornersSave[1], srcPointCornersSave[2], new Scalar(255, 0, 0), 3);
        Imgproc.line(SrcMat, srcPointCornersSave[2], srcPointCornersSave[3], new Scalar(255, 0, 0), 3);
        Imgproc.line(SrcMat, srcPointCornersSave[3], srcPointCornersSave[0], new Scalar(255, 0, 0), 3);
        //畫中心
        Point middlePoint = new Point((srcPointCornersSave[0].x + srcPointCornersSave[2].x) / 2, (srcPointCornersSave[0].y + srcPointCornersSave[2].y) / 2);
        Imgproc.line(SrcMat,  middlePoint, middlePoint, new Scalar(0, 0, 255), 10);


        SrcMat.copyTo(cameraFeed);
        keypointsTarget.release();
        targetMat.release();
        SrcMat.release();
        return true;
    }
    public bool descriptorsORB_Old(Mat RGB, Mat cameraFeed, string targetName)//找出特徵的顏色方法三(可運行但效率不佳放棄)
    {
        if (RGB == null)
        {
            Debug.Log("RGB Mat is Null");
            return false;
        }
        //將傳入的RGB存入Src
        Mat SrcMat = new Mat();

        RGB.copyTo(SrcMat);
        //比對樣本
        Texture2D imgTexture = Resources.Load(targetName) as Texture2D;
        //  Texture2D imgTexture2 = Resources.Load("lenaK") as Texture2D;

        //Texture2D轉Mat
        Mat img1Mat = new Mat(imgTexture.height, imgTexture.width, CvType.CV_8UC3);
        Utils.texture2DToMat(imgTexture, img1Mat);

        //創建 ORB的特徵點裝置
        FeatureDetector detector = FeatureDetector.create(FeatureDetector.ORB);
        DescriptorExtractor extractor = DescriptorExtractor.create(DescriptorExtractor.ORB);
        //產生存放特徵點Mat
        MatOfKeyPoint keypoints1 = new MatOfKeyPoint();
        Mat descriptors1 = new Mat();
        MatOfKeyPoint keypointsSrc = new MatOfKeyPoint();
        Mat descriptorsSrc = new Mat();
        //找特徵點圖1
        detector.detect(img1Mat, keypoints1);
        extractor.compute(img1Mat, keypoints1, descriptors1);
        //找特徵點圖Src
        detector.detect(SrcMat, keypointsSrc);
        extractor.compute(SrcMat, keypointsSrc, descriptorsSrc);

        DescriptorMatcher matcher = DescriptorMatcher.create(DescriptorMatcher.BRUTEFORCE_HAMMINGLUT);
        MatOfDMatch matches = new MatOfDMatch();
        matcher.match(descriptors1, descriptorsSrc, matches);
        DMatch[] arrayDmatch = matches.toArray();

        for (int i = arrayDmatch.Length - 1; i >= 0; i--)
        {
            //   Debug.Log("match " + i + ": " + arrayDmatch[i].distance);
        }
        //做篩選
        double max_dist = 0;
        double min_dist = 100;
        //-- Quick calculation of max and min distances between keypoints
        double dist = new double();
        for (int i = 0; i < matches.rows(); i++)
        {
            dist = arrayDmatch[i].distance;
            if (dist < min_dist) min_dist = dist;
            if (dist > max_dist) max_dist = dist;
        }
        Debug.Log("Max dist :" + max_dist);
        Debug.Log("Min dist :" + min_dist);
        //只畫好的點

        List<DMatch> matchesGoodList = new List<DMatch>();

        for (int i = 0; i < matches.rows(); i++)
        {
            //if (arrayDmatch[i].distance < RateDist.value * min_dist)
            //{
            //    //Debug.Log("match " + i + ": " + arrayDmatch[i].distance);
            //    matchesGoodList.Add(arrayDmatch[i]);
            //}
        }
        MatOfDMatch matchesGood = new MatOfDMatch();
        matchesGood.fromList(matchesGoodList);

        //Draw Keypoints
        Features2d.drawKeypoints(SrcMat, keypointsSrc, SrcMat);

        //做輸出的轉換予宣告

        Mat resultImg = new Mat();
        // Features2d.drawMatches(img1Mat, keypoints1, SrcMat, keypointsSrc, matchesGood, resultImg);

        List<Point> P1 = new List<Point>();
        // List<Point> P2 = new List<Point>();
        List<Point> pSrc = new List<Point>();

        Debug.Log("MatchCount" + matchesGoodList.Count);
        for (int i = 0; i < matchesGoodList.Count; i++)
        {
            P1.Add(new Point(keypoints1.toArray()[matchesGoodList[i].queryIdx].pt.x, keypoints1.toArray()[matchesGoodList[i].queryIdx].pt.y));
            pSrc.Add(new Point(keypointsSrc.toArray()[matchesGoodList[i].trainIdx].pt.x, keypointsSrc.toArray()[matchesGoodList[i].trainIdx].pt.y));
            //Debug.Log("ID = " + matchesGoodList[i].queryIdx );
            //Debug.Log("x,y =" + (int)keypoints1.toArray()[matchesGoodList[i].queryIdx].pt.x + "," + (int)keypoints1.toArray()[matchesGoodList[i].queryIdx].pt.y);
            //Debug.Log("x,y =" + (int)keypoints2.toArray()[matchesGoodList[i].trainIdx].pt.x + "," + (int)keypoints2.toArray()[matchesGoodList[i].trainIdx].pt.y);
        }

        MatOfPoint2f p2fTarget = new MatOfPoint2f(P1.ToArray());
        MatOfPoint2f p2fSrc = new MatOfPoint2f(pSrc.ToArray());

        Mat matrixH = Calib3d.findHomography(p2fTarget, p2fSrc, Calib3d.RANSAC, 3);
        List<Point> srcPointCorners = new List<Point>();
        srcPointCorners.Add(new Point(0, 0));
        srcPointCorners.Add(new Point(img1Mat.width(), 0));
        srcPointCorners.Add(new Point(img1Mat.width(), img1Mat.height()));
        srcPointCorners.Add(new Point(0, img1Mat.height()));

        Mat originalRect = Converters.vector_Point2f_to_Mat(srcPointCorners);
        List<Point> srcPointCornersEnd = new List<Point>();
        srcPointCornersEnd.Add(new Point(0, img1Mat.height()));
        srcPointCornersEnd.Add(new Point(0, 0));
        srcPointCornersEnd.Add(new Point(img1Mat.width(), 0));
        srcPointCornersEnd.Add(new Point(img1Mat.width(), img1Mat.height()));

        Mat changeRect = Converters.vector_Point2f_to_Mat(srcPointCornersEnd);

        Core.perspectiveTransform(originalRect, changeRect, matrixH);
        List<Point> srcPointCornersSave = new List<Point>();

        Converters.Mat_to_vector_Point(changeRect, srcPointCornersSave);

        if ((srcPointCornersSave[2].x - srcPointCornersSave[0].x) < 5 || (srcPointCornersSave[2].y - srcPointCornersSave[0].y) < 5)
        {
            Debug.Log("Match Out Put image is to small");
            SrcMat.copyTo(cameraFeed);
            SrcMat.release();
            Imgproc.putText(cameraFeed, "X-S", new Point(10, 50), 0, 1, new Scalar(255, 255, 255), 2);
            return false;
        }
        //    Features2d.drawMatches(img1Mat, keypoints1, SrcMat, keypointsSrc, matchesGood, resultImg);
        Imgproc.line(SrcMat, srcPointCornersSave[0], srcPointCornersSave[1], new Scalar(255, 0, 0), 3);
        Imgproc.line(SrcMat, srcPointCornersSave[1], srcPointCornersSave[2], new Scalar(255, 0, 0), 3);
        Imgproc.line(SrcMat, srcPointCornersSave[2], srcPointCornersSave[3], new Scalar(255, 0, 0), 3);
        Imgproc.line(SrcMat, srcPointCornersSave[3], srcPointCornersSave[0], new Scalar(255, 0, 0), 3);

        SrcMat.copyTo(cameraFeed);
        keypoints1.release();
        img1Mat.release();
        SrcMat.release();
        return true;
    }
}
