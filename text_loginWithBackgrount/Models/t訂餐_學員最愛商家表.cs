using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t訂餐_學員最愛商家表
{
    public int 識別碼ID { get; set; }

    public int 學員ID { get; set; }

    public int 店家ID { get; set; }

    public virtual t會員_學生 學員 { get; set; } = null!;

    public virtual t訂餐_店家資料表 店家 { get; set; } = null!;
}
