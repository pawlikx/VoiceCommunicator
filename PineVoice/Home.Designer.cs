namespace PineVoice
{
    partial class Home
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Home));
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.allContactsBox = new System.Windows.Forms.ListBox();
            this.favContactsBox = new System.Windows.Forms.ListBox();
            this.addFavBtn = new System.Windows.Forms.Button();
            this.RmFavBtn = new System.Windows.Forms.Button();
            this.settingsBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(157, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Lista dostępnych użytkowników";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(338, 315);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(113, 30);
            this.button2.TabIndex = 3;
            this.button2.Text = "Zadzwoń";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(174, 251);
            this.button5.Margin = new System.Windows.Forms.Padding(2);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(113, 24);
            this.button5.TabIndex = 7;
            this.button5.Text = "Odśwież";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // allContactsBox
            // 
            this.allContactsBox.FormattingEnabled = true;
            this.allContactsBox.Location = new System.Drawing.Point(11, 24);
            this.allContactsBox.Margin = new System.Windows.Forms.Padding(2);
            this.allContactsBox.Name = "allContactsBox";
            this.allContactsBox.Size = new System.Drawing.Size(158, 251);
            this.allContactsBox.TabIndex = 8;
            this.allContactsBox.SelectedIndexChanged += new System.EventHandler(this.listBox2_SelectedIndexChanged);
            // 
            // favContactsBox
            // 
            this.favContactsBox.FormattingEnabled = true;
            this.favContactsBox.Location = new System.Drawing.Point(293, 24);
            this.favContactsBox.Name = "favContactsBox";
            this.favContactsBox.Size = new System.Drawing.Size(157, 251);
            this.favContactsBox.TabIndex = 10;
            this.favContactsBox.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // addFavBtn
            // 
            this.addFavBtn.Location = new System.Drawing.Point(174, 24);
            this.addFavBtn.Name = "addFavBtn";
            this.addFavBtn.Size = new System.Drawing.Size(113, 23);
            this.addFavBtn.TabIndex = 11;
            this.addFavBtn.Text = "Dodaj do ulubionych";
            this.addFavBtn.UseVisualStyleBackColor = true;
            this.addFavBtn.Click += new System.EventHandler(this.addFavBtn_Click);
            // 
            // RmFavBtn
            // 
            this.RmFavBtn.Location = new System.Drawing.Point(175, 54);
            this.RmFavBtn.Name = "RmFavBtn";
            this.RmFavBtn.Size = new System.Drawing.Size(112, 23);
            this.RmFavBtn.TabIndex = 12;
            this.RmFavBtn.Text = "Usuń z ulubionych";
            this.RmFavBtn.UseVisualStyleBackColor = true;
            this.RmFavBtn.Click += new System.EventHandler(this.RmFavBtn_Click);
            // 
            // settingsBtn
            // 
            this.settingsBtn.Location = new System.Drawing.Point(12, 316);
            this.settingsBtn.Name = "settingsBtn";
            this.settingsBtn.Size = new System.Drawing.Size(113, 29);
            this.settingsBtn.TabIndex = 14;
            this.settingsBtn.Text = "Ustawienia";
            this.settingsBtn.UseVisualStyleBackColor = true;
            this.settingsBtn.Click += new System.EventHandler(this.settingsBtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(293, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(153, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Lista ulubionych użytkowników";
            // 
            // Home
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::PineVoice.Properties.Resources.iglyPNG;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(462, 355);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.settingsBtn);
            this.Controls.Add(this.RmFavBtn);
            this.Controls.Add(this.addFavBtn);
            this.Controls.Add(this.favContactsBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.allContactsBox);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button2);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Home";
            this.Text = "PineVoice";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Home_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button addFavBtn;
        private System.Windows.Forms.Button RmFavBtn;
        public System.Windows.Forms.ListBox allContactsBox;
        public System.Windows.Forms.ListBox favContactsBox;
        private System.Windows.Forms.Button settingsBtn;
        private System.Windows.Forms.Label label2;
    }
}

