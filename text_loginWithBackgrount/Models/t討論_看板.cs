using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t討論_看板
{
    public int 看板ID { get; set; }

    public string? 名稱 { get; set; }

    public string? 封面圖 { get; set; }

    public virtual ICollection<t討論_子版> t討論_子版 { get; set; } = new List<t討論_子版>();

    public virtual ICollection<t討論_文章> t討論_文章 { get; set; } = new List<t討論_文章>();

    public virtual ICollection<t討論_留言> t討論_留言 { get; set; } = new List<t討論_留言>();
}
