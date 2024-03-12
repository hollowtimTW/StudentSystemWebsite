using Class_system_Backstage_pj.Models;

namespace text_loginWithBackgrount.Areas.video_management.Repositories
{
    public class CartRepository: ICartRepository
    {
        private readonly studentContext _studentContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartRepository(studentContext studentContext, IHttpContextAccessor httpContextAccessor)
        {
            _studentContext = studentContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<int> AddItem(int videoId, int qty)
        {
            string userId = GetUserId();
            int studentId;
            int.TryParse(userId, out studentId);


            using var transaction = _studentContext.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("user is not logged-in");
                var cart = await GetCart(userId);
                if (cart is null)
                {
                    cart = new T影片ShoppingCart
                    {
                        FStudentId = studentId
                    };
                    _studentContext.T影片ShoppingCarts.Add(cart);
                }
                _studentContext.SaveChanges();
                // cart detail section
                var cartItem = _studentContext.T影片CartDetails
                                  .FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.FVideoId == videoId);
                if (cartItem is not null)
                {
                    cartItem.Quantity += qty;
                }
                else
                {
                    var video = _studentContext.T影片Videos.Find(videoId);
                    cartItem = new T影片CartDetail
                    {
                        FVideoId = videoId,
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

        private async Task<T影片ShoppingCart> GetCart(string userId)
        {
            throw new NotImplementedException();
        }

        private string GetUserId()
        {
            string userId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(p=>p.Type=="StudentId")?.Value;
            return userId;
        }

        private async Task<int> GetCartItemCount(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
