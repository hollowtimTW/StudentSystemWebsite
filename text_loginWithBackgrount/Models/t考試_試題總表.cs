using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t考試_試題總表
{
    public int 試卷ID { get; set; }

    public string? 備註 { get; set; }

    public int? 科目ID { get; set; }

    public int? 老師ID { get; set; }

    public string? 試題 { get; set; }

    public string? 答案 { get; set; }

    public int? 試卷題數 { get; set; }

    public DateTime? 創建時間 { get; set; }

    public DateTime? 更新時間 { get; set; }

    public bool? 共享 { get; set; }

    public virtual ICollection<t考試_考試總表> t考試_考試總表 { get; set; } = new List<t考試_考試總表>();

    public virtual t課程_科目? 科目 { get; set; }

    public virtual t會員_老師? 老師 { get; set; }
}
