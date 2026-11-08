import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { BlogApi } from '../blog-api';
import { BlogPost } from '../blog-post';

@Component({
  selector: 'app-blog-list',
  imports: [RouterLink, DatePipe],
  templateUrl: './blog-list.html',
  styleUrl: './blog-list.css',
})
export class BlogList implements OnInit {
  private readonly blogApi = inject(BlogApi);

  protected readonly posts = signal<BlogPost[]>([]);

  ngOnInit() {
    this.blogApi.getAll().subscribe((posts) => this.posts.set(posts));
  }
}
