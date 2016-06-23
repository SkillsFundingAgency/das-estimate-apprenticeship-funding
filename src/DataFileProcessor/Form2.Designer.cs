namespace DataFileProcessor
{
    partial class Form2
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
            this.txtJson = new System.Windows.Forms.TextBox();
            this.btnJsonBrowse = new System.Windows.Forms.Button();
            this.btnCsvBrowse = new System.Windows.Forms.Button();
            this.txtCsv = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDirBrowse = new System.Windows.Forms.Button();
            this.txtDir = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnProcess = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Standards (json)";
            // 
            // txtJson
            // 
            this.txtJson.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtJson.Location = new System.Drawing.Point(101, 13);
            this.txtJson.Name = "txtJson";
            this.txtJson.Size = new System.Drawing.Size(640, 20);
            this.txtJson.TabIndex = 1;
            // 
            // btnJsonBrowse
            // 
            this.btnJsonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnJsonBrowse.Location = new System.Drawing.Point(747, 12);
            this.btnJsonBrowse.Name = "btnJsonBrowse";
            this.btnJsonBrowse.Size = new System.Drawing.Size(25, 20);
            this.btnJsonBrowse.TabIndex = 2;
            this.btnJsonBrowse.Text = "...";
            this.btnJsonBrowse.UseVisualStyleBackColor = true;
            this.btnJsonBrowse.Click += new System.EventHandler(this.btnJsonBrowse_Click);
            // 
            // btnCsvBrowse
            // 
            this.btnCsvBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCsvBrowse.Location = new System.Drawing.Point(747, 38);
            this.btnCsvBrowse.Name = "btnCsvBrowse";
            this.btnCsvBrowse.Size = new System.Drawing.Size(25, 20);
            this.btnCsvBrowse.TabIndex = 5;
            this.btnCsvBrowse.Text = "...";
            this.btnCsvBrowse.UseVisualStyleBackColor = true;
            this.btnCsvBrowse.Click += new System.EventHandler(this.btnCsvBrowse_Click);
            // 
            // txtCsv
            // 
            this.txtCsv.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCsv.Location = new System.Drawing.Point(101, 39);
            this.txtCsv.Name = "txtCsv";
            this.txtCsv.Size = new System.Drawing.Size(640, 20);
            this.txtCsv.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Durations (csv)";
            // 
            // btnDirBrowse
            // 
            this.btnDirBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDirBrowse.Location = new System.Drawing.Point(747, 93);
            this.btnDirBrowse.Name = "btnDirBrowse";
            this.btnDirBrowse.Size = new System.Drawing.Size(25, 20);
            this.btnDirBrowse.TabIndex = 8;
            this.btnDirBrowse.Text = "...";
            this.btnDirBrowse.UseVisualStyleBackColor = true;
            this.btnDirBrowse.Click += new System.EventHandler(this.btnDirBrowse_Click);
            // 
            // txtDir
            // 
            this.txtDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDir.Location = new System.Drawing.Point(101, 94);
            this.txtDir.Name = "txtDir";
            this.txtDir.Size = new System.Drawing.Size(640, 20);
            this.txtDir.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Output Dir";
            // 
            // btnProcess
            // 
            this.btnProcess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnProcess.Location = new System.Drawing.Point(25, 151);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(747, 23);
            this.btnProcess.TabIndex = 9;
            this.btnProcess.Text = "Process";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 191);
            this.Controls.Add(this.btnProcess);
            this.Controls.Add(this.btnDirBrowse);
            this.Controls.Add(this.txtDir);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCsvBrowse);
            this.Controls.Add(this.txtCsv);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnJsonBrowse);
            this.Controls.Add(this.txtJson);
            this.Controls.Add(this.label1);
            this.MaximumSize = new System.Drawing.Size(9999, 230);
            this.MinimumSize = new System.Drawing.Size(480, 230);
            this.Name = "Form2";
            this.Text = "Convert and Merge Standards Info";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtJson;
        private System.Windows.Forms.Button btnJsonBrowse;
        private System.Windows.Forms.Button btnCsvBrowse;
        private System.Windows.Forms.TextBox txtCsv;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDirBrowse;
        private System.Windows.Forms.TextBox txtDir;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnProcess;
    }
}