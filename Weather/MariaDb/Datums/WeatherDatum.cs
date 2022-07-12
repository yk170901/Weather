using System;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Weather.MariaDb.Datums
{

    // Datum : API로부터 Weather 관련 값을 받아 DB에 저장
    [Table("Weather")]
    public partial class WeatherDatum
    {
        /*public int id { get; set; }*/
        public DateTime fcstDate { get; set; } // 예보일자
        public string fcstTime { get; set; } // 예보시간
        public string category { get; set; } // 자료구분 문자  ex)TMP(Temperiture), TMN(Temperiture Minimum)
        public string fcstValue { get; set; } // 예보 값 = 자료구분 문자열에 따른 예보 값
    }
}
