using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t訂餐_營業時間表
{
    public int 營業時間表ID { get; set; }

    public int 店家ID { get; set; }

    public string? 星期 { get; set; }

    public string? 時段_早_中_晚_全_ { get; set; }

    public string? 開始營業時間 { get; set; }

    public string? 結束營業時間 { get; set; }

    public virtual t訂餐_店家資料表 店家 { get; set; } = null!;
}
