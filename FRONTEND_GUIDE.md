# 📘 Frontend Developer Guide

A comprehensive guide for frontend developers integrating with ReviewBooks API.

---

## 🎯 Table of Contents

- [Getting Started](#getting-started)
- [Authentication](#authentication)
- [API Base URL](#api-base-url)
- [Common Patterns](#common-patterns)
- [Module Integration](#module-integration)
- [Error Handling](#error-handling)
- [Best Practices](#best-practices)

---

## 🚀 Getting Started

### Prerequisites

- API running at: `http://localhost:5072`
- Swagger docs: `http://localhost:5072/swagger`

### Quick Setup

```javascript
// config.js
export const API_BASE_URL = "http://localhost:5072/api";

// Helper function
export const apiRequest = async (endpoint, options = {}) => {
  const token = localStorage.getItem("authToken");

  const config = {
    ...options,
    headers: {
      "Content-Type": "application/json",
      ...(token && { Authorization: `Bearer ${token}` }),
      ...options.headers,
    },
  };

  const response = await fetch(`${API_BASE_URL}${endpoint}`, config);

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || "API request failed");
  }

  return response.json();
};
```

---

## 🔐 Authentication

### 1. Registration Flow

#### Step 1: Register User

```javascript
// POST /api/auth/register
const register = async (email, username, password) => {
  const response = await fetch(`${API_BASE_URL}/auth/register`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email, username, password }),
  });

  const data = await response.json();
  // Expected: { message: "Registration email sent. Please check your email within 30 minutes." }

  return data;
};
```

#### Step 2: Handle Email Verification

User clicks link in email → Backend redirects to:

```
http://localhost:3000?verified=true&token=JWT_TOKEN&message=Email verified successfully!
```

Frontend code to handle redirect:

```javascript
// In your homepage/layout component
useEffect(() => {
  const urlParams = new URLSearchParams(window.location.search);
  const verified = urlParams.get("verified");
  const token = urlParams.get("token");
  const message = urlParams.get("message");

  if (verified === "true" && token) {
    // Save JWT token
    localStorage.setItem("authToken", token);

    // Show success notification
    showNotification(message, "success");

    // Clean URL
    window.history.replaceState({}, "", "/");

    // User is now logged in automatically!
    // Fetch user profile or redirect to dashboard
  } else if (verified === "false") {
    showNotification(message, "error");
  }
}, []);
```

#### Step 3: Resend Verification (if expired)

```javascript
// POST /api/auth/resend-verification
const resendVerification = async (email) => {
  const response = await fetch(`${API_BASE_URL}/auth/resend-verification`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email }),
  });

  return response.json();
};
```

### 2. Login Flow

```javascript
// POST /api/auth/login
const login = async (email, password) => {
  const response = await fetch(`${API_BASE_URL}/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email, password }),
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message); // "Invalid email or password" or "Please verify your email"
  }

  const data = await response.json();
  /* Response:
  {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "expiresAt": "2025-11-01T12:00:00Z",
    "user": {
      "id": "uuid",
      "email": "user@example.com",
      "username": "John",
      "role": "User"
    }
  }
  */

  // Save token
  localStorage.setItem("authToken", data.token);
  localStorage.setItem("user", JSON.stringify(data.user));

  return data;
};
```

### 3. Using JWT Token

```javascript
// Add to all authenticated requests
const headers = {
  "Content-Type": "application/json",
  Authorization: `Bearer ${localStorage.getItem("authToken")}`,
};

// Example: Get current user
const getCurrentUser = async () => {
  const response = await fetch(`${API_BASE_URL}/users/me`, {
    headers: {
      Authorization: `Bearer ${localStorage.getItem("authToken")}`,
    },
  });

  return response.json();
};
```

### 4. Logout

```javascript
const logout = () => {
  localStorage.removeItem("authToken");
  localStorage.removeItem("user");
  // Redirect to login page
  window.location.href = "/login";
};
```

### 5. Check if User is Logged In

```javascript
const isAuthenticated = () => {
  const token = localStorage.getItem("authToken");
  if (!token) return false;

  // Optional: Check token expiration
  try {
    const payload = JSON.parse(atob(token.split(".")[1]));
    const exp = payload.exp * 1000; // Convert to milliseconds
    return Date.now() < exp;
  } catch {
    return false;
  }
};
```

---

## 🌐 API Base URL

```javascript
const API_BASE_URL =
  process.env.REACT_APP_API_URL || "http://localhost:5072/api";
```

---

## 🔄 Common Patterns

### Pagination

All list endpoints support pagination with these query parameters:

```javascript
const getBooks = async (page = 1, pageSize = 12, search = "") => {
  const params = new URLSearchParams({
    pageNumber: page,
    pageSize: pageSize,
    search: search,
  });

  const response = await fetch(`${API_BASE_URL}/books/search?${params}`);
  const data = await response.json();

  /* Response:
  {
    "items": [...],
    "totalCount": 100,
    "pageSize": 12,
    "currentPage": 1,
    "totalPages": 9
  }
  */

  return data;
};
```

### Sorting

```javascript
const getReviews = async (sortBy = "createdAt", isDescending = true) => {
  const params = new URLSearchParams({
    sortBy: sortBy, // 'rating' | 'createdAt'
    isDescending: isDescending,
  });

  const response = await fetch(`${API_BASE_URL}/reviews?${params}`);
  return response.json();
};
```

---

## 📚 Module Integration

### Books Module

#### Search Books

```javascript
// GET /api/books/search?query=harry&pageNumber=1&pageSize=12
const searchBooks = async (query, page = 1, pageSize = 12) => {
  const params = new URLSearchParams({
    query,
    pageNumber: page,
    pageSize: pageSize,
  });

  const response = await fetch(`${API_BASE_URL}/books/search?${params}`);
  return response.json();
};
```

#### Get Book Details (without content)

```javascript
// GET /api/books/{id}
const getBook = async (bookId) => {
  const response = await fetch(`${API_BASE_URL}/books/${bookId}`);
  return response.json();
  /* Response includes:
  {
    "id": "volumeId",
    "title": "Book Title",
    "authors": "Author Name",
    "thumbnail": "https://...",
    "averageRating": 4.5,        // From Google Books
    "ratingsCount": 1000,        // From Google Books
    "systemAverageRating": 4.2,  // From user reviews (null if no reviews)
    "systemRatingsCount": 15     // Number of user reviews
  }
  */
};
```

#### Get Book with Content (for reading)

```javascript
// GET /api/books/{id}/read
const getBookToRead = async (bookId) => {
  const response = await fetch(`${API_BASE_URL}/books/${bookId}/read`);
  return response.json();
  /* Response includes all fields + content:
  {
    ...bookFields,
    "content": "Full book content if available"
  }
  */
};
```

### Reviews Module

#### Get Reviews for a Book

```javascript
// GET /api/reviews/book/{bookId}
const getBookReviews = async (bookId, page = 1) => {
  const params = new URLSearchParams({
    pageNumber: page,
    pageSize: 10,
    sortBy: "createdAt",
    isDescending: true,
  });

  const response = await fetch(
    `${API_BASE_URL}/reviews/book/${bookId}?${params}`
  );
  return response.json();
};
```

#### Create Review

```javascript
// POST /api/reviews
const createReview = async (bookId, rating, comment) => {
  const token = localStorage.getItem("authToken");

  const response = await fetch(`${API_BASE_URL}/reviews`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({
      bookId,
      rating, // 1-5
      comment,
    }),
  });

  return response.json();
};
```

**Note:** After creating/updating/deleting a review, the book's `systemAverageRating` and `systemRatingsCount` are automatically updated!

#### Update Review

```javascript
// PUT /api/reviews/{id}
const updateReview = async (reviewId, rating, comment) => {
  const token = localStorage.getItem("authToken");

  const response = await fetch(`${API_BASE_URL}/reviews/${reviewId}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({
      rating, // Optional
      comment, // Optional
    }),
  });

  return response.json();
};
```

#### Delete Review

```javascript
// DELETE /api/reviews/{id}
const deleteReview = async (reviewId) => {
  const token = localStorage.getItem("authToken");

  const response = await fetch(`${API_BASE_URL}/reviews/${reviewId}`, {
    method: "DELETE",
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  return response.ok;
};
```

### Favorites Module

#### Get My Favorites

```javascript
// GET /api/favorites
const getMyFavorites = async () => {
  const token = localStorage.getItem("authToken");

  const response = await fetch(`${API_BASE_URL}/favorites`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  return response.json();
  /* Response:
  [
    {
      "id": "volumeId",
      "title": "Book Title",
      "authors": "Author",
      "thumbnail": "https://...",
      ...
    }
  ]
  */
};
```

#### Add to Favorites

```javascript
// POST /api/favorites
const addToFavorites = async (bookId) => {
  const token = localStorage.getItem("authToken");

  const response = await fetch(`${API_BASE_URL}/favorites`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({ bookId }),
  });

  return response.json();
};
```

#### Remove from Favorites

```javascript
// DELETE /api/favorites/{bookId}
const removeFromFavorites = async (bookId) => {
  const token = localStorage.getItem("authToken");

  const response = await fetch(`${API_BASE_URL}/favorites/${bookId}`, {
    method: "DELETE",
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  return response.ok;
};
```

#### Check if Book is Favorited

```javascript
// GET /api/favorites/check/{bookId}
const isFavorited = async (bookId) => {
  const token = localStorage.getItem("authToken");

  const response = await fetch(`${API_BASE_URL}/favorites/check/${bookId}`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  const data = await response.json();
  return data.isFavorited; // boolean
};
```

### Forum Module

#### Get Forum Posts

```javascript
// GET /api/forum/posts
const getForumPosts = async (page = 1, search = "") => {
  const params = new URLSearchParams({
    pageNumber: page,
    pageSize: 20,
    search,
  });

  const response = await fetch(`${API_BASE_URL}/forum/posts?${params}`);
  return response.json();
};
```

#### Create Forum Post

```javascript
// POST /api/forum/posts
const createForumPost = async (title, content, bookId = null) => {
  const token = localStorage.getItem("authToken");

  const response = await fetch(`${API_BASE_URL}/forum/posts`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({
      title,
      content,
      bookId, // Optional: link post to a book
    }),
  });

  return response.json();
};
```

#### Get Replies for a Post

```javascript
// GET /api/forum/posts/{postId}/replies
const getPostReplies = async (postId, page = 1) => {
  const params = new URLSearchParams({
    pageNumber: page,
    pageSize: 20,
  });

  const response = await fetch(
    `${API_BASE_URL}/forum/posts/${postId}/replies?${params}`
  );
  return response.json();
};
```

#### Create Reply

```javascript
// POST /api/forum/posts/{postId}/replies
const createReply = async (postId, content) => {
  const token = localStorage.getItem("authToken");

  const response = await fetch(
    `${API_BASE_URL}/forum/posts/${postId}/replies`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({ content }),
    }
  );

  return response.json();
};
```

### Users Module

#### Get Current User

```javascript
// GET /api/users/me
const getCurrentUser = async () => {
  const token = localStorage.getItem("authToken");

  const response = await fetch(`${API_BASE_URL}/users/me`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  return response.json();
};
```

#### Update Profile

```javascript
// PUT /api/users/{id}
const updateProfile = async (userId, username) => {
  const token = localStorage.getItem("authToken");

  const response = await fetch(`${API_BASE_URL}/users/${userId}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({ username }),
  });

  return response.json();
};
```

---

## ⚠️ Error Handling

### Standard Error Response

```json
{
  "message": "Error message here",
  "error": "Detailed error description"
}
```

### Error Handling Pattern

```javascript
const handleApiError = async (response) => {
  if (!response.ok) {
    const error = await response.json();

    switch (response.status) {
      case 400:
        throw new Error(error.message || "Bad request");
      case 401:
        // Unauthorized - redirect to login
        localStorage.removeItem("authToken");
        window.location.href = "/login";
        break;
      case 403:
        throw new Error("You do not have permission to perform this action");
      case 404:
        throw new Error("Resource not found");
      case 500:
        throw new Error("Server error. Please try again later");
      default:
        throw new Error(error.message || "An error occurred");
    }
  }

  return response.json();
};

// Usage
try {
  const data = await handleApiError(response);
} catch (error) {
  showNotification(error.message, "error");
}
```

---

## ✅ Best Practices

### 1. Token Management

```javascript
// Check token expiration before requests
const isTokenExpired = () => {
  const token = localStorage.getItem("authToken");
  if (!token) return true;

  try {
    const payload = JSON.parse(atob(token.split(".")[1]));
    return Date.now() >= payload.exp * 1000;
  } catch {
    return true;
  }
};

// Refresh logic (if token is expiring soon)
if (isTokenExpired()) {
  // Redirect to login or refresh token
  logout();
}
```

### 2. Loading States

```javascript
const [loading, setLoading] = useState(false);
const [error, setError] = useState(null);

const fetchData = async () => {
  setLoading(true);
  setError(null);

  try {
    const data = await apiRequest("/books/search?query=harry");
    // Handle success
  } catch (err) {
    setError(err.message);
  } finally {
    setLoading(false);
  }
};
```

### 3. Debouncing Search

```javascript
import { useEffect, useState } from "react";

const useDebounce = (value, delay = 500) => {
  const [debouncedValue, setDebouncedValue] = useState(value);

  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedValue(value);
    }, delay);

    return () => clearTimeout(handler);
  }, [value, delay]);

  return debouncedValue;
};

// Usage in search
const [searchTerm, setSearchTerm] = useState("");
const debouncedSearch = useDebounce(searchTerm);

useEffect(() => {
  if (debouncedSearch) {
    searchBooks(debouncedSearch);
  }
}, [debouncedSearch]);
```

### 4. Caching Strategy

```javascript
// Simple in-memory cache
const cache = new Map();

const getCachedData = async (key, fetcher, ttl = 300000) => {
  const cached = cache.get(key);

  if (cached && Date.now() - cached.timestamp < ttl) {
    return cached.data;
  }

  const data = await fetcher();
  cache.set(key, { data, timestamp: Date.now() });
  return data;
};

// Usage
const book = await getCachedData(
  `book-${bookId}`,
  () => fetch(`${API_BASE_URL}/books/${bookId}`).then((r) => r.json()),
  600000 // 10 minutes
);
```

### 5. Optimistic Updates

```javascript
const addToFavorites = async (book) => {
  // Optimistically update UI
  setFavorites((prev) => [...prev, book]);

  try {
    await apiRequest("/favorites", {
      method: "POST",
      body: JSON.stringify({ bookId: book.id }),
    });
  } catch (error) {
    // Revert on error
    setFavorites((prev) => prev.filter((b) => b.id !== book.id));
    showNotification("Failed to add to favorites", "error");
  }
};
```

### 6. Displaying Ratings

```javascript
// Show both rating systems
const BookRating = ({ book }) => {
  return (
    <div>
      {/* System rating (from user reviews) - Show first if available */}
      {book.systemRatingsCount > 0 && (
        <div>
          <span>⭐ {book.systemAverageRating?.toFixed(1)}</span>
          <span>({book.systemRatingsCount} reviews)</span>
        </div>
      )}

      {/* Google Books rating - Show as secondary */}
      {book.averageRating && (
        <div className="text-muted">
          <span>Google: {book.averageRating}</span>
          <span>({book.ratingsCount} ratings)</span>
        </div>
      )}
    </div>
  );
};
```

---

## 🔗 Useful Resources

- **Swagger UI**: `http://localhost:5072/swagger` - Interactive API documentation
- **Base URL**: `http://localhost:5072/api`
- **Main README**: [README.md](./README.md) - Backend documentation

---

## 💡 Tips

1. **Always handle 401 errors** - Token expiration requires re-login
2. **Use environment variables** - Don't hardcode API URLs
3. **Implement retry logic** - For network failures
4. **Cache static data** - Books, user profiles (with TTL)
5. **Show loading states** - Better UX during API calls
6. **Validate inputs** - Before sending to API
7. **Use TypeScript** - For better type safety with API responses

---

## 📞 Support

For questions or issues:

- Check Swagger documentation
- Review module-specific README files
- Contact backend team

---

<div align="center">
  <p>Happy coding! 🚀</p>
  <p>Built with ❤️ by LTWeb-4P Team</p>
</div>
