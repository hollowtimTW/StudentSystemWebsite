﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Class_system_Backstage_pj.Models;

public partial class T影片Genre
{
    public int Id { get; set; }

    public string GenreName { get; set; }

    public virtual ICollection<T影片Video> T影片Videos { get; set; } = new List<T影片Video>();
}