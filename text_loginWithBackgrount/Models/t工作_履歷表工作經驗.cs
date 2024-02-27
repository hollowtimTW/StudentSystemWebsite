using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t工作_履歷表工作經驗
{
    public int fId { get; set; }

    public int f履歷ID { get; set; }

    public int f工作經驗ID { get; set; }

    public virtual t工作_履歷資料 f履歷 { get; set; } = null!;

    public virtual t工作_工作經驗 f工作經驗 { get; set; } = null!;
}
