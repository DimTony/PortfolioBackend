using System;
using System.Collections.Generic;

namespace PortfolioBackend.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public List<string> Technologies { get; set; } = new();
        public string Role { get; set; } = "";
        public string Dash { get; set; } = ""; // Dashboard image URL
        public string Thumbnail { get; set; } = ""; // Thumbnail image URL
        public string ReleaseStatus { get; set; } = "";
        public string MaintainStatus { get; set; } = "";
        public string Date { get; set; } = ""; // Consider using DateTime if consistent format
        public ProjectDescription Desc { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<Comment> Comments { get; set; } = new();
    }

    public class ProjectDescription
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Summary { get; set; } = "";
        public List<Objective> Objectives { get; set; } = new();
        public string Footer { get; set; } = "";
    }

    public class Objective
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
    }
}