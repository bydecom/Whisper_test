using Whisper.net;
using Whisper.net.Ggml;
using NAudio.Wave;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Diagnostics;
using System.Text.Encodings.Web;

namespace WhisperSTTUI;

public partial class Form1 : Form
{
    private string? _selectedAudioFile;
    private WhisperFactory? _whisperFactory;
    private WhisperProcessor? _whisperProcessor;

    public Form1()
    {
        InitializeComponent();
        InitializeWhisper();
    }

    private void InitializeWhisper()
    {
        try
        {
            lblStatus.Text = "🔄 Đang khởi tạo Whisper model...";
            lblStatus.ForeColor = Color.FromArgb(0, 120, 215);
            
            // Kiểm tra model có tồn tại không
            string modelPath = "ggml-small-q5_1.bin";
            if (!File.Exists(modelPath))
            {
                lblStatus.Text = "❌ Không tìm thấy model Whisper!";
                lblStatus.ForeColor = Color.Red;
                
                MessageBox.Show(
                    "Không tìm thấy model Whisper!\n\n" +
                    "Vui lòng tải model từ:\n" +
                    "https://huggingface.co/ggerganov/whisper.cpp\n\n" +
                    "Hoặc chạy lệnh sau trong terminal:\n" +
                    "curl -L -o ggml-base.bin https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-base.bin",
                    "Thiếu model Whisper",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            
            // Khởi tạo Whisper
            _whisperFactory = WhisperFactory.FromPath(modelPath);
            _whisperProcessor = _whisperFactory.CreateBuilder()
                .WithLanguage("vi") // Tiếng Việt
                .Build();

            lblStatus.Text = "✅ Model đã sẵn sàng! Kéo thả file audio để bắt đầu.";
            lblStatus.ForeColor = Color.FromArgb(0, 150, 0);
        }
        catch (Exception ex)
        {
            lblStatus.Text = $"❌ Lỗi khởi tạo model: {ex.Message}";
            lblStatus.ForeColor = Color.Red;
            
            MessageBox.Show(
                $"Lỗi khởi tạo Whisper model:\n\n{ex.Message}\n\n" +
                "Gợi ý khắc phục:\n" +
                "1. Đảm bảo có kết nối internet\n" +
                "2. Kiểm tra file model không bị hỏng\n" +
                "3. Thử tải lại model từ Hugging Face",
                "Lỗi khởi tạo model",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void pnlDropZone_DragEnter(object? sender, DragEventArgs e)
    {
        if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
        {
            e.Effect = DragDropEffects.Copy;
            pnlDropZone.BackColor = Color.FromArgb(200, 230, 255);
            lblDropText.Text = "📁 Thả file audio vào đây...";
        }
        else
        {
            e.Effect = DragDropEffects.None;
        }
    }

    private void pnlDropZone_DragDrop(object? sender, DragEventArgs e)
    {
        pnlDropZone.BackColor = Color.FromArgb(240, 248, 255);
        lblDropText.Text = "📁 Kéo thả file audio vào đây";

        if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
        {
            string[]? files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null && files.Length > 0)
            {
                string filePath = files[0];
                if (IsAudioFile(filePath))
                {
                    _selectedAudioFile = filePath;
                    UpdateFileInfo();
                    btnProcess.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn file audio hợp lệ!\n\nĐịnh dạng hỗ trợ: WAV, MP3, M4A, FLAC, OGG", 
                        "File không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }

    private bool IsAudioFile(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLower();
        string[] supportedExtensions = { ".wav", ".mp3", ".m4a", ".flac", ".ogg" };
        return supportedExtensions.Contains(extension);
    }

    private void UpdateFileInfo()
    {
        if (!string.IsNullOrEmpty(_selectedAudioFile))
        {
            var fileInfo = new FileInfo(_selectedAudioFile);
            lblFileInfo.Text = $"📄 File: {fileInfo.Name} | Kích thước: {FormatFileSize(fileInfo.Length)}";
            lblFileInfo.ForeColor = Color.FromArgb(0, 120, 215);
        }
    }

    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }

    private string ConvertTo16kHz(string inputPath)
    {
        try
        {
            string outputPath = Path.Combine(Path.GetTempPath(), $"whisper_temp_{Guid.NewGuid()}.wav");
            
            using (var reader = new AudioFileReader(inputPath))
            {
                // Kiểm tra sample rate hiện tại
                if (reader.WaveFormat.SampleRate == 16000)
                {
                    // Nếu đã là 16kHz, copy file gốc
                    File.Copy(inputPath, outputPath, true);
                    return outputPath;
                }

                // Chuyển đổi về 16kHz
                var resampler = new MediaFoundationResampler(reader, new WaveFormat(16000, 1));
                WaveFileWriter.CreateWaveFile(outputPath, resampler);
            }
            
            return outputPath;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi chuyển đổi sample rate: {ex.Message}");
        }
    }

    private async void btnProcess_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_selectedAudioFile) || _whisperProcessor == null)
            return;

        string? tempFile = null;
        try
        {
            btnProcess.Enabled = false;
            progressBar.Visible = true;
            lblStatus.Text = "🔄 Đang chuẩn bị file audio...";
            lblStatus.ForeColor = Color.FromArgb(0, 120, 215);
            txtResult.Clear();

            // Chuyển đổi file audio về 16kHz nếu cần
            lblStatus.Text = "🔄 Đang chuyển đổi sample rate...";
            tempFile = ConvertTo16kHz(_selectedAudioFile);
            
            lblStatus.Text = "🔄 Đang xử lý file audio...";
            
            // Xử lý file audio đã chuyển đổi
            using var fileStream = File.OpenRead(tempFile);
            var segments = _whisperProcessor.ProcessAsync(fileStream);

            var result = new System.Text.StringBuilder();
            result.AppendLine("🎉 Kết quả chuyển đổi Speech-to-Text:");
            result.AppendLine("=" + new string('=', 50));
            result.AppendLine();

            await foreach (var segment in segments)
            {
                result.AppendLine($"[{segment.Start:mm\\:ss\\.ff} - {segment.End:mm\\:ss\\.ff}] {segment.Text}");
                
                // Cập nhật UI trong quá trình xử lý
                txtResult.Text = result.ToString();
                txtResult.SelectionStart = txtResult.Text.Length;
                txtResult.ScrollToCaret();
                Application.DoEvents();
            }

            result.AppendLine();
            result.AppendLine("=" + new string('=', 50));
            result.AppendLine("✅ Hoàn thành!");

            txtResult.Text = result.ToString();
            lblStatus.Text = "✅ Xử lý hoàn thành!";
            lblStatus.ForeColor = Color.FromArgb(0, 150, 0);
        }
        catch (Exception ex)
        {
            lblStatus.Text = $"❌ Lỗi: {ex.Message}";
            lblStatus.ForeColor = Color.Red;
            txtResult.Text = $"Lỗi xử lý file audio:\n\n{ex.Message}\n\nGợi ý khắc phục:\n" +
                           "1. Đảm bảo file audio có định dạng hỗ trợ\n" +
                           "2. Kiểm tra file không bị hỏng\n" +
                           "3. Thử với file audio khác\n" +
                           "4. Đảm bảo có quyền ghi file tạm thời";
        }
        finally
        {
            // Xóa file tạm thời
            if (!string.IsNullOrEmpty(tempFile) && File.Exists(tempFile))
            {
                try
                {
                    File.Delete(tempFile);
                }
                catch { } // Bỏ qua lỗi xóa file tạm
            }
            
            btnProcess.Enabled = true;
            progressBar.Visible = false;
        }
    }

    private void btnClear_Click(object? sender, EventArgs e)
    {
        _selectedAudioFile = null;
        lblFileInfo.Text = "";
        txtResult.Clear();
        btnProcess.Enabled = false;
        lblStatus.Text = "✅ Model đã sẵn sàng! Kéo thả file audio để bắt đầu.";
        lblStatus.ForeColor = Color.FromArgb(0, 150, 0);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _whisperProcessor?.Dispose();
            _whisperFactory?.Dispose();
            if (components != null)
            {
                components.Dispose();
            }
        }
        base.Dispose(disposing);
    }

    private async void btnAnalyzeGemini_Click(object? sender, EventArgs e)
    {
        try
        {
            string inputText = txtResult.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(inputText))
            {
                MessageBox.Show("Chưa có nội dung để phân tích. Hãy chạy STT trước hoặc nhập nội dung.",
                    "Thiếu nội dung", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            btnAnalyzeGemini.Enabled = false;
            lblStatus.Text = "🔄 Đang gọi Gemini...";
            lblStatus.ForeColor = Color.FromArgb(0, 120, 215);
            progressBar.Visible = true;

            string? apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new Exception("Thiếu biến môi trường GEMINI_API_KEY");
            }

            // Validate API key format cơ bản
            if (!apiKey.StartsWith("AIza") || apiKey.Length < 35 || apiKey.Length > 60)
            {
                throw new Exception("API key không đúng định dạng (phải bắt đầu bằng AIza)");
            }

            // Kiểm tra API key có hợp lệ/mở được endpoint cơ bản hay không
            bool keyOk = await TestGeminiApiKey();
            if (!keyOk)
            {
                throw new Exception("API key không hợp lệ hoặc bị chặn theo region. Vui lòng kiểm tra hoặc thử lại qua VPN.");
            }

            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1beta/models/")
            };

            // Gộp system instruction vào nội dung user để tương thích streamGenerateContent
            string instruction = "Bạn là 1 chuyên gia phân tích ngôn ngữ tiếng Việt, hiện tại bạn cần phân tích số tiền các phần mà tôi đã dùng thông qua đoạn text đã được tôi xử lý từ Speech to text. Có 1 số lưu ý sau:\n1. ngôn ngữ có thể bị chuyển đổi sai 1 chút\nVí dụ:\nGửi, Rữ, Gử là Rưỡi (50)\nMột số keyword có thể là:\nBách Hóa Xanh\nSiêu Thị\nXem Phim\nĐá Banh\nCà Phê\n\n---\nDưới đây là nội dung cần phân tích:\n";

            var requestObj = new JsonObject
            {
                ["contents"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["role"] = "user",
                        ["parts"] = new JsonArray
                        {
                            new JsonObject { ["text"] = instruction + inputText }
                        }
                    }
                },
                ["generationConfig"] = new JsonObject
                {
                    ["thinkingConfig"] = new JsonObject { ["thinkingBudget"] = 0 },
                    ["responseMimeType"] = "application/json",
                    ["responseSchema"] = new JsonObject
                    {
                        ["type"] = "object",
                        ["properties"] = new JsonObject
                        {
                            ["Entities"] = new JsonObject
                            {
                                ["type"] = "array",
                                ["items"] = new JsonObject
                                {
                                    ["type"] = "object",
                                    ["properties"] = new JsonObject
                                    {
                                        ["Category"] = new JsonObject
                                        {
                                            ["type"] = "string",
                                            ["enum"] = new JsonArray { "Ăn uống", "Giải trí", "Chi phí khác", "Tiết kiệm" }
                                        },
                                        ["Value"] = new JsonObject
                                        {
                                            ["type"] = "number",
                                            ["description"] = "Số tiền chi tiêu ứng với category tính theo VND, ví dụ: 10000"
                                        },
                                        ["Payfor"] = new JsonObject
                                        {
                                            ["type"] = "string",
                                            ["description"] = "Hành động mà người đó chi tiêu ví dụ: Đi chợ, mua đồ, xem phim, đá bóng,.."
                                        }
                                    },
                                    ["required"] = new JsonArray { "Category", "Value", "Payfor" }
                                }
                            }
                        },
                        ["required"] = new JsonArray { "Entities" }
                    }
                }
            };

			string json = requestObj.ToJsonString();
			// Log JSON request để debug (không chứa API key)
			Debug.WriteLine("Gemini request: " + json);
			using var content = new StringContent(json, Encoding.UTF8, "application/json");
			using var request = new HttpRequestMessage(HttpMethod.Post, $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}")
			{
				Content = content
			};

			using var response = await httpClient.SendAsync(request);
			string responseBody = await response.Content.ReadAsStringAsync();
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"Gemini trả về {(int)response.StatusCode} {response.ReasonPhrase}:\n" + responseBody);
			}

			// Parse non-streaming response: lấy candidates[0].content.parts[*].text rồi nối lại
			var root = JsonNode.Parse(responseBody);
			string combinedText = string.Empty;
			var candidatesNode = root?["candidates"] as JsonArray;
			if (candidatesNode != null && candidatesNode.Count > 0)
			{
				var first = candidatesNode[0];
				var candidateContent = first?["content"];
				var parts = candidateContent?["parts"] as JsonArray;
				if (parts != null)
				{
					var sbParts = new System.Text.StringBuilder();
					foreach (var p in parts)
					{
						var t = p?["text"]?.ToString();
						if (!string.IsNullOrEmpty(t)) sbParts.Append(t);
					}
					combinedText = sbParts.ToString();
				}
			}

			// Thử parse chuỗi JSON trả về trong text
			txtResult.Clear();
			if (!string.IsNullOrWhiteSpace(combinedText))
			{
				try
				{
					var payload = JsonNode.Parse(combinedText);
					string pretty = payload!.ToJsonString(new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
					txtResult.AppendText(pretty + "\n");
				}
				catch
				{
					// Nếu model trả text không phải JSON hợp lệ, in thô để quan sát
					txtResult.AppendText(combinedText + "\n");
				}
			}
			else
			{
				// In nguyên body nếu không lấy được text
				txtResult.AppendText(responseBody + "\n");
			}

			lblStatus.Text = "✅ Phân tích hoàn tất";
            lblStatus.ForeColor = Color.FromArgb(0, 150, 0);
        }
        catch (Exception ex)
        {
            lblStatus.Text = $"❌ Lỗi Gemini: {ex.Message}";
            lblStatus.ForeColor = Color.Red;
            MessageBox.Show(ex.Message, "Lỗi gọi Gemini", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            progressBar.Visible = false;
            btnAnalyzeGemini.Enabled = true;
        }
    }

    // Test API key/khả dụng endpoint models
    private async Task<bool> TestGeminiApiKey()
    {
        try
        {
            string? apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            if (string.IsNullOrWhiteSpace(apiKey)) return false;
            using var httpClient = new HttpClient();
            string url = $"https://generativelanguage.googleapis.com/v1/models?key={apiKey}";
            using var response = await httpClient.GetAsync(url);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
