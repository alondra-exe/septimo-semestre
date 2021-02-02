using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace CatalogoBotanico.Models.ViewModels
{
    public class PlantViewModel
    {
        public Plantdata Plant { get; set; }
        public Userdata User { get; set; }
        public IEnumerable<Userdata> Users { get; set; }
        public IFormFile Archivo { get; set; }
        public string Image { get; set; }
    }
}