﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Class_system_Backstage_pj.Models;

public partial class T公告本體
{
    public int 公告id { get; set; }

    public string 內容 { get; set; }

    public DateTime? 時間 { get; set; }

    public int? 觀看數 { get; set; }

    public string 標題 { get; set; }

    public string 重要程度 { get; set; }

    public int? 老師id { get; set; }

    public int? 分類id { get; set; }

    public virtual T公告分類 分類 { get; set; }

    public virtual T會員老師 老師 { get; set; }
}