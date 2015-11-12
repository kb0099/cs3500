namespace AgCubio
{
    partial class GameForm
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
            this.ConnectionPanel = new System.Windows.Forms.Panel();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.ServerLabel = new System.Windows.Forms.Label();
            this.NameLabel = new System.Windows.Forms.Label();
            this.serverTextBox = new System.Windows.Forms.TextBox();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.GamePanel = new System.Windows.Forms.Panel();
            this.ConnectionPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConnectionPanel
            // 
            this.ConnectionPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ConnectionPanel.Controls.Add(this.ConnectButton);
            this.ConnectionPanel.Controls.Add(this.ServerLabel);
            this.ConnectionPanel.Controls.Add(this.NameLabel);
            this.ConnectionPanel.Controls.Add(this.serverTextBox);
            this.ConnectionPanel.Controls.Add(this.nameTextBox);
            this.ConnectionPanel.Location = new System.Drawing.Point(442, 260);
            this.ConnectionPanel.Name = "ConnectionPanel";
            this.ConnectionPanel.Size = new System.Drawing.Size(380, 160);
            this.ConnectionPanel.TabIndex = 0;
            // 
            // ConnectButton
            // 
            this.ConnectButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConnectButton.Location = new System.Drawing.Point(115, 104);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(150, 50);
            this.ConnectButton.TabIndex = 4;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // ServerLabel
            // 
            this.ServerLabel.AutoSize = true;
            this.ServerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServerLabel.Location = new System.Drawing.Point(3, 57);
            this.ServerLabel.Name = "ServerLabel";
            this.ServerLabel.Size = new System.Drawing.Size(109, 37);
            this.ServerLabel.TabIndex = 3;
            this.ServerLabel.Text = "Server";
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameLabel.Location = new System.Drawing.Point(3, 7);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(103, 37);
            this.NameLabel.TabIndex = 2;
            this.NameLabel.Text = "Name";
            // 
            // serverTextBox
            // 
            this.serverTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serverTextBox.Location = new System.Drawing.Point(118, 54);
            this.serverTextBox.Name = "serverTextBox";
            this.serverTextBox.Size = new System.Drawing.Size(259, 44);
            this.serverTextBox.TabIndex = 1;
            this.serverTextBox.Text = "localhost";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nameTextBox.Location = new System.Drawing.Point(118, 4);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(259, 44);
            this.nameTextBox.TabIndex = 0;
            this.nameTextBox.Text = "Jim";
            // 
            // GamePanel
            // 
            this.GamePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GamePanel.Location = new System.Drawing.Point(12, 12);
            this.GamePanel.Name = "GamePanel";
            this.GamePanel.Size = new System.Drawing.Size(1240, 657);
            this.GamePanel.TabIndex = 1;
            this.GamePanel.Visible = false;
            this.GamePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.GamePanel_Paint);
            this.GamePanel.Resize += new System.EventHandler(this.GamePanel_Resize);
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.ConnectionPanel);
            this.Controls.Add(this.GamePanel);
            this.Name = "GameForm";
            this.Text = "AgCubio";
            this.ConnectionPanel.ResumeLayout(false);
            this.ConnectionPanel.PerformLayout();
            this.ResumeLayout(false);

            this.DoubleBuffered = true;

        }

        #endregion

        private System.Windows.Forms.Panel ConnectionPanel;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label ServerLabel;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.TextBox serverTextBox;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Panel GamePanel;
    }
}

