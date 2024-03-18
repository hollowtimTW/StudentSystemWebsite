﻿using Class_system_Backstage_pj.Areas.ordering_system.Models;
using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using text_loginWithBackgrount.Areas.ordering_system.Models.forStudentDTO;

namespace text_loginWithBackgrount.Areas.ordering_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentOrderAPIController : ControllerBase
    {
        private readonly studentContext _myDBContext;
        public StudentOrderAPIController(studentContext studentContext)
        {
            _myDBContext = studentContext;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_search"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult totalOrder([FromBody] StoreShow _search)
        {
            var studentID = _search.storeID;
            var spots = (from item in _myDBContext.T訂餐訂單資訊表s
                         join a in _myDBContext.T訂餐訂單詳細資訊表s on item.訂單id equals a.訂單id
                         join b in _myDBContext.T訂餐店家資料表s on a.店家id equals b.店家id
                         join c in _myDBContext.T訂餐餐點資訊表s on a.餐點id equals c.餐點id
                         join d in _myDBContext.T訂餐評論表s on item.訂單id equals d.訂單id
                         where item.學員id == studentID && item.訂單id == a.訂單id
                         group new { a, c, b, d } by new { item.訂單id, item.訂單狀態, item.訂單時間, item.支付方式, d.滿意度星數, d.評論 } into g
                         select new 學生訂單
                         {
                             訂單id = g.Key.訂單id,
                             狀態 = g.Key.訂單狀態.Trim(),
                             日期 = g.Key.訂單時間,
                             支付方式 = g.Key.支付方式.Trim(),
                             評價星數 = g.Key.滿意度星數.Trim() + "/5",
                             評價內容 = g.Key.評論,
                             總價 = "$" + Convert.ToInt32(g.Sum(x => x.a.餐點數量 * x.c.餐點定價)),
                             訂單詳細表 = g.Select(x => new 訂單詳細表
                             {
                                 店家名稱 = x.b.店家名稱,
                                 餐點名稱 = x.c.餐點名稱,
                                 數量 = x.a.餐點數量,
                                 小計 = Convert.ToInt32(x.a.餐點數量 * x.c.餐點定價),
                                 信箱 = x.b.電子信箱,
                                 電話 = x.b.電話.Trim()
                             }).ToList()
                         }).OrderByDescending(x => x.日期).ToList();
            if (!string.IsNullOrEmpty(_search.keyword))
            {
                spots = spots.Where(s => s.訂單id.ToString().Contains(_search.keyword) || (s.訂單詳細表.Select(a => a.店家名稱)).Contains(_search.keyword) || s.日期.Contains(_search.keyword)).ToList();
            }

            int totalCount = spots.Count(); //總共幾筆
            int pagesize = _search.pageSize ?? 10; //一頁有幾筆資料
            int page = _search.page ?? 1; //目前顯示哪一頁
            int totalpage = (int)Math.Ceiling((decimal)(totalCount) / pagesize); //將小數點無條件進位，
            spots = spots.Skip((int)(page - 1) * pagesize).Take(pagesize).ToList();
            //轉換輸出格式，為了前端需求輸出
            StudentPagingDTO sportsPagingDTO = new StudentPagingDTO();
            sportsPagingDTO.TotalPages = totalpage;
            sportsPagingDTO.spotImagesSpots = spots.ToList();
            return Ok(sportsPagingDTO);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">學員ID</param>
        /// <param name="storeID">店家ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("favoriteStore")]
        public async Task<IActionResult> FavoriteStore(int id, int storeID)
        {
            var result = await _myDBContext.T訂餐學員最愛商家表s
                .FirstOrDefaultAsync(a => a.學員id == id && a.店家id == storeID);

            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                T訂餐學員最愛商家表 newStore = new T訂餐學員最愛商家表
                {
                    學員id = id,
                    店家id = storeID
                };
                _myDBContext.T訂餐學員最愛商家表s.Add(newStore);
                await _myDBContext.SaveChangesAsync();
            }
            return Ok();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">學員ID</param>
        /// <returns></returns>https://localhost:7150/api/StudentOrderAPI/favoriteStorelist?id=7
        [HttpGet]
        [Route("favoriteStorelist")]
        public IActionResult FavoriteStore(int id)
        {

            var result = (from item in _myDBContext.T訂餐學員最愛商家表s
                         join a in _myDBContext.T訂餐店家資料表s on item.店家id equals a.店家id
                         where item.學員id == id
                         select item.店家id).ToList();
            var resultdetail = from item in _myDBContext.T訂餐學員最愛商家表s
                               join a in _myDBContext.T訂餐店家資料表s on item.店家id equals a.店家id
                               where item.學員id == id
                               select new {
                                   店家ID=a.店家id,
                                   店家名稱=a.店家名稱,
                                   店家圖片= a.餐廳照片 ?? "/images/user.jpg",
                                   風味列表 = (from tagItem in _myDBContext.T訂餐店家資料表s
                                           join tagE in _myDBContext.T訂餐店家風味表s on tagItem.店家id equals tagE.店家id
                                           join tagF in _myDBContext.T訂餐口味總表s on tagE.口味id equals tagF.口味id
                                           where tagItem.店家名稱 == a.店家名稱
                                           select tagF.風味名稱).ToList()
                               };
            var object01= new {
                店家列表 = result,
                店家資料= resultdetail
            };
            return Ok(object01);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">學生ID</param>
        /// <param name="storeID">店家ID</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("favoriteStoredelete")]
        public IActionResult favoriteStoredelete(int id, int storeID) 
        {
            var result = _myDBContext.T訂餐學員最愛商家表s.Where(a => a.學員id == id && a.店家id == storeID).FirstOrDefault();
            if (result != null) {
                _myDBContext.T訂餐學員最愛商家表s.Remove(result);
                _myDBContext.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }
    }
}
