using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Windows.Threading;
using System.Web;
using Newtonsoft.Json;

namespace ServidorFarmacias
{
    public class ServidorFarmacia
    {
        public CatalogoMedicamentos catalogoMedicamentos { get; set; } = new CatalogoMedicamentos();
        HttpListener server;
        Dispatcher dispatcher;

        public ServidorFarmacia()
        {
            dispatcher = Dispatcher.CurrentDispatcher;
            server = new HttpListener();
            server.Prefixes.Add("http://*:8080/maxmedicamentos/");
            server.Start();            
            server.BeginGetContext(Solicitud, null);
        }

        public void Solicitud(IAsyncResult a)
        {
            var context = server.EndGetContext(a);
            server.BeginGetContext(Solicitud, null);

            if (context.Request.Url.LocalPath == "/maxmedicamentos/medicamentos")
            {
                if (context.Request.HttpMethod == "GET")
                {
                    var info = JsonConvert.SerializeObject(catalogoMedicamentos.medicamentos);
                    byte[] buffer = Encoding.UTF8.GetBytes(info);
 
                    context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                    context.Response.StatusCode = 200;
                }
                else
                {
                    if (context.Request.ContentType.StartsWith("application/json") && context.Request.ContentLength64 > 0)
                    {
                        StreamReader reader = new StreamReader(context.Request.InputStream);
                        string info = reader.ReadToEnd();
                        var medicamento = JsonConvert.DeserializeObject<Medicamento>(info);
                        dispatcher.Invoke(new Action(() =>
                            {
                                if (context.Request.HttpMethod == "POST")
                                {
                                    catalogoMedicamentos.AddMedicamento(medicamento);
                                }
                                else if (context.Request.HttpMethod == "PUT")
                                {
                                    catalogoMedicamentos.EditMedicamento(medicamento);
                                }
                                else if (context.Request.HttpMethod == "DELETE")
                                {
                                    catalogoMedicamentos.DeleteMedicamento(medicamento);
                                }
                            }
                            ));
                        context.Response.StatusCode = 200;
                    }
                    else
                        context.Response.StatusCode = 400;
                }
            }
            else
            {
                context.Response.StatusCode = 404;
            }
            context.Response.Close();
        }
    }
}
