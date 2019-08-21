namespace ReadingBarcodes
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
            this.loadImageButton = new System.Windows.Forms.Button();
            this.readBarcodesButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // loadImageButton
            // 
            this.loadImageButton.Location = new System.Drawing.Point(47, 37);
            this.loadImageButton.Name = "loadImageButton";
            this.loadImageButton.Size = new System.Drawing.Size(127, 23);
            this.loadImageButton.TabIndex = 0;
            this.loadImageButton.Text = "Load image";
            this.loadImageButton.UseVisualStyleBackColor = true;
            this.loadImageButton.Click += new System.EventHandler(this.loadImageButton_Click);
            // 
            // readBarcodesButton
            // 
            this.readBarcodesButton.Location = new System.Drawing.Point(47, 88);
            this.readBarcodesButton.Name = "readBarcodesButton";
            this.readBarcodesButton.Size = new System.Drawing.Size(127, 23);
            this.readBarcodesButton.TabIndex = 1;
            this.readBarcodesButton.Text = "Read barcodes";
            this.readBarcodesButton.UseVisualStyleBackColor = true;
            this.readBarcodesButton.Click += new System.EventHandler(this.readBarcodesButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(225, 147);
            this.Controls.Add(this.readBarcodesButton);
            this.Controls.Add(this.loadImageButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button loadImageButton;
        private System.Windows.Forms.Button readBarcodesButton;
    }
}

