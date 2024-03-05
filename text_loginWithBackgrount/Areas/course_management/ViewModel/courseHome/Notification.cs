using NuGet.Protocol.Plugins;
using Class_system_Backstage_pj.Models;

namespace text_loginWithBackgrount.Areas.course_management.ViewModel.courseHome
{
   

    public class Notification
    {

        public int 訊息id { get; set; }

        public string 發送者類型 { get; set; } = null!;

        public int 發送者id { get; set; }

        public string 接收者類型 { get; set; } = null!;

        public int 接收者id { get; set; }

        public string 發送訊息內容 { get; set; } = null!;

        public int 狀態 { get; set; }

        public DateTime? 時間 { get; set; }
        
        public string SenderName { get; set; }

    }

}
