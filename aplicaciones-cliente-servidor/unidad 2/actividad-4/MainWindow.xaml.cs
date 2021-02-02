using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Newtonsoft.Json;

namespace ClienteVuelo
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DatosVuelos datos = new DatosVuelos();
        DatosVuelos datosEditar = new DatosVuelos();
        ClienteVuelos cliente = new ClienteVuelos();
        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = datos;
            cliente.AlActualizar += Cliente_AlActualizar;
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += Timer_Tick;
            timer.Start();
            cmbEstado.Items.Add("A TIEMPO");
            cmbEstado.Items.Add("ABORDANDO");
            cmbEstado.Items.Add("CANCELADO");
            cmbEstado.Items.Add("RETRASADO");
            btnTerminar.IsEnabled = false;
            txtHora.IsEnabled = false;
            txtVuelo.IsEnabled = false;
            txtDestino.IsEnabled = false;
            cmbEstado.IsEnabled = false;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            cliente.Get();
        }

        private void Cliente_AlActualizar()
        {
            dgVuelos.ItemsSource = cliente.Datos;
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.DataContext = datos;
                btnTerminar.IsEnabled = true;
                txtHora.IsEnabled = true;
                txtVuelo.IsEnabled = true;
                txtDestino.IsEnabled = true;
                cmbEstado.IsEnabled = true;
                txtHora.Clear();
                txtDestino.Clear();
                txtVuelo.Clear();
                cmbEstado.Text = "";
                btnEditar.IsEnabled = false;
                btnEliminar.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnTerminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cliente.Agregar(datos);
                btnTerminar.IsEnabled = false;
                txtHora.Clear();
                txtDestino.Clear();
                txtVuelo.Clear();
                cmbEstado.Text = "";
                btnEliminar.IsEnabled = true;
                btnEditar.IsEnabled = true;
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgVuelos.SelectedIndex != -1)
            {
                try
                {
                    datos = dgVuelos.SelectedItem as DatosVuelos;
                    if (MessageBox.Show($"¿Desea eliminar el vuelo {datos.Vuelo} " +
                        $"con destino a {datos.Destino}?", "¡Atención!",
                        MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) ==
                        MessageBoxResult.OK)
                    {
                        cliente.Eliminar(datos);
                        txtHora.Clear();
                        txtDestino.Clear();
                        txtVuelo.Clear();
                        cmbEstado.Text = "";
                        cliente.Get();
                        timer.Start();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Seleccione el vuelo que desee eliminar.", "¡Atención!",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgVuelos.SelectedIndex != -1)
            {
                try
                {
                    datosEditar.Hora = txtHora.Text;
                    datosEditar.Destino = txtDestino.Text;
                    datosEditar.Vuelo = txtVuelo.Text;
                    datosEditar.Estado = cmbEstado.Text;
                    cliente.Editar(datosEditar);
                    cliente.Get();
                    timer.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
                MessageBox.Show("Necesita seleccionar un vuelo para editarlo.",
                    "¡Atención!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void dgVuelos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgVuelos.SelectedItem != null)
            {
                timer.Stop();
                btnEditar.IsEnabled = true;
                datosEditar = dgVuelos.SelectedItem as DatosVuelos;
                txtHora.Text = datosEditar.Hora;
                txtDestino.Text = datosEditar.Destino;
                txtVuelo.Text = datosEditar.Vuelo;
                cmbEstado.Text = datosEditar.Estado;
                txtHora.IsEnabled = false;
                txtVuelo.IsEnabled = false;
                btnTerminar.IsEnabled = false;
                txtDestino.IsEnabled = true;
                cmbEstado.IsEnabled = true;
                btnEliminar.IsEnabled = true;
            }
        }
    }
}
