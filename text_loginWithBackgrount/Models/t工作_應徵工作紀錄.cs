using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t工作_應徵工作紀錄
{
    public int fId { get; set; }

    public int f學員ID { get; set; }

    public int f職缺ID { get; set; }

    public string f履歷版本內容 { get; set; } = null!;

    public int f通知總表ID { get; set; }

    public DateTime f應徵時間 { get; set; }

    public string f刪除狀態 { get; set; } = null!;

    public virtual t會員_學生 f學員 { get; set; } = null!;

    public virtual t工作_職缺資料 f職缺 { get; set; } = null!;

    public virtual t通知_通知訊息總表 f通知總表 { get; set; } = null!;
}
