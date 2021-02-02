using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.Net;

namespace _2Unidad1Actividad
{
    public class Servidor
    {
        HttpListener server;

        public Servidor()
        {
            server = new HttpListener();
            server.Prefixes.Add("http://localhost/practica1/");
        }

        private void RepuestaSolicitud(IAsyncResult ar)
        {
            var context = server.EndGetContext(ar);
            server.BeginGetContext(RepuestaSolicitud, null);
            var peticion = context.Request;
            //
            if (context.Request.Url.LocalPath == "/practica1/")
            {
                context.Response.StatusCode = 200;
                string datos = "<h3>Nombre del alumno: </h3> <p>Alondra Elizabeth Delgadillo Silos</p>" +
                    "</br> <h2>Datos sobre la materia</h2> </br>" +
                    "<h3>Nombre de la materia: </h3> <p>Desarrollo de aplicaciones cliente-servidor</p> </br>" +
                    "<h3>Horario: </h3> <p>2:00 p.m. - 3:00 p.m. de lunes a viernes</p> </br>" +
                    "<h3>Profesor: </h3> <p>Hector Javier Padilla Lara</p>";
                byte[] buffer = Encoding.UTF8.GetBytes(datos);
                //
                context.Response.ContentType = "text/html";
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
            }
            else if (context.Request.Url.LocalPath == "/practica1/fecha_hora")
            {
                context.Response.StatusCode = 200;
                string fechahora = DateTime.Now.ToString();
                byte[] buffer = Encoding.UTF8.GetBytes(fechahora);
                //
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
            }
            else if (context.Request.Url.LocalPath == "/practica1/suma" &&
                context.Request.QueryString["pnum"]!= null &&
                context.Request.QueryString["pnum"]!= null)
            {
                context.Response.StatusCode = 200;
                int pnum = int.Parse(context.Request.QueryString["pnum"]);
                int snum = int.Parse(context.Request.QueryString["snum"]);
                var resultado = $"Resultado de la suma = {(pnum + snum).ToString()}";
                byte[] buffer = Encoding.UTF8.GetBytes(resultado);
                //
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
                context.Response.Close();
            }
            else
            {
                context.Response.StatusCode = 404;
            }
            context.Response.Close();
        }

        public void IniciarServidor()
        {
            server.Start();
            server.BeginGetContext(RepuestaSolicitud, null);
        }

        public void DetenerServidor()
        {
            server.Stop();
        }
    }
}
