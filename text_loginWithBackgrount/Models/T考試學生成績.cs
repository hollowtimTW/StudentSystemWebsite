﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Class_system_Backstage_pj.Models;

public partial class T考試學生成績
{
    public int Id { get; set; }

    public int? 學生id { get; set; }

    public int? 考試id { get; set; }

    public double? 原始成績 { get; set; }

    public double? 成績 { get; set; }

    public int? 狀態 { get; set; }

    public string 備註 { get; set; }

    public DateTime? 更新時間 { get; set; }

    public virtual T會員學生 學生 { get; set; }

    public virtual T考試考試總表 考試 { get; set; }
}