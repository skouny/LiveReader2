namespace LiveReader2
{
    partial class WebReaderControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buttonOpenForm = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonRestart = new System.Windows.Forms.Button();
            this.checkBoxAutoStart = new System.Windows.Forms.CheckBox();
            this.numericUpDownAutoRestart = new System.Windows.Forms.NumericUpDown();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.buttonClearDB = new System.Windows.Forms.Button();
            this.toolTipMain = new System.Windows.Forms.ToolTip(this.components);
            this.labelAutoRestart = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAutoRestart)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOpenForm
            // 
            this.buttonOpenForm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOpenForm.Location = new System.Drawing.Point(3, 3);
            this.buttonOpenForm.Name = "buttonOpenForm";
            this.buttonOpenForm.Size = new System.Drawing.Size(508, 39);
            this.buttonOpenForm.TabIndex = 0;
            this.buttonOpenForm.Text = "Open";
            this.buttonOpenForm.UseVisualStyleBackColor = true;
            this.buttonOpenForm.Click += new System.EventHandler(this.buttonOpenForm_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(82, 44);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(46, 23);
            this.buttonStart.TabIndex = 1;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(134, 44);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(46, 23);
            this.buttonStop.TabIndex = 2;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonRestart
            // 
            this.buttonRestart.Location = new System.Drawing.Point(186, 44);
            this.buttonRestart.Name = "buttonRestart";
            this.buttonRestart.Size = new System.Drawing.Size(57, 23);
            this.buttonRestart.TabIndex = 3;
            this.buttonRestart.Text = "Restart";
            this.buttonRestart.UseVisualStyleBackColor = true;
            this.buttonRestart.Click += new System.EventHandler(this.buttonRestart_Click);
            // 
            // checkBoxAutoStart
            // 
            this.checkBoxAutoStart.AutoSize = true;
            this.checkBoxAutoStart.Location = new System.Drawing.Point(3, 48);
            this.checkBoxAutoStart.Name = "checkBoxAutoStart";
            this.checkBoxAutoStart.Size = new System.Drawing.Size(73, 17);
            this.checkBoxAutoStart.TabIndex = 5;
            this.checkBoxAutoStart.Text = "Auto Start";
            this.checkBoxAutoStart.UseVisualStyleBackColor = true;
            this.checkBoxAutoStart.CheckedChanged += new System.EventHandler(this.checkBoxAutoStart_CheckedChanged);
            // 
            // numericUpDownAutoRestart
            // 
            this.numericUpDownAutoRestart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAutoRestart.Location = new System.Drawing.Point(461, 44);
            this.numericUpDownAutoRestart.Name = "numericUpDownAutoRestart";
            this.numericUpDownAutoRestart.Size = new System.Drawing.Size(50, 20);
            this.numericUpDownAutoRestart.TabIndex = 6;
            this.numericUpDownAutoRestart.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTipMain.SetToolTip(this.numericUpDownAutoRestart, "Auto restart in minutes");
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 60000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // buttonClearDB
            // 
            this.buttonClearDB.Location = new System.Drawing.Point(249, 43);
            this.buttonClearDB.Name = "buttonClearDB";
            this.buttonClearDB.Size = new System.Drawing.Size(72, 23);
            this.buttonClearDB.TabIndex = 8;
            this.buttonClearDB.Text = "Clear DB";
            this.toolTipMain.SetToolTip(this.buttonClearDB, "Clear from database all the matches of this source");
            this.buttonClearDB.UseVisualStyleBackColor = true;
            this.buttonClearDB.Click += new System.EventHandler(this.buttonClearDB_Click);
            // 
            // labelAutoRestart
            // 
            this.labelAutoRestart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelAutoRestart.AutoSize = true;
            this.labelAutoRestart.Location = new System.Drawing.Point(389, 46);
            this.labelAutoRestart.Margin = new System.Windows.Forms.Padding(0);
            this.labelAutoRestart.Name = "labelAutoRestart";
            this.labelAutoRestart.Size = new System.Drawing.Size(69, 13);
            this.labelAutoRestart.TabIndex = 9;
            this.labelAutoRestart.Text = "Auto Restart:";
            // 
            // WebReaderControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.labelAutoRestart);
            this.Controls.Add(this.buttonClearDB);
            this.Controls.Add(this.numericUpDownAutoRestart);
            this.Controls.Add(this.checkBoxAutoStart);
            this.Controls.Add(this.buttonRestart);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.buttonOpenForm);
            this.MinimumSize = new System.Drawing.Size(430, 70);
            this.Name = "WebReaderControl";
            this.Size = new System.Drawing.Size(515, 68);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAutoRestart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOpenForm;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonRestart;
        private System.Windows.Forms.CheckBox checkBoxAutoStart;
        private System.Windows.Forms.NumericUpDown numericUpDownAutoRestart;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button buttonClearDB;
        private System.Windows.Forms.ToolTip toolTipMain;
        private System.Windows.Forms.Label labelAutoRestart;
    }
}
