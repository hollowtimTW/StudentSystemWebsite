﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Class_system_Backstage_pj.Models;

public partial class TQuizQuestion
{
    public int FQuestionId { get; set; }

    public string FQuestion { get; set; }

    public byte[] FImages { get; set; }

    public int? FType { get; set; }

    public string FOption1 { get; set; }

    public string FOption2 { get; set; }

    public string FOption3 { get; set; }

    public string FOption4 { get; set; }

    public string FOption5 { get; set; }

    public string FOption6 { get; set; }

    public string FAnswer { get; set; }

    public int? FScore { get; set; }

    public virtual ICollection<TQuizPaper> TQuizPapers { get; set; } = new List<TQuizPaper>();
}