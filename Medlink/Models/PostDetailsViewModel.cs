// PostDetailsViewModel.cs
using System.Collections.Generic;

namespace Medlink.Models
{
    public class PostDetailsViewModel
    {
        public PostModel Post { get; set; }
        public List<CommentModel> Comments { get; set; }
    }
}
