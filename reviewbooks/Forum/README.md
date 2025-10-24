# Forum Module

## Overview
Forum module cho phép users tạo posts và comments để thảo luận về sách. Hỗ trợ role-based permissions với User và Admin roles.

## Features
- ✅ Create, Read, Update, Delete forum posts
- ✅ Create, Read, Update, Delete comments on posts
- ✅ View count tracking
- ✅ Pin posts (Admin only)
- ✅ Lock posts (Admin only)
- ✅ Pagination and search for posts
- ✅ Role-based authorization

## Models

### ForumPost
```csharp
{
    Id: Guid
    Title: string (max 200 chars)
    Content: string
    UserId: Guid
    CreatedAt: DateTime
    UpdatedAt: DateTime?
    ViewCount: int
    IsPinned: bool
    IsLocked: bool
    User: User (navigation)
    Comments: ICollection<ForumComment> (navigation)
}
```

### ForumComment
```csharp
{
    Id: Guid
    PostId: Guid
    UserId: Guid
    Content: string
    CreatedAt: DateTime
    UpdatedAt: DateTime?
    Post: ForumPost (navigation)
    User: User (navigation)
}
```

## API Endpoints

### Posts

#### GET /api/forum/posts
Get all forum posts with pagination and search.
- **Auth**: None (public)
- **Query Parameters**:
  - `search`: Search in title and content
  - `sortBy`: "title" | "views" | "comments" | "created" (default: pinned then created)
  - `isDescending`: true | false
  - `pageNumber`: int (default: 1)
  - `pageSize`: int (default: 10)
- **Response**: PageResult<ForumPostDto>

#### GET /api/forum/posts/{id}
Get post by ID (increments view count).
- **Auth**: None (public)
- **Response**: ForumPostDto

#### POST /api/forum/posts
Create new forum post.
- **Auth**: Required (User or Admin)
- **Body**: CreateForumPostDto
```json
{
    "title": "Book Discussion Title",
    "content": "Post content here..."
}
```
- **Response**: 201 Created with ForumPostDto

#### PUT /api/forum/posts/{id}
Update forum post.
- **Auth**: Required
- **Authorization**: User can update own posts, Admin can update any
- **Body**: UpdateForumPostDto
```json
{
    "title": "Updated title (optional)",
    "content": "Updated content (optional)"
}
```
- **Response**: 200 OK with ForumPostDto
- **Errors**: 
  - 403 Forbidden if not authorized
  - 400 Bad Request if post is locked

#### DELETE /api/forum/posts/{id}
Delete forum post.
- **Auth**: Required
- **Authorization**: User can delete own posts, Admin can delete any
- **Response**: 204 No Content
- **Errors**: 403 Forbidden if not authorized

#### PATCH /api/forum/posts/{id}/pin
Pin/Unpin post (toggle).
- **Auth**: Required (Admin only)
- **Response**: 200 OK with updated ForumPostDto
- **Errors**: 403 Forbidden if not Admin

#### PATCH /api/forum/posts/{id}/lock
Lock/Unlock post (toggle).
- **Auth**: Required (Admin only)
- **Response**: 200 OK with updated ForumPostDto
- **Errors**: 403 Forbidden if not Admin

### Comments

#### GET /api/forum/posts/{postId}/comments
Get all comments for a post.
- **Auth**: None (public)
- **Response**: IEnumerable<ForumCommentDto>

#### POST /api/forum/posts/{postId}/comments
Create new comment on post.
- **Auth**: Required (User or Admin)
- **Body**: CreateForumCommentDto
```json
{
    "content": "Comment content here..."
}
```
- **Response**: 201 Created with ForumCommentDto
- **Errors**: 
  - 400 Bad Request if post not found
  - 400 Bad Request if post is locked

#### PUT /api/forum/comments/{id}
Update comment.
- **Auth**: Required
- **Authorization**: User can update own comments, Admin can update any
- **Body**: UpdateForumCommentDto
```json
{
    "content": "Updated comment content"
}
```
- **Response**: 200 OK with ForumCommentDto
- **Errors**: 
  - 403 Forbidden if not authorized
  - 400 Bad Request if post is locked

#### DELETE /api/forum/comments/{id}
Delete comment.
- **Auth**: Required
- **Authorization**: User can delete own comments, Admin can delete any
- **Response**: 204 No Content
- **Errors**: 403 Forbidden if not authorized

## DTOs

### ForumPostDto
```csharp
{
    Id: Guid
    Title: string
    Content: string
    UserId: Guid
    Username: string
    UserAvatar: string?
    CreatedAt: DateTime
    UpdatedAt: DateTime?
    ViewCount: int
    CommentCount: int
    IsPinned: bool
    IsLocked: bool
}
```

### CreateForumPostDto
```csharp
{
    Title: string (required)
    Content: string (required)
}
```

### UpdateForumPostDto
```csharp
{
    Title: string? (optional)
    Content: string? (optional)
}
```

### ForumCommentDto
```csharp
{
    Id: Guid
    PostId: Guid
    UserId: Guid
    Username: string
    UserAvatar: string?
    Content: string
    CreatedAt: DateTime
    UpdatedAt: DateTime?
}
```

### CreateForumCommentDto
```csharp
{
    Content: string (required)
}
```

### UpdateForumCommentDto
```csharp
{
    Content: string (required)
}
```

## Authorization Rules

### Posts
- **Create**: Any authenticated user
- **Read**: Public (no auth required)
- **Update**: Post owner or Admin
- **Delete**: Post owner or Admin
- **Pin/Unpin**: Admin only
- **Lock/Unlock**: Admin only

### Comments
- **Create**: Any authenticated user (if post not locked)
- **Read**: Public (no auth required)
- **Update**: Comment owner or Admin (if post not locked)
- **Delete**: Comment owner or Admin

## Business Rules

1. **View Count**: Automatically increments when post is viewed via GET /api/forum/posts/{id}
2. **Pinned Posts**: Shown at top of list, sorted by creation date
3. **Locked Posts**: 
   - Cannot be edited by non-admins
   - Cannot accept new comments
   - Existing comments cannot be edited by non-admins
4. **Cascade Delete**: 
   - Deleting post deletes all its comments
   - Deleting user deletes all their posts (and cascade to comments)
5. **Comment Count**: Automatically calculated from Comments collection

## Database Schema

### ForumPosts Table
- Primary Key: Id (uniqueidentifier)
- Foreign Key: UserId -> Users(Id) ON DELETE CASCADE
- Indexes: IX_ForumPosts_UserId

### ForumComments Table
- Primary Key: Id (uniqueidentifier)
- Foreign Keys:
  - PostId -> ForumPosts(Id) ON DELETE CASCADE
  - UserId -> Users(Id) ON DELETE NO ACTION (to prevent cascade cycles)
- Indexes: 
  - IX_ForumComments_PostId
  - IX_ForumComments_UserId

## Example Usage

### Create a Post
```http
POST /api/forum/posts
Authorization: Bearer <jwt_token>
Content-Type: application/json

{
    "title": "Discussion: The Great Gatsby",
    "content": "What are your thoughts on the symbolism in this classic novel?"
}
```

### Search Posts
```http
GET /api/forum/posts?search=gatsby&sortBy=views&isDescending=true&pageNumber=1&pageSize=10
```

### Add Comment
```http
POST /api/forum/posts/550e8400-e29b-41d4-a716-446655440000/comments
Authorization: Bearer <jwt_token>
Content-Type: application/json

{
    "content": "I think the green light represents Gatsby's dreams and desires."
}
```

### Pin Post (Admin Only)
```http
PATCH /api/forum/posts/550e8400-e29b-41d4-a716-446655440000/pin
Authorization: Bearer <admin_jwt_token>
```

## Error Handling

All endpoints return standard error responses:
- **400 Bad Request**: Invalid input or business rule violation
- **401 Unauthorized**: Missing or invalid JWT token
- **403 Forbidden**: Valid token but insufficient permissions
- **404 Not Found**: Resource not found
- **500 Internal Server Error**: Server-side error

Error response format:
```json
{
    "message": "Error description"
}
```
