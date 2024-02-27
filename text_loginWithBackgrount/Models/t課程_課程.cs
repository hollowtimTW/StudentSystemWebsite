using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t課程_課程
{
    public int 課程ID { get; set; }

    public int 班級科目ID { get; set; }

    public DateTime 開始時間 { get; set; }

    public DateTime 結束時間 { get; set; }

    public string? 課堂摘要 { get; set; }

    public int? 值日生ID { get; set; }

    public int 狀態 { get; set; }

    public virtual t會員_學生? 值日生 { get; set; }

    public virtual t課程_班級科目 班級科目 { get; set; } = null!;
}
