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

using System.Data.Linq.Mapping;
using System.ComponentModel;

namespace WeatherForecast
{
    [Table]//将CityInfoTable定义为表
    public class CityInfoTable
    {
        //定义表的自增ID，设置为主键
        private int _cityInfoId;

        //Column是定义一个成员为表字段。如果没有这个，那么这个成员不是字段
        [Column(IsPrimaryKey=true,IsDbGenerated=true,DbType="int not null identity",CanBeNull=  false,AutoSync=AutoSync.OnInsert)]
        public int CityInfoId
        {
            get
            {
                return _cityInfoId;
            }
            set
            {
                _cityInfoId = value;
            }
        }

        //定义省份名称
        private string _province;
        [Column]
        public string Province
        {
            get
            {
                return _province;
            }
            set
            {
                _province = value;
            }
        }

        //定义城市名称
        private string _cityName;

        [Column]
        public string CityName
        {
            get { return _cityName; }
            set { _cityName = value; }
        }

        //定义城市代码
        private string _cityCode;

        [Column]
        public string CityCode
        {
            get { return _cityCode; }
            set { _cityCode = value; }
        }

    }
}
