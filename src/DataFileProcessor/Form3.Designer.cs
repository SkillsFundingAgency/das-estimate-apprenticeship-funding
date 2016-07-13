namespace DataFileProcessor
{
    partial class Form3
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtSrc = new System.Windows.Forms.TextBox();
            this.btnSrcBrowse = new System.Windows.Forms.Button();
            this.btnDstBrowse = new System.Windows.Forms.Button();
            this.txtDir = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnProcess = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source (csv)";
            // 
            // txtSrc
            // 
            this.txtSrc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSrc.Location = new System.Drawing.Point(85, 12);
            this.txtSrc.Name = "txtSrc";
            this.txtSrc.Size = new System.Drawing.Size(353, 20);
            this.txtSrc.TabIndex = 1;
            // 
            // btnSrcBrowse
            // 
            this.btnSrcBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSrcBrowse.Location = new System.Drawing.Point(444, 12);
            this.btnSrcBrowse.Name = "btnSrcBrowse";
            this.btnSrcBrowse.Size = new System.Drawing.Size(25, 20);
            this.btnSrcBrowse.TabIndex = 3;
            this.btnSrcBrowse.Text = "...";
            this.btnSrcBrowse.UseVisualStyleBackColor = true;
            this.btnSrcBrowse.Click += new System.EventHandler(this.btnSrcBrowse_Click);
            // 
            // btnDstBrowse
            // 
            this.btnDstBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDstBrowse.Location = new System.Drawing.Point(444, 64);
            this.btnDstBrowse.Name = "btnDstBrowse";
            this.btnDstBrowse.Size = new System.Drawing.Size(25, 20);
            this.btnDstBrowse.TabIndex = 6;
            this.btnDstBrowse.Text = "...";
            this.btnDstBrowse.UseVisualStyleBackColor = true;
            this.btnDstBrowse.Click += new System.EventHandler(this.btnDstBrowse_Click);
            // 
            // txtDir
            // 
            this.txtDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDir.Location = new System.Drawing.Point(85, 64);
            this.txtDir.Name = "txtDir";
            this.txtDir.Size = new System.Drawing.Size(353, 20);
            this.txtDir.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Output Dir";
            // 
            // btnProcess
            // 
            this.btnProcess.Location = new System.Drawing.Point(15, 136);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(454, 23);
            this.btnProcess.TabIndex = 7;
            this.btnProcess.Text = "Process";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 171);
            this.Controls.Add(this.btnProcess);
            this.Controls.Add(this.btnDstBrowse);
            this.Controls.Add(this.txtDir);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSrcBrowse);
            this.Controls.Add(this.txtSrc);
            this.Controls.Add(this.label1);
            this.Name = "Form3";
            this.Text = "Form3";
            this.Load += new System.EventHandler(this.Form3_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSrc;
        private System.Windows.Forms.Button btnSrcBrowse;
        private System.Windows.Forms.Button btnDstBrowse;
        private System.Windows.Forms.TextBox txtDir;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnProcess;
    }
}