using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t工作_工作經驗
{
    public int fId { get; set; }

    public int f學員ID { get; set; }

    public string f公司名稱 { get; set; } = null!;

    public string f職務名稱 { get; set; } = null!;

    public string f起始年月 { get; set; } = null!;

    public string f結束年月 { get; set; } = null!;

    public string f薪水待遇 { get; set; } = null!;

    public string f工作內容 { get; set; } = null!;

    public virtual t會員_學生 f學員 { get; set; } = null!;

    public virtual ICollection<t工作_履歷表工作經驗> t工作_履歷表工作經驗 { get; set; } = new List<t工作_履歷表工作經驗>();
}
