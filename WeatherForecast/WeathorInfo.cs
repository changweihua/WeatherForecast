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
using System.IO.IsolatedStorage;

namespace WeatherForecast
{
    public class WeathorInfo
    {
        public string city { get; set; }
        public string cityid { get; set; }
        public string date_y { get; set; }
        public string week { get; set; }
        public string temp1 { get; set; }
        public string temp2 { get; set; }
        public string temp3 { get; set; }
        public string temp4 { get; set; }
        public string temp5 { get; set; }
        public string weather1 { get; set; }
        public string weather2 { get; set; }
        public string weather3 { get; set; }
        public string weather4 { get; set; }
        public string weather5 { get; set; }
        public string wind1 { get; set; }
        public string info { get; set; }

        public DateTime UpdateTime { get; set; }

        #region 保存天气信息到IsolatedStorage

        public static void SaveIsolated(WeathorInfo w)
        {
            IsolatedStorageSettings iss = IsolatedStorageSettings.ApplicationSettings;
            iss["WeatherInfo"] = w;
        }

        public static WeathorInfo GetIsolated()
        {
            IsolatedStorageSettings iss = IsolatedStorageSettings.ApplicationSettings;
            WeathorInfo w = null;
            WeathorInfo temp = null;
            if (iss.TryGetValue("WeatherInfo", out temp))
            {
                w = temp;
            }
            return w;
        }

        #endregion

    }
}
