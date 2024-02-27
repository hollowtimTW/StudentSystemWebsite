using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t影片_Tag表
{
    public int fTagId { get; set; }

    public string fTag { get; set; } = null!;

    public virtual ICollection<t影片_tag中繼表> t影片_tag中繼表 { get; set; } = new List<t影片_tag中繼表>();
}
