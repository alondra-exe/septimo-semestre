using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Threading;
using System.Security.Policy;
using System.IO;
using System.Web;

namespace Unidad2Actividad3
{
    public class ServidorDeportivo
    {
        HttpListener listener;
        //
        public Puntos Puntos { get; set; } = new Puntos();
        Dispatcher dispatcher;

        public ServidorDeportivo()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://*:8080/actividad3/");
            listener.Start();
            //
            dispatcher = Dispatcher.CurrentDispatcher;
            //
            listener.BeginGetContext(Recibir, null);
        }

        private void Recibir(IAsyncResult ar)
        {
            var context = listener.EndGetContext(ar);
            listener.BeginGetContext(Recibir, null);
            //
            var url = context.Request.Url.LocalPath;
            if (url.EndsWith("/"))
            {
                url = url.Remove(url.Length - 1, 1);
            }
            if (context.Request.HttpMethod == "GET" &&
                url == "/actividad3")
            {
                var index = System.IO.File.ReadAllBytes("Index.html");
                context.Response.ContentType = "text/html";
                context.Response.OutputStream.Write(index, 0, index.Length);
                context.Response.StatusCode = 200;
                context.Response.Close();
            }
            else if (context.Request.HttpMethod == "POST" &&
                url == "/actividad3")
            {
                StreamReader stream = new StreamReader(context.Request.InputStream);
                string variables = stream.ReadToEnd();
                var datos = HttpUtility.ParseQueryString(variables);
                Punto(datos["punto"]);
                context.Response.StatusCode = 200;
                context.Response.Redirect("/actividad3");
            }
            else if (context.Request.HttpMethod == "GET" &&
                url == "/actividad3/deporte")
            {
                if (context.Request.QueryString["equipoU"] != null &&
                    context.Request.QueryString["equipoD"] != null)
                {
                    var nombreU = context.Request.QueryString["equipoU"];
                    var colorU = context.Request.QueryString["colorU"];
                    var nombreD = context.Request.QueryString["equipoD"];
                    var colorD = context.Request.QueryString["colorD"];
                    Agregar(nombreU, colorU, nombreD, colorD);
                    context.Response.StatusCode = 200;
                    context.Response.Redirect("/actividad3");
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.StatusDescription = "Necesita agregar ambos equipos.";
                }
            }
            else
            {
                context.Response.StatusCode = 404;
            }
            context.Response.Close();
        }

        public void Agregar(string equipoU, string colorU,
            string equipoD, string colorD)
        {
            dispatcher.BeginInvoke(
                new Action(() =>
                {
                    Puntos.EquipoU = equipoU;
                    Puntos.ColorU = colorU;
                    Puntos.EquipoD = equipoD;
                    Puntos.ColorD = colorD;
                })
            );
        }

        public void Punto(string punto)
        {
            dispatcher.BeginInvoke(new Action(() =>
            {

                if (punto == "Equipo uno")
                {
                    Puntos.PuntosU++;
                }
                else if (punto == "Equipo dos")
                {
                    Puntos.PuntosD++;
                }
                else
                {
                    Puntos.PuntosD = 0;
                    Puntos.PuntosU = 0;
                }

            }));
        }
    }
}
