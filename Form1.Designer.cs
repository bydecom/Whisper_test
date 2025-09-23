namespace WhisperSTTUI;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;


    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.lblTitle = new Label();
        this.pnlDropZone = new Panel();
        this.lblDropText = new Label();
        this.lblFileInfo = new Label();
        this.btnProcess = new Button();
        this.txtResult = new TextBox();
        this.progressBar = new ProgressBar();
        this.lblStatus = new Label();
        this.btnClear = new Button();
		this.btnAnalyzeGemini = new Button();
        this.pnlDropZone.SuspendLayout();
        this.SuspendLayout();
        
        // 
        // lblTitle
        // 
        this.lblTitle.AutoSize = true;
        this.lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        this.lblTitle.ForeColor = Color.FromArgb(0, 120, 215);
        this.lblTitle.Location = new Point(20, 20);
        this.lblTitle.Name = "lblTitle";
        this.lblTitle.Size = new Size(400, 30);
        this.lblTitle.TabIndex = 0;
        this.lblTitle.Text = "🎤 Speech-to-Text với Whisper.net";
        
        // 
        // pnlDropZone
        // 
        this.pnlDropZone.AllowDrop = true;
        this.pnlDropZone.BackColor = Color.FromArgb(240, 248, 255);
        this.pnlDropZone.BorderStyle = BorderStyle.FixedSingle;
        this.pnlDropZone.Controls.Add(this.lblDropText);
        this.pnlDropZone.Location = new Point(20, 70);
        this.pnlDropZone.Name = "pnlDropZone";
        this.pnlDropZone.Size = new Size(760, 150);
        this.pnlDropZone.TabIndex = 1;
        this.pnlDropZone.DragEnter += new DragEventHandler(this.pnlDropZone_DragEnter);
        this.pnlDropZone.DragDrop += new DragEventHandler(this.pnlDropZone_DragDrop);
        
        // 
        // lblDropText
        // 
        this.lblDropText.AutoSize = true;
        this.lblDropText.Font = new Font("Segoe UI", 14F);
        this.lblDropText.ForeColor = Color.FromArgb(100, 100, 100);
        this.lblDropText.Location = new Point(300, 60);
        this.lblDropText.Name = "lblDropText";
        this.lblDropText.Size = new Size(200, 25);
        this.lblDropText.TabIndex = 0;
        this.lblDropText.Text = "📁 Kéo thả file audio vào đây";
        
        // 
        // lblFileInfo
        // 
        this.lblFileInfo.AutoSize = true;
        this.lblFileInfo.Font = new Font("Segoe UI", 10F);
        this.lblFileInfo.ForeColor = Color.FromArgb(60, 60, 60);
        this.lblFileInfo.Location = new Point(20, 240);
        this.lblFileInfo.Name = "lblFileInfo";
        this.lblFileInfo.Size = new Size(0, 19);
        this.lblFileInfo.TabIndex = 2;
        
        // 
        // btnProcess
        // 
        this.btnProcess.BackColor = Color.FromArgb(0, 120, 215);
        this.btnProcess.Enabled = false;
        this.btnProcess.FlatStyle = FlatStyle.Flat;
        this.btnProcess.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        this.btnProcess.ForeColor = Color.White;
        this.btnProcess.Location = new Point(20, 280);
        this.btnProcess.Name = "btnProcess";
        this.btnProcess.Size = new Size(150, 40);
        this.btnProcess.TabIndex = 3;
        this.btnProcess.Text = "🔄 Xử lý";
        this.btnProcess.UseVisualStyleBackColor = false;
        this.btnProcess.Click += new EventHandler(this.btnProcess_Click);
        
        // 
        // btnClear
        // 
        this.btnClear.BackColor = Color.FromArgb(200, 200, 200);
        this.btnClear.FlatStyle = FlatStyle.Flat;
        this.btnClear.Font = new Font("Segoe UI", 10F);
        this.btnClear.ForeColor = Color.White;
        this.btnClear.Location = new Point(190, 280);
        this.btnClear.Name = "btnClear";
        this.btnClear.Size = new Size(100, 40);
        this.btnClear.TabIndex = 4;
        this.btnClear.Text = "🗑️ Xóa";
        this.btnClear.UseVisualStyleBackColor = false;
        this.btnClear.Click += new EventHandler(this.btnClear_Click);
		
		// 
		// btnAnalyzeGemini
		// 
		this.btnAnalyzeGemini.BackColor = Color.FromArgb(0, 150, 0);
		this.btnAnalyzeGemini.FlatStyle = FlatStyle.Flat;
		this.btnAnalyzeGemini.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
		this.btnAnalyzeGemini.ForeColor = Color.White;
		this.btnAnalyzeGemini.Location = new Point(310, 280);
		this.btnAnalyzeGemini.Name = "btnAnalyzeGemini";
		this.btnAnalyzeGemini.Size = new Size(200, 40);
		this.btnAnalyzeGemini.TabIndex = 8;
		this.btnAnalyzeGemini.Text = "📊 Phân tích (Gemini)";
		this.btnAnalyzeGemini.UseVisualStyleBackColor = false;
		this.btnAnalyzeGemini.Click += new EventHandler(this.btnAnalyzeGemini_Click);
        
        // 
        // progressBar
        // 
        this.progressBar.Location = new Point(20, 340);
        this.progressBar.Name = "progressBar";
        this.progressBar.Size = new Size(760, 20);
        this.progressBar.Style = ProgressBarStyle.Marquee;
        this.progressBar.TabIndex = 5;
        this.progressBar.Visible = false;
        
        // 
        // lblStatus
        // 
        this.lblStatus.AutoSize = true;
        this.lblStatus.Font = new Font("Segoe UI", 10F);
        this.lblStatus.ForeColor = Color.FromArgb(60, 60, 60);
        this.lblStatus.Location = new Point(20, 370);
        this.lblStatus.Name = "lblStatus";
        this.lblStatus.Size = new Size(0, 19);
        this.lblStatus.TabIndex = 6;
        
        // 
        // txtResult
        // 
        this.txtResult.Font = new Font("Segoe UI", 11F);
        this.txtResult.Location = new Point(20, 400);
        this.txtResult.Multiline = true;
        this.txtResult.Name = "txtResult";
        this.txtResult.ReadOnly = true;
        this.txtResult.ScrollBars = ScrollBars.Vertical;
        this.txtResult.Size = new Size(760, 200);
        this.txtResult.TabIndex = 7;
        
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.BackColor = Color.White;
        this.ClientSize = new Size(800, 620);
		this.Controls.Add(this.btnAnalyzeGemini);
        this.Controls.Add(this.txtResult);
        this.Controls.Add(this.lblStatus);
        this.Controls.Add(this.progressBar);
        this.Controls.Add(this.btnClear);
        this.Controls.Add(this.btnProcess);
        this.Controls.Add(this.lblFileInfo);
        this.Controls.Add(this.pnlDropZone);
        this.Controls.Add(this.lblTitle);
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.Name = "Form1";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "Speech-to-Text Demo";
        this.pnlDropZone.ResumeLayout(false);
        this.pnlDropZone.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private Label lblTitle;
    private Panel pnlDropZone;
    private Label lblDropText;
    private Label lblFileInfo;
    private Button btnProcess;
    private Button btnClear;
    private ProgressBar progressBar;
    private Label lblStatus;
    private TextBox txtResult;
	private Button btnAnalyzeGemini;
}
