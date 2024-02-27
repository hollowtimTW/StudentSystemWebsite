using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t考試_學生答題
{
    public int id { get; set; }

    public int? 學生ID { get; set; }

    public int? 考試ID { get; set; }

    public string? 答案 { get; set; }

    public DateTime? 提交時間 { get; set; }

    public virtual t會員_學生? 學生 { get; set; }

    public virtual t考試_考試總表? 考試 { get; set; }
}
