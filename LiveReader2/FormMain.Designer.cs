namespace LiveReader2
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.toolStripContainerMain = new System.Windows.Forms.ToolStripContainer();
            this.statusMain = new System.Windows.Forms.StatusStrip();
            this.labelVersion = new System.Windows.Forms.ToolStripStatusLabel();
            this.labelRestart = new System.Windows.Forms.ToolStripStatusLabel();
            this.labelUpdate = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabWebReaders = new System.Windows.Forms.TabPage();
            this.tabLiveFeeder = new System.Windows.Forms.TabPage();
            this.tabControlLiveFeeder = new System.Windows.Forms.TabControl();
            this.tabFootballGr = new System.Windows.Forms.TabPage();
            this.splitContainerFootballGr = new System.Windows.Forms.SplitContainer();
            this.checkLivestats = new System.Windows.Forms.CheckBox();
            this.checkFullUpdate = new System.Windows.Forms.CheckBox();
            this.hideCompletedFootballGr = new System.Windows.Forms.CheckBox();
            this.updateDayFootballGr = new System.Windows.Forms.Button();
            this.textDayFootballGr = new System.Windows.Forms.TextBox();
            this.calendarFootballGr = new System.Windows.Forms.MonthCalendar();
            this.viewFootballGr = new System.Windows.Forms.ListView();
            this.refreshFootballGr = new System.Windows.Forms.Button();
            this.logFootballGr = new System.Windows.Forms.ListBox();
            this.tabFootballEn = new System.Windows.Forms.TabPage();
            this.splitContainerFootbalEn = new System.Windows.Forms.SplitContainer();
            this.listView1 = new System.Windows.Forms.ListView();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.logFootballEn = new System.Windows.Forms.ListBox();
            this.tabBasketGr = new System.Windows.Forms.TabPage();
            this.splitContainerBasketGr = new System.Windows.Forms.SplitContainer();
            this.hideCompletedBasketGr = new System.Windows.Forms.CheckBox();
            this.updateDayBasketGr = new System.Windows.Forms.Button();
            this.calendarBasketGr = new System.Windows.Forms.MonthCalendar();
            this.textDayBasketGr = new System.Windows.Forms.TextBox();
            this.viewBasketGr = new System.Windows.Forms.ListView();
            this.refreshBasketGr = new System.Windows.Forms.Button();
            this.logBasketGr = new System.Windows.Forms.ListBox();
            this.tabBasketEn = new System.Windows.Forms.TabPage();
            this.splitContainerBasketEn = new System.Windows.Forms.SplitContainer();
            this.listView3 = new System.Windows.Forms.ListView();
            this.button3 = new System.Windows.Forms.Button();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.logBasketEn = new System.Windows.Forms.ListBox();
            this.tabFormMaker = new System.Windows.Forms.TabPage();
            this.tabControlFormMaker = new System.Windows.Forms.TabControl();
            this.tabFormsFootballGr = new System.Windows.Forms.TabPage();
            this.splitContainerFomMaker = new System.Windows.Forms.SplitContainer();
            this.recreateFormMakerGr = new System.Windows.Forms.Button();
            this.createFormMakerGr = new System.Windows.Forms.Button();
            this.calendarFormMaker = new System.Windows.Forms.MonthCalendar();
            this.textDayFormMakerGr = new System.Windows.Forms.TextBox();
            this.refreshFormMakerGr = new System.Windows.Forms.Button();
            this.viewFormMakerGr = new System.Windows.Forms.ListView();
            this.logFormMakerGr = new System.Windows.Forms.ListBox();
            this.tabFormsFootballEn = new System.Windows.Forms.TabPage();
            this.tabBetMixMaker = new System.Windows.Forms.TabPage();
            this.tabControlBetMixMaker = new System.Windows.Forms.TabControl();
            this.tabBetMixFootballGr = new System.Windows.Forms.TabPage();
            this.splitContainerBetMixGr = new System.Windows.Forms.SplitContainer();
            this.recreateBetMixGr = new System.Windows.Forms.Button();
            this.calendarBetMixGr = new System.Windows.Forms.MonthCalendar();
            this.createBetMixGr = new System.Windows.Forms.Button();
            this.refreshBetMix = new System.Windows.Forms.Button();
            this.textDayBetMixGr = new System.Windows.Forms.TextBox();
            this.viewBetMixGr = new System.Windows.Forms.ListView();
            this.logBetMixGr = new System.Windows.Forms.ListBox();
            this.tabBetMixFootballEn = new System.Windows.Forms.TabPage();
            this.toolStripTop = new System.Windows.Forms.ToolStrip();
            this.buttonRestart = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonCleanupAndRestart = new System.Windows.Forms.ToolStripButton();
            this.menuMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showHideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateStandingsFeedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.clearFootballToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearBasketToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanUpRestartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.openDataDirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyMain = new System.Windows.Forms.NotifyIcon(this.components);
            this.timerMain = new System.Windows.Forms.Timer(this.components);
            this.workerUpdate = new System.ComponentModel.BackgroundWorker();
            this.workerFootballGr = new System.ComponentModel.BackgroundWorker();
            this.workerFootballEn = new System.ComponentModel.BackgroundWorker();
            this.workerBasketGr = new System.ComponentModel.BackgroundWorker();
            this.workerBasketEn = new System.ComponentModel.BackgroundWorker();
            this.workerFormMakerGr = new System.ComponentModel.BackgroundWorker();
            this.workerBetMixGr = new System.ComponentModel.BackgroundWorker();
            this.toolStripContainerMain.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainerMain.ContentPanel.SuspendLayout();
            this.toolStripContainerMain.TopToolStripPanel.SuspendLayout();
            this.toolStripContainerMain.SuspendLayout();
            this.statusMain.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.tabLiveFeeder.SuspendLayout();
            this.tabControlLiveFeeder.SuspendLayout();
            this.tabFootballGr.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerFootballGr)).BeginInit();
            this.splitContainerFootballGr.Panel1.SuspendLayout();
            this.splitContainerFootballGr.Panel2.SuspendLayout();
            this.splitContainerFootballGr.SuspendLayout();
            this.tabFootballEn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerFootbalEn)).BeginInit();
            this.splitContainerFootbalEn.Panel1.SuspendLayout();
            this.splitContainerFootbalEn.Panel2.SuspendLayout();
            this.splitContainerFootbalEn.SuspendLayout();
            this.tabBasketGr.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBasketGr)).BeginInit();
            this.splitContainerBasketGr.Panel1.SuspendLayout();
            this.splitContainerBasketGr.Panel2.SuspendLayout();
            this.splitContainerBasketGr.SuspendLayout();
            this.tabBasketEn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBasketEn)).BeginInit();
            this.splitContainerBasketEn.Panel1.SuspendLayout();
            this.splitContainerBasketEn.Panel2.SuspendLayout();
            this.splitContainerBasketEn.SuspendLayout();
            this.tabFormMaker.SuspendLayout();
            this.tabControlFormMaker.SuspendLayout();
            this.tabFormsFootballGr.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerFomMaker)).BeginInit();
            this.splitContainerFomMaker.Panel1.SuspendLayout();
            this.splitContainerFomMaker.Panel2.SuspendLayout();
            this.splitContainerFomMaker.SuspendLayout();
            this.tabBetMixMaker.SuspendLayout();
            this.tabControlBetMixMaker.SuspendLayout();
            this.tabBetMixFootballGr.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBetMixGr)).BeginInit();
            this.splitContainerBetMixGr.Panel1.SuspendLayout();
            this.splitContainerBetMixGr.Panel2.SuspendLayout();
            this.splitContainerBetMixGr.SuspendLayout();
            this.toolStripTop.SuspendLayout();
            this.menuMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainerMain
            // 
            // 
            // toolStripContainerMain.BottomToolStripPanel
            // 
            this.toolStripContainerMain.BottomToolStripPanel.Controls.Add(this.statusMain);
            // 
            // toolStripContainerMain.ContentPanel
            // 
            this.toolStripContainerMain.ContentPanel.Controls.Add(this.tabControlMain);
            this.toolStripContainerMain.ContentPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.toolStripContainerMain.ContentPanel.Size = new System.Drawing.Size(991, 536);
            this.toolStripContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainerMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainerMain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.toolStripContainerMain.Name = "toolStripContainerMain";
            this.toolStripContainerMain.Size = new System.Drawing.Size(991, 593);
            this.toolStripContainerMain.TabIndex = 0;
            this.toolStripContainerMain.Text = "toolStripContainer1";
            // 
            // toolStripContainerMain.TopToolStripPanel
            // 
            this.toolStripContainerMain.TopToolStripPanel.Controls.Add(this.toolStripTop);
            this.toolStripContainerMain.TopToolStripPanel.Margin = new System.Windows.Forms.Padding(3);
            this.toolStripContainerMain.TopToolStripPanel.Padding = new System.Windows.Forms.Padding(3);
            this.toolStripContainerMain.TopToolStripPanel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            // 
            // statusMain
            // 
            this.statusMain.Dock = System.Windows.Forms.DockStyle.None;
            this.statusMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelVersion,
            this.labelRestart,
            this.labelUpdate});
            this.statusMain.Location = new System.Drawing.Point(0, 0);
            this.statusMain.Name = "statusMain";
            this.statusMain.Size = new System.Drawing.Size(991, 24);
            this.statusMain.TabIndex = 0;
            // 
            // labelVersion
            // 
            this.labelVersion.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.labelVersion.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.labelVersion.Margin = new System.Windows.Forms.Padding(3, 3, 0, 2);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(52, 19);
            this.labelVersion.Text = "Version:";
            // 
            // labelRestart
            // 
            this.labelRestart.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.labelRestart.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.labelRestart.Margin = new System.Windows.Forms.Padding(32, 3, 0, 2);
            this.labelRestart.Name = "labelRestart";
            this.labelRestart.Size = new System.Drawing.Size(53, 19);
            this.labelRestart.Text = "Restart: ";
            // 
            // labelUpdate
            // 
            this.labelUpdate.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.labelUpdate.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.labelUpdate.Margin = new System.Windows.Forms.Padding(32, 3, 0, 2);
            this.labelUpdate.Name = "labelUpdate";
            this.labelUpdate.Size = new System.Drawing.Size(52, 19);
            this.labelUpdate.Text = "Update:";
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabWebReaders);
            this.tabControlMain.Controls.Add(this.tabLiveFeeder);
            this.tabControlMain.Controls.Add(this.tabFormMaker);
            this.tabControlMain.Controls.Add(this.tabBetMixMaker);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(991, 536);
            this.tabControlMain.TabIndex = 0;
            // 
            // tabWebReaders
            // 
            this.tabWebReaders.AutoScroll = true;
            this.tabWebReaders.Location = new System.Drawing.Point(4, 23);
            this.tabWebReaders.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabWebReaders.Name = "tabWebReaders";
            this.tabWebReaders.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabWebReaders.Size = new System.Drawing.Size(983, 509);
            this.tabWebReaders.TabIndex = 0;
            this.tabWebReaders.Text = "WebReaders";
            this.tabWebReaders.UseVisualStyleBackColor = true;
            // 
            // tabLiveFeeder
            // 
            this.tabLiveFeeder.Controls.Add(this.tabControlLiveFeeder);
            this.tabLiveFeeder.Location = new System.Drawing.Point(4, 22);
            this.tabLiveFeeder.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabLiveFeeder.Name = "tabLiveFeeder";
            this.tabLiveFeeder.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabLiveFeeder.Size = new System.Drawing.Size(658, 374);
            this.tabLiveFeeder.TabIndex = 4;
            this.tabLiveFeeder.Text = "Live Feeder";
            this.tabLiveFeeder.UseVisualStyleBackColor = true;
            // 
            // tabControlLiveFeeder
            // 
            this.tabControlLiveFeeder.Controls.Add(this.tabFootballGr);
            this.tabControlLiveFeeder.Controls.Add(this.tabFootballEn);
            this.tabControlLiveFeeder.Controls.Add(this.tabBasketGr);
            this.tabControlLiveFeeder.Controls.Add(this.tabBasketEn);
            this.tabControlLiveFeeder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlLiveFeeder.Location = new System.Drawing.Point(4, 3);
            this.tabControlLiveFeeder.Name = "tabControlLiveFeeder";
            this.tabControlLiveFeeder.SelectedIndex = 0;
            this.tabControlLiveFeeder.Size = new System.Drawing.Size(650, 368);
            this.tabControlLiveFeeder.TabIndex = 0;
            // 
            // tabFootballGr
            // 
            this.tabFootballGr.Controls.Add(this.splitContainerFootballGr);
            this.tabFootballGr.Location = new System.Drawing.Point(4, 23);
            this.tabFootballGr.Name = "tabFootballGr";
            this.tabFootballGr.Padding = new System.Windows.Forms.Padding(3);
            this.tabFootballGr.Size = new System.Drawing.Size(642, 341);
            this.tabFootballGr.TabIndex = 0;
            this.tabFootballGr.Text = "Football GR";
            this.tabFootballGr.UseVisualStyleBackColor = true;
            // 
            // splitContainerFootballGr
            // 
            this.splitContainerFootballGr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerFootballGr.Location = new System.Drawing.Point(3, 3);
            this.splitContainerFootballGr.Name = "splitContainerFootballGr";
            this.splitContainerFootballGr.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerFootballGr.Panel1
            // 
            this.splitContainerFootballGr.Panel1.Controls.Add(this.checkLivestats);
            this.splitContainerFootballGr.Panel1.Controls.Add(this.checkFullUpdate);
            this.splitContainerFootballGr.Panel1.Controls.Add(this.hideCompletedFootballGr);
            this.splitContainerFootballGr.Panel1.Controls.Add(this.updateDayFootballGr);
            this.splitContainerFootballGr.Panel1.Controls.Add(this.textDayFootballGr);
            this.splitContainerFootballGr.Panel1.Controls.Add(this.calendarFootballGr);
            this.splitContainerFootballGr.Panel1.Controls.Add(this.viewFootballGr);
            this.splitContainerFootballGr.Panel1.Controls.Add(this.refreshFootballGr);
            // 
            // splitContainerFootballGr.Panel2
            // 
            this.splitContainerFootballGr.Panel2.Controls.Add(this.logFootballGr);
            this.splitContainerFootballGr.Size = new System.Drawing.Size(636, 335);
            this.splitContainerFootballGr.SplitterDistance = 227;
            this.splitContainerFootballGr.TabIndex = 2;
            // 
            // checkLivestats
            // 
            this.checkLivestats.AutoSize = true;
            this.checkLivestats.Checked = true;
            this.checkLivestats.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkLivestats.Location = new System.Drawing.Point(473, 6);
            this.checkLivestats.Name = "checkLivestats";
            this.checkLivestats.Size = new System.Drawing.Size(83, 18);
            this.checkLivestats.TabIndex = 10;
            this.checkLivestats.Text = "Livestats";
            this.checkLivestats.UseVisualStyleBackColor = true;
            // 
            // checkFullUpdate
            // 
            this.checkFullUpdate.AutoSize = true;
            this.checkFullUpdate.Checked = true;
            this.checkFullUpdate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkFullUpdate.Location = new System.Drawing.Point(349, 6);
            this.checkFullUpdate.Name = "checkFullUpdate";
            this.checkFullUpdate.Size = new System.Drawing.Size(97, 18);
            this.checkFullUpdate.TabIndex = 9;
            this.checkFullUpdate.Text = "Full Update";
            this.checkFullUpdate.UseVisualStyleBackColor = true;
            // 
            // hideCompletedFootballGr
            // 
            this.hideCompletedFootballGr.AutoSize = true;
            this.hideCompletedFootballGr.Location = new System.Drawing.Point(185, 6);
            this.hideCompletedFootballGr.Name = "hideCompletedFootballGr";
            this.hideCompletedFootballGr.Size = new System.Drawing.Size(126, 18);
            this.hideCompletedFootballGr.TabIndex = 8;
            this.hideCompletedFootballGr.Text = "Hide Completed";
            this.hideCompletedFootballGr.UseVisualStyleBackColor = true;
            this.hideCompletedFootballGr.CheckedChanged += new System.EventHandler(this.hideCompletedFootballGr_CheckedChanged);
            // 
            // updateDayFootballGr
            // 
            this.updateDayFootballGr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.updateDayFootballGr.Location = new System.Drawing.Point(507, 3);
            this.updateDayFootballGr.Name = "updateDayFootballGr";
            this.updateDayFootballGr.Size = new System.Drawing.Size(126, 23);
            this.updateDayFootballGr.TabIndex = 7;
            this.updateDayFootballGr.Text = "Opap Update";
            this.updateDayFootballGr.UseVisualStyleBackColor = true;
            this.updateDayFootballGr.Click += new System.EventHandler(this.updateDayFootballGr_Click);
            // 
            // textDayFootballGr
            // 
            this.textDayFootballGr.Location = new System.Drawing.Point(3, 3);
            this.textDayFootballGr.Name = "textDayFootballGr";
            this.textDayFootballGr.Size = new System.Drawing.Size(90, 22);
            this.textDayFootballGr.TabIndex = 6;
            this.textDayFootballGr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textDayFootballGr.TextChanged += new System.EventHandler(this.textDayFootballGr_TextChanged);
            this.textDayFootballGr.MouseEnter += new System.EventHandler(this.textDayFootballGr_MouseEnter);
            // 
            // calendarFootballGr
            // 
            this.calendarFootballGr.Location = new System.Drawing.Point(9, 37);
            this.calendarFootballGr.Name = "calendarFootballGr";
            this.calendarFootballGr.TabIndex = 5;
            this.calendarFootballGr.Visible = false;
            this.calendarFootballGr.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.calendarFootballGr_DateSelected);
            this.calendarFootballGr.MouseLeave += new System.EventHandler(this.calendarFootballGr_MouseLeave);
            // 
            // viewFootballGr
            // 
            this.viewFootballGr.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.viewFootballGr.Location = new System.Drawing.Point(0, 31);
            this.viewFootballGr.Name = "viewFootballGr";
            this.viewFootballGr.Size = new System.Drawing.Size(636, 194);
            this.viewFootballGr.TabIndex = 4;
            this.viewFootballGr.UseCompatibleStateImageBehavior = false;
            // 
            // refreshFootballGr
            // 
            this.refreshFootballGr.Location = new System.Drawing.Point(99, 3);
            this.refreshFootballGr.Name = "refreshFootballGr";
            this.refreshFootballGr.Size = new System.Drawing.Size(80, 23);
            this.refreshFootballGr.TabIndex = 3;
            this.refreshFootballGr.Text = "Refresh";
            this.refreshFootballGr.UseVisualStyleBackColor = true;
            this.refreshFootballGr.Click += new System.EventHandler(this.refreshFootballGr_Click);
            // 
            // logFootballGr
            // 
            this.logFootballGr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logFootballGr.FormattingEnabled = true;
            this.logFootballGr.IntegralHeight = false;
            this.logFootballGr.ItemHeight = 14;
            this.logFootballGr.Location = new System.Drawing.Point(0, 0);
            this.logFootballGr.Name = "logFootballGr";
            this.logFootballGr.Size = new System.Drawing.Size(636, 104);
            this.logFootballGr.TabIndex = 0;
            // 
            // tabFootballEn
            // 
            this.tabFootballEn.Controls.Add(this.splitContainerFootbalEn);
            this.tabFootballEn.Location = new System.Drawing.Point(4, 22);
            this.tabFootballEn.Name = "tabFootballEn";
            this.tabFootballEn.Padding = new System.Windows.Forms.Padding(3);
            this.tabFootballEn.Size = new System.Drawing.Size(642, 342);
            this.tabFootballEn.TabIndex = 1;
            this.tabFootballEn.Text = "Football EN";
            this.tabFootballEn.UseVisualStyleBackColor = true;
            // 
            // splitContainerFootbalEn
            // 
            this.splitContainerFootbalEn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerFootbalEn.Location = new System.Drawing.Point(3, 3);
            this.splitContainerFootbalEn.Name = "splitContainerFootbalEn";
            this.splitContainerFootbalEn.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerFootbalEn.Panel1
            // 
            this.splitContainerFootbalEn.Panel1.Controls.Add(this.listView1);
            this.splitContainerFootbalEn.Panel1.Controls.Add(this.button1);
            this.splitContainerFootbalEn.Panel1.Controls.Add(this.comboBox1);
            // 
            // splitContainerFootbalEn.Panel2
            // 
            this.splitContainerFootbalEn.Panel2.Controls.Add(this.logFootballEn);
            this.splitContainerFootbalEn.Size = new System.Drawing.Size(636, 336);
            this.splitContainerFootbalEn.SplitterDistance = 231;
            this.splitContainerFootbalEn.TabIndex = 3;
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Location = new System.Drawing.Point(0, 31);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(636, 198);
            this.listView1.TabIndex = 4;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(130, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Refresh";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(3, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 22);
            this.comboBox1.TabIndex = 2;
            // 
            // logFootballEn
            // 
            this.logFootballEn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logFootballEn.FormattingEnabled = true;
            this.logFootballEn.IntegralHeight = false;
            this.logFootballEn.ItemHeight = 14;
            this.logFootballEn.Location = new System.Drawing.Point(0, 0);
            this.logFootballEn.Name = "logFootballEn";
            this.logFootballEn.Size = new System.Drawing.Size(636, 101);
            this.logFootballEn.TabIndex = 0;
            // 
            // tabBasketGr
            // 
            this.tabBasketGr.Controls.Add(this.splitContainerBasketGr);
            this.tabBasketGr.Location = new System.Drawing.Point(4, 22);
            this.tabBasketGr.Name = "tabBasketGr";
            this.tabBasketGr.Padding = new System.Windows.Forms.Padding(3);
            this.tabBasketGr.Size = new System.Drawing.Size(642, 342);
            this.tabBasketGr.TabIndex = 2;
            this.tabBasketGr.Text = "Basket GR";
            this.tabBasketGr.UseVisualStyleBackColor = true;
            // 
            // splitContainerBasketGr
            // 
            this.splitContainerBasketGr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerBasketGr.Location = new System.Drawing.Point(3, 3);
            this.splitContainerBasketGr.Name = "splitContainerBasketGr";
            this.splitContainerBasketGr.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerBasketGr.Panel1
            // 
            this.splitContainerBasketGr.Panel1.Controls.Add(this.hideCompletedBasketGr);
            this.splitContainerBasketGr.Panel1.Controls.Add(this.updateDayBasketGr);
            this.splitContainerBasketGr.Panel1.Controls.Add(this.calendarBasketGr);
            this.splitContainerBasketGr.Panel1.Controls.Add(this.textDayBasketGr);
            this.splitContainerBasketGr.Panel1.Controls.Add(this.viewBasketGr);
            this.splitContainerBasketGr.Panel1.Controls.Add(this.refreshBasketGr);
            // 
            // splitContainerBasketGr.Panel2
            // 
            this.splitContainerBasketGr.Panel2.Controls.Add(this.logBasketGr);
            this.splitContainerBasketGr.Size = new System.Drawing.Size(636, 336);
            this.splitContainerBasketGr.SplitterDistance = 229;
            this.splitContainerBasketGr.TabIndex = 3;
            // 
            // hideCompletedBasketGr
            // 
            this.hideCompletedBasketGr.AutoSize = true;
            this.hideCompletedBasketGr.Location = new System.Drawing.Point(185, 6);
            this.hideCompletedBasketGr.Name = "hideCompletedBasketGr";
            this.hideCompletedBasketGr.Size = new System.Drawing.Size(126, 18);
            this.hideCompletedBasketGr.TabIndex = 9;
            this.hideCompletedBasketGr.Text = "Hide Completed";
            this.hideCompletedBasketGr.UseVisualStyleBackColor = true;
            this.hideCompletedBasketGr.CheckedChanged += new System.EventHandler(this.hideCompletedBasketGr_CheckedChanged);
            // 
            // updateDayBasketGr
            // 
            this.updateDayBasketGr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.updateDayBasketGr.Location = new System.Drawing.Point(507, 3);
            this.updateDayBasketGr.Name = "updateDayBasketGr";
            this.updateDayBasketGr.Size = new System.Drawing.Size(126, 23);
            this.updateDayBasketGr.TabIndex = 8;
            this.updateDayBasketGr.Text = "Opap Update";
            this.updateDayBasketGr.UseVisualStyleBackColor = true;
            this.updateDayBasketGr.Click += new System.EventHandler(this.updateDayBasketGr_Click);
            // 
            // calendarBasketGr
            // 
            this.calendarBasketGr.Location = new System.Drawing.Point(9, 37);
            this.calendarBasketGr.Name = "calendarBasketGr";
            this.calendarBasketGr.TabIndex = 6;
            this.calendarBasketGr.Visible = false;
            this.calendarBasketGr.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.calendarBasketGr_DateSelected);
            this.calendarBasketGr.MouseLeave += new System.EventHandler(this.calendarBasketGr_MouseLeave);
            // 
            // textDayBasketGr
            // 
            this.textDayBasketGr.Location = new System.Drawing.Point(3, 3);
            this.textDayBasketGr.Name = "textDayBasketGr";
            this.textDayBasketGr.Size = new System.Drawing.Size(90, 22);
            this.textDayBasketGr.TabIndex = 5;
            this.textDayBasketGr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textDayBasketGr.TextChanged += new System.EventHandler(this.textDayBasketGr_TextChanged);
            this.textDayBasketGr.MouseEnter += new System.EventHandler(this.textDayBasketGr_MouseEnter);
            // 
            // viewBasketGr
            // 
            this.viewBasketGr.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.viewBasketGr.Location = new System.Drawing.Point(0, 31);
            this.viewBasketGr.Name = "viewBasketGr";
            this.viewBasketGr.Size = new System.Drawing.Size(636, 196);
            this.viewBasketGr.TabIndex = 4;
            this.viewBasketGr.UseCompatibleStateImageBehavior = false;
            // 
            // refreshBasketGr
            // 
            this.refreshBasketGr.Location = new System.Drawing.Point(99, 3);
            this.refreshBasketGr.Name = "refreshBasketGr";
            this.refreshBasketGr.Size = new System.Drawing.Size(80, 23);
            this.refreshBasketGr.TabIndex = 3;
            this.refreshBasketGr.Text = "Refresh";
            this.refreshBasketGr.UseVisualStyleBackColor = true;
            this.refreshBasketGr.Click += new System.EventHandler(this.refreshBasketGr_Click);
            // 
            // logBasketGr
            // 
            this.logBasketGr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logBasketGr.FormattingEnabled = true;
            this.logBasketGr.IntegralHeight = false;
            this.logBasketGr.ItemHeight = 14;
            this.logBasketGr.Location = new System.Drawing.Point(0, 0);
            this.logBasketGr.Name = "logBasketGr";
            this.logBasketGr.Size = new System.Drawing.Size(636, 103);
            this.logBasketGr.TabIndex = 0;
            // 
            // tabBasketEn
            // 
            this.tabBasketEn.Controls.Add(this.splitContainerBasketEn);
            this.tabBasketEn.Location = new System.Drawing.Point(4, 22);
            this.tabBasketEn.Name = "tabBasketEn";
            this.tabBasketEn.Padding = new System.Windows.Forms.Padding(3);
            this.tabBasketEn.Size = new System.Drawing.Size(642, 342);
            this.tabBasketEn.TabIndex = 3;
            this.tabBasketEn.Text = "Basket EN";
            this.tabBasketEn.UseVisualStyleBackColor = true;
            // 
            // splitContainerBasketEn
            // 
            this.splitContainerBasketEn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerBasketEn.Location = new System.Drawing.Point(3, 3);
            this.splitContainerBasketEn.Name = "splitContainerBasketEn";
            this.splitContainerBasketEn.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerBasketEn.Panel1
            // 
            this.splitContainerBasketEn.Panel1.Controls.Add(this.listView3);
            this.splitContainerBasketEn.Panel1.Controls.Add(this.button3);
            this.splitContainerBasketEn.Panel1.Controls.Add(this.comboBox3);
            // 
            // splitContainerBasketEn.Panel2
            // 
            this.splitContainerBasketEn.Panel2.Controls.Add(this.logBasketEn);
            this.splitContainerBasketEn.Size = new System.Drawing.Size(636, 336);
            this.splitContainerBasketEn.SplitterDistance = 231;
            this.splitContainerBasketEn.TabIndex = 3;
            // 
            // listView3
            // 
            this.listView3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView3.Location = new System.Drawing.Point(0, 31);
            this.listView3.Name = "listView3";
            this.listView3.Size = new System.Drawing.Size(636, 198);
            this.listView3.TabIndex = 4;
            this.listView3.UseCompatibleStateImageBehavior = false;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(130, 3);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "Refresh";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // comboBox3
            // 
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(3, 3);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(121, 22);
            this.comboBox3.TabIndex = 2;
            // 
            // logBasketEn
            // 
            this.logBasketEn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logBasketEn.FormattingEnabled = true;
            this.logBasketEn.IntegralHeight = false;
            this.logBasketEn.ItemHeight = 14;
            this.logBasketEn.Location = new System.Drawing.Point(0, 0);
            this.logBasketEn.Name = "logBasketEn";
            this.logBasketEn.Size = new System.Drawing.Size(636, 101);
            this.logBasketEn.TabIndex = 0;
            // 
            // tabFormMaker
            // 
            this.tabFormMaker.Controls.Add(this.tabControlFormMaker);
            this.tabFormMaker.Location = new System.Drawing.Point(4, 22);
            this.tabFormMaker.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabFormMaker.Name = "tabFormMaker";
            this.tabFormMaker.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabFormMaker.Size = new System.Drawing.Size(658, 374);
            this.tabFormMaker.TabIndex = 1;
            this.tabFormMaker.Text = "Form Maker";
            this.tabFormMaker.UseVisualStyleBackColor = true;
            // 
            // tabControlFormMaker
            // 
            this.tabControlFormMaker.Controls.Add(this.tabFormsFootballGr);
            this.tabControlFormMaker.Controls.Add(this.tabFormsFootballEn);
            this.tabControlFormMaker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlFormMaker.Location = new System.Drawing.Point(4, 3);
            this.tabControlFormMaker.Name = "tabControlFormMaker";
            this.tabControlFormMaker.SelectedIndex = 0;
            this.tabControlFormMaker.Size = new System.Drawing.Size(650, 368);
            this.tabControlFormMaker.TabIndex = 0;
            // 
            // tabFormsFootballGr
            // 
            this.tabFormsFootballGr.Controls.Add(this.splitContainerFomMaker);
            this.tabFormsFootballGr.Location = new System.Drawing.Point(4, 23);
            this.tabFormsFootballGr.Name = "tabFormsFootballGr";
            this.tabFormsFootballGr.Padding = new System.Windows.Forms.Padding(3);
            this.tabFormsFootballGr.Size = new System.Drawing.Size(642, 341);
            this.tabFormsFootballGr.TabIndex = 0;
            this.tabFormsFootballGr.Text = "Football GR";
            this.tabFormsFootballGr.UseVisualStyleBackColor = true;
            // 
            // splitContainerFomMaker
            // 
            this.splitContainerFomMaker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerFomMaker.Location = new System.Drawing.Point(3, 3);
            this.splitContainerFomMaker.Name = "splitContainerFomMaker";
            this.splitContainerFomMaker.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerFomMaker.Panel1
            // 
            this.splitContainerFomMaker.Panel1.Controls.Add(this.recreateFormMakerGr);
            this.splitContainerFomMaker.Panel1.Controls.Add(this.createFormMakerGr);
            this.splitContainerFomMaker.Panel1.Controls.Add(this.calendarFormMaker);
            this.splitContainerFomMaker.Panel1.Controls.Add(this.textDayFormMakerGr);
            this.splitContainerFomMaker.Panel1.Controls.Add(this.refreshFormMakerGr);
            this.splitContainerFomMaker.Panel1.Controls.Add(this.viewFormMakerGr);
            // 
            // splitContainerFomMaker.Panel2
            // 
            this.splitContainerFomMaker.Panel2.Controls.Add(this.logFormMakerGr);
            this.splitContainerFomMaker.Size = new System.Drawing.Size(636, 335);
            this.splitContainerFomMaker.SplitterDistance = 189;
            this.splitContainerFomMaker.TabIndex = 0;
            // 
            // recreateFormMakerGr
            // 
            this.recreateFormMakerGr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.recreateFormMakerGr.Location = new System.Drawing.Point(457, 3);
            this.recreateFormMakerGr.Name = "recreateFormMakerGr";
            this.recreateFormMakerGr.Size = new System.Drawing.Size(95, 23);
            this.recreateFormMakerGr.TabIndex = 5;
            this.recreateFormMakerGr.Text = "Re-Create";
            this.recreateFormMakerGr.UseVisualStyleBackColor = true;
            this.recreateFormMakerGr.Click += new System.EventHandler(this.recreateFormMakerGr_Click);
            // 
            // createFormMakerGr
            // 
            this.createFormMakerGr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.createFormMakerGr.Location = new System.Drawing.Point(558, 3);
            this.createFormMakerGr.Name = "createFormMakerGr";
            this.createFormMakerGr.Size = new System.Drawing.Size(75, 23);
            this.createFormMakerGr.TabIndex = 4;
            this.createFormMakerGr.Text = "Create";
            this.createFormMakerGr.UseVisualStyleBackColor = true;
            this.createFormMakerGr.Click += new System.EventHandler(this.createFormMakerGr_Click);
            // 
            // calendarFormMaker
            // 
            this.calendarFormMaker.Location = new System.Drawing.Point(9, 37);
            this.calendarFormMaker.Name = "calendarFormMaker";
            this.calendarFormMaker.TabIndex = 3;
            this.calendarFormMaker.Visible = false;
            this.calendarFormMaker.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.calendarFormMaker_DateSelected);
            this.calendarFormMaker.MouseLeave += new System.EventHandler(this.calendarFormMaker_MouseLeave);
            // 
            // textDayFormMakerGr
            // 
            this.textDayFormMakerGr.Location = new System.Drawing.Point(3, 3);
            this.textDayFormMakerGr.Name = "textDayFormMakerGr";
            this.textDayFormMakerGr.Size = new System.Drawing.Size(90, 22);
            this.textDayFormMakerGr.TabIndex = 2;
            this.textDayFormMakerGr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textDayFormMakerGr.TextChanged += new System.EventHandler(this.textDayFormMakerGr_TextChanged);
            this.textDayFormMakerGr.MouseEnter += new System.EventHandler(this.textDayFormMakerGr_MouseEnter);
            // 
            // refreshFormMakerGr
            // 
            this.refreshFormMakerGr.Location = new System.Drawing.Point(99, 3);
            this.refreshFormMakerGr.Name = "refreshFormMakerGr";
            this.refreshFormMakerGr.Size = new System.Drawing.Size(80, 23);
            this.refreshFormMakerGr.TabIndex = 1;
            this.refreshFormMakerGr.Text = "Refresh";
            this.refreshFormMakerGr.UseVisualStyleBackColor = true;
            this.refreshFormMakerGr.Click += new System.EventHandler(this.refreshFormMakerGr_Click);
            // 
            // viewFormMakerGr
            // 
            this.viewFormMakerGr.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.viewFormMakerGr.Location = new System.Drawing.Point(0, 31);
            this.viewFormMakerGr.Name = "viewFormMakerGr";
            this.viewFormMakerGr.Size = new System.Drawing.Size(636, 156);
            this.viewFormMakerGr.TabIndex = 0;
            this.viewFormMakerGr.UseCompatibleStateImageBehavior = false;
            // 
            // logFormMakerGr
            // 
            this.logFormMakerGr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logFormMakerGr.FormattingEnabled = true;
            this.logFormMakerGr.IntegralHeight = false;
            this.logFormMakerGr.ItemHeight = 14;
            this.logFormMakerGr.Location = new System.Drawing.Point(0, 0);
            this.logFormMakerGr.Name = "logFormMakerGr";
            this.logFormMakerGr.Size = new System.Drawing.Size(636, 142);
            this.logFormMakerGr.TabIndex = 0;
            // 
            // tabFormsFootballEn
            // 
            this.tabFormsFootballEn.Location = new System.Drawing.Point(4, 22);
            this.tabFormsFootballEn.Name = "tabFormsFootballEn";
            this.tabFormsFootballEn.Padding = new System.Windows.Forms.Padding(3);
            this.tabFormsFootballEn.Size = new System.Drawing.Size(642, 342);
            this.tabFormsFootballEn.TabIndex = 1;
            this.tabFormsFootballEn.Text = "Football EN";
            this.tabFormsFootballEn.UseVisualStyleBackColor = true;
            // 
            // tabBetMixMaker
            // 
            this.tabBetMixMaker.Controls.Add(this.tabControlBetMixMaker);
            this.tabBetMixMaker.Location = new System.Drawing.Point(4, 22);
            this.tabBetMixMaker.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabBetMixMaker.Name = "tabBetMixMaker";
            this.tabBetMixMaker.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabBetMixMaker.Size = new System.Drawing.Size(658, 374);
            this.tabBetMixMaker.TabIndex = 2;
            this.tabBetMixMaker.Text = "BetMix Maker";
            this.tabBetMixMaker.UseVisualStyleBackColor = true;
            // 
            // tabControlBetMixMaker
            // 
            this.tabControlBetMixMaker.Controls.Add(this.tabBetMixFootballGr);
            this.tabControlBetMixMaker.Controls.Add(this.tabBetMixFootballEn);
            this.tabControlBetMixMaker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlBetMixMaker.Location = new System.Drawing.Point(4, 3);
            this.tabControlBetMixMaker.Name = "tabControlBetMixMaker";
            this.tabControlBetMixMaker.SelectedIndex = 0;
            this.tabControlBetMixMaker.Size = new System.Drawing.Size(650, 368);
            this.tabControlBetMixMaker.TabIndex = 0;
            // 
            // tabBetMixFootballGr
            // 
            this.tabBetMixFootballGr.Controls.Add(this.splitContainerBetMixGr);
            this.tabBetMixFootballGr.Location = new System.Drawing.Point(4, 23);
            this.tabBetMixFootballGr.Name = "tabBetMixFootballGr";
            this.tabBetMixFootballGr.Padding = new System.Windows.Forms.Padding(3);
            this.tabBetMixFootballGr.Size = new System.Drawing.Size(642, 341);
            this.tabBetMixFootballGr.TabIndex = 0;
            this.tabBetMixFootballGr.Text = "Football GR";
            this.tabBetMixFootballGr.UseVisualStyleBackColor = true;
            // 
            // splitContainerBetMixGr
            // 
            this.splitContainerBetMixGr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerBetMixGr.Location = new System.Drawing.Point(3, 3);
            this.splitContainerBetMixGr.Name = "splitContainerBetMixGr";
            this.splitContainerBetMixGr.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerBetMixGr.Panel1
            // 
            this.splitContainerBetMixGr.Panel1.Controls.Add(this.recreateBetMixGr);
            this.splitContainerBetMixGr.Panel1.Controls.Add(this.calendarBetMixGr);
            this.splitContainerBetMixGr.Panel1.Controls.Add(this.createBetMixGr);
            this.splitContainerBetMixGr.Panel1.Controls.Add(this.refreshBetMix);
            this.splitContainerBetMixGr.Panel1.Controls.Add(this.textDayBetMixGr);
            this.splitContainerBetMixGr.Panel1.Controls.Add(this.viewBetMixGr);
            // 
            // splitContainerBetMixGr.Panel2
            // 
            this.splitContainerBetMixGr.Panel2.Controls.Add(this.logBetMixGr);
            this.splitContainerBetMixGr.Size = new System.Drawing.Size(636, 335);
            this.splitContainerBetMixGr.SplitterDistance = 194;
            this.splitContainerBetMixGr.TabIndex = 0;
            // 
            // recreateBetMixGr
            // 
            this.recreateBetMixGr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.recreateBetMixGr.Location = new System.Drawing.Point(457, 3);
            this.recreateBetMixGr.Name = "recreateBetMixGr";
            this.recreateBetMixGr.Size = new System.Drawing.Size(95, 23);
            this.recreateBetMixGr.TabIndex = 5;
            this.recreateBetMixGr.Text = "Re-Create";
            this.recreateBetMixGr.UseVisualStyleBackColor = true;
            this.recreateBetMixGr.Click += new System.EventHandler(this.recreateBetMixGr_Click);
            // 
            // calendarBetMixGr
            // 
            this.calendarBetMixGr.Location = new System.Drawing.Point(9, 37);
            this.calendarBetMixGr.Name = "calendarBetMixGr";
            this.calendarBetMixGr.TabIndex = 4;
            this.calendarBetMixGr.Visible = false;
            this.calendarBetMixGr.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.calendarBetMixGr_DateSelected);
            this.calendarBetMixGr.MouseLeave += new System.EventHandler(this.calendarBetMixGr_MouseLeave);
            // 
            // createBetMixGr
            // 
            this.createBetMixGr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.createBetMixGr.Location = new System.Drawing.Point(558, 3);
            this.createBetMixGr.Name = "createBetMixGr";
            this.createBetMixGr.Size = new System.Drawing.Size(75, 23);
            this.createBetMixGr.TabIndex = 3;
            this.createBetMixGr.Text = "Create";
            this.createBetMixGr.UseVisualStyleBackColor = true;
            this.createBetMixGr.Click += new System.EventHandler(this.createBetMixGr_Click);
            // 
            // refreshBetMix
            // 
            this.refreshBetMix.Location = new System.Drawing.Point(99, 3);
            this.refreshBetMix.Name = "refreshBetMix";
            this.refreshBetMix.Size = new System.Drawing.Size(80, 23);
            this.refreshBetMix.TabIndex = 2;
            this.refreshBetMix.Text = "Refresh";
            this.refreshBetMix.UseVisualStyleBackColor = true;
            this.refreshBetMix.Click += new System.EventHandler(this.refreshBetMix_Click);
            // 
            // textDayBetMixGr
            // 
            this.textDayBetMixGr.Location = new System.Drawing.Point(3, 3);
            this.textDayBetMixGr.Name = "textDayBetMixGr";
            this.textDayBetMixGr.Size = new System.Drawing.Size(90, 22);
            this.textDayBetMixGr.TabIndex = 1;
            this.textDayBetMixGr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textDayBetMixGr.TextChanged += new System.EventHandler(this.textDayBetMixGr_TextChanged);
            this.textDayBetMixGr.MouseEnter += new System.EventHandler(this.textDayBetMixGr_MouseEnter);
            // 
            // viewBetMixGr
            // 
            this.viewBetMixGr.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.viewBetMixGr.Location = new System.Drawing.Point(0, 31);
            this.viewBetMixGr.Name = "viewBetMixGr";
            this.viewBetMixGr.Size = new System.Drawing.Size(636, 161);
            this.viewBetMixGr.TabIndex = 0;
            this.viewBetMixGr.UseCompatibleStateImageBehavior = false;
            // 
            // logBetMixGr
            // 
            this.logBetMixGr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logBetMixGr.FormattingEnabled = true;
            this.logBetMixGr.IntegralHeight = false;
            this.logBetMixGr.ItemHeight = 14;
            this.logBetMixGr.Location = new System.Drawing.Point(0, 0);
            this.logBetMixGr.Name = "logBetMixGr";
            this.logBetMixGr.Size = new System.Drawing.Size(636, 137);
            this.logBetMixGr.TabIndex = 0;
            // 
            // tabBetMixFootballEn
            // 
            this.tabBetMixFootballEn.Location = new System.Drawing.Point(4, 22);
            this.tabBetMixFootballEn.Name = "tabBetMixFootballEn";
            this.tabBetMixFootballEn.Padding = new System.Windows.Forms.Padding(3);
            this.tabBetMixFootballEn.Size = new System.Drawing.Size(642, 342);
            this.tabBetMixFootballEn.TabIndex = 1;
            this.tabBetMixFootballEn.Text = "Football EN";
            this.tabBetMixFootballEn.UseVisualStyleBackColor = true;
            // 
            // toolStripTop
            // 
            this.toolStripTop.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonRestart,
            this.toolStripSeparator4,
            this.buttonCleanupAndRestart});
            this.toolStripTop.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStripTop.Location = new System.Drawing.Point(0, 0);
            this.toolStripTop.Name = "toolStripTop";
            this.toolStripTop.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripTop.Size = new System.Drawing.Size(208, 33);
            this.toolStripTop.TabIndex = 0;
            // 
            // buttonRestart
            // 
            this.buttonRestart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonRestart.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.buttonRestart.Image = ((System.Drawing.Image)(resources.GetObject("buttonRestart.Image")));
            this.buttonRestart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRestart.Margin = new System.Windows.Forms.Padding(3);
            this.buttonRestart.Name = "buttonRestart";
            this.buttonRestart.Padding = new System.Windows.Forms.Padding(3);
            this.buttonRestart.Size = new System.Drawing.Size(59, 27);
            this.buttonRestart.Text = "Restart";
            this.buttonRestart.Click += new System.EventHandler(this.buttonRestart_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Margin = new System.Windows.Forms.Padding(3);
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Padding = new System.Windows.Forms.Padding(3);
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 23);
            // 
            // buttonCleanupAndRestart
            // 
            this.buttonCleanupAndRestart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonCleanupAndRestart.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.buttonCleanupAndRestart.Image = ((System.Drawing.Image)(resources.GetObject("buttonCleanupAndRestart.Image")));
            this.buttonCleanupAndRestart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonCleanupAndRestart.Margin = new System.Windows.Forms.Padding(3);
            this.buttonCleanupAndRestart.Name = "buttonCleanupAndRestart";
            this.buttonCleanupAndRestart.Padding = new System.Windows.Forms.Padding(3);
            this.buttonCleanupAndRestart.Size = new System.Drawing.Size(124, 27);
            this.buttonCleanupAndRestart.Text = "Cleanup && Restart";
            this.buttonCleanupAndRestart.ToolTipText = "Cleanup & Restart";
            this.buttonCleanupAndRestart.Click += new System.EventHandler(this.buttonCleanupAndRestart_Click);
            // 
            // menuMain
            // 
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showHideToolStripMenuItem,
            this.updateStandingsFeedToolStripMenuItem,
            this.toolStripSeparator1,
            this.clearFootballToolStripMenuItem,
            this.clearBasketToolStripMenuItem,
            this.restartToolStripMenuItem,
            this.cleanUpRestartToolStripMenuItem,
            this.toolStripSeparator3,
            this.openDataDirToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.menuMain.Name = "contextMenuStrip";
            this.menuMain.Size = new System.Drawing.Size(196, 198);
            // 
            // showHideToolStripMenuItem
            // 
            this.showHideToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.showHideToolStripMenuItem.Name = "showHideToolStripMenuItem";
            this.showHideToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.showHideToolStripMenuItem.Text = "Show/Hide";
            this.showHideToolStripMenuItem.Click += new System.EventHandler(this.showHideToolStripMenuItem_Click);
            // 
            // updateStandingsFeedToolStripMenuItem
            // 
            this.updateStandingsFeedToolStripMenuItem.Name = "updateStandingsFeedToolStripMenuItem";
            this.updateStandingsFeedToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.updateStandingsFeedToolStripMenuItem.Text = "Update Standings Feed";
            this.updateStandingsFeedToolStripMenuItem.Click += new System.EventHandler(this.updateStandingsFeedToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(192, 6);
            // 
            // clearFootballToolStripMenuItem
            // 
            this.clearFootballToolStripMenuItem.Name = "clearFootballToolStripMenuItem";
            this.clearFootballToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.clearFootballToolStripMenuItem.Text = "Clear Football";
            this.clearFootballToolStripMenuItem.Click += new System.EventHandler(this.clearFootballToolStripMenuItem_Click);
            // 
            // clearBasketToolStripMenuItem
            // 
            this.clearBasketToolStripMenuItem.Name = "clearBasketToolStripMenuItem";
            this.clearBasketToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.clearBasketToolStripMenuItem.Text = "Clear Basket";
            this.clearBasketToolStripMenuItem.Click += new System.EventHandler(this.clearBasketToolStripMenuItem_Click);
            // 
            // restartToolStripMenuItem
            // 
            this.restartToolStripMenuItem.Name = "restartToolStripMenuItem";
            this.restartToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.restartToolStripMenuItem.Text = "Restart";
            this.restartToolStripMenuItem.Click += new System.EventHandler(this.restartToolStripMenuItem_Click);
            // 
            // cleanUpRestartToolStripMenuItem
            // 
            this.cleanUpRestartToolStripMenuItem.Name = "cleanUpRestartToolStripMenuItem";
            this.cleanUpRestartToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.cleanUpRestartToolStripMenuItem.Text = "Cleanup && Restart";
            this.cleanUpRestartToolStripMenuItem.Click += new System.EventHandler(this.cleanUpRestartToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(192, 6);
            // 
            // openDataDirToolStripMenuItem
            // 
            this.openDataDirToolStripMenuItem.Name = "openDataDirToolStripMenuItem";
            this.openDataDirToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.openDataDirToolStripMenuItem.Text = "Open Data Dir";
            this.openDataDirToolStripMenuItem.Click += new System.EventHandler(this.openDataDirToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(192, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // notifyMain
            // 
            this.notifyMain.ContextMenuStrip = this.menuMain;
            this.notifyMain.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyMain.Icon")));
            this.notifyMain.Text = "LiveReader2";
            this.notifyMain.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyMain_MouseClick);
            this.notifyMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyMain_MouseDoubleClick);
            // 
            // timerMain
            // 
            this.timerMain.Interval = 2000;
            this.timerMain.Tick += new System.EventHandler(this.timerMain_Tick);
            // 
            // workerUpdate
            // 
            this.workerUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerUpdate_DoWork);
            // 
            // workerFootballGr
            // 
            this.workerFootballGr.WorkerReportsProgress = true;
            this.workerFootballGr.WorkerSupportsCancellation = true;
            this.workerFootballGr.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerFootballGr_DoWork);
            this.workerFootballGr.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.workerFootballGr_ProgressChanged);
            this.workerFootballGr.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerFootballGr_RunWorkerCompleted);
            // 
            // workerFootballEn
            // 
            this.workerFootballEn.WorkerReportsProgress = true;
            this.workerFootballEn.WorkerSupportsCancellation = true;
            // 
            // workerBasketGr
            // 
            this.workerBasketGr.WorkerReportsProgress = true;
            this.workerBasketGr.WorkerSupportsCancellation = true;
            this.workerBasketGr.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerBasketGr_DoWork);
            this.workerBasketGr.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.workerBasketGr_ProgressChanged);
            this.workerBasketGr.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerBasketGr_RunWorkerCompleted);
            // 
            // workerBasketEn
            // 
            this.workerBasketEn.WorkerReportsProgress = true;
            this.workerBasketEn.WorkerSupportsCancellation = true;
            // 
            // workerFormMakerGr
            // 
            this.workerFormMakerGr.WorkerReportsProgress = true;
            this.workerFormMakerGr.WorkerSupportsCancellation = true;
            this.workerFormMakerGr.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerFormMakerGr_DoWork);
            this.workerFormMakerGr.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.workerFormMakerGr_ProgressChanged);
            this.workerFormMakerGr.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerFormMakerGr_RunWorkerCompleted);
            // 
            // workerBetMixGr
            // 
            this.workerBetMixGr.WorkerReportsProgress = true;
            this.workerBetMixGr.WorkerSupportsCancellation = true;
            this.workerBetMixGr.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerBetMixGr_DoWork);
            this.workerBetMixGr.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.workerBetMixGr_ProgressChanged);
            this.workerBetMixGr.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerBetMixGr_RunWorkerCompleted);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(991, 593);
            this.ContextMenuStrip = this.menuMain;
            this.Controls.Add(this.toolStripContainerMain);
            this.Font = new System.Drawing.Font("Verdana", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LiveReader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.toolStripContainerMain.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainerMain.BottomToolStripPanel.PerformLayout();
            this.toolStripContainerMain.ContentPanel.ResumeLayout(false);
            this.toolStripContainerMain.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainerMain.TopToolStripPanel.PerformLayout();
            this.toolStripContainerMain.ResumeLayout(false);
            this.toolStripContainerMain.PerformLayout();
            this.statusMain.ResumeLayout(false);
            this.statusMain.PerformLayout();
            this.tabControlMain.ResumeLayout(false);
            this.tabLiveFeeder.ResumeLayout(false);
            this.tabControlLiveFeeder.ResumeLayout(false);
            this.tabFootballGr.ResumeLayout(false);
            this.splitContainerFootballGr.Panel1.ResumeLayout(false);
            this.splitContainerFootballGr.Panel1.PerformLayout();
            this.splitContainerFootballGr.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerFootballGr)).EndInit();
            this.splitContainerFootballGr.ResumeLayout(false);
            this.tabFootballEn.ResumeLayout(false);
            this.splitContainerFootbalEn.Panel1.ResumeLayout(false);
            this.splitContainerFootbalEn.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerFootbalEn)).EndInit();
            this.splitContainerFootbalEn.ResumeLayout(false);
            this.tabBasketGr.ResumeLayout(false);
            this.splitContainerBasketGr.Panel1.ResumeLayout(false);
            this.splitContainerBasketGr.Panel1.PerformLayout();
            this.splitContainerBasketGr.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBasketGr)).EndInit();
            this.splitContainerBasketGr.ResumeLayout(false);
            this.tabBasketEn.ResumeLayout(false);
            this.splitContainerBasketEn.Panel1.ResumeLayout(false);
            this.splitContainerBasketEn.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBasketEn)).EndInit();
            this.splitContainerBasketEn.ResumeLayout(false);
            this.tabFormMaker.ResumeLayout(false);
            this.tabControlFormMaker.ResumeLayout(false);
            this.tabFormsFootballGr.ResumeLayout(false);
            this.splitContainerFomMaker.Panel1.ResumeLayout(false);
            this.splitContainerFomMaker.Panel1.PerformLayout();
            this.splitContainerFomMaker.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerFomMaker)).EndInit();
            this.splitContainerFomMaker.ResumeLayout(false);
            this.tabBetMixMaker.ResumeLayout(false);
            this.tabControlBetMixMaker.ResumeLayout(false);
            this.tabBetMixFootballGr.ResumeLayout(false);
            this.splitContainerBetMixGr.Panel1.ResumeLayout(false);
            this.splitContainerBetMixGr.Panel1.PerformLayout();
            this.splitContainerBetMixGr.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBetMixGr)).EndInit();
            this.splitContainerBetMixGr.ResumeLayout(false);
            this.toolStripTop.ResumeLayout(false);
            this.toolStripTop.PerformLayout();
            this.menuMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainerMain;
        private System.Windows.Forms.StatusStrip statusMain;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabWebReaders;
        private System.Windows.Forms.TabPage tabFormMaker;
        private System.Windows.Forms.TabPage tabBetMixMaker;
        private System.Windows.Forms.TabPage tabLiveFeeder;
        private System.Windows.Forms.ContextMenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem showHideToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem clearFootballToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearBasketToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cleanUpRestartToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem openDataDirToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyMain;
        private System.Windows.Forms.Timer timerMain;
        private System.ComponentModel.BackgroundWorker workerUpdate;
        private System.Windows.Forms.ToolStripStatusLabel labelVersion;
        private System.Windows.Forms.ToolStripStatusLabel labelRestart;
        private System.Windows.Forms.ToolStripStatusLabel labelUpdate;
        private System.Windows.Forms.TabControl tabControlLiveFeeder;
        private System.Windows.Forms.TabPage tabFootballGr;
        private System.Windows.Forms.TabPage tabFootballEn;
        private System.Windows.Forms.TabControl tabControlFormMaker;
        private System.Windows.Forms.TabPage tabFormsFootballGr;
        private System.Windows.Forms.TabPage tabFormsFootballEn;
        private System.Windows.Forms.TabControl tabControlBetMixMaker;
        private System.Windows.Forms.TabPage tabBetMixFootballGr;
        private System.Windows.Forms.TabPage tabBetMixFootballEn;
        private System.Windows.Forms.TabPage tabBasketGr;
        private System.Windows.Forms.TabPage tabBasketEn;
        private System.Windows.Forms.SplitContainer splitContainerFootballGr;
        private System.Windows.Forms.ListView viewFootballGr;
        private System.Windows.Forms.Button refreshFootballGr;
        private System.Windows.Forms.ListBox logFootballGr;
        private System.ComponentModel.BackgroundWorker workerFootballGr;
        private System.Windows.Forms.SplitContainer splitContainerFootbalEn;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ListBox logFootballEn;
        private System.Windows.Forms.SplitContainer splitContainerBasketGr;
        private System.Windows.Forms.ListView viewBasketGr;
        private System.Windows.Forms.Button refreshBasketGr;
        private System.Windows.Forms.ListBox logBasketGr;
        private System.Windows.Forms.SplitContainer splitContainerBasketEn;
        private System.Windows.Forms.ListView listView3;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.ListBox logBasketEn;
        private System.ComponentModel.BackgroundWorker workerFootballEn;
        private System.ComponentModel.BackgroundWorker workerBasketGr;
        private System.ComponentModel.BackgroundWorker workerBasketEn;
        private System.Windows.Forms.TextBox textDayFootballGr;
        private System.Windows.Forms.MonthCalendar calendarFootballGr;
        private System.Windows.Forms.SplitContainer splitContainerFomMaker;
        private System.Windows.Forms.ListView viewFormMakerGr;
        private System.Windows.Forms.ListBox logFormMakerGr;
        private System.Windows.Forms.Button refreshFormMakerGr;
        private System.ComponentModel.BackgroundWorker workerFormMakerGr;
        private System.Windows.Forms.TextBox textDayFormMakerGr;
        private System.Windows.Forms.MonthCalendar calendarFormMaker;
        private System.Windows.Forms.Button createFormMakerGr;
        private System.Windows.Forms.SplitContainer splitContainerBetMixGr;
        private System.Windows.Forms.ListView viewBetMixGr;
        private System.Windows.Forms.ListBox logBetMixGr;
        private System.Windows.Forms.Button createBetMixGr;
        private System.Windows.Forms.Button refreshBetMix;
        private System.Windows.Forms.TextBox textDayBetMixGr;
        private System.Windows.Forms.MonthCalendar calendarBetMixGr;
        private System.ComponentModel.BackgroundWorker workerBetMixGr;
        private System.Windows.Forms.ToolStrip toolStripTop;
        private System.Windows.Forms.ToolStripButton buttonRestart;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton buttonCleanupAndRestart;
        private System.Windows.Forms.Button recreateFormMakerGr;
        private System.Windows.Forms.Button recreateBetMixGr;
        private System.Windows.Forms.TextBox textDayBasketGr;
        private System.Windows.Forms.MonthCalendar calendarBasketGr;
        private System.Windows.Forms.Button updateDayFootballGr;
        private System.Windows.Forms.Button updateDayBasketGr;
        private System.Windows.Forms.CheckBox hideCompletedFootballGr;
        private System.Windows.Forms.CheckBox hideCompletedBasketGr;
        private System.Windows.Forms.CheckBox checkLivestats;
        private System.Windows.Forms.CheckBox checkFullUpdate;
        private System.Windows.Forms.ToolStripMenuItem updateStandingsFeedToolStripMenuItem;
    }
}