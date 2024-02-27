using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t訂餐_訂單資訊表
{
    public int 訂單ID { get; set; }

    public string? 訂單狀態 { get; set; }

    public int 學員ID { get; set; }

    public string? 訂單時間 { get; set; }

    public string? 支付方式 { get; set; }

    public virtual ICollection<t訂餐_訂單詳細資訊表> t訂餐_訂單詳細資訊表 { get; set; } = new List<t訂餐_訂單詳細資訊表>();

    public virtual ICollection<t訂餐_評論表> t訂餐_評論表 { get; set; } = new List<t訂餐_評論表>();

    public virtual t會員_學生 學員 { get; set; } = null!;
}
