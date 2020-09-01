using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerfulCamera.CameraTools
{
    public static class HWindowExtensions
    {
        public static bool DispImageFit(this HWindow CurrentHWindow, HObject t_image)
        {
            int current_beginRow, current_beginCol, current_endRow, current_endCol;         // 获取图像的当前显示部分
            if (t_image != null && CurrentHWindow != null)
            {
                try
                {
                    if (t_image.CountObj() == 0)
                    {
                        return false;
                    }
                    HTuple width, height;
                    HOperatorSet.GetImageSize(t_image, out width, out height);
                    current_beginRow = 0;
                    current_beginCol = 0;
                    HOperatorSet.SetPart(CurrentHWindow, current_beginRow, current_beginCol, height, width);
                    current_endRow = height;
                    current_endCol = width;
                    HOperatorSet.DispObj(t_image, CurrentHWindow);//显示图像
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

    }
}
