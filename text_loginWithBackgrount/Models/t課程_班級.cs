using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t課程_班級
{
    public int 班級ID { get; set; }

    public string 班級名稱 { get; set; } = null!;

    public int 班級導師ID { get; set; }

    public DateTime 入學日期 { get; set; }

    public DateTime 結訓日期 { get; set; }

    public int 狀態 { get; set; }

    public virtual ICollection<t會員_出勤> t會員_出勤 { get; set; } = new List<t會員_出勤>();

    public virtual ICollection<t考試_考試總表> t考試_考試總表 { get; set; } = new List<t考試_考試總表>();

    public virtual ICollection<t課程_學生班級> t課程_學生班級 { get; set; } = new List<t課程_學生班級>();

    public virtual ICollection<t課程_班級科目> t課程_班級科目 { get; set; } = new List<t課程_班級科目>();

    public virtual t會員_老師 班級導師 { get; set; } = null!;
}
