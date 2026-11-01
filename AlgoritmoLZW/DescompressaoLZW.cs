using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmoLZW
{
    internal class DescompressaoLZW
    {
        public static List<int> LerArquivo(string caminhoArquivo, ref int width, ref int height)
        {
            FileStream arquivo = new FileStream(caminhoArquivo, FileMode.Open);
            BinaryReader leitor = new BinaryReader(arquivo);

            //pega a largura e altura q foram salvas no começo do arquivo
            width = leitor.ReadInt32();
            height = leitor.ReadInt32();

            List<int> codigosComprimidos = new List<int>();

            //vai lendo de 2 em 2 bytes (ushort) ate acabar o arquivo e colocando na lista
            while (arquivo.Position < arquivo.Length)
            {
                ushort codigoLido = leitor.ReadUInt16();
                codigosComprimidos.Add(codigoLido);
            }

            leitor.Close();
            arquivo.Close();

            return codigosComprimidos;
        }

        //faz o inverso do preencheVetBytes, pega o vet de bytes e monta a imagem dnv
        public static void montaImagem(byte[] pixelB, Bitmap imgDest)
        {
            int width = imgDest.Width;
            int height = imgDest.Height;
            int pos = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int r = pixelB[pos];
                    int g = pixelB[pos + 1];
                    int b = pixelB[pos + 2];
                    pos += 3;

                    Color newcolor = Color.FromArgb(r, g, b);
                    imgDest.SetPixel(x, y, newcolor);
                }
            }
        }

        public static Bitmap descompressao(string caminhoArquivo)
        {
            int width = 0, height = 0;
            List<int> codigosComprimidos = LerArquivo(caminhoArquivo, ref width, ref height);

            DicionarioLZW dicionario = new DicionarioLZW();
            byte[] pixelB = new byte[width * height * 3];
            int posPixel = 0;

            //começa pegando o primeiro cod da lista (q sempre ja ta nos 256 iniciais do dic)
            int primeiroCodigo = codigosComprimidos[0];
            byte[] sequenciaAnterior = dicionario.BuscarSequencia(primeiroCodigo);

            for (int i = 0; i < sequenciaAnterior.Length; i++)
            {
                pixelB[posPixel] = sequenciaAnterior[i];
                posPixel++;
            }

            //percorre o resto da lista de codigos pra ir montando o dic e preenchendo o vet pixelB ao mesmo tempo
            int pos = 1;
            while (pos < codigosComprimidos.Count)
            {
                int codigoAtual = codigosComprimidos[pos];
                byte[] sequenciaAtual = dicionario.BuscarSequencia(codigoAtual);//busca a seq desse cod no dic

                //isso aqui é pra quando o cod acabou de ser criado na compressao e ainda n deu tempo de entrar no dic daqui
                //ai a seq dele vai ser a seq anterior + o primeiro byte dela mesma
                if (sequenciaAtual == null)
                {
                    sequenciaAtual = new byte[sequenciaAnterior.Length + 1];
                    for (int i = 0; i < sequenciaAnterior.Length; i++)
                    {
                        sequenciaAtual[i] = sequenciaAnterior[i];
                    }
                    sequenciaAtual[sequenciaAnterior.Length] = sequenciaAnterior[0];
                }

                //coloca os bytes da seq atual no vet da imagem
                for (int i = 0; i < sequenciaAtual.Length; i++)
                {
                    pixelB[posPixel] = sequenciaAtual[i];
                    posPixel++;
                }

                //junta a seq anterior com o primeiro byte da seq atual pra adicionar no dic igualzinho fez na compressao
                byte[] sequenciaNova = new byte[sequenciaAnterior.Length + 1];
                for (int i = 0; i < sequenciaAnterior.Length; i++)
                {
                    sequenciaNova[i] = sequenciaAnterior[i];
                }
                sequenciaNova[sequenciaAnterior.Length] = sequenciaAtual[0];//coloca o primeiro byte da atual
                dicionario.Adicionar(sequenciaNova);

                sequenciaAnterior = new byte[sequenciaAtual.Length];
                for (int v = 0; v < sequenciaAtual.Length; v++)//seq anterior agora é a seq atual pra usar de base na prox volta
                {
                    sequenciaAnterior[v] = sequenciaAtual[v];
                }

                pos++;
            }

            Bitmap imgDest = new Bitmap(width, height);
            montaImagem(pixelB, imgDest);
            return imgDest;
        }
    }
}
