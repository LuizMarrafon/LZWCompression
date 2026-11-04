using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmoLZW
{
    internal class descompressaoLZW
    {
        // recebe o nome (caminho) do arquivo comprimido, aplica o algortimo para descomprimir ele e retorna a imagem descomprimida
        public static Bitmap descompressao(string caminhoArquivo)
        {
            int width = 0, height = 0;

            // lê o arquivo comprimido e retorna a lista de códigos comprimidos, e também preenche as variáveis width e height com os valores salvos no começo do arquivo
            Fila codigosComprimidos = LerArq(caminhoArquivo, ref width, ref height);

            // cria um dicionário vazio e um vetor de bytes para armazenar os pixels da imagem descomprimida
            dicionarioLZW dicionario = new dicionarioLZW();

            // e um vetor de bytes para armazenar os pixels da imagem descomprimida
            byte[] pixelB = new byte[width * height * 3];

            // variável para controlar a pos atual no vetor de bytes da imagem descomprimida
            int posPixel = 0;

            // começa pegando o primeiro cod da lista (q sempre ja ta nos 256 iniciais do dic)
            NoFila atual = codigosComprimidos.getInicio();
            int primeiroCodigo = atual.getValor();

            // busca a seq do primeiro cod no dic e coloca no vet da imagem
            byte[] sequenciaAnterior = dicionario.buscarSequencia(primeiroCodigo);

            // coloca os bytes da seq anterior no vet da imagem
            for(int i = 0; i < sequenciaAnterior.Length; i++) 
            {
                pixelB[posPixel] = sequenciaAnterior[i];
                posPixel++;
            }

            // percorre o resto da lista de codigos pra ir montando o dic e preenchendo o vet pixelB ao mesmo tempo
            atual = atual.getProx();
            while (atual != null)
            {
                int codigoAtual = atual.getValor();
                byte[] sequenciaAtual = dicionario.buscarSequencia(codigoAtual);//busca a seq desse cod no dic

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
                dicionario.add(sequenciaNova);

                sequenciaAnterior = new byte[sequenciaAtual.Length];
                for (int v = 0; v < sequenciaAtual.Length; v++)//seq anterior agora é a seq atual pra usar de base na prox volta
                    sequenciaAnterior[v] = sequenciaAtual[v];

                atual = atual.getProx();
            }

            Bitmap imgDest = new Bitmap(width, height);
            montaImagem(pixelB, imgDest);
            return imgDest;
        }

        public static Fila LerArq(string caminhoArquivo, ref int width, ref int height)
        {
            // abre o arquivo comprimido e lê os códigos comprimidos, retornando uma lista com eles e preenchendo as variáveis width e height que estao por referencia
            FileStream arquivo = new FileStream(caminhoArquivo, FileMode.Open);

            // como explicamos antes na compressao utilizamos um binaryReader
            BinaryReader leitor = new BinaryReader(arquivo);

            // pega a largura e altura q foram salvas no começo do arquivo
            width = leitor.ReadInt32();
            height = leitor.ReadInt32();

            Fila codigosComprimidos = new Fila();

            // vai lendo de 2 em 2 bytes (ushort) ate acabar o arquivo e colocando na lista
            while(arquivo.Position < arquivo.Length)
            {
                ushort codigoLido = leitor.ReadUInt16();
                codigosComprimidos.enqueue(codigoLido);
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
    }
}
