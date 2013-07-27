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
using System.Windows.Media.Imaging;

namespace ForeCastTemplate
{
    public partial class ForecastTemplate : UserControl
    {
        public ForecastTemplate()
        {
            InitializeComponent();
        }

        //定义Image的Source属性，这样可以从外部访问Source属性
        public string _imageUri;

        public string ImageUri
        { 
            get
            {
                return _imageUri;
            }
            set
            {
                _imageUri = value;
                BitmapImage bmp = new BitmapImage(new Uri(value, UriKind.Relative));
                WImg.Source = bmp;
            }
        }

        public string _weekday;

        public string Weekday
        {
            get
            {
                return _weekday;
            }
            set
            {
                _weekday = value;
                weekDay.Text = value;
            }
        }

        private string _temp;

        public string Temp
        {
            get
            {
                return _temp;
            }
            set
            {
                _temp = value;
                temp.Text = value;
            }
        }


    }
}
