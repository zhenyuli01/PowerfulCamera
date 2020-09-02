using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HalconDotNet;
using PowerfulCamera.CameraTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Camera = PowerfulCamera.CameraTools.Camera; //指定来源

namespace PowerfulCamera.CameraSetting
{
    public class CameraSettingPageViewModel : ViewModelBase
    {
        #region ##成员
        private int WorkNum = 0;
        private string currentItemName;
        private HWindow currentHWindow;
        public List<HWindow> MainWindowHWindowList = new List<HWindow>();
        public List<Camera> CameraWorkList = Cameras.CameraWorkList;
        private bool mergeIsChecked;            //合并
        private bool intersectionIsChecked;     //交集
        private bool subtractionIsChecked;      //差集
        private string leftButtonVisibility;
        private string rightButtonVisibility;
        public int CurrentDisplayCameraID = 0;
        #endregion


        #region ##属性
        public bool MergeIsChecked
        {
            set
            {
                mergeIsChecked = value;
                RaisePropertyChanged();
            }
            get => mergeIsChecked;
        }
        public string LeftButtonVisibility
        {
            set { leftButtonVisibility = value;RaisePropertyChanged(); }
            get => leftButtonVisibility;
        }
        public string RightButtonVisibility
        {
            set { rightButtonVisibility = value;RaisePropertyChanged(); }
            get => rightButtonVisibility;
        }
        public bool IntersectionIsChecked
        {
            set
            {
                intersectionIsChecked = value;
                RaisePropertyChanged();
            }
            get => intersectionIsChecked;
        }
        public bool SubtractionIsChecked
        {
            set
            {
                subtractionIsChecked = value;
                RaisePropertyChanged();
            }
            get => subtractionIsChecked;
        }
        public string CurrentItemName
        {
            set
            {
                currentItemName = value; 
                RaisePropertyChanged();
            }
            get => currentItemName;
        }
        public HWindow CurrentHWindow
        {
            set => currentHWindow = value;
            get => currentHWindow;
        }
        public RelayCommand LeftButtonCommand { set; get; } //左
        public RelayCommand RightButtonCommand { set; get; }//右
        public RelayCommand CleanROICommand { set; get; }
        public RelayCommand DrawLineCommand { set; get; }
        public RelayCommand DrawCircleCommand { set; get; }
        public RelayCommand DrawEllipseCommand { set; get; }
        public RelayCommand DrawRectangleCommand { set; get; }
        public RelayCommand DrawSpinRectangleCommand { set; get; }
        public RelayCommand DrawAnythingCommand { set; get; }
        public RelayCommand StopGrabClick { set; get; }
        #endregion

        /// <summary>
        /// CameraSettingPageViewModel构造函数
        /// </summary>
        public CameraSettingPageViewModel()
        {
            CurrentDisplayCameraID = 0;
            LeftButtonVisibility = "Collapsed";
            RightButtonVisibility = "Visible";
            if (CameraWorkList.Count() > 0)
                CurrentItemName = CameraWorkList[0].Name;
            if (CameraWorkList.Count() == 1 || CameraWorkList.Count() == 0)
            {
                LeftButtonVisibility = "Collapsed";
                RightButtonVisibility = "Collapsed";
            }
            LeftButtonCommand = new RelayCommand(() =>
            {
                LeftButtonEvent();
            });
            RightButtonCommand = new RelayCommand(() =>
            {
                RightButtonEvent();
            });
            StopGrabClick = new RelayCommand(() =>
            {
                HObject Image = null;
                if (CameraWorkList[CurrentDisplayCameraID].IsOpen)
                {
                    CameraWorkList[CurrentDisplayCameraID].Stop();
                    CameraWorkList[CurrentDisplayCameraID].GrabImage(out Image);
                }
                else if (!CameraWorkList[CurrentDisplayCameraID].IsOpen)
                    CameraWorkList[CurrentDisplayCameraID].Play();
            });
            DrawLineCommand = new RelayCommand(() =>
            {

            });
        }

        /// <summary>
        /// 主任务
        /// </summary>
        public void Mission()
        {
            /*切换窗口线程,切换完成，自动退出*/
            Task.Run(() =>
            {
                while (true)
                {
                    if (this.CurrentHWindow != null)
                    {
                        MainWindowHWindowList.Clear();
                        for (int i = 0; i < CameraWorkList.Count(); i++)
                        {
                            WorkNum++;
                            CameraWorkList[i].Stop();
                            MainWindowHWindowList.Add(CameraWorkList[i].CurrentHWindow);
                            CameraWorkList[i].CurrentHWindow = this.CurrentHWindow;
                            CameraWorkList[i].Play();
                        }
                        break;
                    }
                }
                Console.WriteLine($"切换窗口线程退出");
            });
        }

        public void LeftButtonEvent()
        {
            CurrentDisplayCameraID--;
            if (CurrentDisplayCameraID == 0)
            {
                LeftButtonVisibility = "Collapsed";
                RightButtonVisibility = "Visible";
                CurrentItemName = CameraWorkList[CurrentDisplayCameraID].Name;
            }
        }

        public void RightButtonEvent()
        {
            CurrentDisplayCameraID++;
            if (CurrentDisplayCameraID == CameraWorkList.Count()-1)
            {
                RightButtonVisibility = "Collapsed";
                LeftButtonVisibility = "Visible";
                CurrentItemName = CameraWorkList[CurrentDisplayCameraID].Name;
            }                
        }
    }
}
