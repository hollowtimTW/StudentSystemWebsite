﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Class_system_Backstage_pj.Models;

public partial class T影片CartDetail
{
    public int Id { get; set; }

    public int ShoppingCartId { get; set; }

    public int FVideoId { get; set; }

    public int Quantity { get; set; }

    public double UnitPrice { get; set; }

    public virtual T影片Video FVideo { get; set; }

    public virtual T影片ShoppingCart ShoppingCart { get; set; }
}