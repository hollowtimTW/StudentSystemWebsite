﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Class_system_Backstage_pj.Models;

public partial class T討論留言
{
    public int 留言id { get; set; }

    public int? 學生id { get; set; }

    public string 內容 { get; set; }

    public string 時間 { get; set; }

    public int? 文章id { get; set; }

    public int? 子版id { get; set; }

    public int? 看板id { get; set; }

    public string 刪除 { get; set; }

    public virtual ICollection<T討論留言點讚> T討論留言點讚s { get; set; } = new List<T討論留言點讚>();

    public virtual T討論子版 子版 { get; set; }

    public virtual T會員學生 學生 { get; set; }

    public virtual T討論文章 文章 { get; set; }

    public virtual T討論看板 看板 { get; set; }
}