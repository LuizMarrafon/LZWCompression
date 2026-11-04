using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmoLZW
{
    public class dicionarioLZW
    {
        private const int QuantidadeSimbolosIniciais = 256;
        private const int CapacidadeMaxima = 4096;
        private byte[][] tabela;
        private int proximoCodigo;

        public dicionarioLZW()
        {
            //inicializa a tabela com 4096 linhas, cada linha é um vetor de bytes (pq cada código pode ter uma sequência de bytes diferente)
            tabela = new byte[CapacidadeMaxima][];

            for (int codigo = 0; codigo < QuantidadeSimbolosIniciais; codigo++)
            {
                // cada código de 0 a 255 vai ter um vetor de bytes com 1 byte só, que é o próprio valor do código (pq os códigos de 0 a 255 representam os bytes isolados)
                tabela[codigo] = new byte[1];
                tabela[codigo][0] = (byte)codigo;
            }

            //vai servir pra saber onde ta o próximo código a ser inserido na tabela, é o TL do vetor
            proximoCodigo = QuantidadeSimbolosIniciais;
        }

        // verifica se os dois veoters de bytes são iguais, retorna true se forem iguais, false se forem diferentes
        private bool sequenciasIguais(byte[] sequencia, byte[] sequenciaTabela)
        {
            bool flag = false;
            if(sequencia != null && sequenciaTabela != null && sequencia.Length == sequenciaTabela.Length)
            {
                flag = true;
                int i = 0;
                while(i < sequencia.Length && flag)
                {
                    if(sequencia[i] != sequenciaTabela[i])
                        flag = false;
                    i++;
                }
            }
            return flag;
        }

        // recebe um vetor que contém a sequência nova, que é a sequencia atual + o próximo byte, e retorna o código dela no dicionário, ou -1 se não existir
        public int buscarCodigo(byte[] sequencia)
        {
            int codigoEncontrado = -1;
            if (sequencia.Length == 1) // se a seq tem só 1 byte, o codigo dela no dic ja é o proprio valor do byte (pq de 0 a 255 o indice é igual ao valor)
                codigoEncontrado = sequencia[0];
            else //se tem mais de 1 byte, só pode estar da posição 256 pra frente, entao nem precisa procurar de 0 a 255
            {
                int codigo = QuantidadeSimbolosIniciais;
                while(codigo < proximoCodigo && codigoEncontrado == -1)
                {
                    // se o tamanho da sequencia for igual ao tamanho do vetor da linha do código do dicionario e o primeiro byte ja forem iguais, chama a funcao
                    if(tabela[codigo].Length == sequencia.Length && tabela[codigo][0] == sequencia[0])
                        if(sequenciasIguais(sequencia, tabela[codigo]))
                            codigoEncontrado = codigo;
                    codigo++;
                }
            }
            return codigoEncontrado;
        }

        public int add(byte[] sequencia)
        {
            int codigoAdicionado = -1;

            if (sequencia != null && sequencia.Length > 0 && proximoCodigo < CapacidadeMaxima)
            {
                tabela[proximoCodigo] = sequencia;
                codigoAdicionado = proximoCodigo;
                proximoCodigo++;
            }

            return codigoAdicionado;
        }

        // recebe um código e retorna a sequência de bytes correspondente no dicionário, ou null se não existir
        public byte[] buscarSequencia(int codigo)
        {
            byte[] sequenciaEncontrada = null;
            if (codigo >= 0 && codigo < proximoCodigo)
                sequenciaEncontrada = tabela[codigo];
            return sequenciaEncontrada;
        }

        // ---------------- funcoes de exibir ----------------------------

        //salva em um txt todas as sequencias novas q o dicionario aprendeu (da posicao 256 pra frente) pra poder visualizar
        public void salvarDicionarioTxt(string caminhoTxt)
        {
            // cria um StreamWriter para escrever no arquivo de texto
            StreamWriter escritor = new StreamWriter(caminhoTxt);

            // escreve as informações do dicionário no arquivo de texto
            escritor.WriteLine("Posicoes ocupadas no dicionario: " + proximoCodigo + " de " + CapacidadeMaxima);
            escritor.WriteLine("Codigos 0 a 255: bytes isolados iniciais");
            escritor.WriteLine("Codigos 256 ate " + (proximoCodigo - 1) + ": novas sequencias aprendidas");
            escritor.WriteLine("--------------------------------------------------");

            // escreve as seq aprendidas no arquivo de texto, a partir do código 256
            for (int codigo = QuantidadeSimbolosIniciais; codigo < proximoCodigo; codigo++)
            {
                // cria uma string com a seq de bytes correspondente ao código atual
                string seqTexto = "";
                for (int i = 0; i < tabela[codigo].Length; i++)
                    seqTexto += tabela[codigo][i] + " ";
                escritor.WriteLine("Codigo " + codigo + " -> [ " + seqTexto + "]");
            }

            // fecha o StreamWriter so pra para de escrever no arquivo, senao ele nao salva nada
            escritor.Close();
        }
    }
}
