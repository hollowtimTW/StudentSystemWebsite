using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t會員_出勤
{
    public int 出勤ID { get; set; }

    public int 學生ID { get; set; }

    public int 班級ID { get; set; }

    public DateTime 簽到時間 { get; set; }

    public string? 狀態 { get; set; }

    public virtual t會員_學生 學生 { get; set; } = null!;

    public virtual t課程_班級 班級 { get; set; } = null!;
}
