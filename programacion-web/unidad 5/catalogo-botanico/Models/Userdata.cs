using System;
using System.Collections.Generic;

namespace CatalogoBotanico.Models
{
    public partial class Userdata
    {
        public Userdata()
        {
            Plantdata = new HashSet<Plantdata>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Bio { get; set; }
        public ulong Active { get; set; }
        public int Code { get; set; }

        public virtual ICollection<Plantdata> Plantdata { get; set; }
    }
}
