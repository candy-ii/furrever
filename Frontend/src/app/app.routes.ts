import { Routes } from '@angular/router';
import { BlogList } from './blog-list/blog-list';
import { BlogDetail } from './blog-detail/blog-detail';
import { BlogEditor } from './blog-editor/blog-editor';

export const routes: Routes = [
  // Public blog pages
  { path: 'blog', component: BlogList },
  { path: 'blog/:id', component: BlogDetail },

  // Admin blog management
  { path: 'admin/blog/new', component: BlogEditor },
  { path: 'admin/blog/edit/:id', component: BlogEditor },
];
