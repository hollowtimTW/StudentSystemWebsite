using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t公告_本體
{
    public int 公告ID { get; set; }

    public string? 內容 { get; set; }

    public DateTime? 時間 { get; set; }

    public int? 觀看數 { get; set; }

    public string? 標題 { get; set; }

    public string? 重要程度 { get; set; }

    public int? 老師ID { get; set; }

    public int? 分類ID { get; set; }

    public virtual t公告_分類? 分類 { get; set; }

    public virtual t會員_老師? 老師 { get; set; }
}
