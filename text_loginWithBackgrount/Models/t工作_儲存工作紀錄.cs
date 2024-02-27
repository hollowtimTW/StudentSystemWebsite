using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t工作_儲存工作紀錄
{
    public int fId { get; set; }

    public int f學員ID { get; set; }

    public int f職缺ID { get; set; }

    public DateTime f儲存時間 { get; set; }

    public virtual t會員_學生 f學員 { get; set; } = null!;

    public virtual t工作_職缺資料 f職缺 { get; set; } = null!;
}
