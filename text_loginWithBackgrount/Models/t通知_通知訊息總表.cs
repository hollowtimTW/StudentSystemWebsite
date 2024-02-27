using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t通知_通知訊息總表
{
    public int fId { get; set; }

    public string f發送者類型 { get; set; } = null!;

    public string f接收者類型 { get; set; } = null!;

    public string? f訊息類別 { get; set; }

    public string? f內容 { get; set; }

    public DateTime? f發送時間 { get; set; }

    public int? f父訊息ID { get; set; }

    public string? f刪除狀態 { get; set; }

    public virtual ICollection<t通知_通知訊息總表> Inversef父訊息 { get; set; } = new List<t通知_通知訊息總表>();

    public virtual t通知_通知訊息總表? f父訊息 { get; set; }

    public virtual ICollection<t工作_應徵工作紀錄> t工作_應徵工作紀錄 { get; set; } = new List<t工作_應徵工作紀錄>();

    public virtual ICollection<t通知_公司通知> t通知_公司通知 { get; set; } = new List<t通知_公司通知>();

    public virtual ICollection<t通知_學生通知> t通知_學生通知 { get; set; } = new List<t通知_學生通知>();

    public virtual ICollection<t通知_老師通知> t通知_老師通知 { get; set; } = new List<t通知_老師通知>();
}
