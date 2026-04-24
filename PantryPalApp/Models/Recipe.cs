using System.ComponentModel.DataAnnotations;

namespace PantryPalApp.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Cuisine { get; set; }

        [Required]
        public int PrepMinutes { get; set; }

        [Required]
        public int CookMinutes { get; set; }

        public string? SourceUrl { get; set; }
    }
}