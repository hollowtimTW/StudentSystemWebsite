using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t課程_老師科目
{
    public int 老師科目ID { get; set; }

    public int 老師ID { get; set; }

    public int 科目ID { get; set; }

    public int 狀態 { get; set; }

    public virtual t課程_科目 科目 { get; set; } = null!;

    public virtual t會員_老師 老師 { get; set; } = null!;
}
