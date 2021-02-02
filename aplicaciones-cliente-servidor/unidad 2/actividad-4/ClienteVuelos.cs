using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClienteVuelo
{
    public class ClienteVuelos
    {
        HttpClient cliente = new HttpClient();
        public IEnumerable<DatosVuelos> Datos { get; set; }
        public ClienteVuelos()
        {
            cliente.BaseAddress = new Uri("http://vuelos.itesrc.net/");
        }

        public delegate void actualizar();
        public event actualizar AlActualizar;

        // Agregar vuelo
        public async void Agregar(DatosVuelos v)
        {
            if (v.Estado == "A TIEMPO" ||
                v.Estado == "ABORDANDO" ||
                v.Estado == "CANCELADO" ||
                v.Estado == "RETRASADO")
            {
                var json = JsonConvert.SerializeObject(v);
                var result = await cliente.PostAsync("/Tablero",
                    new StringContent(json, Encoding.UTF8, "application/json"));
                result.EnsureSuccessStatusCode();
            }
            else
                MessageBox.Show("Seleccione un estado válido.");
        }
        
        // Editar vuelo
        public async void Editar(DatosVuelos v)
        {
            var json = JsonConvert.SerializeObject(v);
            var result = await cliente.PutAsync("/Tablero",
                new StringContent(json, Encoding.UTF8, "application/json"));
            result.EnsureSuccessStatusCode();
        }

        // Eliminar vuelo
        public async void Eliminar(DatosVuelos v)
        {
            var json = JsonConvert.SerializeObject(v);
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Delete, "/Tablero");
            message.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await cliente.SendAsync(message);
            result.EnsureSuccessStatusCode();
        }

        // GET
        public async void Get()
        {
            var cliente = new HttpClient();
            var respuesta = await cliente.GetAsync("http://vuelos.itesrc.net/Tablero");
            if (respuesta.IsSuccessStatusCode)
            {
                var jsonString = await respuesta.Content.ReadAsStringAsync();
                Datos = JsonConvert.DeserializeObject<IEnumerable<DatosVuelos>>
                    (jsonString);
                AlActualizar?.Invoke();
            }
        }
    }
}
