using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t通知_公司通知
{
    public int fId { get; set; }

    public int f公司ID { get; set; }

    public int f通知總表ID { get; set; }

    public virtual t工作_公司資料 f公司 { get; set; } = null!;

    public virtual t通知_通知訊息總表 f通知總表 { get; set; } = null!;
}
