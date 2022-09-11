namespace FilesMinificationTool
{
    partial class frmFilesMinificationTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFilesMinificationTool));
            this.lbl4FolderPath = new System.Windows.Forms.Label();
            this.txt4FolderPath = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.lblImp = new System.Windows.Forms.Label();
            this.gb4MinificationFilesInfo = new System.Windows.Forms.GroupBox();
            this.rb4MinifySingleCSSFile4DiffDesti = new System.Windows.Forms.RadioButton();
            this.rb4MinifySingleCSSFile4SameDesti = new System.Windows.Forms.RadioButton();
            this.rb4MinifySingleJSFile4DiffDesti = new System.Windows.Forms.RadioButton();
            this.rb4MinifySingleJSFile4SameDesti = new System.Windows.Forms.RadioButton();
            this.rb4MinifyAllJS4DiffDesti = new System.Windows.Forms.RadioButton();
            this.rb4MinifyAllJS4SameDesti = new System.Windows.Forms.RadioButton();
            this.rb4MinifyAllCSS4DiffDesti = new System.Windows.Forms.RadioButton();
            this.rb4MinifyAllCSS4SameDesti = new System.Windows.Forms.RadioButton();
            this.rb4MinifyAll4DiffDesti = new System.Windows.Forms.RadioButton();
            this.rb4MinifyAll4SameDesti = new System.Windows.Forms.RadioButton();
            this.gb4MinificationFilesInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl4FolderPath
            // 
            this.lbl4FolderPath.AutoSize = true;
            this.lbl4FolderPath.Location = new System.Drawing.Point(15, 249);
            this.lbl4FolderPath.Name = "lbl4FolderPath";
            this.lbl4FolderPath.Size = new System.Drawing.Size(61, 13);
            this.lbl4FolderPath.TabIndex = 9;
            this.lbl4FolderPath.Text = "Folder Path";
            // 
            // txt4FolderPath
            // 
            this.txt4FolderPath.Location = new System.Drawing.Point(101, 245);
            this.txt4FolderPath.Multiline = true;
            this.txt4FolderPath.Name = "txt4FolderPath";
            this.txt4FolderPath.Size = new System.Drawing.Size(356, 24);
            this.txt4FolderPath.TabIndex = 7;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(389, 282);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(68, 23);
            this.btnStart.TabIndex = 12;
            this.btnStart.Text = "Start Minify";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lblImp
            // 
            this.lblImp.AutoSize = true;
            this.lblImp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblImp.ForeColor = System.Drawing.Color.Red;
            this.lblImp.Location = new System.Drawing.Point(79, 250);
            this.lblImp.Name = "lblImp";
            this.lblImp.Size = new System.Drawing.Size(12, 13);
            this.lblImp.TabIndex = 13;
            this.lblImp.Text = "*";
            // 
            // gb4MinificationFilesInfo
            // 
            this.gb4MinificationFilesInfo.Controls.Add(this.rb4MinifySingleCSSFile4DiffDesti);
            this.gb4MinificationFilesInfo.Controls.Add(this.rb4MinifySingleCSSFile4SameDesti);
            this.gb4MinificationFilesInfo.Controls.Add(this.rb4MinifySingleJSFile4DiffDesti);
            this.gb4MinificationFilesInfo.Controls.Add(this.rb4MinifySingleJSFile4SameDesti);
            this.gb4MinificationFilesInfo.Controls.Add(this.rb4MinifyAllJS4DiffDesti);
            this.gb4MinificationFilesInfo.Controls.Add(this.rb4MinifyAllJS4SameDesti);
            this.gb4MinificationFilesInfo.Controls.Add(this.rb4MinifyAllCSS4DiffDesti);
            this.gb4MinificationFilesInfo.Controls.Add(this.rb4MinifyAllCSS4SameDesti);
            this.gb4MinificationFilesInfo.Controls.Add(this.rb4MinifyAll4DiffDesti);
            this.gb4MinificationFilesInfo.Controls.Add(this.rb4MinifyAll4SameDesti);
            this.gb4MinificationFilesInfo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gb4MinificationFilesInfo.Location = new System.Drawing.Point(12, 12);
            this.gb4MinificationFilesInfo.Name = "gb4MinificationFilesInfo";
            this.gb4MinificationFilesInfo.Size = new System.Drawing.Size(446, 221);
            this.gb4MinificationFilesInfo.TabIndex = 14;
            this.gb4MinificationFilesInfo.TabStop = false;
            this.gb4MinificationFilesInfo.Text = "Minification Files Details";
            // 
            // rb4MinifySingleCSSFile4DiffDesti
            // 
            this.rb4MinifySingleCSSFile4DiffDesti.AutoSize = true;
            this.rb4MinifySingleCSSFile4DiffDesti.Location = new System.Drawing.Point(206, 182);
            this.rb4MinifySingleCSSFile4DiffDesti.Name = "rb4MinifySingleCSSFile4DiffDesti";
            this.rb4MinifySingleCSSFile4DiffDesti.Size = new System.Drawing.Size(188, 17);
            this.rb4MinifySingleCSSFile4DiffDesti.TabIndex = 9;
            this.rb4MinifySingleCSSFile4DiffDesti.TabStop = true;
            this.rb4MinifySingleCSSFile4DiffDesti.Text = "MinifySingleCSSFile_DifferentDesti";
            this.rb4MinifySingleCSSFile4DiffDesti.UseVisualStyleBackColor = true;
            // 
            // rb4MinifySingleCSSFile4SameDesti
            // 
            this.rb4MinifySingleCSSFile4SameDesti.AutoSize = true;
            this.rb4MinifySingleCSSFile4SameDesti.Location = new System.Drawing.Point(17, 182);
            this.rb4MinifySingleCSSFile4SameDesti.Name = "rb4MinifySingleCSSFile4SameDesti";
            this.rb4MinifySingleCSSFile4SameDesti.Size = new System.Drawing.Size(175, 17);
            this.rb4MinifySingleCSSFile4SameDesti.TabIndex = 8;
            this.rb4MinifySingleCSSFile4SameDesti.TabStop = true;
            this.rb4MinifySingleCSSFile4SameDesti.Text = "MinifySingleCSSFile_SameDesti";
            this.rb4MinifySingleCSSFile4SameDesti.UseVisualStyleBackColor = true;
            // 
            // rb4MinifySingleJSFile4DiffDesti
            // 
            this.rb4MinifySingleJSFile4DiffDesti.AutoSize = true;
            this.rb4MinifySingleJSFile4DiffDesti.Location = new System.Drawing.Point(206, 145);
            this.rb4MinifySingleJSFile4DiffDesti.Name = "rb4MinifySingleJSFile4DiffDesti";
            this.rb4MinifySingleJSFile4DiffDesti.Size = new System.Drawing.Size(179, 17);
            this.rb4MinifySingleJSFile4DiffDesti.TabIndex = 7;
            this.rb4MinifySingleJSFile4DiffDesti.TabStop = true;
            this.rb4MinifySingleJSFile4DiffDesti.Text = "MinifySingleJSFile_DifferentDesti";
            this.rb4MinifySingleJSFile4DiffDesti.UseVisualStyleBackColor = true;
            // 
            // rb4MinifySingleJSFile4SameDesti
            // 
            this.rb4MinifySingleJSFile4SameDesti.AutoSize = true;
            this.rb4MinifySingleJSFile4SameDesti.Location = new System.Drawing.Point(17, 145);
            this.rb4MinifySingleJSFile4SameDesti.Name = "rb4MinifySingleJSFile4SameDesti";
            this.rb4MinifySingleJSFile4SameDesti.Size = new System.Drawing.Size(166, 17);
            this.rb4MinifySingleJSFile4SameDesti.TabIndex = 6;
            this.rb4MinifySingleJSFile4SameDesti.TabStop = true;
            this.rb4MinifySingleJSFile4SameDesti.Text = "MinifySingleJSFile_SameDesti";
            this.rb4MinifySingleJSFile4SameDesti.UseVisualStyleBackColor = true;
            // 
            // rb4MinifyAllJS4DiffDesti
            // 
            this.rb4MinifyAllJS4DiffDesti.AutoSize = true;
            this.rb4MinifyAllJS4DiffDesti.Location = new System.Drawing.Point(206, 105);
            this.rb4MinifyAllJS4DiffDesti.Name = "rb4MinifyAllJS4DiffDesti";
            this.rb4MinifyAllJS4DiffDesti.Size = new System.Drawing.Size(145, 17);
            this.rb4MinifyAllJS4DiffDesti.TabIndex = 5;
            this.rb4MinifyAllJS4DiffDesti.TabStop = true;
            this.rb4MinifyAllJS4DiffDesti.Text = "MinifyAllJS_DifferentDesti";
            this.rb4MinifyAllJS4DiffDesti.UseVisualStyleBackColor = true;
            // 
            // rb4MinifyAllJS4SameDesti
            // 
            this.rb4MinifyAllJS4SameDesti.AutoSize = true;
            this.rb4MinifyAllJS4SameDesti.Location = new System.Drawing.Point(17, 105);
            this.rb4MinifyAllJS4SameDesti.Name = "rb4MinifyAllJS4SameDesti";
            this.rb4MinifyAllJS4SameDesti.Size = new System.Drawing.Size(132, 17);
            this.rb4MinifyAllJS4SameDesti.TabIndex = 4;
            this.rb4MinifyAllJS4SameDesti.TabStop = true;
            this.rb4MinifyAllJS4SameDesti.Text = "MinifyAllJS_SameDesti";
            this.rb4MinifyAllJS4SameDesti.UseVisualStyleBackColor = true;
            // 
            // rb4MinifyAllCSS4DiffDesti
            // 
            this.rb4MinifyAllCSS4DiffDesti.AutoSize = true;
            this.rb4MinifyAllCSS4DiffDesti.Location = new System.Drawing.Point(206, 66);
            this.rb4MinifyAllCSS4DiffDesti.Name = "rb4MinifyAllCSS4DiffDesti";
            this.rb4MinifyAllCSS4DiffDesti.Size = new System.Drawing.Size(154, 17);
            this.rb4MinifyAllCSS4DiffDesti.TabIndex = 3;
            this.rb4MinifyAllCSS4DiffDesti.TabStop = true;
            this.rb4MinifyAllCSS4DiffDesti.Text = "MinifyAllCSS_DifferentDesti";
            this.rb4MinifyAllCSS4DiffDesti.UseVisualStyleBackColor = true;
            // 
            // rb4MinifyAllCSS4SameDesti
            // 
            this.rb4MinifyAllCSS4SameDesti.AutoSize = true;
            this.rb4MinifyAllCSS4SameDesti.Location = new System.Drawing.Point(17, 66);
            this.rb4MinifyAllCSS4SameDesti.Name = "rb4MinifyAllCSS4SameDesti";
            this.rb4MinifyAllCSS4SameDesti.Size = new System.Drawing.Size(141, 17);
            this.rb4MinifyAllCSS4SameDesti.TabIndex = 2;
            this.rb4MinifyAllCSS4SameDesti.TabStop = true;
            this.rb4MinifyAllCSS4SameDesti.Text = "MinifyAllCSS_SameDesti";
            this.rb4MinifyAllCSS4SameDesti.UseVisualStyleBackColor = true;
            // 
            // rb4MinifyAll4DiffDesti
            // 
            this.rb4MinifyAll4DiffDesti.AutoSize = true;
            this.rb4MinifyAll4DiffDesti.Location = new System.Drawing.Point(206, 28);
            this.rb4MinifyAll4DiffDesti.Name = "rb4MinifyAll4DiffDesti";
            this.rb4MinifyAll4DiffDesti.Size = new System.Drawing.Size(133, 17);
            this.rb4MinifyAll4DiffDesti.TabIndex = 1;
            this.rb4MinifyAll4DiffDesti.TabStop = true;
            this.rb4MinifyAll4DiffDesti.Text = "MinifyAll_DifferentDesti";
            this.rb4MinifyAll4DiffDesti.UseVisualStyleBackColor = true;
            // 
            // rb4MinifyAll4SameDesti
            // 
            this.rb4MinifyAll4SameDesti.AutoSize = true;
            this.rb4MinifyAll4SameDesti.Location = new System.Drawing.Point(17, 28);
            this.rb4MinifyAll4SameDesti.Name = "rb4MinifyAll4SameDesti";
            this.rb4MinifyAll4SameDesti.Size = new System.Drawing.Size(120, 17);
            this.rb4MinifyAll4SameDesti.TabIndex = 0;
            this.rb4MinifyAll4SameDesti.TabStop = true;
            this.rb4MinifyAll4SameDesti.Text = "MinifyAll_SameDesti";
            this.rb4MinifyAll4SameDesti.UseVisualStyleBackColor = true;
            // 
            // frmFilesMinificationTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(199)))), ((int)(((byte)(222)))));
            this.ClientSize = new System.Drawing.Size(472, 313);
            this.Controls.Add(this.gb4MinificationFilesInfo);
            this.Controls.Add(this.lblImp);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.lbl4FolderPath);
            this.Controls.Add(this.txt4FolderPath);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmFilesMinificationTool";
            this.Text = "Files Minification Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmFilesMinificationTool_FormClosing);
            this.Load += new System.EventHandler(this.frmFilesMinificationTool_Load);
            this.gb4MinificationFilesInfo.ResumeLayout(false);
            this.gb4MinificationFilesInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl4FolderPath;
        private System.Windows.Forms.TextBox txt4FolderPath;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lblImp;
        private System.Windows.Forms.GroupBox gb4MinificationFilesInfo;
        private System.Windows.Forms.RadioButton rb4MinifyAll4SameDesti;
        private System.Windows.Forms.RadioButton rb4MinifyAll4DiffDesti;
        private System.Windows.Forms.RadioButton rb4MinifyAllCSS4SameDesti;
        private System.Windows.Forms.RadioButton rb4MinifyAllCSS4DiffDesti;
        private System.Windows.Forms.RadioButton rb4MinifyAllJS4SameDesti;
        private System.Windows.Forms.RadioButton rb4MinifyAllJS4DiffDesti;
        private System.Windows.Forms.RadioButton rb4MinifySingleJSFile4SameDesti;
        private System.Windows.Forms.RadioButton rb4MinifySingleJSFile4DiffDesti;
        private System.Windows.Forms.RadioButton rb4MinifySingleCSSFile4SameDesti;
        private System.Windows.Forms.RadioButton rb4MinifySingleCSSFile4DiffDesti;
    }
}

