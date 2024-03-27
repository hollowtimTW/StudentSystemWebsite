
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using text_loginWithBackgrount.Areas.video_management.LineBotMessage.Dtos;
using text_loginWithBackgrount.Areas.video_management.LineBotMessage.Providers;
using Newtonsoft.Json;





namespace text_loginWithBackgrount.Areas.video_management.LineBotMessage.Domain
{
    public class LinePayService
    {
        public LinePayService()
        {
            client = new HttpClient();
            _jsonProvider = new JsonProvider();
        }

        private readonly string channelId = "2004344523";
        private readonly string channelSecretKey = "6db6e67b698382ac06b289b2f9d458ad";


        private readonly string linePayBaseApiUrl = "https://sandbox-api-pay.line.me";

        private static HttpClient client;
        private readonly JsonProvider _jsonProvider;
        //    LinePayService 新增 function
        //在 LinePayService 中新增 SendPaymentRequest function
        // 送出建立交易請求至 Line Pay Server
        public async Task<PaymentResponseDto> SendPaymentRequest(PaymentRequestDto dto)
        {
            var json = _jsonProvider.Serialize(dto);
            // 產生 GUID Nonce
            var nonce = Guid.NewGuid().ToString();
            // 要放入 signature 中的 requestUrl
            var requestUrl = "/v3/payments/request";

            //使用 channelSecretKey & requestUrl & jsonBody & nonce 做簽章
            var signature = SignatureProvider.HMACSHA256("6db6e67b698382ac06b289b2f9d458ad", "6db6e67b698382ac06b289b2f9d458ad" + requestUrl + json + nonce);

            var request = new HttpRequestMessage(HttpMethod.Post, linePayBaseApiUrl + requestUrl)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            // 帶入 Headers
            client.DefaultRequestHeaders.Add("X-LINE-ChannelId", "2004344523");
            client.DefaultRequestHeaders.Add("X-LINE-Authorization-Nonce", nonce);
            client.DefaultRequestHeaders.Add("X-LINE-Authorization", signature);

            var response = await client.SendAsync(request);
            var linePayResponse = _jsonProvider.Deserialize<PaymentResponseDto>(await response.Content.ReadAsStringAsync());

            Console.WriteLine(nonce);
            Console.WriteLine(signature);

            return linePayResponse;
        }
        public async Task<PaymentConfirmResponseDto> ConfirmPayment(string transactionId, string orderId, PaymentConfirmDto dto) //加上 OrderId 去找資料
        {
            var json = _jsonProvider.Serialize(dto);

            var nonce = Guid.NewGuid().ToString();
            var requestUrl = string.Format("/v3/payments/{0}/confirm", transactionId);
            var signature = SignatureProvider.HMACSHA256(channelSecretKey, channelSecretKey + requestUrl + json + nonce);

            var request = new HttpRequestMessage(HttpMethod.Post, String.Format(linePayBaseApiUrl + requestUrl, transactionId))
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            client.DefaultRequestHeaders.Add("X-LINE-ChannelId", channelId);
            client.DefaultRequestHeaders.Add("X-LINE-Authorization-Nonce", nonce);
            client.DefaultRequestHeaders.Add("X-LINE-Authorization", signature);

            var response = await client.SendAsync(request);
            var responseDto = _jsonProvider.Deserialize<PaymentConfirmResponseDto>(await response.Content.ReadAsStringAsync());
            return responseDto;
        }
        public async void TransactionCancel(string transactionId)
        {
            //使用者取消交易則會到這裏。
            Console.WriteLine($"訂單 {transactionId} 已取消");
        }
    }
  
}
