using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmoLZW
{
    internal class compressaoLZW
    {
        public static void preencheVetBytes(Bitmap org, ref byte[] pixelB)
        {
            int width = org.Width;
            int height = org.Height;

            int pos = 0;
            //isso aqui bota a imagem inteira em um vetor de byte, pq como cada pixel são 3 bytes representados pelo RGB, fica mais facil de mexer 
            //quando ja ta tudo em um vetor direto, se fosse fazer a compressão enquanto anda pela imagem, seria muito mais complicado
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color cor = org.GetPixel(x, y);
                    pixelB[pos] = cor.R;
                    pixelB[pos + 1] = cor.G;
                    pixelB[pos + 2] = cor.B;
                    pos += 3;
                }
            }
        }

        public static List<int> compreesão(Bitmap org)
        {
            int width = org.Width;
            int height = org.Height;
            byte[] pixelB = new byte[width * height * 3];
            preencheVetBytes(org, ref pixelB);//alimenta o vetor com os bytes de cada pixel da imagem
            DicionarioLZW dicionario = new DicionarioLZW();

            // Cria o vetor para guardar os códigos comprimidos (ou List<int>)
            List<int> codigosComprimidos = new List<int>();
            // Começa a sequência atual (I) com o primeiro byte (posição 0)
            byte[] sequenciaAtual = new byte[1];

            sequenciaAtual[0] = pixelB[0];
            //guarda o cod da seq atual pra nao precisar buscar ela de novo no dic quando cair no else (como tem 1 byte só, o cod de 0 a 255 ja é o proprio byte)
            int codigoDaAtual = pixelB[0];

            //percorre o vetor com os bytes e aplica o algoritmo de compressão LZW, montando o dicionario e criando o vetor comprimido ao mesmo tempo
            int pos = 1;
            while (pos < pixelB.Length)
            {
                byte proximoByte = pixelB[pos];
                byte[] sequenciaNova = new byte[sequenciaAtual.Length + 1];//vai unir o prox Byte e a sequencia atual
                for (int i = 0; i < sequenciaAtual.Length; i++)
                {
                    sequenciaNova[i] = sequenciaAtual[i];
                }
                sequenciaNova[sequenciaAtual.Length] = proximoByte;// coloca o prox
                int codigoEncontrado = dicionario.BuscarCodigo(sequenciaNova);//verifica se o codigo existe no dicionario
                //como sempre queremos pegar a maior sequencia que existe só colocamos o cod de uma sequencia na lista de comprimidos quando não achamos no dic
                // uma sequencia x, ai colocamos a x-1 no vet de comprimidos e adicionamos a x no dicionario
                if (codigoEncontrado != -1)//se o codigo existe vai ser != -1
                {
                    codigoDaAtual = codigoEncontrado;//ja salva o cod encontrado (q é >= 256) pra usar direto se na prox volta nao achar
                    sequenciaAtual = new byte[sequenciaNova.Length];
                    for (int v = 0; v < sequenciaNova.Length; v++)//seq atual agora é a seq nova(ela sera usada de base)
                    {
                        sequenciaAtual[v] = sequenciaNova[v];
                    }
                }
                else
                {
                    codigosComprimidos.Add(codigoDaAtual);
                    dicionario.Adicionar(sequenciaNova);
                    sequenciaAtual = new byte[1];
                    sequenciaAtual[0] = proximoByte;
                    codigoDaAtual = proximoByte;//como voltou a ter 1 byte só, o cod dela de 0 a 255 ja é o proprio proximoByte
                }

                pos++;
            }
            codigosComprimidos.Add(codigoDaAtual);

            dicionario.SalvarDicionarioTxt("../../dicionario.txt");

            return codigosComprimidos;
        }

        public static void SalvarArquivo(string caminhoArquivo, int width, int height, List<int> codigosComprimidos)
        {
            FileStream arquivo = new FileStream(caminhoArquivo, FileMode.Create);
            //o BinaryWriter grava os valores direto em binario (bytes puros) no arquivo, pq se salvasse como texto cada numero ia virar caracter e ocupar mais bytes
            BinaryWriter escritor = new BinaryWriter(arquivo);

            //salva a largura e altura no começo do arquivo pra saber o tamanho da imagem na hora de reconstruir
            escritor.Write(width);
            escritor.Write(height);

            //como os codigos do dic vao ate 4095 eles nao cabem em 1 byte (que só vai ate 255), e na List usamos int que gasta 4 bytes (32 bits)
            //entao convertemos pra ushort que gasta só 2 bytes (16 bits) por codigo, diminuindo pela metade o tamanho que os codigos ocupam no arquivo
            for (int i = 0; i < codigosComprimidos.Count; i++)
            {
                escritor.Write((ushort)codigosComprimidos[i]);
            }

            escritor.Close();
            arquivo.Close();
        }
    }
}
