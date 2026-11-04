using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmoLZW
{
    // fizemos uma fila que contém ponteiros para o início e para o fim
    internal class Fila
    {
        private NoFila inicio;
        private NoFila fim;
        private int qntd;
        public Fila()
        {
            // o java ja colocava null automatico nos ponteiros, c# nao tenho certeza
            inicio = null;
            fim = null;
            qntd = 0;
        }

        public bool isEmpty()
        {
            return inicio == null;
        }

        public void enqueue(int valor)
        {
            NoFila novoNo = new NoFila(valor);

            if(inicio == null) // se a lista estiver vazia, o início e o fim apontam para o novo nó
                inicio = fim = novoNo;
            else // se nao tiver vazia, entao vai pro final
            {
                fim.setProx(novoNo);
                fim = novoNo;
            }
            qntd++;
        }

        // retorna o valor de uma determinada posição, percorrendo a partir do inicio, se nao achou, return -1;
        public int obter(int indice)
        {
            NoFila atual = inicio;
            int contador = 0;
            bool continua = true;

            while(atual != null && continua)
            {
                if(contador == indice)
                    continua = false;
                else
                {
                    atual = atual.getProx();
                    contador++;
                }
            }
            if(contador == indice)
                return atual.getValor();
            else
                return -1;
        }

        public int getQntd()
        {
            return qntd;
        }

        public NoFila getInicio()
        {
            return inicio;
        }

        public NoFila getFim()
        {
            return fim;
        }

        public void setInicio(NoFila inicio)
        {
            this.inicio = inicio;
        }

        public void setFim(NoFila fim)
        {
            this.fim = fim;
        }
    }
}

