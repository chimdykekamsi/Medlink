using System.ComponentModel.DataAnnotations;

namespace Medlink.Models
{
    public class LikeModel
    {
        [Key]
        public int id { get; set; }
        public string? user_id { get; set; }
        public int post_id { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
    }
}
