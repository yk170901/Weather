using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.MariaDb.Datums
{
    public class WeatherListDatum
    {
        public string fcstTime { get; set; }
        public string POP { get; set; } // 강수확률
        public string REH { get; set; } // 습도
        public string TMP { get; set; } // 기온
    }
}
