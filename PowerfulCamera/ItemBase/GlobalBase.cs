using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PowerfulCamera.ItemBase
{
    public static class GlobalBase
    {
        public static string ItemDir = Environment.CurrentDirectory; //工作目录
        public static string CameraDir = ItemDir + "\\CameraConfig";   //摄像机列表目录

        public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
        {
            MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
            return expressionBody.Member.Name;
        }
    }
}
