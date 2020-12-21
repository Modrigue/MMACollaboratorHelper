namespace MMACollaboratorHelper
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.buttonQuit = new System.Windows.Forms.Button();
            this.pictureboxHeader = new System.Windows.Forms.PictureBox();
            this.comboboxGenres = new System.Windows.Forms.ComboBox();
            this.labelGenre = new System.Windows.Forms.Label();
            this.buttonGo = new System.Windows.Forms.Button();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelFilter = new System.Windows.Forms.Label();
            this.comboboxFilters = new System.Windows.Forms.ComboBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.labelBand = new System.Windows.Forms.Label();
            this.textboxBand = new System.Windows.Forms.TextBox();
            this.checkboxDownloadNewAlbumsOnly = new System.Windows.Forms.CheckBox();
            this.tooltipTextboxBand = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureboxHeader)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonQuit
            // 
            this.buttonQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonQuit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonQuit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonQuit.Location = new System.Drawing.Point(305, 361);
            this.buttonQuit.Name = "buttonQuit";
            this.buttonQuit.Size = new System.Drawing.Size(108, 28);
            this.buttonQuit.TabIndex = 0;
            this.buttonQuit.Text = "Quit";
            this.buttonQuit.UseVisualStyleBackColor = true;
            this.buttonQuit.Click += new System.EventHandler(this.buttonQuit_Click);
            // 
            // pictureboxHeader
            // 
            this.pictureboxHeader.Image = global::MMACollaboratorHelper.Properties.Resources.MMA_Logo;
            this.pictureboxHeader.Location = new System.Drawing.Point(-3, 0);
            this.pictureboxHeader.Name = "pictureboxHeader";
            this.pictureboxHeader.Size = new System.Drawing.Size(439, 195);
            this.pictureboxHeader.TabIndex = 1;
            this.pictureboxHeader.TabStop = false;
            // 
            // comboboxGenres
            // 
            this.comboboxGenres.Cursor = System.Windows.Forms.Cursors.Hand;
            this.comboboxGenres.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboboxGenres.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboboxGenres.FormattingEnabled = true;
            this.comboboxGenres.Location = new System.Drawing.Point(65, 246);
            this.comboboxGenres.Name = "comboboxGenres";
            this.comboboxGenres.Size = new System.Drawing.Size(220, 24);
            this.comboboxGenres.TabIndex = 2;
            // 
            // labelGenre
            // 
            this.labelGenre.AutoSize = true;
            this.labelGenre.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGenre.Location = new System.Drawing.Point(14, 249);
            this.labelGenre.Name = "labelGenre";
            this.labelGenre.Size = new System.Drawing.Size(45, 16);
            this.labelGenre.TabIndex = 3;
            this.labelGenre.Text = "Genre";
            // 
            // buttonGo
            // 
            this.buttonGo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonGo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonGo.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonGo.Location = new System.Drawing.Point(305, 246);
            this.buttonGo.Name = "buttonGo";
            this.buttonGo.Size = new System.Drawing.Size(108, 81);
            this.buttonGo.TabIndex = 4;
            this.buttonGo.Text = "GO!";
            this.buttonGo.UseVisualStyleBackColor = true;
            this.buttonGo.Click += new System.EventHandler(this.buttonGo_Click);
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.BackColor = System.Drawing.Color.Transparent;
            this.labelVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVersion.ForeColor = System.Drawing.Color.Gray;
            this.labelVersion.Location = new System.Drawing.Point(11, 207);
            this.labelVersion.Margin = new System.Windows.Forms.Padding(3);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(62, 13);
            this.labelVersion.TabIndex = 5;
            this.labelVersion.Text = "Version X.Y";
            // 
            // labelFilter
            // 
            this.labelFilter.AutoSize = true;
            this.labelFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFilter.Location = new System.Drawing.Point(14, 278);
            this.labelFilter.Name = "labelFilter";
            this.labelFilter.Size = new System.Drawing.Size(41, 16);
            this.labelFilter.TabIndex = 6;
            this.labelFilter.Text = "Letter";
            // 
            // comboboxFilters
            // 
            this.comboboxFilters.Cursor = System.Windows.Forms.Cursors.Hand;
            this.comboboxFilters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboboxFilters.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboboxFilters.FormattingEnabled = true;
            this.comboboxFilters.Location = new System.Drawing.Point(65, 275);
            this.comboboxFilters.Name = "comboboxFilters";
            this.comboboxFilters.Size = new System.Drawing.Size(50, 24);
            this.comboboxFilters.TabIndex = 7;
            // 
            // labelStatus
            // 
            this.labelStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatus.ForeColor = System.Drawing.Color.Green;
            this.labelStatus.Location = new System.Drawing.Point(17, 364);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(268, 23);
            this.labelStatus.TabIndex = 8;
            this.labelStatus.Text = "Result";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelStatus.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(-3, 0);
            this.progressBar1.MarqueeAnimationSpeed = 30;
            this.progressBar1.Maximum = 100000;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(443, 4);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 9;
            this.progressBar1.Visible = false;
            // 
            // labelBand
            // 
            this.labelBand.AutoSize = true;
            this.labelBand.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBand.Location = new System.Drawing.Point(14, 308);
            this.labelBand.Name = "labelBand";
            this.labelBand.Size = new System.Drawing.Size(40, 16);
            this.labelBand.TabIndex = 10;
            this.labelBand.Text = "Band";
            // 
            // textboxBand
            // 
            this.textboxBand.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textboxBand.Location = new System.Drawing.Point(65, 305);
            this.textboxBand.Name = "textboxBand";
            this.textboxBand.Size = new System.Drawing.Size(220, 20);
            this.textboxBand.TabIndex = 11;
            this.textboxBand.Click += new System.EventHandler(this.textboxBand_Click);
            this.textboxBand.TextChanged += new System.EventHandler(this.textboxBand_TextChanged);
            this.textboxBand.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textboxBand_KeyDown);
            // 
            // checkboxDownloadNewAlbumsOnly
            // 
            this.checkboxDownloadNewAlbumsOnly.AutoSize = true;
            this.checkboxDownloadNewAlbumsOnly.Checked = true;
            this.checkboxDownloadNewAlbumsOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkboxDownloadNewAlbumsOnly.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkboxDownloadNewAlbumsOnly.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkboxDownloadNewAlbumsOnly.Location = new System.Drawing.Point(45, 334);
            this.checkboxDownloadNewAlbumsOnly.Name = "checkboxDownloadNewAlbumsOnly";
            this.checkboxDownloadNewAlbumsOnly.Size = new System.Drawing.Size(190, 20);
            this.checkboxDownloadNewAlbumsOnly.TabIndex = 12;
            this.checkboxDownloadNewAlbumsOnly.Text = "Download new albums only";
            this.checkboxDownloadNewAlbumsOnly.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 401);
            this.Controls.Add(this.checkboxDownloadNewAlbumsOnly);
            this.Controls.Add(this.textboxBand);
            this.Controls.Add(this.labelBand);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.comboboxFilters);
            this.Controls.Add(this.labelFilter);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.buttonGo);
            this.Controls.Add(this.labelGenre);
            this.Controls.Add(this.comboboxGenres);
            this.Controls.Add(this.pictureboxHeader);
            this.Controls.Add(this.buttonQuit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MMA Collaborator Helper";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.pictureboxHeader)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonQuit;
        private System.Windows.Forms.PictureBox pictureboxHeader;
        private System.Windows.Forms.ComboBox comboboxGenres;
        private System.Windows.Forms.Label labelGenre;
        private System.Windows.Forms.Button buttonGo;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelFilter;
        private System.Windows.Forms.ComboBox comboboxFilters;
        private System.Windows.Forms.Label labelStatus;
        public System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label labelBand;
        private System.Windows.Forms.TextBox textboxBand;
        private System.Windows.Forms.CheckBox checkboxDownloadNewAlbumsOnly;
        private System.Windows.Forms.ToolTip tooltipTextboxBand;
    }
}

