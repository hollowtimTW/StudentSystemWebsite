using Class_system_Backstage_pj.Models;
using Microsoft.EntityFrameworkCore;

namespace text_loginWithBackgrount.Areas.video_management.Repositories
{
    public class HomeRepository : IHomeRepository
    {       
        
        private readonly studentContext _studentContext;

         public HomeRepository(studentContext studentContext)
            {
                _studentContext = studentContext;
            }

            public async Task<IEnumerable<T影片Genre>> Genres()
            {
                return await _studentContext.T影片Genres.ToListAsync();
            }
         
            public async Task<IEnumerable<T影片Video>> GetVideos(string sTerm = "", int genreId = 0)
            {
                sTerm = sTerm.ToLower();
                IEnumerable<T影片Video> videos = await (
                    from video in _studentContext.T影片Videos
                    join subject in _studentContext.T課程科目s
                    on video.科目id equals subject.科目id
                    where string.IsNullOrWhiteSpace(sTerm) || (video != null && video.FVideoName.ToLower().StartsWith(sTerm))
                    select new T影片Video
                    {
                        FVideoId = video.FVideoId,
                        FVideoTitle = video.FVideoTitle,
                        FUrl = video.FUrl,
                        科目id = video.科目id,
                        FPrice = video.FPrice,
                        FVideoName = video.FVideoName,
                        GenreId = video.GenreId,
                        科目 = new T課程科目 // 使用 T課程科目 的預設建構函數
                        {
                            科目id = subject.科目id,
                            科目名稱 = subject.科目名稱,
                            科目類別id = subject.科目類別id,
                            狀態 = subject.狀態,
                            // 其他屬性，視需要添加
                        }
                    }
                ).ToListAsync();

                if (genreId > 0)
                {
                    videos = videos.Where(v => v.GenreId == genreId).ToList();
                }

                return videos;
            }
        }
    }


