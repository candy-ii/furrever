// Mirrors Backend/Models/BlogPost.cs. The API sends camelCase JSON.
export interface BlogPost {
  id: number;
  title: string;
  content: string;
  excerpt?: string;
  authorName: string;
  createdAt: string;
  updatedAt?: string;
  reactionCount: number;
  reactions?: BlogReaction[];
}

export interface BlogReaction {
  id: number;
  reactionType: string;
  userId: string;
  createdAt: string;
}
