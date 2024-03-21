using Class_system_Backstage_pj.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace text_loginWithBackgrount.Areas.video_management.Repositories
{
    public class UserOrderRepository : IUserOrderRepository
    {
        public readonly IHttpContextAccessor _httpContextAccessor;
        private readonly studentContext _studentContext;

        public UserOrderRepository(studentContext studentContext, IHttpContextAccessor httpContextAccessor)
        {
            _studentContext = studentContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<T影片Order>> UserOrders()
        {
            var userId = GetUserId();
            if (userId == 0)
                throw new Exception("User is not logged-in");

            var today = DateTime.Today; // 获取今天的日期

            var orders = await _studentContext.T影片Orders
                            .Include(x => x.FOrderStatus)
                            .Include(x => x.T影片OrderDetails)
                            .ThenInclude(x => x.FVideo)
                            .ThenInclude(x => x.Genre)
                            .Where(a => a.FStudentId == userId)
                            .ToListAsync(); // 将查询结果加载到内存中

            // 先按创建日期倒序排序
            orders = orders.OrderByDescending(a => a.CreateDate.Value).ToList();

            // 将今天的订单移到列表的顶部
            var todayOrders = orders.Where(a => a.CreateDate.Value.Date == today).ToList();
            var otherOrders = orders.Except(todayOrders).ToList();
            orders = todayOrders.Concat(otherOrders).ToList();

            return orders;
        }
        private int GetUserId()
        {
            string userId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(p => p.Type == "StudentId")?.Value;
            int studentId;
            int.TryParse(userId, out studentId);
            return studentId;
        }
    }
}
