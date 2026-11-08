using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]   // -> /api/blog
public class BlogController : ControllerBase
{
    private readonly AppDbContext _db;

    public BlogController(AppDbContext db) => _db = db;

    // ─── Public endpoints (guests + users) ─────────────────────────────

    // GET /api/blog — list every post, newest first, with reaction counts.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        var posts = await _db.BlogPosts
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new
            {
                p.Id,
                p.Title,
                // Send a plain-text excerpt (first 200 chars) for the listing page.
                Excerpt = p.Content.Length > 200
                    ? p.Content.Substring(0, 200) + "…"
                    : p.Content,
                p.AuthorName,
                p.CreatedAt,
                ReactionCount = p.Reactions.Count
            })
            .ToListAsync();

        return Ok(posts);
    }

    // GET /api/blog/{id} — full post with its reactions.
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetById(int id)
    {
        var post = await _db.BlogPosts
            .Include(p => p.Reactions)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post is null) return NotFound();

        return Ok(new
        {
            post.Id,
            post.Title,
            post.Content,
            post.AuthorName,
            post.CreatedAt,
            post.UpdatedAt,
            Reactions = post.Reactions.Select(r => new
            {
                r.Id,
                r.ReactionType,
                r.UserId,
                r.CreatedAt
            }),
            ReactionCount = post.Reactions.Count
        });
    }

    // ─── Admin endpoints ────────────────────────────────────────────────
    // (No auth guard yet — will be added when auth exists.)

    // POST /api/blog — create a new post.
    [HttpPost]
    public async Task<ActionResult<BlogPost>> Create(BlogPost post)
    {
        post.CreatedAt = DateTime.UtcNow;
        post.UpdatedAt = DateTime.UtcNow;

        _db.BlogPosts.Add(post);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
    }

    // PUT /api/blog/{id} — update an existing post.
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, BlogPost incoming)
    {
        var post = await _db.BlogPosts.FindAsync(id);
        if (post is null) return NotFound();

        post.Title = incoming.Title;
        post.Content = incoming.Content;
        post.AuthorName = incoming.AuthorName;
        post.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/blog/{id} — delete a post and its reactions.
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _db.BlogPosts
            .Include(p => p.Reactions)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post is null) return NotFound();

        _db.BlogPosts.Remove(post);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ─── Reaction endpoint (users) ──────────────────────────────────────

    // POST /api/blog/{id}/react — toggle a reaction on a post.
    [HttpPost("{id}/react")]
    public async Task<ActionResult<object>> React(int id, [FromBody] BlogReactionRequest req)
    {
        if (!await _db.BlogPosts.AnyAsync(p => p.Id == id))
            return NotFound();

        // Check if this user already reacted to this post.
        var existing = await _db.BlogReactions
            .FirstOrDefaultAsync(r => r.BlogPostId == id && r.UserId == req.UserId);

        if (existing is not null)
        {
            // Toggle off — remove the reaction.
            _db.BlogReactions.Remove(existing);
            await _db.SaveChangesAsync();
            return Ok(new { removed = true });
        }

        // Add new reaction.
        var reaction = new BlogReaction
        {
            BlogPostId = id,
            UserId = req.UserId,
            ReactionType = req.ReactionType ?? "like",
            CreatedAt = DateTime.UtcNow
        };
        _db.BlogReactions.Add(reaction);
        await _db.SaveChangesAsync();

        return Ok(new { removed = false, reaction.Id });
    }
}

public class BlogReactionRequest
{
    public string UserId { get; set; } = "";
    public string? ReactionType { get; set; }
}
