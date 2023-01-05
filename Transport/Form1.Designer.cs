namespace Transport
{
    partial class Form1
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
            this.btnTransfer = new System.Windows.Forms.Button();
            this.lb = new System.Windows.Forms.ListBox();
            this.lblPath = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnTransfer
            // 
            this.btnTransfer.Location = new System.Drawing.Point(12, 19);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(317, 129);
            this.btnTransfer.TabIndex = 0;
            this.btnTransfer.Text = "Transfer";
            this.btnTransfer.UseVisualStyleBackColor = true;
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // lb
            // 
            this.lb.FormattingEnabled = true;
            this.lb.Location = new System.Drawing.Point(335, 19);
            this.lb.Name = "lb";
            this.lb.Size = new System.Drawing.Size(192, 329);
            this.lb.TabIndex = 1;
            // 
            // lblPath
            // 
            this.lblPath.Location = new System.Drawing.Point(12, 325);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(317, 23);
            this.lblPath.TabIndex = 2;
            this.lblPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 362);
            this.Controls.Add(this.lblPath);
            this.Controls.Add(this.lb);
            this.Controls.Add(this.btnTransfer);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnTransfer;
        private System.Windows.Forms.ListBox lb;
        private System.Windows.Forms.Label lblPath;
    }
}

