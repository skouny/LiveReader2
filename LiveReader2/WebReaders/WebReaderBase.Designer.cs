namespace LiveReader2
{
    partial class WebReaderBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WebReaderBase));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.listBoxLog = new System.Windows.Forms.ListBox();
            this.groupBoxCpuUsage = new System.Windows.Forms.GroupBox();
            this.numericUpDownMatchBreak = new System.Windows.Forms.NumericUpDown();
            this.checkBoxUseParallelProcessing = new System.Windows.Forms.CheckBox();
            this.numericUpDownLoopBreak = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxShowLoopLog = new System.Windows.Forms.CheckBox();
            this.numericUpDownPageRefresh = new System.Windows.Forms.NumericUpDown();
            this.checkBoxShowDetailedLog = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.groupBoxCpuUsage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMatchBreak)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLoopBreak)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPageRefresh)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer.Location = new System.Drawing.Point(0, 94);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.webBrowser);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.listBoxLog);
            this.splitContainer.Size = new System.Drawing.Size(782, 465);
            this.splitContainer.SplitterDistance = 328;
            this.splitContainer.TabIndex = 0;
            // 
            // webBrowser
            // 
            this.webBrowser.AllowNavigation = false;
            this.webBrowser.AllowWebBrowserDrop = false;
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.ScriptErrorsSuppressed = true;
            this.webBrowser.Size = new System.Drawing.Size(778, 324);
            this.webBrowser.TabIndex = 0;
            this.webBrowser.WebBrowserShortcutsEnabled = false;
            this.webBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser_DocumentCompleted);
            // 
            // listBoxLog
            // 
            this.listBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxLog.FormattingEnabled = true;
            this.listBoxLog.HorizontalScrollbar = true;
            this.listBoxLog.IntegralHeight = false;
            this.listBoxLog.Location = new System.Drawing.Point(0, 0);
            this.listBoxLog.Name = "listBoxLog";
            this.listBoxLog.Size = new System.Drawing.Size(778, 129);
            this.listBoxLog.TabIndex = 2;
            // 
            // groupBoxCpuUsage
            // 
            this.groupBoxCpuUsage.Controls.Add(this.numericUpDownMatchBreak);
            this.groupBoxCpuUsage.Controls.Add(this.checkBoxUseParallelProcessing);
            this.groupBoxCpuUsage.Controls.Add(this.numericUpDownLoopBreak);
            this.groupBoxCpuUsage.Controls.Add(this.label2);
            this.groupBoxCpuUsage.Controls.Add(this.label1);
            this.groupBoxCpuUsage.Location = new System.Drawing.Point(2, 2);
            this.groupBoxCpuUsage.Name = "groupBoxCpuUsage";
            this.groupBoxCpuUsage.Size = new System.Drawing.Size(207, 86);
            this.groupBoxCpuUsage.TabIndex = 1;
            this.groupBoxCpuUsage.TabStop = false;
            this.groupBoxCpuUsage.Text = "CPU Usage Settings";
            // 
            // numericUpDownMatchBreak
            // 
            this.numericUpDownMatchBreak.Location = new System.Drawing.Point(109, 42);
            this.numericUpDownMatchBreak.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.numericUpDownMatchBreak.Name = "numericUpDownMatchBreak";
            this.numericUpDownMatchBreak.Size = new System.Drawing.Size(90, 20);
            this.numericUpDownMatchBreak.TabIndex = 6;
            this.numericUpDownMatchBreak.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownMatchBreak.ValueChanged += new System.EventHandler(this.numericUpDownMatchBreak_ValueChanged);
            // 
            // checkBoxUseParallelProcessing
            // 
            this.checkBoxUseParallelProcessing.Location = new System.Drawing.Point(10, 63);
            this.checkBoxUseParallelProcessing.Name = "checkBoxUseParallelProcessing";
            this.checkBoxUseParallelProcessing.Size = new System.Drawing.Size(189, 22);
            this.checkBoxUseParallelProcessing.TabIndex = 4;
            this.checkBoxUseParallelProcessing.Text = "Use Parallel Processing";
            this.checkBoxUseParallelProcessing.UseVisualStyleBackColor = true;
            this.checkBoxUseParallelProcessing.CheckedChanged += new System.EventHandler(this.checkBoxUseParallelProcessing_CheckedChanged);
            // 
            // numericUpDownLoopBreak
            // 
            this.numericUpDownLoopBreak.Location = new System.Drawing.Point(109, 19);
            this.numericUpDownLoopBreak.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.numericUpDownLoopBreak.Name = "numericUpDownLoopBreak";
            this.numericUpDownLoopBreak.Size = new System.Drawing.Size(90, 20);
            this.numericUpDownLoopBreak.TabIndex = 5;
            this.numericUpDownLoopBreak.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numericUpDownLoopBreak.ValueChanged += new System.EventHandler(this.numericUpDownLoopBreak_ValueChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Match Break (ms):";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Loop Break (ms):";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.checkBoxShowLoopLog);
            this.groupBox1.Controls.Add(this.numericUpDownPageRefresh);
            this.groupBox1.Controls.Add(this.checkBoxShowDetailedLog);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(215, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(567, 86);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Misc Settings";
            // 
            // checkBoxShowLoopLog
            // 
            this.checkBoxShowLoopLog.AutoSize = true;
            this.checkBoxShowLoopLog.Location = new System.Drawing.Point(9, 45);
            this.checkBoxShowLoopLog.Name = "checkBoxShowLoopLog";
            this.checkBoxShowLoopLog.Size = new System.Drawing.Size(101, 17);
            this.checkBoxShowLoopLog.TabIndex = 6;
            this.checkBoxShowLoopLog.Text = "Show Loop Log";
            this.checkBoxShowLoopLog.UseVisualStyleBackColor = true;
            this.checkBoxShowLoopLog.CheckedChanged += new System.EventHandler(this.checkBoxShowLoopLog_CheckedChanged);
            // 
            // numericUpDownPageRefresh
            // 
            this.numericUpDownPageRefresh.Location = new System.Drawing.Point(150, 20);
            this.numericUpDownPageRefresh.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.numericUpDownPageRefresh.Name = "numericUpDownPageRefresh";
            this.numericUpDownPageRefresh.Size = new System.Drawing.Size(90, 20);
            this.numericUpDownPageRefresh.TabIndex = 5;
            this.numericUpDownPageRefresh.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numericUpDownPageRefresh.ValueChanged += new System.EventHandler(this.numericUpDownPageRefresh_ValueChanged);
            // 
            // checkBoxShowDetailedLog
            // 
            this.checkBoxShowDetailedLog.AutoSize = true;
            this.checkBoxShowDetailedLog.Location = new System.Drawing.Point(9, 63);
            this.checkBoxShowDetailedLog.Name = "checkBoxShowDetailedLog";
            this.checkBoxShowDetailedLog.Size = new System.Drawing.Size(116, 17);
            this.checkBoxShowDetailedLog.TabIndex = 4;
            this.checkBoxShowDetailedLog.Text = "Show Detailed Log";
            this.checkBoxShowDetailedLog.UseVisualStyleBackColor = true;
            this.checkBoxShowDetailedLog.CheckedChanged += new System.EventHandler(this.checkBoxShowDetailedLog_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(138, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Page Refresh Interval (min):";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // WebReaderBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBoxCpuUsage);
            this.Controls.Add(this.splitContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "WebReaderBase";
            this.Text = "WebReaderBase";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WebReaderBase_FormClosing);
            this.Load += new System.EventHandler(this.WebReaderBase_Load);
            this.Shown += new System.EventHandler(this.WebReaderBase_Shown);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.groupBoxCpuUsage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMatchBreak)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLoopBreak)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPageRefresh)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.ListBox listBoxLog;
        private System.Windows.Forms.GroupBox groupBoxCpuUsage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxUseParallelProcessing;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxShowDetailedLog;
        private System.Windows.Forms.Label label3;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.NumericUpDown numericUpDownMatchBreak;
        private System.Windows.Forms.NumericUpDown numericUpDownLoopBreak;
        private System.Windows.Forms.NumericUpDown numericUpDownPageRefresh;
        private System.Windows.Forms.CheckBox checkBoxShowLoopLog;
    }
}