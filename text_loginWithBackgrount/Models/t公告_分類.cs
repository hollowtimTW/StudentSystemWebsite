using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t公告_分類
{
    public int 分類ID { get; set; }

    public string? 名稱 { get; set; }

    public virtual ICollection<t公告_本體> t公告_本體 { get; set; } = new List<t公告_本體>();
}
