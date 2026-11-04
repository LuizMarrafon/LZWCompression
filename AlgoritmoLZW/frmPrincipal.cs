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

        // evento de click do botão "Abrir Imagem" 
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

        // evento de click do botão "Limpar"
        private void btnLimpar_Click(object sender, EventArgs e)
        {
            pictBoxImg1.Image = null;
            pictBoxImg2.Image = null;
        }

        // evento de click do botão "Comprimir LZW"
        private void compressao_Click(object sender, EventArgs e)
        {
            // verifica se a imagem foi carregada, se sim chama a função de compressão e salva o arquivo comprimido
            if(pictBoxImg1.Image != null)
            {
                imageBitmap = (Bitmap)image;

                // chama a função de compressão, que retorna a lista de códigos comprimidos, e também salva o dicionário gerado (dicionario.txt)
                Fila codigos = compressaoLZW.compressao(imageBitmap);

                // salva o arquivo comprimido (.lzw) com os códigos gerados, e também salva a largura e altura da imagem no começo do arquivo
                compressaoLZW.salvarArq("../../imagem.lzw", imageBitmap.Width, imageBitmap.Height, codigos);

                // calcula a redução de tamanho do arquivo comprimido em relação ao tamanho original da imagem (em bytes RGB)
                long bytesOriginais = imageBitmap.Width * imageBitmap.Height * 3;
                long bytesComprimidos = new FileInfo("../../imagem.lzw").Length;
                double reducao = (1.0 - ((double)bytesComprimidos / bytesOriginais)) * 100.0;

                // exibe uma mensagem informando que a compressão foi realizada com sucesso, mostrando as dimensões da imagem, o tamanho original, a quantidade de códigos gerados,
                // o tamanho do arquivo comprimido e a redução de tamanho em porcentagem, além de informar que os arquivos 'imagem.lzw' e 'dicionario.txt' foram salvos

                MessageBox.Show("Imagem comprimida com sucesso!\n\n" + "Dimensões: " + imageBitmap.Width + "x" + imageBitmap.Height + " pixels\n" + "Tamanho original (bytes RGB): " 
                                + bytesOriginais + " bytes\n" + "\n" + "Tamanho do arquivo comprimido (.lzw): " 
                + bytesComprimidos + " bytes\n" + "Redução de tamanho: " + reducao.ToString("F2") + "%\n\n" + "Arquivos 'imagem.lzw' e 'dicionario.txt' salvos na pasta do projeto.");
            }
        }

        // evento de click do botão "Descomprimir LZW"
        private void descompressao_Click(object sender, EventArgs e)
        {
            // abre o arquivo comprimido (.lzw) e chama a função de descompressão, que retorna a imagem descomprimida, e exibe a imagem no pictBoxImg2
            openFileDialog.FileName = "";
            openFileDialog.Filter = "Arquivos LZW (*.lzw)|*.lzw";
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap imgDest = descompressaoLZW.descompressao(openFileDialog.FileName);
                pictBoxImg2.Image = imgDest;
            }
        }
    }
}
