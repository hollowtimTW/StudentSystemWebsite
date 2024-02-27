using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t會員_老師
{
    public int 老師ID { get; set; }

    public string 姓名 { get; set; } = null!;

    public string? 性別 { get; set; }

    public string 身分證字號 { get; set; } = null!;

    public string 信箱 { get; set; } = null!;

    public string? 手機 { get; set; }

    public string? 地址 { get; set; }

    public string 密碼 { get; set; } = null!;

    public byte[]? 圖片 { get; set; }

    public DateTime? 生日 { get; set; }

    public DateTime 註冊日期 { get; set; }

    public DateTime 修改日期 { get; set; }

    public string 狀態 { get; set; } = null!;

    public string? 鎖定 { get; set; }

    public string? 其他登入 { get; set; }

    public string? 學校 { get; set; }

    public string? 科系 { get; set; }

    public string? 學位 { get; set; }

    public string? 畢肄 { get; set; }

    public string? salt { get; set; }

    public string? token { get; set; }

    public virtual ICollection<t公告_本體> t公告_本體 { get; set; } = new List<t公告_本體>();

    public virtual ICollection<t影片_問題單表> t影片_問題單表 { get; set; } = new List<t影片_問題單表>();

    public virtual ICollection<t會員_老師登入紀錄> t會員_老師登入紀錄 { get; set; } = new List<t會員_老師登入紀錄>();

    public virtual ICollection<t考試_試題總表> t考試_試題總表 { get; set; } = new List<t考試_試題總表>();

    public virtual ICollection<t課程_班級> t課程_班級 { get; set; } = new List<t課程_班級>();

    public virtual ICollection<t課程_班級科目> t課程_班級科目 { get; set; } = new List<t課程_班級科目>();

    public virtual ICollection<t課程_老師科目> t課程_老師科目 { get; set; } = new List<t課程_老師科目>();

    public virtual ICollection<t通知_老師通知> t通知_老師通知 { get; set; } = new List<t通知_老師通知>();
}
