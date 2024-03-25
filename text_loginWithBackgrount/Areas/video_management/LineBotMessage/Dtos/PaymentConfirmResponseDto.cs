namespace text_loginWithBackgrount.Areas.video_management.LineBotMessage.Dtos
{
    //這邊命名空間不一樣需要多加注意
    public class PaymentResponseDto
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public ResponseInfoDto Info { get; set; }
    }

    public class ResponseInfoDto
    {
        public ResponsePaymentUrlDto PaymentUrl { get; set; }
        public long TransactionId { get; set; }
        public string PaymentAccessToken { get; set; }
    }

    public class ResponsePaymentUrlDto
    {
        public string Web { get; set; }
        public string App { get; set; }
    }
}
