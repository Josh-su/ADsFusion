namespace ADsFusion
{
    partial class ServerCredentials
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
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.txtbGroups = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtbGroup = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtbUsername = new System.Windows.Forms.TextBox();
            this.txtbPassword = new System.Windows.Forms.TextBox();
            this.txtbDomain = new System.Windows.Forms.TextBox();
            this.TestConnection = new System.Windows.Forms.Button();
            this.Connection = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(265, 147);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(70, 23);
            this.button2.TabIndex = 56;
            this.button2.Text = "Enlever";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(189, 147);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(70, 23);
            this.button1.TabIndex = 55;
            this.button1.Text = "Ajouter";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(13, 152);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(176, 13);
            this.label12.TabIndex = 54;
            this.label12.Text = "Groupe où se trouve les utilisateurs*";
            // 
            // txtbGroups
            // 
            this.txtbGroups.Location = new System.Drawing.Point(14, 177);
            this.txtbGroups.Name = "txtbGroups";
            this.txtbGroups.Size = new System.Drawing.Size(321, 20);
            this.txtbGroups.TabIndex = 46;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label9.Location = new System.Drawing.Point(13, 127);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(114, 13);
            this.label9.TabIndex = 53;
            this.label9.Text = "Groupe administrateur*";
            // 
            // txtbGroup
            // 
            this.txtbGroup.Location = new System.Drawing.Point(133, 124);
            this.txtbGroup.Name = "txtbGroup";
            this.txtbGroup.Size = new System.Drawing.Size(202, 20);
            this.txtbGroup.TabIndex = 45;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(13, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 50;
            this.label4.Text = "Mot de passe*";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(13, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 49;
            this.label2.Text = "Identifiant*";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(13, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 48;
            this.label3.Text = "Domaine*";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 31);
            this.label1.TabIndex = 47;
            this.label1.Text = "Serveur 1";
            // 
            // txtbUsername
            // 
            this.txtbUsername.Location = new System.Drawing.Point(133, 72);
            this.txtbUsername.Name = "txtbUsername";
            this.txtbUsername.Size = new System.Drawing.Size(202, 20);
            this.txtbUsername.TabIndex = 43;
            // 
            // txtbPassword
            // 
            this.txtbPassword.Location = new System.Drawing.Point(133, 98);
            this.txtbPassword.Name = "txtbPassword";
            this.txtbPassword.Size = new System.Drawing.Size(202, 20);
            this.txtbPassword.TabIndex = 44;
            this.txtbPassword.UseSystemPasswordChar = true;
            // 
            // txtbDomain
            // 
            this.txtbDomain.Location = new System.Drawing.Point(133, 46);
            this.txtbDomain.Name = "txtbDomain";
            this.txtbDomain.Size = new System.Drawing.Size(202, 20);
            this.txtbDomain.TabIndex = 42;
            // 
            // TestConnection
            // 
            this.TestConnection.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.TestConnection.Location = new System.Drawing.Point(14, 204);
            this.TestConnection.Name = "TestConnection";
            this.TestConnection.Size = new System.Drawing.Size(154, 23);
            this.TestConnection.TabIndex = 57;
            this.TestConnection.Text = "Test connection";
            this.TestConnection.UseVisualStyleBackColor = true;
            this.TestConnection.Click += new System.EventHandler(this.TestConnection_Click);
            // 
            // Connection
            // 
            this.Connection.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.Connection.Location = new System.Drawing.Point(174, 204);
            this.Connection.Name = "Connection";
            this.Connection.Size = new System.Drawing.Size(161, 23);
            this.Connection.TabIndex = 58;
            this.Connection.Text = "Connecter";
            this.Connection.UseVisualStyleBackColor = true;
            this.Connection.Click += new System.EventHandler(this.Connection_Click);
            // 
            // ServerCredentials
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 237);
            this.Controls.Add(this.Connection);
            this.Controls.Add(this.TestConnection);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtbGroups);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtbGroup);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtbUsername);
            this.Controls.Add(this.txtbPassword);
            this.Controls.Add(this.txtbDomain);
            this.Name = "ServerCredentials";
            this.Text = "ServerCredentials";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ServerCredentials_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtbGroups;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtbGroup;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtbUsername;
        private System.Windows.Forms.TextBox txtbPassword;
        private System.Windows.Forms.TextBox txtbDomain;
        private System.Windows.Forms.Button TestConnection;
        private System.Windows.Forms.Button Connection;
    }
}