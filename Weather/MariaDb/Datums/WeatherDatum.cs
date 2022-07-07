using System;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Weather.MariaDb.Datums
{
    [Table("Weather")]
    public partial class WeatherDatum
    {
        public DateTime fcstDate { get; set; } // 예보일자
        public string fcstTime { get; set; } // 예보시간
        public string category { get; set; } // 자료구분 문자  ex)TMP(Temperiture), TMN(Temperiture Minimum)
        public string fcstValue { get; set; } // 예보 값 = 자료구분 문자열에 따른 예보 값
    }
}
