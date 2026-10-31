using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmoLZW
{
    internal class compressaoLZW
    {




        public static void preencheVetBytes(Bitmap org, ref byte[]  pixelB)
        {
            int width = org.Width;
            int height = org.Height;
            
            int pos = 0;

            for (int y=0;y<height;y++)
            {
                for(int x=0;x<width;x++)
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
            preencheVetBytes(org,ref pixelB);//alimenta o vetor com os bytes de cada pixel da imagem
            DicionarioLZW dicionario = new DicionarioLZW();


            // Cria o vetor para guardar os códigos comprimidos (ou List<int>)
            List<int> codigosComprimidos = new List<int>();
            // Começa a sequência atual (I) com o primeiro byte (posição 0)
            byte[] sequenciaAtual = new byte[1];

            sequenciaAtual[0] = pixelB[0];

            //percorre o vetor com os bytes e aplica o algoritmo de compressão LZW, montando o dicionario e criando o vetor comprimido ao mesmo tempo
            int pos = 1;
            while(pos<pixelB.Length)
            {
                byte proximoByte = pixelB[pos];
                byte[] sequenciaNova = new byte[sequenciaAtual.Length+1];//vai unir o prox Byte e a sequencia atual
                for (int i = 0; i < sequenciaAtual.Length; i++)
                {
                    sequenciaNova[i] = sequenciaAtual[i];
                }
                sequenciaNova[sequenciaAtual.Length] = proximoByte;// coloca o prox
                int codigoEncontrado= dicionario.BuscarCodigo(sequenciaNova);//verifica se o codigo existe no dicionario
                //como sempre queremos pegar a maior sequencia que existe só colocamos o cod de uma sequencia na lista de comprimidos quando não achamos no dic
                // uma sequencia x, ai colocamos a x-1 no vet de comprimidos e adicionamos a x no dicionario
                if (codigoEncontrado!=-1)//se o codigo existe vai ser != -1
                {
                    sequenciaAtual = new byte[sequenciaNova.Length];
                    for (int v=0;v<sequenciaNova.Length;v++)//seq atual agora é a seq nova(ela sera usada de base)
                    {
                        sequenciaAtual[v] = sequenciaNova[v];
                    }
                }
                else
                {
                    int codigoDaAtual = dicionario.BuscarCodigo(sequenciaAtual);

                    
                    codigosComprimidos.Add(codigoDaAtual);
                    dicionario.Adicionar(sequenciaNova);
                    sequenciaAtual = new byte[1];
                    sequenciaAtual[0] = proximoByte;


                }



                pos++;
            }
            int ultimoCodigo = dicionario.BuscarCodigo(sequenciaAtual);
            codigosComprimidos.Add(ultimoCodigo);

            return codigosComprimidos;
        }
    }
}
