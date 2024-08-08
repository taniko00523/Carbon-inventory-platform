using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace Carbon_inventory_platform.Controllers
{
    // Controllers/FeedbackController.cs
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public FeedbackController(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitFeedback(Feedback feedback)
        {
            feedback.CreatedAt = DateTime.Now;
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            // 在此處調用 LINE BOT 發送消息給開發者
            await SendLineNotification(feedback);

            return RedirectToAction("Index", "Home");
        }

        private async Task SendLineNotification(Feedback feedback)
        {
            // 使用 LINE BOT SDK 發送通知
            var lineBotToken = "PIwWKGqT9xvzJZBi9PrqOLMFBhQXXdSNM1esJcb092jPxGgnpILDR3sQSBbowF5oH0/H8zAXKaUaFHkVJqtMeSbL4YnZJ8iOwRkZQiEMsIV87vPeB3dcTw75Vh3owtj6l8+NktYB1RWGp5iaOnZQVgdB04t89/1O/w1cDnyilFU=";
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {lineBotToken}");
            string companyName = string.Empty;
            if (_signInManager.IsSignedIn(User)){
                string userId = _userManager.GetUserId(User);
                Company company = await _context.Companies.Where(x => x.UserId == userId).FirstOrDefaultAsync();
                if (company != null)
                {
                    companyName = company.Name;
                }
            }
            var message = new
            {
                to = "U8d42bf93ed12db68c679b3319a6f8b1a",
                messages = new[]
                {
                new
                {
                    type = "text",
                    text = $"新回報問題\n\n使用者名稱: {feedback.UserName}\n公司: {companyName}\n電子郵件: {feedback.Email}\n訊息: {feedback.Message}"
                }
            }
            };

            var content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
            await httpClient.PostAsync("https://api.line.me/v2/bot/message/push", content);
        }
    }

}
