using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t訂餐_評論表
{
    public int 評論表ID { get; set; }

    public int 訂單ID { get; set; }

    public string? 評論 { get; set; }

    public string? 滿意度_星數_ { get; set; }

    public virtual t訂餐_訂單資訊表 訂單 { get; set; } = null!;
}
