using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t工作_履歷資料
{
    public int fId { get; set; }

    public int f學員ID { get; set; }

    public string f履歷名稱 { get; set; } = null!;

    public string f履歷狀態 { get; set; } = null!;

    public string? f專長技能 { get; set; }

    public string? f語文能力 { get; set; }

    public string f有無工作經驗 { get; set; } = null!;

    public string? f工作性質 { get; set; }

    public string? f工作時段 { get; set; }

    public string? f配合輪班 { get; set; }

    public string? f希望職稱 { get; set; }

    public string? f希望薪水待遇 { get; set; }

    public string? f希望工作地點 { get; set; }

    public string? f自傳 { get; set; }

    public DateTime? f建立時間 { get; set; }

    public DateTime? f最後更新時間 { get; set; }

    public string? f刪除狀態 { get; set; }

    public string? f刪除或關閉原因 { get; set; }

    public virtual t會員_學生 f學員 { get; set; } = null!;

    public virtual ICollection<t工作_履歷表工作經驗> t工作_履歷表工作經驗 { get; set; } = new List<t工作_履歷表工作經驗>();
}
