using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t影片_tag中繼表
{
    public int fId { get; set; }

    public int fVideoId { get; set; }

    public int fTagId { get; set; }

    public virtual t影片_Tag表 fTag { get; set; } = null!;

    public virtual t影片_Video fVideo { get; set; } = null!;
}
