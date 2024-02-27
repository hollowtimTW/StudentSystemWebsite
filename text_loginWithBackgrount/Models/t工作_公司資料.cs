using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t工作_公司資料
{
    public int fId { get; set; }

    public string f統一編號 { get; set; } = null!;

    public string f公司名稱 { get; set; } = null!;

    public string f帳號 { get; set; } = null!;

    public string f密碼 { get; set; } = null!;

    public string f帳號狀態 { get; set; } = null!;

    public string? f聯絡人 { get; set; }

    public string? f聯絡人電話 { get; set; }

    public string? f聯絡人Email { get; set; }

    public string? f負責人 { get; set; }

    public string? f公司電話 { get; set; }

    public string? f公司地址 { get; set; }

    public string? f公司簡介 { get; set; }

    public DateTime? f註冊時間 { get; set; }

    public DateTime? f審核通過時間 { get; set; }

    public DateTime? f最後更新時間 { get; set; }

    public string? f刪除狀態 { get; set; }

    public virtual ICollection<t工作_職缺資料> t工作_職缺資料 { get; set; } = new List<t工作_職缺資料>();

    public virtual ICollection<t通知_公司通知> t通知_公司通知 { get; set; } = new List<t通知_公司通知>();
}
