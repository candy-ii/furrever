import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BlogPost } from './blog-post';

// Every call to the blog backend lives here — keeps components thin.
@Service()
export class BlogApi {
  private readonly http = inject(HttpClient);

  // "/api/..." is forwarded to the ASP.NET backend by proxy.conf.json

  /** List every blog post (newest first, with excerpts). */
  getAll() {
    return this.http.get<BlogPost[]>('/api/blog');
  }

  /** Fetch a single post by id (full HTML content + reactions). */
  getById(id: number) {
    return this.http.get<BlogPost>(`/api/blog/${id}`);
  }

  /** Create a new blog post (admin). */
  create(post: { title: string; content: string; authorName: string }) {
    return this.http.post<BlogPost>('/api/blog', post);
  }

  /** Update an existing blog post (admin). */
  update(id: number, post: { title: string; content: string; authorName: string }) {
    return this.http.put(`/api/blog/${id}`, post);
  }

  /** Delete a blog post (admin). */
  delete(id: number) {
    return this.http.delete(`/api/blog/${id}`);
  }

  /** Toggle a reaction (like) on a post. */
  react(postId: number, userId: string, reactionType: string = 'like') {
    return this.http.post<{ removed: boolean; id?: number }>(
      `/api/blog/${postId}/react`,
      { userId, reactionType }
    );
  }
}
