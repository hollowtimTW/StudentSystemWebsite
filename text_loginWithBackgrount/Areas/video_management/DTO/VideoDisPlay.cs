using Class_system_Backstage_pj.Models;

namespace text_loginWithBackgrount.Areas.video_management.DTO
{
    public class VideoDisPlay
    {
        public IEnumerable<T影片Video> Videos { get; set; }
        public IEnumerable<T影片Genre> Genres { get; set; }
        public string STerm { get; set; } = "";
        public int GenreId { get; set; } = 0;
    }
}
