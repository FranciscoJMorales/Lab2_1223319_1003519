using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClasesGenericas.Estructuras
{
    public class Nodo<T>
    {
        public Nodo<T> Padre { get; set; }
        public Nodo<T> Izquierda { get; set; }
        public Nodo<T> Derecha { get; set; }
        public T Valor { get; set; }
    }
}
