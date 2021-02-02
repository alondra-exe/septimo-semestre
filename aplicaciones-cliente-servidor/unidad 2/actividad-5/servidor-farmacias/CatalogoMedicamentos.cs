using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace ServidorFarmacias
{
    public class CatalogoMedicamentos
    {
        public ObservableCollection<Medicamento> medicamentos { get; set; } = new ObservableCollection<Medicamento>();

        private void SaveMedicamento()
        {
            string info = JsonConvert.SerializeObject(medicamentos);
            File.WriteAllText("medicamentos.json", info);
        }
        public void AddMedicamento(Medicamento m)
        {
            medicamentos.Add(m);
            SaveMedicamento();
        }
        public void EditMedicamento(Medicamento m)
        {
            var medicamento = medicamentos.FirstOrDefault(a => a.NombreComercial == m.NombreComercial);
            if (medicamento != null)
            {
                medicamento.Precio = m.Precio;
                medicamento.Stock = m.Stock;
                SaveMedicamento();
            }
        }
        public void DeleteMedicamento(Medicamento m)
        {
            var medicamento = medicamentos.FirstOrDefault(a => a.NombreComercial == m.NombreComercial);
            if (medicamento != null)
            {
                medicamentos.Remove(medicamento);
                SaveMedicamento();
            }
        }
        private void LoadMedicamento()
        {
            if (File.Exists("medicamentos.json"))
            {
                var carga = JsonConvert.DeserializeObject<ObservableCollection<Medicamento>>(File.ReadAllText("medicamentos.json"));
                foreach (var m in carga)
                {
                    medicamentos.Add(m);
                }
            }
        }

        public CatalogoMedicamentos()
        {
            LoadMedicamento();
        }
    }
}
