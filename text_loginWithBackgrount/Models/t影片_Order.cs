using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t影片_Order
{
    public int fOrderId { get; set; }

    public int fStudentId { get; set; }

    public DateTime fDate { get; set; }

    public virtual t會員_學生 fStudent { get; set; } = null!;

    public virtual ICollection<t影片_OrderDEtail> t影片_OrderDEtail { get; set; } = new List<t影片_OrderDEtail>();
}
