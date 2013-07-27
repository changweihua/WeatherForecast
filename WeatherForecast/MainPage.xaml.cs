using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Net.NetworkInformation;

namespace WeatherForecast
{
    public partial class MainPage : PhoneApplicationPage
    {
        SelectedCity sc = null;
        string uriFormat = "http://m.weather.com.cn/data/{0}.html";
        WeathorInfo weather = null;
        string[] weekMsg = { "星期一", "星期二", "星期三", "星期四", "星期五", "星期六", "星期日" };

        #region 检查网络

        private bool networkIsAvailable;
        private NetworkInterfaceType _currentNetworkType;//网络连接类型

        void InitialNetwork()
        {
            networkIsAvailable = Microsoft.Phone.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            _currentNetworkType = Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType;

        }

        #endregion


        // 构造函数
        public MainPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //using (CityDataContext db = new CityDataContext(CityDataContext.connectionString))
            //{
            //    IQueryable<CityInfoTable> queries =
            //        from c in db.CityInfos where c.Province == "江苏" && c.CityName == "镇江" select c;
            //    MessageBox.Show(queries.First().CityName + queries.First().CityCode);
            //}
            //WebClient wb = new WebClient();
            //wb.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wb_DownloadStringCompleted);
            //wb.DownloadStringAsync(new Uri("http://m.weather.com.cn/data/101040200.html", UriKind.Absolute));
            //UpdateWeather();
            about.Text = "此程序由常伟华编写，设计参考了CSDN，如果有问题，请联系15023215505";
            #region 判断是否中断进入

            IsolatedStorageSettings iss = IsolatedStorageSettings.ApplicationSettings;
            bool recov;
            if (iss.TryGetValue("Recovery", out recov))
            {
                if (recov)
                {
                    if (PhoneApplicationService.Current.State.ContainsKey("CurrentPage"))
                    {
                        string temp = Convert.ToString(PhoneApplicationService.Current.State["CurrentPage"]);
                        if (!temp.Equals("MainPage.xaml") && temp.Length > 0)
                        {
                            NavigationService.Navigate(new Uri("/WeatherForecast;component/" + temp, UriKind.Relative));
                        }
                        else
                        {
                            iss["Recovery"] = false;
                        }
                    }                    
                }
                else
                {
                    PhoneApplicationService.Current.State["CurrentPage"] = "MainPage.xaml";
                }
            }
            else
            {
                PhoneApplicationService.Current.State["CurrentPage"] = "MainPage.xaml";
            }

            #endregion

            //if (networkIsAvailable)
            //{
            //    switch (_currentNetworkType)
            //    {
            //        case NetworkInterfaceType.Ethernet:
            //            MessageBox.Show("Ethernet网络");
            //            break;
            //        case NetworkInterfaceType.MobileBroadbandCdma:
            //            MessageBox.Show("CDMA网络");
            //            break;
            //        case NetworkInterfaceType.MobileBroadbandGsm:
            //            MessageBox.Show("GSM网络");
            //            break;
            //        case NetworkInterfaceType.None:
            //            MessageBox.Show("网络不可用");
            //            break;
            //        case NetworkInterfaceType.Wireless80211:
            //            MessageBox.Show("Wireless网络");
            //            break;
            //        default:
            //            MessageBox.Show("其他网络");
            //            break;
            //    }
            //    UpdateWeather();
            //}
            //else
            //{
            //    MessageBox.Show("没有连接到任何网络");
            //}

            UpdateWeather();

        }

        bool isUpdated()
        {
            sc = SelectedCity.GetIsolated();
            weather = WeathorInfo.GetIsolated();
            if (sc == null)//如果从IsolatedStorage里面取到的为空，就是没有设置地区，默认为永川
            {
                sc = new SelectedCity();
                sc.Citycode = "101040200";
                sc.Cityname = "永川";
                sc.Province = "重庆";
                return true;
            }
            if (weather == null)//如果从来没更新过天气，就更新
            {
                return true;
            }
            if (weather.UpdateTime == null || !sc.Cityname.Equals(weather.city))//如果更改了地区，或者没有更新过天气，更新
            {
                return true;
            }
            TimeSpan ts1 = new TimeSpan(weather.UpdateTime.Ticks);
            TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            if (ts.Hours >= 3)//如果更新时间超过3小时，更新
            {
                return true;
            }
            return false;
        }

        void UpdateWeather()
        {
            if (!isUpdated())
            {
                UpdateUI();
                return;
            }
            string uri = string.Format(uriFormat, sc.Citycode);
            WebClient wb = new WebClient();
            wb.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wb_DownloadStringCompleted);
            wb.DownloadStringAsync(new Uri(uri, UriKind.Absolute));
        }

        void wb_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("获取天气预报失败");
                return;
            }

            if (e.Result.Length <= 0 || e.Cancelled)
            {
                MessageBox.Show("获取天气预报失败");
                return;
            }

            SelectedCity.SaveIsolated(sc);//保存更新好的城市

            JObject json = JObject.Parse(e.Result);
            weather = new WeathorInfo {
                city = (string)json["weatherinfo"]["city"],
                cityid = (string)json["weatherinfo"]["cityid"],
                date_y = (string)json["weatherinfo"]["date_y"],
                week = (string)json["weatherinfo"]["week"],
                info = (string)json["weatherinfo"]["index48_d"],
                wind1 = (string)json["weatherinfo"]["wind1"],
                temp1 = (string)json["weatherinfo"]["temp1"],
                temp2 = (string)json["weatherinfo"]["temp2"],
                temp3 = (string)json["weatherinfo"]["temp3"],
                temp4 = (string)json["weatherinfo"]["temp4"],
                temp5 = (string)json["weatherinfo"]["temp5"],
                weather1 = (string)json["weatherinfo"]["weather1"],
                weather2 = (string)json["weatherinfo"]["weather2"],
                weather3 = (string)json["weatherinfo"]["weather3"],
                weather4 = (string)json["weatherinfo"]["weather4"],
                weather5 = (string)json["weatherinfo"]["weather5"]
            };
            //MessageBox.Show(weather.city);
            weather.UpdateTime = DateTime.Now;
            WeathorInfo.SaveIsolated(weather);//保存更新好的天气

            UpdateUI();

        }

        void UpdateUI()
        {
            day1.Temp = weather.temp2;
            day2.Temp = weather.temp3;
            day3.Temp = weather.temp4;
            day4.Temp = weather.temp5;

            todayTemp.Text = weather.temp1;
            todayWhe.Text = weather.weather1 + Environment.NewLine + weather.wind1;
            todaydate.Text = weather.date_y + Environment.NewLine + weather.week;
            wtInfo.Text = weather.info;

            PageTitle.Text = weather.city;

            int i;

            for (i = 0; i < 7; i++)
            {
                if (weekMsg[i] == weather.week)
                {
                    break;
                }
            }

            day1.Weekday = weekMsg[(i + 1) % 7];
            day2.Weekday = weekMsg[(i + 2) % 7];
            day3.Weekday = weekMsg[(i + 3) % 7];
            day4.Weekday = weekMsg[(i + 4) % 7];

            day1.ImageUri = GetImgUri(weather.weather2);
            day2.ImageUri = GetImgUri(weather.weather3);
            day3.ImageUri = GetImgUri(weather.weather4);
            day4.ImageUri = GetImgUri(weather.weather5);

            todayImage.Source = new BitmapImage(new Uri(GetImgUri(weather.weather1), UriKind.Relative));

        }


        /// <summary>
        /// 返回天气图片的Uri。图片的Building属性为Resource
        /// </summary>
        /// <param name="weather"></param>
        /// <returns></returns>
        String GetImgUri(string weather)
        {
            string uri = "/WeatherForecast;component/Images/";
            if (weather == "晴")
            {
                return uri + "sunday.jpg";
            }
            else if (weather == "阴")
            {
                return uri + "overcast.jpg";
            }
            else if (weather == "雷阵雨")
            {
                return uri + "ThunderShower.jpg";
            }
            else if (weather.Contains("多云"))
            {
                return uri + "cloudy.jpg";
            }
            else if (weather.Contains("雨"))
            {
                return uri + "Rain.jpg";
            }
            else
            {
                return uri + "cloudy.jpg";
            }
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/WeatherForecast;component/SetPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            if (InfoMsg.Visibility == System.Windows.Visibility.Collapsed)
            {
                InfoMsg.Visibility = Visibility.Visible;
            }
            else
            {
                InfoMsg.Visibility = Visibility.Collapsed;
            }
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            InfoMsg.Visibility = Visibility.Collapsed;
        }
    }
}