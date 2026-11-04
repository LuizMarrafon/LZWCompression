using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmoLZW
{
    public class NoFila
    {
        private int valor;
        private NoFila prox;

        public NoFila(int valor)
        {
            this.valor = valor;
            this.prox = null;
        }

        public int getValor()
        {
            return valor;
        }

        public NoFila getProx()
        {
            return prox;
        }

        public void setValor(int valor)
        {
            this.valor = valor;
        }

        public void setProx(NoFila prox)
        {
            this.prox = prox;
        }
    }
}
