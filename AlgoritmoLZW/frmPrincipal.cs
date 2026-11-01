using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace AlgoritmoLZW
{
    public partial class frmPrincipal : Form
    {
        private Image image;
        private Bitmap imageBitmap;

        public frmPrincipal()
        {
            InitializeComponent();
        }

        private void btnAbrirImagem_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = "";
            openFileDialog.Filter = "Arquivos de Imagem (*.jpg;*.gif;*.bmp;*.png)|*.jpg;*.gif;*.bmp;*.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                image = Image.FromFile(openFileDialog.FileName);
                pictBoxImg1.Image = image;
                pictBoxImg1.SizeMode = PictureBoxSizeMode.Normal;
            }
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            pictBoxImg1.Image = null;
            pictBoxImg2.Image = null;
        }

        private void compressao_Click(object sender, EventArgs e)
        {
            if (pictBoxImg1.Image != null)
            {
                imageBitmap = (Bitmap)image;
                List<int> codigos = compressaoLZW.compreesão(imageBitmap);
                compressaoLZW.SalvarArquivo("../../imagem.lzw", imageBitmap.Width, imageBitmap.Height, codigos);
                MessageBox.Show("Imagem comprimida com sucesso!");
            }
        }

        private void descompressao_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = "";
            openFileDialog.Filter = "Arquivos LZW (*.lzw)|*.lzw";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap imgDest = DescompressaoLZW.descompressao(openFileDialog.FileName);
                pictBoxImg2.Image = imgDest;
            }
        }
    }
}
