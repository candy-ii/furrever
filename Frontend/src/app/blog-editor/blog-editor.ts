import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BlogApi } from '../blog-api';

@Component({
  selector: 'app-blog-editor',
  imports: [FormsModule, RouterLink],
  templateUrl: './blog-editor.html',
  styleUrl: './blog-editor.css',
})
export class BlogEditor implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly blogApi = inject(BlogApi);

  protected title = '';
  protected authorName = '';
  protected content = '';

  /** null = creating a new post; number = editing existing post id. */
  protected editingId: number | null = null;
  protected readonly saving = signal(false);

  ngOnInit() {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.editingId = Number(idParam);
      this.blogApi.getById(this.editingId).subscribe((post) => {
        this.title = post.title;
        this.authorName = post.authorName;
        this.content = post.content;
      });
    }
  }

  save() {
    if (!this.title.trim() || !this.content.trim()) {
      alert('Title and content are required.');
      return;
    }

    this.saving.set(true);
    const payload = {
      title: this.title,
      content: this.content,
      authorName: this.authorName || 'Admin',
    };

    if (this.editingId) {
      this.blogApi.update(this.editingId, payload).subscribe({
        next: () => this.router.navigate(['/blog', this.editingId]),
        error: () => this.saving.set(false),
      });
    } else {
      this.blogApi.create(payload).subscribe({
        next: (created) => this.router.navigate(['/blog', created.id]),
        error: () => this.saving.set(false),
      });
    }
  }

  // ─── Simple rich-text toolbar helpers ─────────────────────────────────
  // These use document.execCommand which works in all modern browsers.
  // It's the simplest way to get bold/italic/heading without pulling in a
  // heavy editor library. The output is captured from the contenteditable div.

  execCmd(command: string, value: string = '') {
    document.execCommand(command, false, value);
  }

  insertHeading() {
    document.execCommand('formatBlock', false, 'h2');
  }

  insertParagraph() {
    document.execCommand('formatBlock', false, 'p');
  }

  onContentChange(event: Event) {
    this.content = (event.target as HTMLElement).innerHTML;
  }
}
