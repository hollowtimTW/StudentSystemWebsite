﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Class_system_Backstage_pj.Models;

public partial class T工作推薦職缺
{
    public int FId { get; set; }

    public int F學員Id { get; set; }

    public int F職缺Id { get; set; }

    public string F推薦程度 { get; set; }

    public virtual T會員學生 F學員 { get; set; }

    public virtual T工作職缺資料 F職缺 { get; set; }
}