namespace PrimaveraPatcher
{
    partial class PrimaveraPatcher
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
            this.outputLabel = new System.Windows.Forms.Label();
            this.updateLink = new System.Windows.Forms.LinkLabel();
            this.closeButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // outputLabel
            // 
            this.outputLabel.AutoSize = true;
            this.outputLabel.Location = new System.Drawing.Point(12, 9);
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Size = new System.Drawing.Size(12, 13);
            this.outputLabel.TabIndex = 0;
            this.outputLabel.Text = "\\";
            // 
            // updateLink
            // 
            this.updateLink.AutoSize = true;
            this.updateLink.Location = new System.Drawing.Point(13, 26);
            this.updateLink.Name = "updateLink";
            this.updateLink.Size = new System.Drawing.Size(12, 13);
            this.updateLink.TabIndex = 1;
            this.updateLink.TabStop = true;
            this.updateLink.Text = "\\";
            this.updateLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.updateLink_LinkClicked);
            // 
            // closeButton
            // 
            this.closeButton.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closeButton.Location = new System.Drawing.Point(47, 52);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(97, 23);
            this.closeButton.TabIndex = 2;
            this.closeButton.Text = "Update Later";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Visible = false;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // PrimaveraPatcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(203, 87);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.updateLink);
            this.Controls.Add(this.outputLabel);
            this.Name = "PrimaveraPatcher";
            this.Text = "Patch Check";
            this.Load += new System.EventHandler(this.PrimaveraPatcher_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label outputLabel;
        private System.Windows.Forms.LinkLabel updateLink;
        private System.Windows.Forms.Button closeButton;
    }
}

