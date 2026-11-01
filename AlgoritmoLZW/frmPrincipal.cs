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

                long bytesOriginais = imageBitmap.Width * imageBitmap.Height * 3;
                long bytesComprimidos = new FileInfo("../../imagem.lzw").Length;
                double reducao = (1.0 - ((double)bytesComprimidos / bytesOriginais)) * 100.0;

                MessageBox.Show("Imagem comprimida com sucesso!\n\n" +
                                "Dimensões: " + imageBitmap.Width + "x" + imageBitmap.Height + " pixels\n" +
                                "Tamanho original (bytes RGB): " + bytesOriginais + " bytes\n" +
                                "Quantidade de códigos gerados: " + codigos.Count + "\n" +
                                "Tamanho do arquivo comprimido (.lzw): " + bytesComprimidos + " bytes\n" +
                                "Redução de tamanho: " + reducao.ToString("F2") + "%\n\n" +
                                "Arquivos 'imagem.lzw' e 'dicionario.txt' salvos na pasta do projeto.");
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
