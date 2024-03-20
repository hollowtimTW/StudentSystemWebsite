using Class_system_Backstage_pj.Models;

namespace text_loginWithBackgrount.Areas.video_management.Repositories
{
    public interface IUserOrderRepository
    {
        Task<IEnumerable<T影片Order>> UserOrders();
    }
}
