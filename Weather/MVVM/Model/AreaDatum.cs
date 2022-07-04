using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Weather.MVVM.Model
{
    // attribute
    [Table("Area")] // System.ComponentModel.DataAnnotations.Schema;
    public partial class AreaDatum
    {
        [Key]
        public string areaId { get; set; } // 행정구역코드
        public string area1 { get; set; } // 1단계
        public string area2 { get; set; } // 2단계
        public int nx { get; set; } // 격자 X
        public int ny { get; set; } // 격자 Y
    }
}
