# 🎤 Speech-to-Text UI Demo với Whisper.net

Ứng dụng Windows Forms với giao diện kéo thả file audio để chuyển đổi giọng nói thành văn bản sử dụng Whisper.net.

## ✨ Tính năng

- 🖱️ **Kéo thả file audio** - Giao diện thân thiện, dễ sử dụng
- 🎯 **Hỗ trợ tiếng Việt** - Nhận dạng chính xác tiếng Việt
- ⏱️ **Hiển thị timestamp** - Xem thời gian cho từng đoạn văn bản
- 📁 **Đa định dạng** - Hỗ trợ WAV, MP3, M4A, FLAC, OGG
- 🚀 **Chạy local** - Không cần internet sau khi tải model
- 📊 **Progress bar** - Theo dõi tiến trình xử lý
- 🎨 **Giao diện đẹp** - Thiết kế hiện đại, dễ nhìn

## 🖼️ Giao diện

```
┌─────────────────────────────────────────────────────────────┐
│  🎤 Speech-to-Text với Whisper.net                         │
├─────────────────────────────────────────────────────────────┤
│  ┌─────────────────────────────────────────────────────┐   │
│  │  📁 Kéo thả file audio vào đây                     │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│  📄 File: example.wav | Kích thước: 2.5 MB                │
│                                                             │
│  [🔄 Xử lý] [🗑️ Xóa]                                      │
│                                                             │
│  ████████████████████████████████████████████████████████   │
│                                                             │
│  ✅ Model đã sẵn sàng! Kéo thả file audio để bắt đầu.      │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │  🎉 Kết quả chuyển đổi Speech-to-Text:             │   │
│  │  ================================================== │   │
│  │                                                     │   │
│  │  [00:00 - 00:05] Xin chào, đây là demo...          │   │
│  │  [00:05 - 00:10] Speech to Text với Whisper...     │   │
│  │                                                     │   │
│  │  ================================================== │   │
│  │  ✅ Hoàn thành!                                     │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

## 🚀 Cách sử dụng

### 1. Chạy ứng dụng
```bash
dotnet run
```

### 2. Sử dụng giao diện
1. **Kéo thả file audio** vào vùng màu xanh
2. **Kiểm tra thông tin file** hiển thị bên dưới
3. **Nhấn nút "🔄 Xử lý"** để bắt đầu chuyển đổi
4. **Chờ kết quả** hiển thị trong ô văn bản
5. **Nhấn "🗑️ Xóa"** để xóa và chọn file khác

### 3. Lần đầu chạy
- Ứng dụng sẽ tự động tải model Whisper (~150MB)
- Quá trình này có thể mất vài phút tùy tốc độ mạng
- Thanh progress bar sẽ hiển thị tiến trình

## 📋 Yêu cầu hệ thống

- **Windows 10/11** (x64)
- **.NET 8.0 Runtime**
- **Kết nối internet** (để tải model lần đầu)
- **Dung lượng ổ cứng**: ~150MB (cho model)
- **RAM**: Tối thiểu 4GB (khuyến nghị 8GB)

## 🎵 Định dạng file audio hỗ trợ

| Định dạng | Mô tả | Chất lượng |
|-----------|-------|------------|
| **WAV** | Không nén | ⭐⭐⭐⭐⭐ |
| **FLAC** | Nén không mất | ⭐⭐⭐⭐⭐ |
| **M4A** | Nén AAC | ⭐⭐⭐⭐ |
| **MP3** | Nén MPEG | ⭐⭐⭐ |
| **OGG** | Nén Vorbis | ⭐⭐⭐ |

## ⚙️ Cấu hình

### Cấu hình API key qua file .env
Tạo file `.env` ở thư mục gốc dự án (cùng cấp `WhisperSTTUI.sln`) với nội dung:
```
GEMINI_API_KEY=YOUR_API_KEY
```
Ứng dụng sẽ tự động nạp `.env` khi khởi động. Nếu không có `.env`, bạn cũng có thể đặt biến môi trường hệ thống `GEMINI_API_KEY`.

### Thay đổi ngôn ngữ
Trong file `Form1.cs`, dòng 28:
```csharp
.WithLanguage("vi") // Thay đổi thành "en", "ja", "ko", etc.
```

### Thay đổi model
Trong file `Form1.cs`, dòng 26:
```csharp
_whisperFactory = WhisperFactory.FromPath("ggml-base.bin"); // Thay đổi model
```

## 🔧 Troubleshooting

### ❌ Lỗi "Model not found"
- **Nguyên nhân**: Chưa tải model hoặc mất kết nối internet
- **Giải pháp**: Đảm bảo có kết nối internet và chờ tải model hoàn tất

### ❌ Lỗi "File không hợp lệ"
- **Nguyên nhân**: File không phải định dạng audio hỗ trợ
- **Giải pháp**: Chuyển đổi file sang WAV, MP3, M4A, FLAC, hoặc OGG

### ❌ Chất lượng nhận dạng kém
- **Nguyên nhân**: Chất lượng file audio thấp, tiếng ồn
- **Giải pháp**: 
  - Sử dụng file audio chất lượng cao (44.1kHz, 16-bit)
  - Giảm tiếng ồn xung quanh
  - Nói rõ ràng, tốc độ vừa phải
  - Sử dụng microphone chất lượng tốt

### ❌ Ứng dụng chạy chậm
- **Nguyên nhân**: File audio quá dài, thiếu RAM
- **Giải pháp**:
  - Cắt file audio thành các đoạn ngắn hơn
  - Tăng RAM hoặc đóng các ứng dụng khác
  - Sử dụng model nhỏ hơn (ggml-tiny.bin)

## 📝 Ghi chú

- Model sẽ được lưu trong thư mục ứng dụng với tên `ggml-base.bin`
- Lần chạy tiếp theo sẽ nhanh hơn vì không cần tải model
- Kết quả có thể khác nhau tùy thuộc vào chất lượng file audio
- Ứng dụng hỗ trợ xử lý file audio dài (có thể mất vài phút)

## 🎯 Tips sử dụng hiệu quả

1. **Chuẩn bị file audio tốt**:
   - Ghi âm trong môi trường yên tĩnh
   - Sử dụng microphone chất lượng
   - Tránh tiếng ồn xung quanh

2. **Tối ưu hiệu suất**:
   - Cắt file audio thành đoạn 5-10 phút
   - Đóng các ứng dụng không cần thiết
   - Sử dụng SSD để tăng tốc độ

3. **Cải thiện độ chính xác**:
   - Nói rõ ràng, không quá nhanh
   - Sử dụng từ vựng chuẩn
   - Tránh giọng địa phương quá nặng

