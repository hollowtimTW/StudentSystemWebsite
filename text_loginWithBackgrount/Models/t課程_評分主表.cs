using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t課程_評分主表
{
    public int 評分主表ID { get; set; }

    public int 班級科目ID { get; set; }

    public int 學生ID { get; set; }

    public DateTime 提交時間 { get; set; }

    public int 狀態 { get; set; }

    public string 改進意見 { get; set; } = null!;

    public virtual ICollection<t課程_評分> t課程_評分 { get; set; } = new List<t課程_評分>();

    public virtual t會員_學生 學生 { get; set; } = null!;

    public virtual t課程_班級科目 班級科目 { get; set; } = null!;
}
