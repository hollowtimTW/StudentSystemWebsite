using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t會員_學生
{
    public int 學生ID { get; set; }

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

    public virtual ICollection<t工作_儲存工作紀錄> t工作_儲存工作紀錄 { get; set; } = new List<t工作_儲存工作紀錄>();

    public virtual ICollection<t工作_履歷資料> t工作_履歷資料 { get; set; } = new List<t工作_履歷資料>();

    public virtual ICollection<t工作_工作經驗> t工作_工作經驗 { get; set; } = new List<t工作_工作經驗>();

    public virtual ICollection<t工作_應徵工作紀錄> t工作_應徵工作紀錄 { get; set; } = new List<t工作_應徵工作紀錄>();

    public virtual ICollection<t影片_Order> t影片_Order { get; set; } = new List<t影片_Order>();

    public virtual ICollection<t影片_問題單表> t影片_問題單表 { get; set; } = new List<t影片_問題單表>();

    public virtual ICollection<t會員_出勤> t會員_出勤 { get; set; } = new List<t會員_出勤>();

    public virtual ICollection<t會員_學生登入紀錄> t會員_學生登入紀錄 { get; set; } = new List<t會員_學生登入紀錄>();

    public virtual ICollection<t考試_學生成績> t考試_學生成績 { get; set; } = new List<t考試_學生成績>();

    public virtual ICollection<t考試_學生答題> t考試_學生答題 { get; set; } = new List<t考試_學生答題>();

    public virtual ICollection<t考試_考試權限> t考試_考試權限 { get; set; } = new List<t考試_考試權限>();

    public virtual ICollection<t訂餐_學員口味表> t訂餐_學員口味表 { get; set; } = new List<t訂餐_學員口味表>();

    public virtual ICollection<t訂餐_學員最愛商家表> t訂餐_學員最愛商家表 { get; set; } = new List<t訂餐_學員最愛商家表>();

    public virtual ICollection<t訂餐_訂單資訊表> t訂餐_訂單資訊表 { get; set; } = new List<t訂餐_訂單資訊表>();

    public virtual ICollection<t討論_文章> t討論_文章 { get; set; } = new List<t討論_文章>();

    public virtual ICollection<t討論_文章點讚> t討論_文章點讚 { get; set; } = new List<t討論_文章點讚>();

    public virtual ICollection<t討論_留言> t討論_留言 { get; set; } = new List<t討論_留言>();

    public virtual ICollection<t討論_留言點讚> t討論_留言點讚 { get; set; } = new List<t討論_留言點讚>();

    public virtual ICollection<t課程_學生班級> t課程_學生班級 { get; set; } = new List<t課程_學生班級>();

    public virtual ICollection<t課程_評分主表> t課程_評分主表 { get; set; } = new List<t課程_評分主表>();

    public virtual ICollection<t課程_課程> t課程_課程 { get; set; } = new List<t課程_課程>();

    public virtual ICollection<t通知_學生通知> t通知_學生通知 { get; set; } = new List<t通知_學生通知>();
}
