using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unidad2Actividad3
{
    public class Puntos : INotifyPropertyChanged
    {
        private int puntosU;

        public int PuntosU
        {
            get { return puntosU; }
            set
            {
                puntosU = value;
                OnPropertyChanged("PuntosU");
            }
        }


        private string equipoU;
        public string EquipoU
        {
            get { return equipoU; }
            set
            {
                equipoU = value;
                OnPropertyChanged("EquipoU");
            }
        }

        private string colorU;
        public string ColorU
        {
            get { return colorU; }
            set
            {
                colorU = value;
                OnPropertyChanged("ColorU");
            }
        }

        //

        private int puntosD;

        public int PuntosD
        {
            get { return puntosD; }
            set
            {
                puntosD = value;
                OnPropertyChanged("PuntosD");
            }
        }

        private string equipoD;
        public string EquipoD
        {
            get { return equipoD; }
            set
            {
                equipoD = value;
                OnPropertyChanged("EquipoD");
            }
        }

        private string colorD;
        public string ColorD
        {
            get { return colorD; }
            set
            {
                colorD = value;
                OnPropertyChanged("ColorD");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propiedad)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
        }
    }
}