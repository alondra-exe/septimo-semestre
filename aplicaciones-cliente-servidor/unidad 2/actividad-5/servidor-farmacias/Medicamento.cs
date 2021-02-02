using System.ComponentModel;

namespace ServidorFarmacias
{
    public class Medicamento : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void Actualizar(string actualizar)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(actualizar));
        }

        private string nombre;
        private string nombreComercial;
        private string contenido;
        private string precio;
        private string stock;

        public string Nombre
        {
            get => nombre;
            set
            {
                nombre = value;
                Actualizar("Nombre");
            }
        }
        
        public string NombreComercial
        {
            get => nombreComercial;
            set
            {
                nombreComercial = value;
                Actualizar("NombreComercial");
            }
        }

        public string Contenido
        {
            get => contenido;
            set
            {
                contenido = value;
                Actualizar("Contenido");
            }
        }

        public string Precio
        {
            get => precio;
            set
            {
                precio = value;
                Actualizar("Precio");
            }
        }

        public string Stock
        {
            get => stock;
            set
            {
                stock = value;
                Actualizar("Stock");
            }
        }
    }
}