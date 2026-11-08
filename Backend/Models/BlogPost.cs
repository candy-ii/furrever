namespace Backend.Models;

// One row in the "BlogPosts" table. After adding a property here, generate a
// migration so the column gets added: dotnet ef migrations add <Name>
public class BlogPost
{
    public int Id { get; set; }

    /// <summary>The title shown on the blog listing and detail pages.</summary>
    public string Title { get; set; } = "";

    /// <summary>HTML content produced by the rich-text editor.</summary>
    public string Content { get; set; } = "";

    /// <summary>Display name of the admin who wrote the post.</summary>
    public string AuthorName { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property — one post has many reactions.
    public ICollection<BlogReaction> Reactions { get; set; } = new List<BlogReaction>();
}
