using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t影片_Video
{
    public int fVideoId { get; set; }

    public string fVideoTitle { get; set; } = null!;

    public string fURL { get; set; } = null!;

    public int 科目ID { get; set; }

    public decimal fPrice { get; set; }

    public string? fImagePath { get; set; }

    public virtual ICollection<t影片_OrderDEtail> t影片_OrderDEtail { get; set; } = new List<t影片_OrderDEtail>();

    public virtual ICollection<t影片_tag中繼表> t影片_tag中繼表 { get; set; } = new List<t影片_tag中繼表>();

    public virtual t課程_科目 科目 { get; set; } = null!;
}
