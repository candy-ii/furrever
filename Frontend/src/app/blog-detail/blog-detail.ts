import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { BlogApi } from '../blog-api';
import { BlogPost } from '../blog-post';

@Component({
  selector: 'app-blog-detail',
  imports: [RouterLink, DatePipe],
  templateUrl: './blog-detail.html',
  styleUrl: './blog-detail.css',
})
export class BlogDetail implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly blogApi = inject(BlogApi);

  protected readonly post = signal<BlogPost | null>(null);
  protected readonly liked = signal(false);

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.blogApi.getById(id).subscribe((post) => this.post.set(post));
  }

  toggleLike() {
    const p = this.post();
    if (!p) return;

    // Using a simple random id as the "user" until auth exists.
    const userId = localStorage.getItem('furrever_user_id') ?? this.generateUserId();

    this.blogApi.react(p.id, userId).subscribe((res) => {
      this.liked.set(!res.removed);
      // Refresh the post to get updated reaction count.
      this.blogApi.getById(p.id).subscribe((updated) => this.post.set(updated));
    });
  }

  deletePost() {
    const p = this.post();
    if (!p) return;
    if (!confirm('Are you sure you want to delete this post?')) return;

    this.blogApi.delete(p.id).subscribe(() => {
      window.location.href = '/blog';
    });
  }

  private generateUserId(): string {
    const id = 'user_' + Math.random().toString(36).substring(2, 10);
    localStorage.setItem('furrever_user_id', id);
    return id;
  }
}
