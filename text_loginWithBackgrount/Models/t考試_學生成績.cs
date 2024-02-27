using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t考試_學生成績
{
    public int id { get; set; }

    public int? 學生ID { get; set; }

    public int? 考試ID { get; set; }

    public double? 原始成績 { get; set; }

    public double? 成績 { get; set; }

    public int? 狀態 { get; set; }

    public string? 備註 { get; set; }

    public DateTime? 更新時間 { get; set; }

    public virtual t會員_學生? 學生 { get; set; }

    public virtual t考試_考試總表? 考試 { get; set; }
}
