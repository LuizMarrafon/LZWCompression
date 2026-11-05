using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlgoritmoLZW
{
    internal class compressaoLZW
    {
        // a funcao de compressao é chamada quando o usuario aperta no botão no frm
        // recebe a imagem original, aplica o algoritmo de compressão LZW e retorna a lista de códigos comprimidos, além disso também salva em um txt o dicionário gerado
        public static Fila compressao(Bitmap imgOrg)
        {
            int width = imgOrg.Width;
            int height = imgOrg.Height;

            // cria um vetor de bytes que vai guardar todos os bytes da imagem (3 bytes por pixel, RGB)
            byte[] pixelB = new byte[width * height * 3];

            // chama a função que explicamos acima, e passa o vetor que acabamos de criar
            preencheVetBytes(imgOrg, pixelB);

            // crio o dicionário
            dicionarioLZW dicionario = new dicionarioLZW();

            // cria a fila para guardar os códigos comprimidos
            Fila fila = new Fila();

            // cria um vetor de bytes que começa com a sequência atual com o primeiro byte (posição 0)
            byte[] sequenciaAtual = new byte[1];
            sequenciaAtual[0] = pixelB[0];

            // guarda o cod da seq atual pra nao precisar buscar ela de novo no dic quando cair no else (como tem 1 byte só, o cod de 0 a 255 ja é o proprio byte)
            int codigoDaAtual = pixelB[0];

            // percorre o vetor com os bytes da imagem e aplica o algoritmo de compressão LZW, montando o dicionario e criando a fila de comprimido ao mesmo tempo
            // começa de um pq a gente ja colocou o primeiro código da imagem ali em cima
            for (int pos = 1; pos < pixelB.Length; pos++)
            {
                byte proximoByte = pixelB[pos];

                // cria um vetor de bytes que é a sequencia atual + o proximo byte, pra ver se essa nova sequencia existe no dicionario
                byte[] sequenciaNova = new byte[sequenciaAtual.Length + 1]; // pode unir o prox Byte e a sequencia atual
                for (int i = 0; i < sequenciaAtual.Length; i++)
                    sequenciaNova[i] = sequenciaAtual[i];
                sequenciaNova[sequenciaAtual.Length] = proximoByte; // coloca o prox

                int codigoEncontrado = dicionario.buscarCodigo(sequenciaNova); // verifica se o codigo existe no dicionario

                // como sempre queremos pegar a maior sequencia que existe só colocamos o cod de uma sequencia na lista de comprimidos quando não achamos no dic
                // uma sequencia x, ai colocamos a x-1 no fila de comprimidos e adicionamos a x no dicionario
                if (codigoEncontrado != -1) // se o codigo existe vai ser != -1
                {
                    codigoDaAtual = codigoEncontrado;//ja salva o cod encontrado (q é >= 256) pra usar direto se na prox volta nao achar
                    sequenciaAtual = new byte[sequenciaNova.Length];
                    for (int v = 0; v < sequenciaNova.Length; v++)//seq atual agora é a seq nova(ela sera usada de base)
                        sequenciaAtual[v] = sequenciaNova[v];
                }
                else
                {
                    // coloca na fila de comprimidos o código da sequencia atual (que é a maior que existe no dicionario)
                    fila.enqueue(codigoDaAtual);

                    // coloco no dicionario a sequencia nova (que é a sequencia atual + o proximo byte, que nao existe no dic)
                    dicionario.add(sequenciaNova);

                    sequenciaAtual = new byte[1];
                    sequenciaAtual[0] = proximoByte;
                    codigoDaAtual = proximoByte; //como voltou a ter 1 byte só, o cod dela de 0 a 255 ja é o proprio proximoByte
                }
            }

            // como o for termina sem colocar na fila de comprimidos o código da última sequência, precisamos colocar ele aqui fora
            fila.enqueue(codigoDaAtual);

            // salva o dicionario em um arquivo de texto, pra poder ver ele depois
            dicionario.salvarDicionarioTxt("../../dicionario.txt");

            // retorna a fila com os códigos comprimidos
            return fila;
        }

        public static void preencheVetBytes(Bitmap imgOrg, byte[] pixelB)
        {
            int width = imgOrg.Width;
            int height = imgOrg.Height;
            int pos = 0;

            //isso aqui bota a imagem inteira em um vetor de bytes, pq como cada pixel são 3 bytes representados pelo RGB, fica mais facil de mexer 
            //quando ja ta tudo em um vetor direto (e o livro recomenda, linearização da imagem), se fosse fazer a compressão enquanto anda pela imagem, seria muito mais complicado
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color cor = imgOrg.GetPixel(x, y);
                    pixelB[pos] = cor.R;
                    pixelB[pos + 1] = cor.G;
                    pixelB[pos + 2] = cor.B;
                    pos += 3;
                }
            }
        }
        
        public static void salvarArq(string caminhoArquivo, int width, int height, Fila codigosComprimidos)
        {
            // cria um arquivo para salvar os códigos comprimidos, e também a largura e altura da imagem no começo do arquivo
            FileStream arquivo = new FileStream(caminhoArquivo, FileMode.Create);

            // o BinaryWriter grava os valores direto em binario (bytes puros) no arquivo, pq se salvasse como texto cada numero ia virar caracter e ocupar mais bytes
            BinaryWriter escritor = new BinaryWriter(arquivo);

            // salva a largura e altura no começo do arquivo pra saber o tamanho da imagem na hora de reconstruir
            // cabeçalho com largura e altura (4 bytes cada)
            escritor.Write(width);
            escritor.Write(height);

            NoFila atual = codigosComprimidos.getInicio();
            while (atual != null)
            {
                int codigo1 = atual.getValor();
                atual = atual.getProx();

                // se o atual for != de null, significa que temos um par de códigos para gravar (2 códigos de 12 bits = 24 bits = 3 bytes)
                // se o atual for null, significa que temos apenas 1 código sobrando (1 código de 12 bits = 12 bits = 2 bytes), nessa parte nao tem muito o que fazer e vamos utilizar 
                // 2 bytes para gravar o código restante, mesmo que isso desperdice 4 bits do arquivo final
                if (atual != null)
                {
                    // pega o valor do segundo código do par e avança o ponteiro da fila
                    int codigo2 = atual.getValor();
                    atual = atual.getProx();

                    // aqui começa a gravação dos dois codigos em 3 bytes, cada código tem 12 bits, então precisamos fazer algumas operações de bit para separar os bits corretamente
                    // o primeiro byte vai ter os 8 bits inicias do primeiro código, o segundo byte vai ter os 4 bits finais do primeiro código e os 4 bits iniciais do segundo código, e o terceiro byte vai ter os 8 bits finais do segundo código
                    // o >>4 empurra os bits 4 casas para a direita, "jogando fora"
                    // o &0x0F pega os 4 bits finais do primeiro código, e o <<4 empurra esses 4 bits para a esquerda, "abrindo espaço" para os 4 bits iniciais do segundo código
                    // e o | junta os dois pedaços, formando o segundo byte completo (4 bits + 4 bits = 8 bits)
                    // o >>8 empurra os bits 8 casas para a direita, ficando apenas com os 4 bits iniciais do segundo codigo
                    // por fim o &0xFF pega os 8 bits finais do segundo código, que é o que queremos para o terceiro byte
                    byte byte1 = (byte)(codigo1 >> 4);
                    byte byte2 = (byte)(((codigo1 & 0x0F) << 4) | (codigo2 >> 8));
                    byte byte3 = (byte)(codigo2 & 0xFF);

                    escritor.Write(byte1);
                    escritor.Write(byte2);
                    escritor.Write(byte3);
                }
                else
                {
                    // aqui a quantidade codigo é impar, então só temos 1 código para gravar, e vamos utilizar 2 bytes para gravar esse código de 12 bits, mesmo que isso desperdice 4 bits do arquivo final
                    // ele empurra os 4 bits finais pra fora, e depois no byte2 ele pega os 4 bits finais do codigo1 e empurra 4 bits pra esquerda
                    byte byte1 = (byte)(codigo1 >> 4);
                    byte byte2 = (byte)((codigo1 & 0x0F) << 4);

                    escritor.Write(byte1);
                    escritor.Write(byte2);
                }
            }

            escritor.Close();
            arquivo.Close();
        }
    }
}