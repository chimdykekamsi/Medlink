using System.ComponentModel.DataAnnotations;

namespace Medlink.Models
{
    public class PostModel
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string? user_id { get; set; }
        [Required]
        public string? title { get; set; }
        [Required]
        public string? details { get; set; }
        public string? image { get; set; }
        public int comment_count { get; set; } = 0;

        public int like_count { get; set; } = 0;
        public DateTime created_at { get; set; } = DateTime.Now;
    }
}
