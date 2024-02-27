using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t訂餐_店家風味表
{
    public int 識別碼ID { get; set; }

    public int 店家ID { get; set; }

    public int 口味ID { get; set; }

    public virtual t訂餐_口味總表 口味 { get; set; } = null!;

    public virtual t訂餐_店家資料表 店家 { get; set; } = null!;
}
