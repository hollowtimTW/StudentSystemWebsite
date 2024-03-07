using Microsoft.AspNetCore.Mvc;


namespace text_loginWithBackgrount.Components
{
    public class Chat3ViewComponent: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
