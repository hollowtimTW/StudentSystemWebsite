using Class_system_Backstage_pj.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace text_loginWithBackgrount.Areas.video_management.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly studentContext _studentContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartRepository(studentContext studentContext, IHttpContextAccessor httpContextAccessor)
        {
            _studentContext = studentContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<int> AddItem(int FVideoId, int qty)
        {

            int userId = GetUserId();
        

            using var transaction = _studentContext.Database.BeginTransaction();
            try
            {
              
                //if (string.IsNullOrEmpty(userId))
                //    throw new Exception("user is not logged-in");
                var cart = await GetCart(userId);
                if (cart is null)
                {
                    cart = new T影片ShoppingCart
                    {
                        FStudentId = userId
                    };
                    _studentContext.T影片ShoppingCarts.Add(cart);
                }
                _studentContext.SaveChanges();
                // cart detail section
                var cartItem = _studentContext.T影片CartDetails
                                  .FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.FVideoId == FVideoId);
                if (cartItem is not null)
                {
                    cartItem.Quantity += qty;
                }
                else
                {
                    var video = _studentContext.T影片Videos.Find(FVideoId);
                    cartItem = new T影片CartDetail
                    {
                        FVideoId = FVideoId,
                        ShoppingCartId = cart.Id,
                        Quantity = qty,
                        UnitPrice = (double)video.FPrice  // it is a new line after update
                    };
                    _studentContext.T影片CartDetails.Add(cartItem);
                }
                _studentContext.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }
        public async Task<int> RemoveItem(int FVideoId)
        {
            //using var transaction = _db.Database.BeginTransaction();
            int userId = GetUserId();
            try
            {
                if (userId != 0)
                    throw new Exception("user is not logged-in");
                var cart = await GetCart(userId);
                if (cart is null)
                    throw new Exception("Invalid cart");
                // cart detail section
                var cartItem = _studentContext.T影片CartDetails
                                  .FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.FVideoId == FVideoId);
                if (cartItem is null)
                    throw new Exception("Not items in cart");
                else if (cartItem.Quantity == 1)
                    _studentContext.T影片CartDetails.Remove(cartItem);
                else
                    cartItem.Quantity = cartItem.Quantity - 1;
                _studentContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        public async Task<T影片ShoppingCart> GetUserCart()
        {
            var userId = GetUserId();
            if (userId == null)
                throw new Exception("Invalid userid");
            var shoppingCart = await _studentContext.T影片ShoppingCarts
                                  .Include(a => a.T影片CartDetails)
                                  .ThenInclude(a => a.FVideo)
                                  .ThenInclude(a => a.Genre)
                                  .Where(a => a.FStudentId == userId).FirstOrDefaultAsync();
            return shoppingCart;
        }
        public async Task<T影片ShoppingCart> GetCart(int userId)
        {
            var cart = await _studentContext.T影片ShoppingCarts.FirstOrDefaultAsync(x => x.FStudentId == userId);
            return cart;
        }
        public async Task<int> GetCartItemCount(int userId)
        {
            if (userId!=0) /*(string.IsNullOrEmpty(userId))*/ // updated line
            {
                userId = GetUserId();
            }
            var count = await (from cart in _studentContext.T影片ShoppingCarts
                               join cartDetail in _studentContext.T影片CartDetails
                               on cart.Id equals cartDetail.ShoppingCartId
                               where cart.FStudentId == userId
                               select cartDetail.Id).CountAsync(); // CountAsync() directly counts the items

            return count;


        }

        public async Task<bool> DoCheckout()
        {
            using var transaction = _studentContext.Database.BeginTransaction();
            try
            {
                // logic
                // move data from cartDetail to order and order detail then we will remove cart detail
                var userId = GetUserId();
                if (userId != 0)
                    throw new Exception("User is not logged-in");
                var cart = await GetCart(userId);
                if (cart is null)
                    throw new Exception("Invalid cart");
                var cartDetail = _studentContext.T影片CartDetails
                                    .Where(a => a.ShoppingCartId == cart.Id).ToList();
                if (cartDetail.Count == 0)
                    throw new Exception("Cart is empty");
                var order = new T影片Order
                {
                    FStudentId = userId,
                    CreateDate = DateTime.UtcNow,
                    FOrderOrderStatusId = 1//pending
                };
                _studentContext.T影片Orders.Add(order);
                _studentContext.SaveChanges();
                foreach (var item in cartDetail)
                {
                    var orderDetail = new T影片OrderDetail
                    {
                       FVideoId = item.FVideoId,
                        FOrderId = order.FOrderId,
                        FQuantity = item.Quantity,//3013要改資料庫把string Quantity改成int
                        FUnitPrice = item.UnitPrice
                    };
                    _studentContext.T影片OrderDetails.Add(orderDetail);
                }
                _studentContext.SaveChanges();

                // removing the cartdetails
                _studentContext.T影片CartDetails.RemoveRange(cartDetail);
                _studentContext.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }


        private int GetUserId()
        {
            string userId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(p=>p.Type=="StudentId")?.Value;
            int studentId;
            int.TryParse(userId, out studentId);
            return studentId;
        }

     
    }
}
