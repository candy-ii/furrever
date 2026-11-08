namespace Backend.Models;

// One row in the "BlogReactions" table. Tracks a user's reaction to a blog post.
public class BlogReaction
{
    public int Id { get; set; }

    /// <summary>FK to the blog post being reacted to.</summary>
    public int BlogPostId { get; set; }

    /// <summary>
    /// Identifier for the user who reacted. This will be a proper User FK once
    /// authentication is wired up; for now it's a simple string so the feature
    /// can work end-to-end without an auth system.
    /// </summary>
    public string UserId { get; set; } = "";

    /// <summary>"like" or "heart" — kept as a string for flexibility.</summary>
    public string ReactionType { get; set; } = "like";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property back to the post.
    public BlogPost? BlogPost { get; set; }
}
