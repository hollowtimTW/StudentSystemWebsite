﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Class_system_Backstage_pj.Models;

public partial class T課程通知表
{
    public int 訊息id { get; set; }

    public string 發送者類型 { get; set; }

    public int 發送者id { get; set; }

    public string 接收者類型 { get; set; }

    public int 接收者id { get; set; }

    public string 發送訊息內容 { get; set; }

    public int 狀態 { get; set; }

    public DateTime 時間 { get; set; }
}