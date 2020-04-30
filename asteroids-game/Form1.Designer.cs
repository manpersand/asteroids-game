namespace asteroids_game
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this._txtPlayerName = new System.Windows.Forms.TextBox();
            this._dgvHighScores = new System.Windows.Forms.DataGridView();
            this._btnNewGame = new System.Windows.Forms.Button();
            this._pnlMenu = new System.Windows.Forms.Panel();
            this._btnControls = new System.Windows.Forms.Button();
            this._btnHighScores = new System.Windows.Forms.Button();
            this._btnOK = new System.Windows.Forms.Button();
            this._btnHome = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this._lblHighScore = new System.Windows.Forms.Label();
            this._lblControls = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._dgvHighScores)).BeginInit();
            this._pnlMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // _txtPlayerName
            // 
            this._txtPlayerName.BackColor = System.Drawing.Color.Black;
            this._txtPlayerName.Font = new System.Drawing.Font("Impact", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._txtPlayerName.ForeColor = System.Drawing.Color.White;
            this._txtPlayerName.Location = new System.Drawing.Point(296, 344);
            this._txtPlayerName.Name = "_txtPlayerName";
            this._txtPlayerName.Size = new System.Drawing.Size(309, 43);
            this._txtPlayerName.TabIndex = 1;
            // 
            // _dgvHighScores
            // 
            this._dgvHighScores.AllowUserToResizeColumns = false;
            this._dgvHighScores.AllowUserToResizeRows = false;
            this._dgvHighScores.BackgroundColor = System.Drawing.Color.Black;
            this._dgvHighScores.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Castellar", 18F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._dgvHighScores.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this._dgvHighScores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgvHighScores.ColumnHeadersVisible = false;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Impact", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this._dgvHighScores.DefaultCellStyle = dataGridViewCellStyle2;
            this._dgvHighScores.GridColor = System.Drawing.Color.Black;
            this._dgvHighScores.Location = new System.Drawing.Point(147, 218);
            this._dgvHighScores.Name = "_dgvHighScores";
            this._dgvHighScores.ReadOnly = true;
            this._dgvHighScores.RowHeadersVisible = false;
            this._dgvHighScores.RowTemplate.Height = 25;
            this._dgvHighScores.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this._dgvHighScores.Size = new System.Drawing.Size(606, 268);
            this._dgvHighScores.TabIndex = 2;
            this._dgvHighScores.TabStop = false;
            // 
            // _btnNewGame
            // 
            this._btnNewGame.BackColor = System.Drawing.Color.Transparent;
            this._btnNewGame.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this._btnNewGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnNewGame.Font = new System.Drawing.Font("Impact", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnNewGame.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(234)))), ((int)(((byte)(25)))));
            this._btnNewGame.Location = new System.Drawing.Point(31, 12);
            this._btnNewGame.Name = "_btnNewGame";
            this._btnNewGame.Size = new System.Drawing.Size(309, 47);
            this._btnNewGame.TabIndex = 0;
            this._btnNewGame.TabStop = false;
            this._btnNewGame.Text = "New Game";
            this._btnNewGame.UseVisualStyleBackColor = false;
            // 
            // _pnlMenu
            // 
            this._pnlMenu.BackColor = System.Drawing.Color.Transparent;
            this._pnlMenu.Controls.Add(this._btnControls);
            this._pnlMenu.Controls.Add(this._btnHighScores);
            this._pnlMenu.Controls.Add(this._btnNewGame);
            this._pnlMenu.Font = new System.Drawing.Font("Impact", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._pnlMenu.ForeColor = System.Drawing.Color.WhiteSmoke;
            this._pnlMenu.Location = new System.Drawing.Point(265, 264);
            this._pnlMenu.Name = "_pnlMenu";
            this._pnlMenu.Size = new System.Drawing.Size(371, 205);
            this._pnlMenu.TabIndex = 3;
            // 
            // _btnControls
            // 
            this._btnControls.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this._btnControls.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnControls.Font = new System.Drawing.Font("Impact", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnControls.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(234)))), ((int)(((byte)(25)))));
            this._btnControls.Location = new System.Drawing.Point(31, 146);
            this._btnControls.Name = "_btnControls";
            this._btnControls.Size = new System.Drawing.Size(309, 47);
            this._btnControls.TabIndex = 2;
            this._btnControls.TabStop = false;
            this._btnControls.Text = "Controls";
            this._btnControls.UseVisualStyleBackColor = true;
            // 
            // _btnHighScores
            // 
            this._btnHighScores.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this._btnHighScores.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnHighScores.Font = new System.Drawing.Font("Impact", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnHighScores.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(234)))), ((int)(((byte)(25)))));
            this._btnHighScores.Location = new System.Drawing.Point(31, 79);
            this._btnHighScores.Name = "_btnHighScores";
            this._btnHighScores.Size = new System.Drawing.Size(309, 47);
            this._btnHighScores.TabIndex = 1;
            this._btnHighScores.TabStop = false;
            this._btnHighScores.Text = "High Scores";
            this._btnHighScores.UseVisualStyleBackColor = true;
            // 
            // _btnOK
            // 
            this._btnOK.BackColor = System.Drawing.Color.Transparent;
            this._btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnOK.Font = new System.Drawing.Font("Impact", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnOK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(234)))), ((int)(((byte)(25)))));
            this._btnOK.Location = new System.Drawing.Point(296, 405);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(149, 43);
            this._btnOK.TabIndex = 3;
            this._btnOK.TabStop = false;
            this._btnOK.Text = "OK";
            this._btnOK.UseVisualStyleBackColor = false;
            // 
            // _btnHome
            // 
            this._btnHome.BackColor = System.Drawing.Color.Black;
            this._btnHome.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this._btnHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnHome.Font = new System.Drawing.Font("Impact", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnHome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(234)))), ((int)(((byte)(25)))));
            this._btnHome.Location = new System.Drawing.Point(355, 528);
            this._btnHome.Name = "_btnHome";
            this._btnHome.Size = new System.Drawing.Size(190, 49);
            this._btnHome.TabIndex = 4;
            this._btnHome.TabStop = false;
            this._btnHome.Text = "Home";
            this._btnHome.UseVisualStyleBackColor = false;
            // 
            // _btnCancel
            // 
            this._btnCancel.BackColor = System.Drawing.Color.Transparent;
            this._btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnCancel.Font = new System.Drawing.Font("Impact", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(234)))), ((int)(((byte)(25)))));
            this._btnCancel.Location = new System.Drawing.Point(451, 405);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(154, 43);
            this._btnCancel.TabIndex = 5;
            this._btnCancel.TabStop = false;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.UseVisualStyleBackColor = false;
            // 
            // _lblHighScore
            // 
            this._lblHighScore.AutoSize = true;
            this._lblHighScore.Font = new System.Drawing.Font("Impact", 60F, System.Drawing.FontStyle.Bold);
            this._lblHighScore.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(234)))), ((int)(((byte)(25)))));
            this._lblHighScore.Location = new System.Drawing.Point(228, 97);
            this._lblHighScore.Name = "_lblHighScore";
            this._lblHighScore.Size = new System.Drawing.Size(445, 98);
            this._lblHighScore.TabIndex = 7;
            this._lblHighScore.Text = "High Scores";
            // 
            // _lblControls
            // 
            this._lblControls.AutoSize = true;
            this._lblControls.Font = new System.Drawing.Font("Impact", 60F, System.Drawing.FontStyle.Bold);
            this._lblControls.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(234)))), ((int)(((byte)(25)))));
            this._lblControls.Location = new System.Drawing.Point(263, 97);
            this._lblControls.Name = "_lblControls";
            this._lblControls.Size = new System.Drawing.Size(330, 98);
            this._lblControls.TabIndex = 8;
            this._lblControls.Text = "Controls";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImage = global::asteroids_game.Properties.Resources.asteroids_bg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this._lblControls);
            this.Controls.Add(this._lblHighScore);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnHome);
            this.Controls.Add(this._btnOK);
            this.Controls.Add(this._txtPlayerName);
            this.Controls.Add(this._pnlMenu);
            this.Controls.Add(this._dgvHighScores);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this._dgvHighScores)).EndInit();
            this._pnlMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox _txtPlayerName;
        private System.Windows.Forms.DataGridView _dgvHighScores;
        private System.Windows.Forms.Button _btnNewGame;
        private System.Windows.Forms.Panel _pnlMenu;
        private System.Windows.Forms.Button _btnControls;
        private System.Windows.Forms.Button _btnHighScores;
        private System.Windows.Forms.Button _btnOK;
        private System.Windows.Forms.Button _btnHome;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.Label _lblHighScore;
        private System.Windows.Forms.Label _lblControls;
    }
}

