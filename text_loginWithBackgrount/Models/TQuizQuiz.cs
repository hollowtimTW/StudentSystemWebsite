﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Class_system_Backstage_pj.Models;

public partial class TQuizQuiz
{
    public int FQuizId { get; set; }

    public string FQname { get; set; }

    public string FNote { get; set; }

    public string FQcode { get; set; }

    public int? FLimitTime { get; set; }

    public int? FTeacherId { get; set; }

    public int? FPublic { get; set; }

    public int? FClosed { get; set; }

    public DateTime? FCreateTime { get; set; }

    public virtual T會員老師 FTeacher { get; set; }

    public virtual ICollection<TQuizRecord> TQuizRecords { get; set; } = new List<TQuizRecord>();
}