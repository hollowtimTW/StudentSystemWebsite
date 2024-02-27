using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t訂餐_學員口味表
{
    public int 識別碼ID { get; set; }

    public int 學員ID { get; set; }

    public int 口味ID { get; set; }

    public virtual t訂餐_口味總表 口味 { get; set; } = null!;

    public virtual t會員_學生 學員 { get; set; } = null!;
}
