using Class_system_Backstage_pj.Areas.video_management.Controllers;
using Class_system_Backstage_pj.Models;
using Humanizer.Localisation;

namespace text_loginWithBackgrount.Areas.video_management.Repositories
{
    public interface IHomeRepository
    {
        Task<IEnumerable<T影片Video>> GetVideos(string sTerm = "", int genreId = 0);
        Task<IEnumerable<T影片Genre>> Genres();
    }
}
