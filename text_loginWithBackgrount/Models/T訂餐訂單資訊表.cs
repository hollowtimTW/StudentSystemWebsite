﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Class_system_Backstage_pj.Models;

public partial class T訂餐訂單資訊表
{
    public int 訂單id { get; set; }

    public string 訂單狀態 { get; set; }

    public int 學員id { get; set; }

    public string 訂單時間 { get; set; }

    public string 支付方式 { get; set; }

    public virtual ICollection<T訂餐訂單詳細資訊表> T訂餐訂單詳細資訊表s { get; set; } = new List<T訂餐訂單詳細資訊表>();

    public virtual ICollection<T訂餐評論表> T訂餐評論表s { get; set; } = new List<T訂餐評論表>();

    public virtual T會員學生 學員 { get; set; }
}