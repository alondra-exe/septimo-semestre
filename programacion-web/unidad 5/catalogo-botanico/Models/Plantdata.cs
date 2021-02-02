using System;
using System.Collections.Generic;

namespace CatalogoBotanico.Models
{
    public partial class Plantdata
    {
        public int Id { get; set; }
        public string CommonName { get; set; }
        public string ScientificName { get; set; }
        public string Division { get; set; }
        public string Family { get; set; }
        public string Subfamily { get; set; }
        public string Gender { get; set; }
        public string Info { get; set; }
        public int IdUser { get; set; }

        public virtual Userdata IdUserNavigation { get; set; }
    }
}
