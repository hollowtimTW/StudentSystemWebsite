using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t會員_老師登入紀錄
{
    public int 老師登入紀錄ID { get; set; }

    public int 老師ID { get; set; }

    public DateTime 登入時間 { get; set; }

    public string 狀態 { get; set; } = null!;

    public virtual t會員_老師 老師 { get; set; } = null!;
}
