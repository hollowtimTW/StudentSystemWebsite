using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t討論_留言
{
    public int 留言ID { get; set; }

    public int? 學生ID { get; set; }

    public string? 內容 { get; set; }

    public DateTime? 時間 { get; set; }

    public int? 總讚數 { get; set; }

    public int? 總倒讚數 { get; set; }

    public int? 文章ID { get; set; }

    public int? 子版ID { get; set; }

    public int? 看板ID { get; set; }

    public virtual ICollection<t討論_留言點讚> t討論_留言點讚 { get; set; } = new List<t討論_留言點讚>();

    public virtual t討論_子版? 子版 { get; set; }

    public virtual t會員_學生? 學生 { get; set; }

    public virtual t討論_文章? 文章 { get; set; }

    public virtual t討論_看板? 看板 { get; set; }
}
