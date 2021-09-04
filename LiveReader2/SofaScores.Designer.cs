namespace LiveReader2
{
    partial class SofaScores
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SofaScores));
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.textCurrentDay = new System.Windows.Forms.TextBox();
            this.buttonStart = new System.Windows.Forms.Button();
            this.labelCurrentDay = new System.Windows.Forms.Label();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.panelBrowser = new System.Windows.Forms.Panel();
            this.splitContainerBottom = new System.Windows.Forms.SplitContainer();
            this.listLog = new System.Windows.Forms.ListBox();
            this.listWarning = new System.Windows.Forms.ListBox();
            this.timerMain = new System.Windows.Forms.Timer(this.components);
            this.timerLoadComplete = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStripMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.readDayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readMatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewUrlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cachePathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanelMain.SuspendLayout();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBottom)).BeginInit();
            this.splitContainerBottom.Panel1.SuspendLayout();
            this.splitContainerBottom.Panel2.SuspendLayout();
            this.splitContainerBottom.SuspendLayout();
            this.contextMenuStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 1;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Controls.Add(this.panelTop, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.splitContainerMain, 0, 1);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 2;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(780, 391);
            this.tableLayoutPanelMain.TabIndex = 0;
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.textCurrentDay);
            this.panelTop.Controls.Add(this.buttonStart);
            this.panelTop.Controls.Add(this.labelCurrentDay);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTop.Location = new System.Drawing.Point(3, 3);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(774, 24);
            this.panelTop.TabIndex = 0;
            // 
            // textCurrentDay
            // 
            this.textCurrentDay.Location = new System.Drawing.Point(81, 3);
            this.textCurrentDay.Name = "textCurrentDay";
            this.textCurrentDay.Size = new System.Drawing.Size(99, 20);
            this.textCurrentDay.TabIndex = 2;
            this.textCurrentDay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonStart
            // 
            this.buttonStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.buttonStart.ForeColor = System.Drawing.Color.Green;
            this.buttonStart.Location = new System.Drawing.Point(186, 1);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(62, 23);
            this.buttonStart.TabIndex = 1;
            this.buttonStart.Text = "Start!";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // labelCurrentDay
            // 
            this.labelCurrentDay.AutoSize = true;
            this.labelCurrentDay.Location = new System.Drawing.Point(9, 6);
            this.labelCurrentDay.Name = "labelCurrentDay";
            this.labelCurrentDay.Size = new System.Drawing.Size(66, 13);
            this.labelCurrentDay.TabIndex = 0;
            this.labelCurrentDay.Text = "Current Day:";
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(3, 33);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.panelBrowser);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.splitContainerBottom);
            this.splitContainerMain.Size = new System.Drawing.Size(774, 355);
            this.splitContainerMain.SplitterDistance = 219;
            this.splitContainerMain.TabIndex = 1;
            // 
            // panelBrowser
            // 
            this.panelBrowser.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBrowser.Location = new System.Drawing.Point(0, 0);
            this.panelBrowser.Name = "panelBrowser";
            this.panelBrowser.Size = new System.Drawing.Size(774, 219);
            this.panelBrowser.TabIndex = 0;
            // 
            // splitContainerBottom
            // 
            this.splitContainerBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerBottom.Location = new System.Drawing.Point(0, 0);
            this.splitContainerBottom.Name = "splitContainerBottom";
            // 
            // splitContainerBottom.Panel1
            // 
            this.splitContainerBottom.Panel1.Controls.Add(this.listLog);
            // 
            // splitContainerBottom.Panel2
            // 
            this.splitContainerBottom.Panel2.Controls.Add(this.listWarning);
            this.splitContainerBottom.Size = new System.Drawing.Size(774, 132);
            this.splitContainerBottom.SplitterDistance = 376;
            this.splitContainerBottom.TabIndex = 0;
            // 
            // listLog
            // 
            this.listLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listLog.FormattingEnabled = true;
            this.listLog.HorizontalScrollbar = true;
            this.listLog.IntegralHeight = false;
            this.listLog.Location = new System.Drawing.Point(0, 0);
            this.listLog.Name = "listLog";
            this.listLog.ScrollAlwaysVisible = true;
            this.listLog.Size = new System.Drawing.Size(376, 132);
            this.listLog.TabIndex = 0;
            this.listLog.DoubleClick += new System.EventHandler(this.listLog_DoubleClick);
            // 
            // listWarning
            // 
            this.listWarning.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listWarning.FormattingEnabled = true;
            this.listWarning.HorizontalScrollbar = true;
            this.listWarning.IntegralHeight = false;
            this.listWarning.Location = new System.Drawing.Point(0, 0);
            this.listWarning.Name = "listWarning";
            this.listWarning.ScrollAlwaysVisible = true;
            this.listWarning.Size = new System.Drawing.Size(394, 132);
            this.listWarning.TabIndex = 0;
            this.listWarning.DoubleClick += new System.EventHandler(this.listDayErrors_DoubleClick);
            // 
            // timerMain
            // 
            this.timerMain.Tick += new System.EventHandler(this.timerMain_Tick);
            // 
            // timerLoadComplete
            // 
            this.timerLoadComplete.Tick += new System.EventHandler(this.timerLoadComplete_Tick);
            // 
            // contextMenuStripMain
            // 
            this.contextMenuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.readDayToolStripMenuItem,
            this.readMatchToolStripMenuItem,
            this.viewUrlToolStripMenuItem,
            this.cachePathToolStripMenuItem});
            this.contextMenuStripMain.Name = "contextMenuStripMain";
            this.contextMenuStripMain.Size = new System.Drawing.Size(153, 114);
            // 
            // readDayToolStripMenuItem
            // 
            this.readDayToolStripMenuItem.Name = "readDayToolStripMenuItem";
            this.readDayToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.readDayToolStripMenuItem.Text = "ReadDay";
            this.readDayToolStripMenuItem.Click += new System.EventHandler(this.readDayToolStripMenuItem_Click);
            // 
            // readMatchToolStripMenuItem
            // 
            this.readMatchToolStripMenuItem.Name = "readMatchToolStripMenuItem";
            this.readMatchToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.readMatchToolStripMenuItem.Text = "ReadMatch";
            this.readMatchToolStripMenuItem.Click += new System.EventHandler(this.readMatchToolStripMenuItem_Click);
            // 
            // viewUrlToolStripMenuItem
            // 
            this.viewUrlToolStripMenuItem.Name = "viewUrlToolStripMenuItem";
            this.viewUrlToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.viewUrlToolStripMenuItem.Text = "View Url";
            this.viewUrlToolStripMenuItem.Click += new System.EventHandler(this.viewUrlToolStripMenuItem_Click);
            // 
            // cachePathToolStripMenuItem
            // 
            this.cachePathToolStripMenuItem.Name = "cachePathToolStripMenuItem";
            this.cachePathToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.cachePathToolStripMenuItem.Text = "Cache Path";
            this.cachePathToolStripMenuItem.Click += new System.EventHandler(this.cachePathToolStripMenuItem_Click);
            // 
            // SofaScores
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(780, 391);
            this.ContextMenuStrip = this.contextMenuStripMain;
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SofaScores";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SofaScores";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SofaScores_FormClosing);
            this.Load += new System.EventHandler(this.SofaScores_Load);
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.splitContainerBottom.Panel1.ResumeLayout(false);
            this.splitContainerBottom.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBottom)).EndInit();
            this.splitContainerBottom.ResumeLayout(false);
            this.contextMenuStripMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.TextBox textCurrentDay;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Label labelCurrentDay;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.SplitContainer splitContainerBottom;
        private System.Windows.Forms.Panel panelBrowser;
        private System.Windows.Forms.ListBox listLog;
        private System.Windows.Forms.ListBox listWarning;
        private System.Windows.Forms.Timer timerMain;
        private System.Windows.Forms.Timer timerLoadComplete;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripMain;
        private System.Windows.Forms.ToolStripMenuItem readDayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem readMatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewUrlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cachePathToolStripMenuItem;

    }
}