using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t考試_考試總表
{
    public int 考試ID { get; set; }

    public string? 考試名稱 { get; set; }

    public string? 考試類型 { get; set; }

    public bool? 描述 { get; set; }

    public string? 備註 { get; set; }

    public int? 試卷ID { get; set; }

    public int? 班級ID { get; set; }

    public DateTime? 開始時間 { get; set; }

    public DateTime? 結束時間 { get; set; }

    public int? 發布者 { get; set; }

    public int? 提交次數 { get; set; }

    public virtual ICollection<t考試_學生成績> t考試_學生成績 { get; set; } = new List<t考試_學生成績>();

    public virtual ICollection<t考試_學生答題> t考試_學生答題 { get; set; } = new List<t考試_學生答題>();

    public virtual ICollection<t考試_考試權限> t考試_考試權限 { get; set; } = new List<t考試_考試權限>();

    public virtual t課程_班級? 班級 { get; set; }

    public virtual t考試_試題總表? 試卷 { get; set; }
}
