using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lab3_1223319_1003519.Helpers;
using Lab3_1223319_1003519.Models;
using System.IO;
using ClasesGenericas.Estructuras;

namespace Lab3_1223319_1003519.Controllers
{
    public class FarmacoController : Controller
    {
        // GET: Farmaco
        public ActionResult Index()
        {
            if (Storage.Instance.PrimeraSesion)
            {
                //CargarArchivo();
                Storage.Instance.dir = Server.MapPath("/Farmacos.csv");
                Storage.Instance.dir = Storage.Instance.dir.Replace("\\", "//");
                Storage.Instance.dir = Storage.Instance.dir.Insert(Storage.Instance.dir.IndexOf(':') + 1, " ");
                Storage.Instance.dir = Storage.Instance.dir.Remove(Storage.Instance.dir.IndexOf("//Lab3_1223319_1003519"), 22);
                AbrirArchivo();
                Storage.Instance.PrimeraSesion = false;
            }
            return View(Storage.Instance.Indice);
        }

        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {

            string name = collection["search"];
            List<Farmaco> resultados = new List<Farmaco>();
            Farmaco resultado = Storage.Instance.Indice.Search(new Farmaco { Nombre = name }, Farmaco.CompararNombre);
            if (resultado != null)
                resultados.Add(resultado);
            return View("Resultados", resultados);

        }

        // GET: Farmaco/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Farmaco/Create
        public ActionResult Create()
        {
            return View("Pedido");
        }

        // POST: Farmaco/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Farmaco/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Farmaco/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Farmaco/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Farmaco/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult AgregarProducto(int id)
        {
            bool valido = true;
            string aux = Storage.Instance.ListadoFarmacos[id];
            InfoFarmaco nuevo = new InfoFarmaco
            {
                ID = Int32.Parse(aux.Substring(0, aux.IndexOf(',')))
            };
            aux = aux.Remove(0, aux.IndexOf(',') + 1);
            //Nombre
            if (aux.IndexOf('\"') == 0)
            {
                aux = aux.Remove(0, 1);
                nuevo.Nombre = aux.Substring(0, aux.IndexOf('\"'));
                aux = aux.Remove(0, aux.IndexOf('\"') + 2);
            }
            else
            {
                nuevo.Nombre = aux.Substring(0, aux.IndexOf(','));
                aux = aux.Remove(0, aux.IndexOf(',') + 1);
            }
            for (int i = 0; i < Storage.Instance.Pedidos.Count; i++)
            {
                if (Storage.Instance.Pedidos[i].Nombre == nuevo.Nombre)
                {
                    valido = false;
                    i = Storage.Instance.Pedidos.Count;
                }
            }
            if (valido)
            {
                //Descripcion
                if (aux.IndexOf('\"') == 0)
                {
                    aux = aux.Remove(0, 1);
                    nuevo.Descripcion = aux.Substring(0, aux.IndexOf('\"'));
                    aux = aux.Remove(0, aux.IndexOf('\"') + 2);
                }
                else
                {
                    nuevo.Descripcion = aux.Substring(0, aux.IndexOf(','));
                    aux = aux.Remove(0, aux.IndexOf(',') + 1);
                }
                //Productora
                if (aux.IndexOf('\"') == 0)
                {
                    aux = aux.Remove(0, 1);
                    nuevo.Productora = aux.Substring(0, aux.IndexOf('\"'));
                    aux = aux.Remove(0, aux.IndexOf('\"') + 3);
                }
                else
                {
                    nuevo.Productora = aux.Substring(0, aux.IndexOf(','));
                    aux = aux.Remove(0, aux.IndexOf(',') + 2);
                }
                nuevo.Precio = double.Parse(aux.Substring(0, aux.IndexOf(',')));
                aux = aux.Remove(0, aux.IndexOf(',') + 1);
                nuevo.Existencia = Int32.Parse(aux);
                return View("AgregarProducto", nuevo);
            }
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AgregarProducto(int id, FormCollection collection)
        {
            int cant = Int32.Parse(collection["cant"]);
            FarmacoPrecio nuevo = new FarmacoPrecio();
            string texto = Storage.Instance.ListadoFarmacos[id];
            nuevo.ID = Int32.Parse(texto.Substring(0, texto.IndexOf(',')));
            texto = texto.Remove(0, texto.IndexOf(",") + 1);
            if (texto.IndexOf('\"') == 0)
            {
                texto = texto.Remove(0, 1);
                nuevo.Nombre = texto.Substring(0, texto.IndexOf('\"'));
                texto = texto.Remove(0, texto.IndexOf('\"') + 2);
            }
            else
            {
                nuevo.Nombre = texto.Substring(0, texto.IndexOf(','));
                texto = texto.Remove(0, texto.IndexOf(',') + 1);
            }
            texto = texto.Substring(texto.IndexOf('$') + 1);
            nuevo.Precio = double.Parse(texto.Substring(0, texto.IndexOf(',')));
            texto = texto.Remove(0, texto.IndexOf(',') + 1);
            nuevo.Cantidad = Int32.Parse(texto);
            if (cant <= nuevo.Cantidad)
            {
                nuevo.Cantidad = cant;
                Storage.Instance.Pedidos.Add(nuevo);
            }
            return RedirectToAction("Index");
        }

        public ActionResult NuevoPedido()
        {
            if (Storage.Instance.Pedidos.Count != 0)
            {
                for(int i = 0; i < Storage.Instance.Pedidos.Count; i++)
                {
                    if (Storage.Instance.Pedidos[i].Nombre == "Total:")
                        Storage.Instance.Pedidos.Remove(Storage.Instance.Pedidos[i]);
                }
                double total = 0;
                int cantidad = 0;
                foreach (FarmacoPrecio producto in Storage.Instance.Pedidos)
                {
                    total += producto.Precio * producto.Cantidad;
                    cantidad += producto.Cantidad;
                }
                Storage.Instance.Pedidos.Add(new FarmacoPrecio { ID = 0, Nombre = "Total:", Cantidad = cantidad, Precio = total });
                return View("Pedido", Storage.Instance.Pedidos);
            }
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult NuevoPedido(FormCollection collection)
        {
            Storage.Instance.Pedidos.Remove(Storage.Instance.Pedidos[Storage.Instance.Pedidos.Count - 1]);
            foreach (FarmacoPrecio producto in Storage.Instance.Pedidos)
            {
                Farmaco aux = Storage.Instance.Indice.Search(new Farmaco{ Nombre = producto.Nombre }, Farmaco.CompararNombre);
                aux.Cantidad -= producto.Cantidad;
                string temporal = Storage.Instance.ListadoFarmacos[aux.ID];
                Storage.Instance.ListadoFarmacos[aux.ID] = Storage.Instance.ListadoFarmacos[aux.ID].Substring(0, temporal.LastIndexOf(',') + 1);
                Storage.Instance.ListadoFarmacos[aux.ID] += aux.Cantidad.ToString();
                Sobreescribir();
                if (aux.Cantidad == 0)
                {
                    Storage.Instance.Indice.Delete(aux, Farmaco.CompararNombre);
                    Storage.Instance.SinExistencias.Add(aux, Farmaco.CompararNombre);
                }
            }
            Storage.Instance.Pedidos.Clear();
            return RedirectToAction("Index");
        }

        public ActionResult Eliminar(int id)
        {
            try
            {
                for (int i = 0; i < Storage.Instance.Pedidos.Count; i++)
                {
                    if (Storage.Instance.Pedidos[i].ID == id)
                        Storage.Instance.Pedidos.Remove(Storage.Instance.Pedidos[i]);
                }
                if (Storage.Instance.Pedidos.Count == 1 && Storage.Instance.Pedidos[0].Nombre == "Total:")
                    Storage.Instance.Pedidos.Clear();
                return RedirectToAction("NuevoPedido");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult MostrarReabastecer()
        {
            return View("Reabastecer", Storage.Instance.SinExistencias);
        }

        public ActionResult Reabastecer(int id)
        {
            try
            {
                string aux = Storage.Instance.ListadoFarmacos[id];
                string aux2 = "";
                aux2 = aux.Substring(0, aux.LastIndexOf(',') + 1);
                aux = aux.Remove(0, aux.LastIndexOf(',') + 1);
                if (Int32.Parse(aux) == 0)
                {
                    Random rnd = new Random();
                    int nuevaCantidad = rnd.Next(1, 16);
                    aux2 += nuevaCantidad.ToString();
                    Storage.Instance.ListadoFarmacos[id] = aux2;
                    Farmaco nuevo = new Farmaco();
                    aux2 = aux2.Remove(0, aux2.IndexOf(",") + 1);
                    if (aux2.IndexOf('\"') == 0)
                    {
                        aux2 = aux2.Remove(0, 1);
                        nuevo.Nombre = aux2.Substring(0, aux2.IndexOf('\"'));
                        aux2 = aux2.Remove(0, aux2.IndexOf('\"') + 2);
                    }
                    else
                    {
                        nuevo.Nombre = aux2.Substring(0, aux2.IndexOf(','));
                        aux2 = aux2.Remove(0, aux2.IndexOf(',') + 1);
                    }
                    Farmaco temporal = Storage.Instance.SinExistencias.Remove(nuevo, Farmaco.CompararNombre);
                    temporal.Cantidad = nuevaCantidad;
                    Storage.Instance.Indice.Add(temporal, Farmaco.CompararNombre);
                }
                Sobreescribir();
            }
            catch
            {
            }


            return RedirectToAction("Index");
            }
         

        //Se debe cambiar la ruta del archivo para que no ocurran errores en la carga.
        public void AbrirArchivo()
        {
            try
            {
                using (StreamReader lector = new StreamReader(Storage.Instance.dir))
                {
                    Storage.Instance.Farmacos = lector.ReadToEnd();
                }
                Storage.Instance.ListadoFarmacos = Storage.Instance.Farmacos.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string[] text = (string[])Storage.Instance.ListadoFarmacos.Clone();
                Storage.Instance.Indice.Clear();
                for (int i = 1; i < text.Length; i++)
                {
                    if (text[i] != "")
                    {
                        Farmaco nuevo = new Farmaco
                        {
                            ID = Int32.Parse(text[i].Substring(0, text[i].IndexOf(",")))
                        };
                        text[i] = text[i].Remove(0, text[i].IndexOf(",") + 1);
                        if (text[i].IndexOf('\"') == 0)
                        {
                            text[i] = text[i].Remove(0, 1);
                            nuevo.Nombre = text[i].Substring(0, text[i].IndexOf('\"'));
                        }
                        else
                        {
                            nuevo.Nombre = text[i].Substring(0, text[i].IndexOf(','));
                        }
                        text[i] = text[i].Substring(text[i].LastIndexOf(',') + 1);
                        nuevo.Cantidad = Int32.Parse(text[i]);
                        if (nuevo.Cantidad > 0)
                            Storage.Instance.Indice.Add(nuevo, Farmaco.CompararNombre);
                        else
                            Storage.Instance.SinExistencias.Add(nuevo, Farmaco.CompararNombre);
                    }
                }
            }
            catch
            {
            }
        }

        public void GuardarArchivo()
        {
            try
            {
                using (StreamWriter escritor = new StreamWriter(Storage.Instance.dir, false))
                {
                    escritor.Write(Storage.Instance.Farmacos);
                }
            }
            catch
            {
            }
        }

        public void Sobreescribir()
        {
            Storage.Instance.Farmacos = "";
            for (int i = 0; i < Storage.Instance.ListadoFarmacos.Length; i++)
            {
                if (Storage.Instance.ListadoFarmacos[i] != "")
                {
                    Storage.Instance.Farmacos += Storage.Instance.ListadoFarmacos[i];
                    if (i != Storage.Instance.ListadoFarmacos.Length - 1)
                        Storage.Instance.Farmacos += "\r\n";
                }
            }
            GuardarArchivo();
        }
    }
}
