using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using PantryPalApp.Models;

namespace PantryPalApp.Controllers
{
    public class RecipesController : Controller
    {
        private readonly string _connectionString;

        public RecipesController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PantryPalConnection")!;
        }

        public IActionResult Index()
        {
            List<Recipe> recipes = new();

            using MySqlConnection conn = new(_connectionString);
            conn.Open();

            string query = @"SELECT recipe_id, user_id, title, cuisine, prep_minutes, cook_minutes, source_url
                             FROM recipes";

            using MySqlCommand cmd = new(query, conn);
            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                recipes.Add(new Recipe
                {
                    RecipeId = reader.GetInt32("recipe_id"),
                    UserId = reader.GetInt32("user_id"),
                    Title = reader.GetString("title"),
                    Cuisine = reader.IsDBNull(reader.GetOrdinal("cuisine")) ? null : reader.GetString("cuisine"),
                    PrepMinutes = reader.GetInt32("prep_minutes"),
                    CookMinutes = reader.GetInt32("cook_minutes"),
                    SourceUrl = reader.IsDBNull(reader.GetOrdinal("source_url")) ? null : reader.GetString("source_url")
                });
            }

            return View(recipes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Recipe recipe)
        {
            if (!ModelState.IsValid)
                return View(recipe);

            using MySqlConnection conn = new(_connectionString);
            conn.Open();

            string query = @"INSERT INTO recipes (user_id, title, cuisine, prep_minutes, cook_minutes, source_url)
                             VALUES (@UserId, @Title, @Cuisine, @PrepMinutes, @CookMinutes, @SourceUrl)";

            using MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@UserId", recipe.UserId);
            cmd.Parameters.AddWithValue("@Title", recipe.Title);
            cmd.Parameters.AddWithValue("@Cuisine", (object?)recipe.Cuisine ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PrepMinutes", recipe.PrepMinutes);
            cmd.Parameters.AddWithValue("@CookMinutes", recipe.CookMinutes);
            cmd.Parameters.AddWithValue("@SourceUrl", (object?)recipe.SourceUrl ?? DBNull.Value);

            cmd.ExecuteNonQuery();

            return RedirectToAction("Index");
        }
    }
}