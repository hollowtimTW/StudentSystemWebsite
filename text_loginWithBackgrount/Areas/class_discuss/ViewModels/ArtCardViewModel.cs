namespace text_loginWithBackgrount.Areas.class_discuss.ViewModels
{
    public class ArtCardViewModel
    {
        public int 文章id { get; set; }
        public int? 子版id { get; set; }
        public int? 看板id { get; set; }
        public string? 看板 { get; set; }
        public string? 分類 { get; set; }
        public string? 標題 { get; set; }
        public string? 內容 { get; set; }
        public string? 作者 { get; set; }
        public string? 日期 { get; set; }
        public string? 頭像 { get; set; }
    }
}