﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Class_system_Backstage_pj.Models;

public partial class T影片OrderDetail
{
    public int FSeqNo { get; set; }

    public int FOrderId { get; set; }

    public int FVideoId { get; set; }

    public decimal FPrice { get; set; }

    public string FType { get; set; }

    public virtual T影片Order FOrder { get; set; }

    public virtual T影片Video FVideo { get; set; }

    public virtual ICollection<T影片問題單表> T影片問題單表s { get; set; } = new List<T影片問題單表>();
}