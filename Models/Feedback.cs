using System.ComponentModel.DataAnnotations;

namespace Carbon_inventory_platform.Models
{
    // Models/Feedback.cs
    public class Feedback
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "請填寫使用者名稱")]
        [MaxLength(100)]
        [Display(Name = "使用者名稱")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "請填寫電子郵件")]
        [EmailAddress(ErrorMessage = "電子信箱格式錯誤")]
        [MaxLength(200)]
        [Display(Name = "電子郵件")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "請填寫訊息內容")]
        [MaxLength(2000, ErrorMessage = "訊息請勿超過 2000 字")]
        [Display(Name = "訊息")]
        public string Message { get; set; } = string.Empty;

        [Display(Name = "回報時間")]
        public DateTime CreatedAt { get; set; }
    }
}
