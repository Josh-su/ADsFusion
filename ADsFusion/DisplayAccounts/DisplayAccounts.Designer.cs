﻿using System.Windows.Forms;

namespace ADsFusion
{
    /// <summary>
    /// 
    /// </summary>
    partial class DisplayAccounts
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.détailsComptesLiéToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.impressionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.motDePasseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.supprimerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.label1 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label3 = new System.Windows.Forms.Label();
            this.button10 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.userBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.domainDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sAMAccountNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.displayNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.givenNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.snDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mailDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.titleDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.userBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(48, 45);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(296, 25);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.TextBox1_TextChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.détailsComptesLiéToolStripMenuItem,
            this.impressionToolStripMenuItem,
            this.motDePasseToolStripMenuItem,
            this.supprimerToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(224, 134);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Image = global::ADsFusion.Properties.Resources.search_file_20;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(223, 26);
            this.toolStripMenuItem1.Text = "Détails";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.ToolStripMenuItem1_Click);
            // 
            // détailsComptesLiéToolStripMenuItem
            // 
            this.détailsComptesLiéToolStripMenuItem.Image = global::ADsFusion.Properties.Resources.connection_20;
            this.détailsComptesLiéToolStripMenuItem.Name = "détailsComptesLiéToolStripMenuItem";
            this.détailsComptesLiéToolStripMenuItem.Size = new System.Drawing.Size(223, 26);
            this.détailsComptesLiéToolStripMenuItem.Text = "Détails Comptes lié";
            this.détailsComptesLiéToolStripMenuItem.Click += new System.EventHandler(this.DétailsComptesLiéToolStripMenuItem_Click);
            // 
            // impressionToolStripMenuItem
            // 
            this.impressionToolStripMenuItem.Image = global::ADsFusion.Properties.Resources.printer_20;
            this.impressionToolStripMenuItem.Name = "impressionToolStripMenuItem";
            this.impressionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.P)));
            this.impressionToolStripMenuItem.Size = new System.Drawing.Size(223, 26);
            this.impressionToolStripMenuItem.Text = "Impression";
            this.impressionToolStripMenuItem.Click += new System.EventHandler(this.ImpressionToolStripMenuItem_Click);
            // 
            // motDePasseToolStripMenuItem
            // 
            this.motDePasseToolStripMenuItem.Enabled = false;
            this.motDePasseToolStripMenuItem.Image = global::ADsFusion.Properties.Resources.reset_password_20;
            this.motDePasseToolStripMenuItem.Name = "motDePasseToolStripMenuItem";
            this.motDePasseToolStripMenuItem.Size = new System.Drawing.Size(223, 26);
            this.motDePasseToolStripMenuItem.Text = "Réinitialiser le mot de passe";
            // 
            // supprimerToolStripMenuItem
            // 
            this.supprimerToolStripMenuItem.Enabled = false;
            this.supprimerToolStripMenuItem.Image = global::ADsFusion.Properties.Resources.poubelle_de_recyclage_20;
            this.supprimerToolStripMenuItem.Name = "supprimerToolStripMenuItem";
            this.supprimerToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.supprimerToolStripMenuItem.Size = new System.Drawing.Size(223, 26);
            this.supprimerToolStripMenuItem.Text = "Supprimer";
            this.supprimerToolStripMenuItem.Click += new System.EventHandler(this.SupprimerToolStripMenuItem_Click);
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 18;
            this.listBox1.Location = new System.Drawing.Point(12, 78);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox1.Size = new System.Drawing.Size(458, 328);
            this.listBox1.TabIndex = 2;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.ListBox1_SelectedIndexChanged);
            this.listBox1.DoubleClick += new System.EventHandler(this.ListBox1_DoubleClick);
            this.listBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ListBox1_MouseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(262, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(211, 15);
            this.label1.TabIndex = 8;
            this.label1.Text = "Dernière Maj : dd.MM.yyyy HH:mm:ss";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(348, 42);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(122, 30);
            this.button5.TabIndex = 11;
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.Button5_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(264, 9);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(206, 23);
            this.progressBar1.Step = 3;
            this.progressBar1.TabIndex = 19;
            this.progressBar1.Visible = false;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 432);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label3.Size = new System.Drawing.Size(195, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "selected items : ???? | total items : ????";
            // 
            // button10
            // 
            this.button10.Image = global::ADsFusion.Properties.Resources.reglages_20;
            this.button10.Location = new System.Drawing.Point(120, 6);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(30, 30);
            this.button10.TabIndex = 6;
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.Button10_Click);
            // 
            // button9
            // 
            this.button9.Enabled = false;
            this.button9.Image = global::ADsFusion.Properties.Resources.poubelle_de_recyclage_20;
            this.button9.Location = new System.Drawing.Point(192, 6);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(30, 30);
            this.button9.TabIndex = 8;
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.Button9_Click);
            // 
            // button8
            // 
            this.button8.Image = global::ADsFusion.Properties.Resources.file_20;
            this.button8.Location = new System.Drawing.Point(156, 6);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(30, 30);
            this.button8.TabIndex = 7;
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.Button8_Click);
            // 
            // button6
            // 
            this.button6.Image = global::ADsFusion.Properties.Resources.question_20;
            this.button6.Location = new System.Drawing.Point(84, 6);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(30, 30);
            this.button6.TabIndex = 5;
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.Button6_Click);
            // 
            // button4
            // 
            this.button4.Image = global::ADsFusion.Properties.Resources.data_storage_20;
            this.button4.Location = new System.Drawing.Point(12, 6);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(30, 30);
            this.button4.TabIndex = 3;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.Button4_Click);
            // 
            // button1
            // 
            this.button1.Image = global::ADsFusion.Properties.Resources.filtre_20;
            this.button1.Location = new System.Drawing.Point(12, 42);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(30, 30);
            this.button1.TabIndex = 10;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // button2
            // 
            this.button2.Image = global::ADsFusion.Properties.Resources.rafraichir_20;
            this.button2.Location = new System.Drawing.Point(228, 6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(30, 30);
            this.button2.TabIndex = 9;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // button3
            // 
            this.button3.Image = global::ADsFusion.Properties.Resources.warning_20;
            this.button3.Location = new System.Drawing.Point(48, 6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(30, 30);
            this.button3.TabIndex = 4;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.Button3_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.domainDataGridViewTextBoxColumn,
            this.sAMAccountNameDataGridViewTextBoxColumn,
            this.displayNameDataGridViewTextBoxColumn,
            this.givenNameDataGridViewTextBoxColumn,
            this.snDataGridViewTextBoxColumn,
            this.mailDataGridViewTextBoxColumn,
            this.titleDataGridViewTextBoxColumn,
            this.descriptionDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.userBindingSource;
            this.dataGridView1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dataGridView1.Location = new System.Drawing.Point(492, 78);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.Size = new System.Drawing.Size(858, 328);
            this.dataGridView1.TabIndex = 21;
            // 
            // userBindingSource
            // 
            this.userBindingSource.DataSource = typeof(ADsFusion.User);
            // 
            // domainDataGridViewTextBoxColumn
            // 
            this.domainDataGridViewTextBoxColumn.DataPropertyName = "Domain";
            this.domainDataGridViewTextBoxColumn.HeaderText = "Domain";
            this.domainDataGridViewTextBoxColumn.Name = "domainDataGridViewTextBoxColumn";
            this.domainDataGridViewTextBoxColumn.ReadOnly = true;
            this.domainDataGridViewTextBoxColumn.Width = 68;
            // 
            // sAMAccountNameDataGridViewTextBoxColumn
            // 
            this.sAMAccountNameDataGridViewTextBoxColumn.DataPropertyName = "SAMAccountName";
            this.sAMAccountNameDataGridViewTextBoxColumn.HeaderText = "SAMAccountName";
            this.sAMAccountNameDataGridViewTextBoxColumn.Name = "sAMAccountNameDataGridViewTextBoxColumn";
            this.sAMAccountNameDataGridViewTextBoxColumn.ReadOnly = true;
            this.sAMAccountNameDataGridViewTextBoxColumn.Width = 123;
            // 
            // displayNameDataGridViewTextBoxColumn
            // 
            this.displayNameDataGridViewTextBoxColumn.DataPropertyName = "DisplayName";
            this.displayNameDataGridViewTextBoxColumn.HeaderText = "DisplayName";
            this.displayNameDataGridViewTextBoxColumn.Name = "displayNameDataGridViewTextBoxColumn";
            this.displayNameDataGridViewTextBoxColumn.ReadOnly = true;
            this.displayNameDataGridViewTextBoxColumn.Width = 94;
            // 
            // givenNameDataGridViewTextBoxColumn
            // 
            this.givenNameDataGridViewTextBoxColumn.DataPropertyName = "GivenName";
            this.givenNameDataGridViewTextBoxColumn.HeaderText = "GivenName";
            this.givenNameDataGridViewTextBoxColumn.Name = "givenNameDataGridViewTextBoxColumn";
            this.givenNameDataGridViewTextBoxColumn.ReadOnly = true;
            this.givenNameDataGridViewTextBoxColumn.Width = 88;
            // 
            // snDataGridViewTextBoxColumn
            // 
            this.snDataGridViewTextBoxColumn.DataPropertyName = "Sn";
            this.snDataGridViewTextBoxColumn.HeaderText = "Sn";
            this.snDataGridViewTextBoxColumn.Name = "snDataGridViewTextBoxColumn";
            this.snDataGridViewTextBoxColumn.ReadOnly = true;
            this.snDataGridViewTextBoxColumn.Width = 45;
            // 
            // mailDataGridViewTextBoxColumn
            // 
            this.mailDataGridViewTextBoxColumn.DataPropertyName = "Mail";
            this.mailDataGridViewTextBoxColumn.HeaderText = "Mail";
            this.mailDataGridViewTextBoxColumn.Name = "mailDataGridViewTextBoxColumn";
            this.mailDataGridViewTextBoxColumn.ReadOnly = true;
            this.mailDataGridViewTextBoxColumn.Width = 51;
            // 
            // titleDataGridViewTextBoxColumn
            // 
            this.titleDataGridViewTextBoxColumn.DataPropertyName = "Title";
            this.titleDataGridViewTextBoxColumn.HeaderText = "Title";
            this.titleDataGridViewTextBoxColumn.Name = "titleDataGridViewTextBoxColumn";
            this.titleDataGridViewTextBoxColumn.ReadOnly = true;
            this.titleDataGridViewTextBoxColumn.Width = 52;
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Description";
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            this.descriptionDataGridViewTextBoxColumn.ReadOnly = true;
            this.descriptionDataGridViewTextBoxColumn.Width = 85;
            // 
            // DisplayAccounts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1449, 454);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button3);
            this.Name = "DisplayAccounts";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DisplayAccounts";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DisplayAccounts_FormClosing);
            this.Load += new System.EventHandler(this.DisplayAccounts_LoadAsync);
            this.LocationChanged += new System.EventHandler(this.DisplayAccounts_LocationChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DisplayAccounts_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DisplayAccounts_KeyUp);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.userBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem motDePasseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem supprimerToolStripMenuItem;
        private System.Windows.Forms.ListBox listBox1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        public System.Windows.Forms.ProgressBar progressBar1;
        private Label label3;
        private ToolStripMenuItem impressionToolStripMenuItem;
        private ToolStripMenuItem détailsComptesLiéToolStripMenuItem;
        private DataGridView dataGridView1;
        private BindingSource userBindingSource;
        private DataGridViewTextBoxColumn domainDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn sAMAccountNameDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn displayNameDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn givenNameDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn snDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn mailDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn titleDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
    }
}

