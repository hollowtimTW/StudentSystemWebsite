using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t課程_科目分類
{
    public int 科目類別ID { get; set; }

    public string 科目類別名稱 { get; set; } = null!;

    public string? 科目類別封面 { get; set; }

    public int 狀態 { get; set; }

    public virtual ICollection<t課程_科目> t課程_科目 { get; set; } = new List<t課程_科目>();
}
