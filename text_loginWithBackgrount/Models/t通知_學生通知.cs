using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t通知_學生通知
{
    public int fId { get; set; }

    public int f學生ID { get; set; }

    public int f通知總表ID { get; set; }

    public virtual t會員_學生 f學生 { get; set; } = null!;

    public virtual t通知_通知訊息總表 f通知總表 { get; set; } = null!;
}
