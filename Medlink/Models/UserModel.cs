using System.ComponentModel.DataAnnotations;

namespace Medlink.Models
{
    public class UserModel
    {
        [Key]
        public int id { get; set; }
        public string? user_id { get; set; }
        public string? profile { get; set; }
        [Required]
        public string? fullname { get; set; }
        [Required]
        public string? username { get; set; }
        [Required]
        public string? password { get; set; }
        [Required]
        public string? email { get; set; }
        public string? specialty { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
    }
}
