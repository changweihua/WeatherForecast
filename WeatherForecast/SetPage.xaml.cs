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
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace WeatherForecast
{
    public partial class SetPage : PhoneApplicationPage
    {

        string[] prov = { "北京", "上海", "天津", "重庆", "黑龙江", "吉林", "辽宁", "内蒙古", "河北", "山西", "陕西" };
        SelectedCity sc = null;

        public SetPage()
        {
            InitializeComponent();
        }

        private void provincelp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListPicker lp = sender as ListPicker;
            string p = lp.SelectedItem.ToString();
            ProvSelectedChanged(p);
        }

        private void citylp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>  
        /// 省份改变，地区绑定，并选择原来设置的地区  
        /// </summary>  
        /// <param name="prov"></param>  
        void ProvSelectedChanged(String prov)
        {
            List<String> l = CityDataBind(prov);
            if (sc != null && sc.Province == prov)
            {
                int i = l.IndexOf(sc.Cityname);
                citylp.SelectedIndex = i;
            }
        }  

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //provincelp.ItemsSource = prov;
            //ProvDataBind();
            #region 判定是否是中断退出后进入，是就恢复数据
            IsolatedStorageSettings iss = IsolatedStorageSettings.ApplicationSettings;
            bool recov;
            if (iss.TryGetValue("Recovery", out recov))
            {
                if (recov)
                {
                    if (PhoneApplicationService.Current.State.ContainsKey("CurrentPage"))
                    {
                        string temp = Convert.ToString(PhoneApplicationService.Current.State["CurrentPage"]);
                        if (temp.Equals("SetPage.xaml"))
                        {
                            int provIndex = 0;
                            int cityIndex = 0;
                            if (PhoneApplicationService.Current.State.ContainsKey("provIndex"))
                            {
                                provIndex = Convert.ToInt32(PhoneApplicationService.Current.State["provIndex"]);
                            }
                            if (PhoneApplicationService.Current.State.ContainsKey("cityIndex"))
                            {
                                cityIndex = Convert.ToInt32(PhoneApplicationService.Current.State["cityIndex"]);
                            }
                            RecovListPickerItems(provIndex, cityIndex);//恢复数据  
                            iss["Recovery"] = false;//设置为已经恢复  

                        }
                        else
                            ProvDataBind();
                    }
                    else
                        ProvDataBind();
                }
                else
                    ProvDataBind();
            }
            else
                ProvDataBind();
            #endregion
            PhoneApplicationService.Current.State["CurrentPage"] = "SetPage.xaml";  
        }

        /// <summary>  
        /// 恢复数据的时候绑定city的ItemSource,并且绑定两个ListPicker的当前选项  
        /// </summary>  
        /// <param name="provIndex"></param>  
        /// <param name="cityIndex"></param>  
        void RecovListPickerItems(int provIndex, int cityIndex)
        {
            provincelp.ItemsSource = prov;
            if (provIndex < 0 || provIndex >= prov.Length)
            {
                provIndex = 0;
            }
            provincelp.SelectedIndex = provIndex;
            List<String> l = CityDataBind(prov[provIndex]);
            if (cityIndex < 0 || cityIndex >= l.Count())
            {
                cityIndex = 0;
            }
            citylp.SelectedIndex = cityIndex;

        }

        /// <summary>  
        /// 离开页面保存当前两个ListPicker的selectedIndex  
        /// </summary>  
        /// <param name="e"></param>  
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            PhoneApplicationService.Current.State["provIndex"] = provincelp.SelectedIndex;
            PhoneApplicationService.Current.State["cityIndex"] = citylp.SelectedIndex;
        }  

        void ProvDataBind()
        {
            //给省份的ListPick绑定数据
            provincelp.ItemsSource = prov;
            sc = SelectedCity.GetIsolated();
            if (sc == null)
            {
                return;
            }
            int i;
            for (i = 0; i < prov.Length; i++)
            {
                if (prov[i] == sc.Province)
                {
                    break;
                }
            }
            provincelp.SelectedIndex = i;
        }

        List<string> CityDataBind(string prov)
        {
            IList<CityInfoTable> list = null;
            using (CityDataContext db = new CityDataContext(CityDataContext.connectionString))
            {
                IQueryable<CityInfoTable> queries =
                    from c in db.CityInfos where c.Province == prov select c;
                list = queries.ToList();
            }
            List<string> l = new List<string>();
            foreach (var item in list)
            {
                l.Add(item.CityName);
            }
            citylp.ItemsSource = l;
            return l;
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            string cityname = citylp.SelectedItem.ToString();
            string prov = provincelp.SelectedItem.ToString();
            string cityCode = string.Empty;
            //从数据库取出城市的代码
            using (var db = new CityDataContext(CityDataContext.connectionString))
            {
                var queries =
                    from c in db.CityInfos where (c.Province == prov && c.CityName == cityname) select c;
                cityCode = queries.First().CityCode;
            }
            SelectedCity.SaveIsolated(new SelectedCity { Citycode = cityCode, Cityname = cityname, Province = prov });
            NavigationService.Navigate(new Uri("/WeatherForecast;component/MainPage.xaml", UriKind.Relative));
        }

    }
}