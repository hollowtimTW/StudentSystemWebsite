﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Class_system_Backstage_pj.Models;

public partial class T訂餐餐點資訊表
{
    public int 餐點id { get; set; }

    public int 店家id { get; set; }

    public string 餐點名稱 { get; set; }

    public string 餐點描述 { get; set; }

    public decimal? 餐點定價 { get; set; }

    public string 餐點照片 { get; set; }
    public string 上架 { get; set; }

    public virtual ICollection<T訂餐訂單詳細資訊表> T訂餐訂單詳細資訊表s { get; set; } = new List<T訂餐訂單詳細資訊表>();

    public virtual T訂餐店家資料表 店家 { get; set; }
}