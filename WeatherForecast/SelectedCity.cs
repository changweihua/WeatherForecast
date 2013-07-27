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
    public class SelectedCity
    {
        private string _province;

        public string Province
        {
            get { return _province; }
            set { _province = value; }
        }

        private string _citycode;

        public string Citycode
        {
            get { return _citycode; }
            set { _citycode = value; }
        }

        private string _cityname;

        public string Cityname
        {
            get { return _cityname; }
            set { _cityname = value; }
        }

        #region  往Isolate的StorageSettings保存和取回设置的城市信息

        public static void SaveIsolated(SelectedCity sc)
        {
            IsolatedStorageSettings iss = IsolatedStorageSettings.ApplicationSettings;
            iss["SelectedCity"] = sc;
        }

        public static SelectedCity GetIsolated()
        {
            IsolatedStorageSettings iss = IsolatedStorageSettings.ApplicationSettings;
            SelectedCity sc = null;
            SelectedCity temp = null;
            if (iss.TryGetValue("SelectedCity", out temp))//取值的时候先判断是否存在
            {
                sc = temp;
            }
            return temp;
        }

        #endregion

    }
}
