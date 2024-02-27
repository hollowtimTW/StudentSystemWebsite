using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t工作_職缺資料
{
    public int fId { get; set; }

    public string f職務名稱 { get; set; } = null!;

    public string f職缺狀態 { get; set; } = null!;

    public int f公司ID { get; set; }

    public string? f薪水待遇 { get; set; }

    public string? f工作性質 { get; set; }

    public string? f工作地點 { get; set; }

    public string? f工作時段 { get; set; }

    public string? f輪班需求 { get; set; }

    public int? f需求人數 { get; set; }

    public string? f工作內容 { get; set; }

    public string? f學歷要求 { get; set; }

    public string? f語文條件 { get; set; }

    public string? f工作技能 { get; set; }

    public string? f其他條件 { get; set; }

    public DateTime? f建立時間 { get; set; }

    public DateTime? f最後更新時間 { get; set; }

    public string? f刪除狀態 { get; set; }

    public string? f刪除或關閉原因 { get; set; }

    public virtual t工作_公司資料 f公司 { get; set; } = null!;

    public virtual ICollection<t工作_儲存工作紀錄> t工作_儲存工作紀錄 { get; set; } = new List<t工作_儲存工作紀錄>();

    public virtual ICollection<t工作_應徵工作紀錄> t工作_應徵工作紀錄 { get; set; } = new List<t工作_應徵工作紀錄>();
}
