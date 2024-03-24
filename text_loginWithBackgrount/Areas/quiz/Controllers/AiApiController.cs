using Microsoft.AspNetCore.Mvc;
using Claudia;
using Azure;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text;
using Org.BouncyCastle.Asn1.Ocsp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MongoDB.Bson;
using OpenAI_API.Chat;

namespace text_loginWithBackgrount.Areas.quiz.Controllers
{

    [Route("Api/Ai/[action]")]
    public class AiApiController : Controller
    {

        // Claude
        [HttpPost]
        public async Task<IActionResult> SendClaude([FromForm] string content)
        {
            var anthropic = new Anthropic
            {
                ApiKey = "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"
            };
            try
            {
                var message = await anthropic.Messages.CreateAsync(new()
                {
                    Model = "claude-3-opus-20240229",
                    MaxTokens = 1024,
                    Messages = [new() { Role = "user", Content = content }]
                });
                var text = message?.Content?[0]?.Text?.ToString();

                return Json(text);
            }
            catch (Exception ex)
            {
                return Content("在試一次，換嘗試其他模型");
            }

        }

        // Gemini
        [HttpPost]
        public async Task<IActionResult> SendGemini([FromForm] string content)
        {
            var ApiKey = "AIzaSyCr6ZEDPO1o5rRs5Lmc8-wj7lBB2DpIS0o";
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={ApiKey}";

            var jsonContent = $"{{\"contents\":[{{\"parts\":[{{\"text\":\"{content}\"}}]}}]}}";

            using (var httpClient = new HttpClient())
            {
                var content_ = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(url, content_);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonString);
                    var text = jsonObject?.candidates?[0]?.content?.parts?[0]?.text?.ToString();
                    return Content(text);
                }
                else
                {
                    Console.WriteLine($"Failed to make request. Status code: {response.StatusCode}");
                    return Content("在試一次，換嘗試其他模型");
                }
            }
        }


        // OpenAi
        [HttpPost]
        public async Task<IActionResult> SendOpenAi([FromForm] string content)
        {
            try
            {
                var ApiKey = "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";
                var api = new OpenAI_API.OpenAIAPI(ApiKey);
                var result = await api.Chat.CreateChatCompletionAsync(content);
                var text = result.Choices[0].Message.Content;

                return Content(text);
            }
            catch (Exception ex)
            {
                return Content("在試一次，換嘗試其他模型");
            }

        }

    }


}
