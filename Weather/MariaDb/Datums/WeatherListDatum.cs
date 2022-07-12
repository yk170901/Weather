using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.MariaDb.Datums
{
    // Datum : DB로부터 Weather 값을 받아와 WPF에 디스플레이
    public class WeatherListDatum
    {
        public string fcstTime { get; set; } // 예보시간
        public string TMP { get; set; } // 기온
        public string POP { get; set; } // 강수확률
        public string REH { get; set; } // 습도
    }
}
