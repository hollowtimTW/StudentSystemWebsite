using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t訂餐_訂單詳細資訊表
{
    public int 訂單詳細表ID { get; set; }

    public int 訂單ID { get; set; }

    public int 店家ID { get; set; }

    public int 餐點ID { get; set; }

    public int? 餐點數量 { get; set; }

    public decimal? 金額小記 { get; set; }

    public virtual t訂餐_店家資料表 店家 { get; set; } = null!;

    public virtual t訂餐_訂單資訊表 訂單 { get; set; } = null!;

    public virtual t訂餐_餐點資訊表 餐點 { get; set; } = null!;
}
