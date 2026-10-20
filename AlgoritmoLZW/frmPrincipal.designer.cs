namespace AlgoritmoLZW
{
    partial class frmPrincipal
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
            this.pictBoxImg1 = new System.Windows.Forms.PictureBox();
            this.pictBoxImg2 = new System.Windows.Forms.PictureBox();
            this.btnAbrirImagem = new System.Windows.Forms.Button();
            this.btnLimpar = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.ditalacao = new System.Windows.Forms.Button();
            this.ditalacaoDMA = new System.Windows.Forms.Button();
            this.erosao = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxImg1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxImg2)).BeginInit();
            this.SuspendLayout();
            // 
            // pictBoxImg1
            // 
            this.pictBoxImg1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pictBoxImg1.Location = new System.Drawing.Point(7, 7);
            this.pictBoxImg1.Margin = new System.Windows.Forms.Padding(4);
            this.pictBoxImg1.Name = "pictBoxImg1";
            this.pictBoxImg1.Size = new System.Drawing.Size(800, 615);
            this.pictBoxImg1.TabIndex = 102;
            this.pictBoxImg1.TabStop = false;
            // 
            // pictBoxImg2
            // 
            this.pictBoxImg2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pictBoxImg2.Location = new System.Drawing.Point(815, 7);
            this.pictBoxImg2.Margin = new System.Windows.Forms.Padding(4);
            this.pictBoxImg2.Name = "pictBoxImg2";
            this.pictBoxImg2.Size = new System.Drawing.Size(800, 615);
            this.pictBoxImg2.TabIndex = 105;
            this.pictBoxImg2.TabStop = false;
            // 
            // btnAbrirImagem
            // 
            this.btnAbrirImagem.Location = new System.Drawing.Point(7, 630);
            this.btnAbrirImagem.Margin = new System.Windows.Forms.Padding(4);
            this.btnAbrirImagem.Name = "btnAbrirImagem";
            this.btnAbrirImagem.Size = new System.Drawing.Size(135, 28);
            this.btnAbrirImagem.TabIndex = 106;
            this.btnAbrirImagem.Text = "Abrir Imagem";
            this.btnAbrirImagem.UseVisualStyleBackColor = true;
            this.btnAbrirImagem.Click += new System.EventHandler(this.btnAbrirImagem_Click);
            // 
            // btnLimpar
            // 
            this.btnLimpar.Location = new System.Drawing.Point(149, 630);
            this.btnLimpar.Margin = new System.Windows.Forms.Padding(4);
            this.btnLimpar.Name = "btnLimpar";
            this.btnLimpar.Size = new System.Drawing.Size(135, 28);
            this.btnLimpar.TabIndex = 107;
            this.btnLimpar.Text = "Limpar";
            this.btnLimpar.UseVisualStyleBackColor = true;
            this.btnLimpar.Click += new System.EventHandler(this.btnLimpar_Click);
            // 
            // ditalacao
            // 
            this.ditalacao.Location = new System.Drawing.Point(297, 630);
            this.ditalacao.Margin = new System.Windows.Forms.Padding(4);
            this.ditalacao.Name = "ditalacao";
            this.ditalacao.Size = new System.Drawing.Size(277, 28);
            this.ditalacao.TabIndex = 109;
            this.ditalacao.Text = "Dilatação";
            this.ditalacao.UseVisualStyleBackColor = true;
            this.ditalacao.Click += new System.EventHandler(this.ditalacao_Click);
            // 
            // ditalacaoDMA
            // 
            this.ditalacaoDMA.Location = new System.Drawing.Point(582, 630);
            this.ditalacaoDMA.Margin = new System.Windows.Forms.Padding(4);
            this.ditalacaoDMA.Name = "ditalacaoDMA";
            this.ditalacaoDMA.Size = new System.Drawing.Size(277, 28);
            this.ditalacaoDMA.TabIndex = 110;
            this.ditalacaoDMA.Text = "Dilatação DMA";
            this.ditalacaoDMA.UseVisualStyleBackColor = true;
            this.ditalacaoDMA.Click += new System.EventHandler(this.ditalacaoDMA_Click);
            // 
            // erosao
            // 
            this.erosao.Location = new System.Drawing.Point(867, 630);
            this.erosao.Margin = new System.Windows.Forms.Padding(4);
            this.erosao.Name = "erosao";
            this.erosao.Size = new System.Drawing.Size(277, 28);
            this.erosao.TabIndex = 111;
            this.erosao.Text = "Erosão";
            this.erosao.UseVisualStyleBackColor = true;
            this.erosao.Click += new System.EventHandler(this.erosao_Click);
            // 
            // frmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1627, 665);
            this.Controls.Add(this.erosao);
            this.Controls.Add(this.ditalacaoDMA);
            this.Controls.Add(this.ditalacao);
            this.Controls.Add(this.btnLimpar);
            this.Controls.Add(this.btnAbrirImagem);
            this.Controls.Add(this.pictBoxImg2);
            this.Controls.Add(this.pictBoxImg1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Formulário Principal";
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxImg1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxImg2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictBoxImg1;
        private System.Windows.Forms.PictureBox pictBoxImg2;
        private System.Windows.Forms.Button btnAbrirImagem;
        private System.Windows.Forms.Button btnLimpar;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button ditalacao;
        private System.Windows.Forms.Button ditalacaoDMA;
        private System.Windows.Forms.Button erosao;
    }
}

