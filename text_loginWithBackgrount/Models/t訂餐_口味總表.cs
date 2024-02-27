using System;
using System.Collections.Generic;

namespace text_loginWithBackgrount.Models;

public partial class t訂餐_口味總表
{
    public int 口味ID { get; set; }

    public string 風味名稱 { get; set; } = null!;

    public virtual ICollection<t訂餐_學員口味表> t訂餐_學員口味表 { get; set; } = new List<t訂餐_學員口味表>();

    public virtual ICollection<t訂餐_店家風味表> t訂餐_店家風味表 { get; set; } = new List<t訂餐_店家風味表>();
}
