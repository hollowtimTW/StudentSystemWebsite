using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t課程_評分
{
    public int 評分ID { get; set; }

    public int 評分主表ID { get; set; }

    public string 評分分類 { get; set; } = null!;

    public string 評分題目 { get; set; } = null!;

    public int 評分 { get; set; }

    public int 狀態 { get; set; }

    public virtual t課程_評分主表 評分主表 { get; set; } = null!;
}
