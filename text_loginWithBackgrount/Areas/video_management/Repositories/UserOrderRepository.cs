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
            var orders = await _db.Orders
                            .Include(x => x.OrderStatus)
                            .Include(x => x.OrderDetail)
                            .ThenInclude(x => x.Book)
                            .ThenInclude(x => x.Genre)
                            .Where(a => a.UserId == userId)
                            .ToListAsync();
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
