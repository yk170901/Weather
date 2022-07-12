using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Weather.MariaDb.Datums
{
    // Datum : D 드라이브에 있는 Area CSV로부터 Area 관련 값을 읽어 DB에 저장
    [Table("Area")]
    public partial class AreaDatum
    {
        [Key]
        public string areaId { get; set; } // 행정구역코드
        public string area1 { get; set; } // 1단계
        public string area2 { get; set; } // 2단계
        public string area3 { get; set; } // 3단계
        public byte nx { get; set; } // 격자 X
        public byte ny { get; set; } // 격자 Y
    }
}
