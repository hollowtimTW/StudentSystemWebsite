using Microsoft.AspNetCore.Mvc.RazorPages;
using static text_loginWithBackgrount.Areas.course_management.Model.RateModel;

namespace text_loginWithBackgrount.Areas.course_management.Model
{
    public class RateModel
    {
        public List<評分類別> 評分類別列表 { get; set; }

        public RateModel()
        {
             評分類別列表 = new List<評分類別>();

            AddCategory("課程滿意度", new List<string> {
                "這門課程對您是否有幫助？",
                "您對這門課程的教學內容是否滿意?",
                "是否認為課程的內容與您的預期相符？",
                "是否認為課程的教學教材和設備周邊符合課程"
            });

            AddCategory("教師滿意度", new List<string> {
                "您覺得老師在課堂上的表現如何？",
                "您覺得老師對於學生提出的問題能夠及時和有效地回應嗎？",
                "老師是否使用多種教學方法和教材來幫助您理解課程內容嗎？",
                "您覺得老師的教學對於您的學習效果有何影響"
            });

            AddCategory("學習成效度", new List<string> {
                "請您評估自己在這門課程中的學習進步？",
                "課程的編排和時間上是否讓您能夠有效吸收課程內容？",
                "您認為自己在課程中達到了預期的學習目標嗎？",
                "您能夠應用課堂上所學的知識和技能於實際情況中嗎"
            });

        }

        public void AddCategory(string 分類名稱, List<string> 題目內容列表)
        {
            var 分類 = new 評分類別
            {
                分類名稱 = 分類名稱,
                題目 = new List<評分項目>()
            };

            foreach (var 題目內容 in 題目內容列表)
            {
                分類.題目.Add(new 評分項目 { 題目內容 = 題目內容 });
            }

            評分類別列表.Add(分類);
        }


        public class 評分類別
        {
            public string 分類名稱 { get; set; }
            public List<評分項目> 題目 { get; set; }
        }

        public class 評分項目
        {
            public string 題目內容 { get; set; }
        }

       

    }
}
