﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.InteropServices;
using System.IO;
using PowerfulCamera.ItemBase;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace PowerfulCamera.CameraTools
{
    public interface IDelayTime
    {
        void DelayMs(int ms);
    }

    public class Node<T>
    {
        T data;
        Node<T> node;
        public Node(T _data, Node<T> _node)
        {
            this.data = _data;
            this.node = _node;
        }
        public Node(T _data) : this(_data, null) { }
    }
    public class Camera : ViewModelBase
    {
        #region ##成员
        public HTuple CameraHandle;     //相机操作句柄
        public HObject CurrentImage;    //当前图片
        public bool IsOpen;             //是否打开了相机,运行态
        public bool IsConnect;          //是否连接了相机  
        private HWindow currentHWindow;
        #endregion

        #region 属性
        private int _Index;
        public int Index
        {
            set => _Index = value;
            get => _Index;
        }
        private string _Name;
        public string Name
        {
            set => _Name = value;
            get => _Name;
        }
        public HWindow CurrentHWindow
        {
            set => currentHWindow = value;
            get => currentHWindow;
        }
        private string _Device;
        public string Device
        {
            set { _Device = value; RaisePropertyChanged(); }
            get => _Device;
        }
        private double _CameraFPS;
        public double CameraFPS
        {
            set { _CameraFPS = value; RaisePropertyChanged(); }
            get => _CameraFPS;
        }
        private string _ColorSpace;
        public string ColorSpace
        {
            set => _ColorSpace = value;
            get => _ColorSpace;
        }
        private int _ExposureTimeRaw;
        public int ExposureTimeRaw //曝光时间
        {
            set => _ExposureTimeRaw = value;
            get => _ExposureTimeRaw;
        }
        private int _GainRaw;
        public int GainRaw          //增益
        {
            set => _GainRaw = value;
            get => _GainRaw;
        }
        public bool CameraSwitchIsOn
        {
            set
            {
                if(value)
                {
                    Play();
                }
                else
                {
                    Stop();
                }
                RaisePropertyChanged();
            }
            get
            {
                return IsOpen;
            }
        }
        #endregion

        #region ##方法区域
        /// <summary>
        /// 摄像机无参构造方法
        /// </summary>
        public Camera() : this(0, "default", "default", "Gray") { }

        /// <summary>
        /// 摄像机构造方法
        /// </summary>
        /// <param name="index">索引,无特别要求</param>
        /// <param name="name">名称,例:ChinaVision17_X64</param>
        /// <param name="device">设备,例:DHS-500VC</param>
        /// <param name="colorSpace">色彩,例:Gray</param>
        /// <param name="exposureTimeRaw">曝光时间</param>
        /// <param name="gainRaw">增益</param>
        public Camera(int index, string name, string device, string colorSpace)
        {
            Index = index;
            Name = name;
            Device = device;
            ColorSpace = colorSpace;
        }

        /// <summary>
        /// 摄像机设置参数
        /// </summary>
        public void SetParam(string paramname, int value)
        {
            if(CameraHandle != null)
            {
                HOperatorSet.SetFramegrabberParam(CameraHandle, paramname, value);
            }
        }

        /// <summary>
        /// 摄像机连接
        /// </summary>
        public bool Connect()
        {
            if (!IsConnect && !IsOpen)
            {
                try
                {
                    HOperatorSet.OpenFramegrabber(Name, 1, 1, 0, 0, 0, 0, "progressive", -1, ColorSpace, -1, "false", "default", Device, 0, -1, out CameraHandle);
                    HOperatorSet.GrabImageStart(CameraHandle, -1);
                    if (Name == "ChinaVision17_X64")
                    {
                        //SetParam("", ExposureTimeRaw); //曝光:1 -- 905.1
                        SetParam("gain", GainRaw); //增益:1.25 -- 7.88
                    }
                    else if(Name == "GigEVision2")
                    {
                        SetParam("ExposureTimeRaw", ExposureTimeRaw);
                        SetParam("GainRaw", GainRaw);
                    }
                    IsOpen = true;
                    IsConnect = true;
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    MessageBox.Show("Error:摄像机连接失败" + Name + " 错误代码:" + ex.ToString(), "摄像机读取错误",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 摄像机继续采集,通常结合Stop使用
        /// </summary>
        public bool Play()
        {
            if(IsConnect && !IsOpen)
            {
                IsOpen = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 摄像机暂停采集,通常结合Play使用
        /// </summary>
        public bool Stop()
        {
            if(IsOpen && IsConnect)
            {
                IsOpen = false;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 摄像机关闭
        /// </summary>
        public bool Close()
        {
            if(CameraHandle != null)
            {
                try
                {
                    HOperatorSet.CloseFramegrabber(CameraHandle);
                    IsOpen = false;
                    IsConnect = false;
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("摄像机关闭失败:" + ex);
                }
            }
            return false;
        }

        /// <summary>
        /// 摄像机采集图形
        /// </summary>
        public async void GrabImageAsync()
        {
            await Task.Run(() =>
            {
                HObject ho_Image = null;
                HOperatorSet.GenEmptyObj(out ho_Image);
                DateTime dtStart;
                DateTime dtEnd;
                TimeSpan ts;
                double TS;
                if (CameraHandle != null)
                {
                    try
                    {
                        while (true)
                        {
                            while (IsOpen && IsConnect)
                            {
                                try
                                {
                                    if (CurrentHWindow != null)
                                    {
                                        dtStart = DateTime.Now;
                                        HOperatorSet.GrabImageAsync(out ho_Image, CameraHandle, -1);
                                        HOperatorSet.CopyImage(ho_Image, out CurrentImage);     //拷贝保存当前图像
                                                                                                //HalconWindowEX.DispImageFit(ho_Image);                                
                                        if (CurrentHWindow.DispImageFit(ho_Image))
                                        {
                                            dtEnd = DateTime.Now;
                                            ts = dtEnd.Subtract(dtStart);
                                            TS = ts.TotalMilliseconds;
                                            CameraFPS = Math.Round(1000 / TS, 2);   //计算FPS
                                        }
                                    }
                                    else
                                    {
                                        CameraFPS = 0;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                }
                                ho_Image.Dispose();
                                if (CurrentImage != null)
                                {
                                    unchecked
                                    {
                                        CurrentImage.Dispose();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("采集图像失败" + ex);
                    }
                }
            });
        }

        public static bool operator +(Camera camera1, Camera camera2)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 摄像机序列化
        /// </summary>
        public void WriteConfig()
        {
            string fileName = Environment.CurrentDirectory + "\\CameraConfig\\" + "Camera"+Index+".txt";
            string jsonString;
            try
            {
                if(this != null)
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                    };
                    jsonString = JsonSerializer.Serialize(this, options);
                    File.WriteAllText(fileName, jsonString);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// 摄像机反序列化
        /// </summary>
        /// <param name="index">索引</param>       
        public static Camera ReadConfig(string name)
        {
            Camera camera = null;
            string fileName = Environment.CurrentDirectory + "\\CameraConfig\\" + name;
            string jsonString;
            try
            {
                jsonString = File.ReadAllText(fileName);
                camera = JsonSerializer.Deserialize<Camera>(jsonString);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show("Error:无法读取摄像机"+name+" 错误代码:" + ex.ToString(), "摄像机读取错误", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return camera;
        }
        #endregion

    }






    public sealed class Cameras : IDelayTime
    {
        public static Dictionary<int, Camera> CameraWorkList = new Dictionary<int, Camera>();
        #region ##方法区域
        //[DllImport("hAcqChinaVision17_X64.dll")]
        //private extern static void 
        /*找相机,返回字典*/           
        public static bool FindCameras(out Dictionary<int, Camera> keyValues)
        {
            int index = 0;
            keyValues = new Dictionary<int, Camera>();
            FileInfo[] files = new DirectoryInfo(GlobalBase.CameraDir).GetFiles();                       //扫描文件 GetFiles("*.txt");可以实现扫描扫描txt文件 
            foreach (FileInfo fi in files)
            {                
                Console.WriteLine("读取到文件:"+fi.Name);
                Camera camera = Camera.ReadConfig(fi.Name);
                if(!keyValues.ContainsKey(camera.Index))
                {
                    keyValues.Add(camera.Index, camera);
                }
                else
                {
                    MessageBox.Show("Error:相机编号冲突:"+camera.Index, "错误",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                index++;
            }
            return true;
        }

        /*增加相机*/
        public static bool AddCameras(Camera camera)
        {
            return false;
        }

        /*移除相机*/
        public static bool RemoveCameras(int index)
        {
            return false;
        }

        public void DelayMs(int ms)
        {
            throw new NotImplementedException();
        }
        #endregion
    }


}
