using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t課程_學生班級
{
    public int 學生班級ID { get; set; }

    public int 學生ID { get; set; }

    public int 班級ID { get; set; }

    public int 狀態 { get; set; }

    public virtual t會員_學生 學生 { get; set; } = null!;

    public virtual t課程_班級 班級 { get; set; } = null!;
}
