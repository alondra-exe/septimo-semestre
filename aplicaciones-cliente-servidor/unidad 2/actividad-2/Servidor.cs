using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.Net;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.ComponentModel;

namespace Unidad2Actividad2
{
    public class Servidor : INotifyPropertyChanged
    {
        HttpListener listener;
        //
        private string escrito;
        public string Escrito
        {
            get { return escrito; }
            set
            {
                escrito = value;
                OnProperyChanged("Escrito");
            }
        }
        private string colores;
        public string Colores
        {
            get { return colores; }
            set
            {
                colores = value;
                OnProperyChanged("Colores");
            }
        }
        //
        Dispatcher dispatcher;
        //
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnProperyChanged(string propiedad)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
        }
        //
        public Servidor()
        {
            dispatcher = Dispatcher.CurrentDispatcher;
            //
            listener = new HttpListener();
            listener.Prefixes.Add("http://*:80/actividad2/");
            listener.Start();
            listener.BeginGetContext(OnRequest, null);
        }
        //
        private void OnRequest(IAsyncResult ar)
        {
            var context = listener.EndGetContext(ar);
            listener.BeginGetContext(OnRequest, null);
            //
            if (context.Request.Url.LocalPath == "/actividad2/" ||
                context.Request.Url.LocalPath == "/actividad2")
            {
                var buffer = File.ReadAllBytes("Index.html");
                context.Response.ContentType = "text/html";
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
                context.Response.StatusCode = 200;
            }
            else if (context.Request.Url.LocalPath == "/actividad2/texto" &&
                context.Request.HttpMethod == "GET")
            {
                if (context.Request.QueryString["escrito"] != null)
                {
                    var escrito = context.Request.QueryString["escrito"];
                    var colores = context.Request.QueryString["colores"];
                    Agregar(escrito, colores);
                    context.Response.StatusCode = 200;
                    context.Response.Redirect("/actividad2/");
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.StatusDescription = "Es necesario escribir un texto";
                }
            }
            else
            {
                context.Response.StatusCode = 404;
            }
            context.Response.Close();
        }
        //
        private void Agregar(string escrito, string colores)
        {
            dispatcher.BeginInvoke(
                new Action(() =>
                {
                    Escrito = escrito;
                    Colores = colores;
                })
            );
        }
    }
}