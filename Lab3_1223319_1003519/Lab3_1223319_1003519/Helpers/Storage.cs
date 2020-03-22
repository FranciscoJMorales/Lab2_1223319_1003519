using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lab3_1223319_1003519.Models;
using ClasesGenericas.Estructuras;

namespace Lab3_1223319_1003519.Helpers
{
    public class Storage
    {
        private static Storage _instance;

        public static Storage Instance
        {
            get
            {
                if (_instance == null) _instance = new Storage();
                return _instance;
            }
        }

        public Arbol<Farmaco> Indice = new Arbol<Farmaco>();
        public Arbol<Farmaco> SinExistencias = new Arbol<Farmaco>();
        public List<FarmacoPrecio> Pedidos = new List<FarmacoPrecio>();
        public bool PrimeraSesion = true;
        public string Farmacos;
        public string[] ListadoFarmacos;
        public string dir;
    }
}