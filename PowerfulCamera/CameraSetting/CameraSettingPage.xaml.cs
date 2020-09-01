using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PowerfulCamera.CameraSetting
{
    /// <summary>
    /// CameraSettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class CameraSettingPage
    {
        public CameraSettingPageViewModel cameraSettingPageViewModel;
        public CameraSettingPage()
        {
            InitializeComponent();
            cameraSettingPageViewModel = new CameraSettingPageViewModel();
            this.DataContext = cameraSettingPageViewModel;
            cameraSettingPageViewModel.Mission();
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            for (int i = 0; i < cameraSettingPageViewModel.CameraWorkList.Count(); i++)
            {
                cameraSettingPageViewModel.CameraWorkList[i].Stop();
                cameraSettingPageViewModel.CameraWorkList[i].CurrentHWindow = cameraSettingPageViewModel.MainWindowHWindowList[i];
                cameraSettingPageViewModel.CameraWorkList[i].Play();
            }
        }
    }
}
