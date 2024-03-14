using Class_system_Backstage_pj.Models;

namespace text_loginWithBackgrount.Areas.video_management.Repositories
{
    public interface ICartRepository
    {
        Task<int> AddItem(int videoId, int qty);
        Task<int> RemoveItem(int videoId);
        Task<T影片ShoppingCart> GetUserCart();
        Task<int> GetCartItemCount(int userId=0);
        Task<T影片ShoppingCart> GetCart(int userId);
        Task<bool> DoCheckout();
    }
}
