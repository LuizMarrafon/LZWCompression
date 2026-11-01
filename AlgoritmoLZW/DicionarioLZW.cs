using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmoLZW
{
    internal class DicionarioLZW
    {
        private const int QuantidadeSimbolosIniciais = 256;
        private const int CapacidadeMaxima = 4096;

        private byte[][] tabela;
        //vai servir pra saber onde ta o próximo código a ser inserido na tabela, é o TL do vetor
        private int proximoCodigo;

        public DicionarioLZW()
        {
            tabela = new byte[CapacidadeMaxima][];

            for (int codigo = 0; codigo < QuantidadeSimbolosIniciais; codigo++)
            {
                tabela[codigo] = new byte[1];
                tabela[codigo][0] = (byte)codigo;
            }

            proximoCodigo = QuantidadeSimbolosIniciais;
        }

        private bool SequenciasIguais(byte[] sequencia, byte[] sequenciaTabela)
        {
            bool flag = false;
            if(sequencia != null && sequenciaTabela != null && sequencia.Length == sequenciaTabela.Length)
            {
                flag = true;
                int i = 0;
                while(i < sequencia.Length && flag)
                {
                    if(sequencia[i] != sequenciaTabela[i])
                    {
                        flag = false;
                    }
                    i++;
                }
            }
            return flag;
        }

        public int BuscarCodigo(byte[] sequencia)
        {
            int codigoEncontrado = -1;
            //se a seq tem só 1 byte, o codigo dela no dic ja é o proprio valor do byte (pq de 0 a 255 o indice é igual ao valor)
            if (sequencia.Length == 1)
            {
                codigoEncontrado = sequencia[0];
            }
            else
            {
                //se tem mais de 1 byte, só pode estar da posição 256 pra frente, entao nem precisa procurar de 0 a 255
                int codigo = QuantidadeSimbolosIniciais;
                while (codigo < proximoCodigo && codigoEncontrado == -1)
                {
                    //só chama a funcao pra comparar tudo se o tamanho e o primeiro byte ja forem iguais
                    if (tabela[codigo].Length == sequencia.Length && tabela[codigo][0] == sequencia[0])
                    {
                        if (SequenciasIguais(sequencia, tabela[codigo]))
                        {
                            codigoEncontrado = codigo;
                        }
                    }
                    codigo++;
                }
            }
            return codigoEncontrado;
        }

        public int Adicionar(byte[] sequencia)
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

        public byte[] BuscarSequencia(int codigo)
        {
            byte[] sequenciaEncontrada = null;

            if (codigo >= 0 && codigo < proximoCodigo)
            {
                sequenciaEncontrada = tabela[codigo];
            }

            return sequenciaEncontrada;
        }

        //----------------------- exibir
        //salva em um txt todas as sequencias novas q o dicionario aprendeu (da posicao 256 pra frente) pra poder visualizar
        public void SalvarDicionarioTxt(string caminhoTxt)
        {
            StreamWriter escritor = new StreamWriter(caminhoTxt);
            escritor.WriteLine("Posicoes ocupadas no dicionario: " + proximoCodigo + " de " + CapacidadeMaxima);
            escritor.WriteLine("Codigos 0 a 255: bytes isolados iniciais");
            escritor.WriteLine("Codigos 256 ate " + (proximoCodigo - 1) + ": novas sequencias aprendidas");
            escritor.WriteLine("--------------------------------------------------");

            for (int codigo = QuantidadeSimbolosIniciais; codigo < proximoCodigo; codigo++)
            {
                string seqTexto = "";
                for (int i = 0; i < tabela[codigo].Length; i++)
                {
                    seqTexto += tabela[codigo][i] + " ";
                }
                escritor.WriteLine("Codigo " + codigo + " -> [ " + seqTexto + "]");
            }

            escritor.Close();
        }
    }
}
