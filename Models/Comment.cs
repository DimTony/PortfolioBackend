using System;

namespace PortfolioBackend.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = "";
        public string AuthorName { get; set; } = "";
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}