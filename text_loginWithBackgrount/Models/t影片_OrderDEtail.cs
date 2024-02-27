using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t影片_OrderDEtail
{
    public int fSeqNO { get; set; }

    public int fOrderId { get; set; }

    public int fVideoId { get; set; }

    public decimal fPrice { get; set; }

    public string fType { get; set; } = null!;

    public virtual t影片_Order fOrder { get; set; } = null!;

    public virtual t影片_Video fVideo { get; set; } = null!;

    public virtual ICollection<t影片_問題單表> t影片_問題單表 { get; set; } = new List<t影片_問題單表>();
}
