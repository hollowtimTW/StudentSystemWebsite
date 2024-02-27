using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t訂餐_餐點資訊表
{
    public int 餐點ID { get; set; }

    public int 店家ID { get; set; }

    public string? 餐點名稱 { get; set; }

    public string? 餐點描述 { get; set; }

    public decimal? 餐點定價 { get; set; }

    public string? 餐點照片 { get; set; }

    public virtual ICollection<t訂餐_訂單詳細資訊表> t訂餐_訂單詳細資訊表 { get; set; } = new List<t訂餐_訂單詳細資訊表>();

    public virtual t訂餐_店家資料表 店家 { get; set; } = null!;
}
