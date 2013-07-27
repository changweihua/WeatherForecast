using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Data.Linq;

namespace WeatherForecast
{
    /// <summary>
    /// 定义一个数据库
    /// </summary>
    public class CityDataContext : DataContext
    {
        //定义数据库连接字符串
        public static string connectionString = "Data Source=isostore:/CityInfo.sdf";

        //传递数据库连接字符串到DataContext基类
        public CityDataContext(string connectionString)
            : base(connectionString)
        {

        }

        //定义一个数据表，如果有多个数据表也可以继续添加为成员变量
        public Table<CityInfoTable> CityInfos
        {
            get
            {
                return this.GetTable<CityInfoTable>();
            }
        }

    }
}
