using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t影片_問題單表
{
    public int fId { get; set; }

    public string fDate { get; set; } = null!;

    public int fStudentId { get; set; }

    public int fTeacherId { get; set; }

    public string? fSubject { get; set; }

    public string? fProgress { get; set; }

    public int? fParentId { get; set; }

    public string? fType { get; set; }

    public int fSeqNO { get; set; }

    public virtual t影片_OrderDEtail fSeqNONavigation { get; set; } = null!;

    public virtual t會員_學生 fStudent { get; set; } = null!;

    public virtual t會員_老師 fTeacher { get; set; } = null!;
}
