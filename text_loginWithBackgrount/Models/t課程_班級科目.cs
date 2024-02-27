using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t課程_班級科目
{
    public int 班級科目ID { get; set; }

    public int 班級ID { get; set; }

    public int 科目ID { get; set; }

    public int 老師ID { get; set; }

    public int 狀態 { get; set; }

    public virtual ICollection<t課程_評分主表> t課程_評分主表 { get; set; } = new List<t課程_評分主表>();

    public virtual ICollection<t課程_課程> t課程_課程 { get; set; } = new List<t課程_課程>();

    public virtual t課程_班級 班級 { get; set; } = null!;

    public virtual t課程_科目 科目 { get; set; } = null!;

    public virtual t會員_老師 老師 { get; set; } = null!;
}
