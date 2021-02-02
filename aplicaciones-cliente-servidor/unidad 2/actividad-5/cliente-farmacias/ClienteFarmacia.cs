using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Actividad5
{
    public class ClienteFarmacia
    {
        HttpClient cliente = new HttpClient();
        public ClienteFarmacia()
        {
            cliente.BaseAddress = new Uri("http://localhost:8080/maxmedicamentos");
        }
        public async void Agregar(DatosFarmacia medicamentos)
        {
            var json = JsonConvert.SerializeObject(medicamentos);
            var result = await cliente.PostAsync("/maxmedicamentos/medicamentos", new StringContent(json, Encoding.UTF8, "application/json"));
            result.EnsureSuccessStatusCode();
        }
        public async void Editar(DatosFarmacia medicamentos)
        {
            var json = JsonConvert.SerializeObject(medicamentos);
            var result = await cliente.PutAsync("/maxmedicamentos/medicamentos", new StringContent(json, Encoding.UTF8, "application/json"));
            result.EnsureSuccessStatusCode();
        }
        public async void Eliminar(DatosFarmacia medicamentos)
        {
            var json = JsonConvert.SerializeObject(medicamentos);
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Delete, "/maxmedicamentos/medicamentos");
            message.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await cliente.SendAsync(message);
            result.EnsureSuccessStatusCode();
        }

        public delegate void Invokar();
        public event Invokar AlInvokar;
        public IEnumerable<DatosFarmacia> model { get; set; }
        public async void Get()
        {
            var client = new HttpClient();
            var result = await client.GetAsync("http://localhost:8080/maxmedicamentos/medicamentos");
            if (result.IsSuccessStatusCode)
            {
                var jsonString = await result.Content.ReadAsStringAsync();
                model = JsonConvert.DeserializeObject<IEnumerable<DatosFarmacia>>(jsonString);
                AlInvokar?.Invoke();
            }
        }
    }
}
