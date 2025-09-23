using System;
using System.IO;

namespace WhisperSTTUI;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
		LoadDotEnv();
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }    

	private static void LoadDotEnv()
	{
		try
		{
			string? baseDir = AppContext.BaseDirectory;
			string? current = baseDir;
			string? envPath = null;
			// Duyệt lên đến thư mục gốc ổ đĩa để tìm .env
			while (current != null)
			{
				string candidate = Path.Combine(current, ".env");
				if (File.Exists(candidate))
				{
					envPath = candidate;
					break;
				}
				current = Directory.GetParent(current)?.FullName;
			}

			// Thử thêm thư mục làm việc hiện tại nếu chưa tìm thấy
			if (envPath == null)
			{
				string cwdCandidate = Path.Combine(Directory.GetCurrentDirectory(), ".env");
				if (File.Exists(cwdCandidate)) envPath = cwdCandidate;
			}

			if (envPath == null) return;

			foreach (var rawLine in File.ReadAllLines(envPath))
			{
				var line = rawLine.Trim();
				// Loại bỏ BOM nếu có ở đầu file/dòng
				line = line.TrimStart('\uFEFF');
				if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;
				int eq = line.IndexOf('=');
				if (eq <= 0) continue;
				string key = line.Substring(0, eq).Trim().TrimStart('\uFEFF');
				string value = line.Substring(eq + 1).Trim();
				// Bỏ comment inline dạng key=value # comment
				int hash = value.IndexOf('#');
				if (hash >= 0) value = value.Substring(0, hash).Trim();
				// Loại bỏ cặp quote nếu bọc bởi "..." hoặc '...'
				if ((value.StartsWith("\"") && value.EndsWith("\"")) || (value.StartsWith("'") && value.EndsWith("'")))
				{
					value = value.Substring(1, value.Length - 2);
				}
				if (key.Length == 0) continue;
				Environment.SetEnvironmentVariable(key, value, EnvironmentVariableTarget.Process);
			}
		}
		catch { }
	}
}