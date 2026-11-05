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
            // abre o arquivo comprimido e lê os códigos comprimidos, retornando uma fila com eles e preenchendo as variáveis width e height que estao por referencia
            FileStream arquivo = new FileStream(caminhoArquivo, FileMode.Open);

            // como explicamos antes na compressao utilizamos um binaryReader, que lê os bytes puros do arquivo do mesmo jeito que o BinaryWriter gravou
            BinaryReader leitor = new BinaryReader(arquivo);

            // pega a largura e altura q foram salvas no começo do arquivo (cabeçalho de 4 bytes cada)
            width = leitor.ReadInt32();
            height = leitor.ReadInt32();

            Fila codigosComprimidos = new Fila();

            // vai lendo o arquivo ate acabar, desfazendo o que foi feito no salvarArq (3 bytes viram 2 códigos de 12 bits dnv)
            while (arquivo.Position < arquivo.Length)
            {
                // calcula quantos bytes ainda faltam ler no arquivo, pra saber se ainda tem um par de códigos (3 bytes) ou se sobrou só o último código ímpar (2 bytes)
                long bytesRestantes = arquivo.Length - arquivo.Position;

                // se ainda tem 3 bytes ou mais, significa que temos um par de códigos para ler (3 bytes = 24 bits = 2 códigos de 12 bits)
                if (bytesRestantes >= 3)
                {
                    // lê os 3 bytes que foram gravados juntos no salvarArq
                    byte byte1 = leitor.ReadByte();
                    byte byte2 = leitor.ReadByte();
                    byte byte3 = leitor.ReadByte();

                    // aqui é o inverso da compressão, juntamos os pedaços de bits pra montar os dois códigos de 12 bits de novo
                    // o byte1 tem os 8 bits iniciais do codigo1, entao o <<4 empurra ele 4 casas pra esquerda, "abrindo espaço" pros 4 bits finais que faltam
                    // o byte2 tem os 4 bits finais do codigo1 na metade da esquerda, entao o >>4 empurra eles pra direita, jogando fora os 4 bits do codigo2
                    // o | junta os dois pedaços, formando o codigo1 completo (8 bits + 4 bits = 12 bits)
                    int codigo1 = (byte1 << 4) | (byte2 >> 4);

                    // o &0x0F pega só os 4 bits finais do byte2 (que sao os 4 bits iniciais do codigo2), e o <<8 empurra eles 8 casas pra esquerda, "abrindo espaço" pro byte3
                    // o byte3 tem os 8 bits finais do codigo2, entao o | junta os dois pedaços, formando o codigo2 completo (4 bits + 8 bits = 12 bits)
                    int codigo2 = ((byte2 & 0x0F) << 8) | byte3;

                    // coloca os dois códigos na fila na mesma ordem em que foram gravados
                    codigosComprimidos.enqueue(codigo1);
                    codigosComprimidos.enqueue(codigo2);
                }
                else
                {
                    // aqui a quantidade de códigos era ímpar, então sobrou só 1 código gravado em 2 bytes no final do arquivo
                    byte byte1 = leitor.ReadByte();
                    byte byte2 = leitor.ReadByte();

                    // monta o código do mesmo jeito que o codigo1 de cima, o byte1 empurra 4 casas pra esquerda e o byte2 empurra 4 casas pra direita
                    // jogando fora os 4 bits que ficaram vazios (os 4 bits que foram desperdiçados no salvarArq)
                    int codigo1 = (byte1 << 4) | (byte2 >> 4);
                    codigosComprimidos.enqueue(codigo1);
                }
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
