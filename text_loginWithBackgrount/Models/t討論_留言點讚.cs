using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t討論_留言點讚
{
    public int? 學生ID { get; set; }

    public int? 留言ID { get; set; }

    public string? 讚or倒讚 { get; set; }

    public int ID { get; set; }

    public virtual t會員_學生? 學生 { get; set; }

    public virtual t討論_留言? 留言 { get; set; }
}
