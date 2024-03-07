using System.ComponentModel.DataAnnotations;

namespace Class_system_Backstage_pj.Areas.video_management.ViewModel
{
    public class videocreatViewModel
    {
        public string? videoTitle { get; set; }

        public string? videoPrice { get; set; }

        public string? subjectid { get; set; }
        //有改東西
        public string? videoName { get; set; }
        public int? GenreId { get; set; }
    }
}
