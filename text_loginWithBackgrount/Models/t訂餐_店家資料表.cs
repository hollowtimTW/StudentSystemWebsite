using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t訂餐_店家資料表
{
    public int 店家ID { get; set; }

    public string 店家名稱 { get; set; } = null!;

    public string 地址 { get; set; } = null!;

    public string 電話 { get; set; } = null!;

    public string 餐廳介紹 { get; set; } = null!;

    public string? 餐廳照片 { get; set; }

    public string 電子信箱 { get; set; } = null!;

    public string 密碼 { get; set; } = null!;

    public virtual ICollection<t訂餐_學員最愛商家表> t訂餐_學員最愛商家表 { get; set; } = new List<t訂餐_學員最愛商家表>();

    public virtual ICollection<t訂餐_店家風味表> t訂餐_店家風味表 { get; set; } = new List<t訂餐_店家風味表>();

    public virtual ICollection<t訂餐_營業時間表> t訂餐_營業時間表 { get; set; } = new List<t訂餐_營業時間表>();

    public virtual ICollection<t訂餐_訂單詳細資訊表> t訂餐_訂單詳細資訊表 { get; set; } = new List<t訂餐_訂單詳細資訊表>();

    public virtual ICollection<t訂餐_餐點資訊表> t訂餐_餐點資訊表 { get; set; } = new List<t訂餐_餐點資訊表>();
}
