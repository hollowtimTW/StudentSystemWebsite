using Class_system_Backstage_pj.Models;
using Humanizer.Localisation;
using Microsoft.EntityFrameworkCore;
using text_loginWithBackgrount.Data;

namespace text_loginWithBackgrount.Areas.video_management.ViewModel
{
    public class VideoShopping
    {
        //    private readonly studentContext _studentContext;

        //    public VideoShopping(studentContext _studentContext)
        //    {
        //        _studentContext = _studentContext;
        //    }

        //    public async Task<IEnumerable<T影片Genre>> Genres()
        //    {
        //        return await _studentContext.T影片Genres.ToListAsync();
        //    }
        //    public async Task<IEnumerable<T影片Video>> GetVideos(string sTerm = "", int genreId = 0)
        //    {
        //        sTerm = sTerm.ToLower();
        //        IEnumerable<T影片Video> videos = await (
        //            from video in _studentContext.T影片Videos
        //            join subject in _studentContext.T課程科目s
        //            on video.科目id equals subject.科目id
        //            where string.IsNullOrWhiteSpace(sTerm) || (video != null && video.FVideoName.ToLower().StartsWith(sTerm))
        //            select new T影片Video
        //            {
        //                FVideoId = video.FVideoId,
        //                FVideoTitle = video.FVideoTitle,
        //                FUrl = video.FUrl,
        //                科目id = video.科目id,
        //                FPrice = video.FPrice,
        //                FVideoName = video.FVideoName,
        //                GenreId = video.GenreId,
        //                科目 = subject
        //            }
        //        ).ToListAsync();

        //        if (genreId > 0)
        //        {
        //            videos = videos.Where(v => v.GenreId == genreId).ToList();
        //        }

        //        return videos;
        //    }
    }
}

