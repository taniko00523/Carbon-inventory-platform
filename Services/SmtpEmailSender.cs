using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Carbon_inventory_platform.Services
{
    public class EmailSettings
    {
        public string Host { get; set; } = "";
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
        public string FromAddress { get; set; } = "";
        public string FromName { get; set; } = "碳盤查平台";
    }

    /// <summary>
    /// B6：用 SMTP 寄信，取代 Identity 預設「靜靜吞掉信件」的 no-op 實作。
    /// Host 沒有設定時安靜地不寄（跟現有 LINE 通知沒設定就安靜停用的作法一致），
    /// 不會讓忘記密碼／信箱驗證等呼叫端因為寄信失敗而整頁 500。
    /// </summary>
    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IOptions<EmailSettings> settings, ILogger<SmtpEmailSender> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrWhiteSpace(_settings.Host))
            {
                _logger.LogWarning("尚未設定 SMTP（Email:Host），略過寄送給 {Email} 的信件（主旨：{Subject}）。", email, subject);
                return;
            }

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromAddress, _settings.FromName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };
            message.To.Add(email);

            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.UserName, _settings.Password),
            };

            try
            {
                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                // 寄信失敗不應該讓呼叫端（忘記密碼、帳號建立等）整頁 500，留紀錄即可。
                _logger.LogError(ex, "寄送信件給 {Email} 失敗（主旨：{Subject}）。", email, subject);
            }
        }
    }
}
