using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t課程_科目
{
    public int 科目ID { get; set; }

    public string 科目名稱 { get; set; } = null!;

    public int 科目類別ID { get; set; }

    public int 狀態 { get; set; }

    public virtual ICollection<t影片_Video> t影片_Video { get; set; } = new List<t影片_Video>();

    public virtual ICollection<t考試_試題總表> t考試_試題總表 { get; set; } = new List<t考試_試題總表>();

    public virtual ICollection<t課程_班級科目> t課程_班級科目 { get; set; } = new List<t課程_班級科目>();

    public virtual ICollection<t課程_老師科目> t課程_老師科目 { get; set; } = new List<t課程_老師科目>();

    public virtual t課程_科目分類 科目類別 { get; set; } = null!;
}
