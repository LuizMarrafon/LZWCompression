using System;
using System.Collections.Generic;
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
                    if (SequenciasIguais(sequencia, tabela[codigo]))
                    {
                        codigoEncontrado = codigo;
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
    }
}
