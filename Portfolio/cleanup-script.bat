#!/bin/bash
# Script để dọn dẹp và chuẩn bị React Portfolio
# Chạy trong PowerShell hoặc CMD

echo "🚀 Đang dọn dẹp và chuẩn bị React Portfolio..."

# Di chuyển tới thư mục Portfolio
cd "d:\IT\LTWeb\Portfolio"

# 1. Đổi tên index-react.html thành index.html chính
echo "📝 Đổi tên index-react.html thành index.html..."
if exist index-react.html (
    if exist index.html (
        del index.html
    )
    ren index-react.html index.html
    echo "✅ Đã đổi tên thành công"
) else (
    echo "❌ Không tìm thấy index-react.html"
)

# 2. Xóa các file cũ không cần thiết
echo "🗑️ Xóa các file cũ..."
if exist style.css (
    del style.css
    echo "✅ Đã xóa style.css"
)
if exist func.js (
    del func.js
    echo "✅ Đã xóa func.js"
)
if exist styles.tsx (
    del styles.tsx
    echo "✅ Đã xóa styles.tsx"
)
if exist src\components\ParticleBackground.tsx (
    del src\components\ParticleBackground.tsx
    echo "✅ Đã xóa ParticleBackground.tsx cũ"
)

# 3. Tạo thư mục public nếu chưa có
echo "📁 Tạo thư mục public..."
if not exist public (
    mkdir public
    echo "✅ Đã tạo thư mục public"
) else (
    echo "✅ Thư mục public đã tồn tại"
)

# 4. Tạo favicon đơn giản
echo "🎨 Tạo favicon..."
echo ^<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100"^>^<circle cx="50" cy="50" r="40" fill="%23B6C99B"/^>^<text x="50" y="60" text-anchor="middle" fill="white" font-size="40" font-family="Arial"^>T^</text^>^</svg^> > public\portfolio-icon.svg

echo "🎉 HOÀN TẤT! Cấu trúc thư mục đã được dọn dẹp."
echo ""
echo "📋 CẤU TRÚC THƒ MỤC SAU KHI DỌN DẸP:"
echo "Portfolio/"
echo "├── src/                     (React components)"
echo "├── public/                  (Static assets)" 
echo "├── index.html               (React entry point)"
echo "├── package.json             (Dependencies)"
echo "├── vite.config.ts           (Vite config)"
echo "└── tsconfig.json            (TypeScript config)"
echo ""
echo "🔧 BƯỚC TIẾP THEO:"
echo "1. Chạy: npm install --legacy-peer-deps"
echo "2. Chạy: npm run dev"
echo "3. Mở http://localhost:5173"