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
            List<Farmaco> resultados = new List<Farmaco>
            {
                Storage.Instance.Indice.Search(new Farmaco { Nombre = name }, Farmaco.CompararNombre)
            };
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
            string[] datos = Storage.Instance.ListadoFarmacos[id - 1].Split(';');
            InfoFarmaco nuevo = new InfoFarmaco
            {
                ID = Int32.Parse(datos[0]),
                Nombre = datos[1],
                Descripcion = datos[2],
                Productora = datos[3],
                Precio = double.Parse(datos[4]),
                Existencia = int.Parse(datos[5])
            };
            return View("AgregarProducto", nuevo);
        }

        [HttpPost]
        public ActionResult AgregarProducto(int id, FormCollection collection)
        {
            int cant = Int32.Parse(collection["cant"]);
            Farmaco nuevo = new Farmaco();
            string texto = Storage.Instance.ListadoFarmacos[id - 1];
            nuevo.ID = Int32.Parse(texto.Substring(0, texto.IndexOf(";")));
            texto = texto.Remove(0, texto.IndexOf(";") + 1);
            nuevo.Nombre = texto.Substring(0, texto.IndexOf(";"));
            while (texto.IndexOf(';') >= 0)
            {
                texto = texto.Remove(0, texto.IndexOf(';') + 1);
            }
            nuevo.Cantidad = int.Parse(texto);
            if (cant <= nuevo.Cantidad)
            {
                nuevo.Cantidad = cant;
                Storage.Instance.Pedidos.Add(nuevo);
            }
            return RedirectToAction("Index");
        }

        public ActionResult NuevoPedido()
        {
            return View("Pedido", Storage.Instance.Pedidos);
        }

        [HttpPost]
        public ActionResult NuevoPedido(FormCollection collection)
        {
            foreach (Farmaco producto in Storage.Instance.Pedidos)
            {
                Farmaco aux = Storage.Instance.Indice.Search(producto, Farmaco.CompararNombre);
                aux.Cantidad -= producto.Cantidad;
                string temporal = Storage.Instance.ListadoFarmacos[aux.ID - 1];
                while (Storage.Instance.ListadoFarmacos[aux.ID - 1].IndexOf(';') >= 0)
                {
                    temporal += Storage.Instance.ListadoFarmacos[aux.ID - 1].Substring(0, Storage.Instance.ListadoFarmacos[aux.ID - 1].IndexOf(';') + 1);
                    Storage.Instance.ListadoFarmacos[aux.ID - 1] = Storage.Instance.ListadoFarmacos[aux.ID - 1].Remove(0, Storage.Instance.ListadoFarmacos[aux.ID - 1].IndexOf(';') + 1);
                }
                Storage.Instance.ListadoFarmacos[aux.ID - 1] = temporal;
                Storage.Instance.ListadoFarmacos[aux.ID - 1] += aux.Cantidad.ToString();
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

        public ActionResult MostrarReabastecer()
        {
            return View("Reabastecer", Storage.Instance.SinExistencias);
        }

        public ActionResult Reabastecer(int id)
        {
            try
            {
                for (int i = 0; i < Storage.Instance.ListadoFarmacos.Length; i++)
                {
                    if (Storage.Instance.ListadoFarmacos[i] != "")
                    {
                        if (id == Int32.Parse(Storage.Instance.ListadoFarmacos[i].Substring(0, 1)))
                        {
                            string aux = Storage.Instance.ListadoFarmacos[i];
                            string aux2 = "";
                            while (aux.Contains(";"))
                            {
                                aux2 += aux.Substring(0, aux.IndexOf(";") + 1);
                                aux = aux.Remove(0, aux.IndexOf(";") + 1);
                            }
                            if (Int32.Parse(aux) == 0)
                            {
                                Random rnd = new Random();
                                int nuevaCantidad = rnd.Next(1, 16);
                                aux2 += nuevaCantidad.ToString();
                                Storage.Instance.ListadoFarmacos[i] = aux2;
                                Farmaco nuevo = new Farmaco();
                                aux2 = aux2.Remove(0, aux2.IndexOf(";") + 1);
                                nuevo.Nombre = aux2.Substring(0, aux2.IndexOf(";"));
                                Farmaco temporal = Storage.Instance.SinExistencias.Remove(nuevo, Farmaco.CompararNombre);
                                temporal.Cantidad = nuevaCantidad;
                                Storage.Instance.Indice.Add(temporal, Farmaco.CompararNombre);
                            }
                        }
                    }
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
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] != "")
                    {
                        InfoFarmaco Infonuevo = new InfoFarmaco
                        {
                            ID = Int32.Parse(text[i].Substring(0, text[i].IndexOf(",")))
                        };                                 
                        Farmaco Farmaconuevo = new Farmaco
                        {
                            ID = Int32.Parse(text[i].Substring(0, text[i].IndexOf(",")))
                        };
                        text[i] = text[i].Remove(0, text[i].IndexOf(",") + 1);

                        if (text[i].Substring(0, 1)== "\""){
                            //Farmaconuevo.Nombre = 
                            Infonuevo.Nombre =text[i].Substring(0, text[i].IndexOf("\","));
                            text[i] = text[i].Remove(0, text[i].IndexOf("\",") + 2);
                        }
                        else
                        {
                            Infonuevo.Nombre = text[i].Substring(0, text[i].IndexOf(","));
                            text[i] = text[i].Remove(0, text[i].IndexOf(",") + 1);
                        }
                        if (text[i].Substring(0, 1).Equals("\""))
                        {
                            Infonuevo.Descripcion = text[i].Substring(0, text[i].IndexOf("\","));
                            text[i] = text[i].Remove(0, text[i].IndexOf("\",") + 2);
                        }
                        else
                        {
                            Infonuevo.Descripcion = text[i].Substring(0, text[i].IndexOf(","));
                            text[i] = text[i].Remove(0, text[i].IndexOf(",") + 1);
                        }
                        if (text[i].Substring(0, 1).Equals("\""))
                        {
                            Infonuevo.Productora = text[i].Substring(0, text[i].IndexOf("\","));
                            text[i] = text[i].Remove(0, text[i].IndexOf("\",") + 2);
                        }
                        else
                        {
                            Infonuevo.Productora = text[i].Substring(0, text[i].IndexOf(""));
                            text[i] = text[i].Remove(0, text[i].IndexOf(",") + 2);
                        }

                        if (text[i].Substring(0, 1).Equals("$"))
                        {
                            Infonuevo.Productora = text[i].Substring(0, text[i].IndexOf(","));
                            text[i] = text[i].Remove(0, text[i].IndexOf(",") + 1);
                        }
                        else
                        {
                            Infonuevo.Precio = double.Parse(text[i].Substring(0, text[i].IndexOf(",")));
                            text[i] = text[i].Remove(0, text[i].IndexOf(",") + 1);
                        }
                        Infonuevo.Existencia = Int32.Parse(text[i]);

                        Farmaconuevo.Nombre = Infonuevo.Nombre;
                        Farmaconuevo.Cantidad = Int32.Parse(text[i]);
                        if (Farmaconuevo.Cantidad > 0)
                            Storage.Instance.Indice.Add(Farmaconuevo, Farmaco.CompararNombre);
                        else
                            Storage.Instance.SinExistencias.Add(Farmaconuevo, Farmaco.CompararNombre);
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
