using Whisper.net;
using Whisper.net.Ggml;
using NAudio.Wave;

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
            string modelPath = "ggml-base.bin";
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
}
