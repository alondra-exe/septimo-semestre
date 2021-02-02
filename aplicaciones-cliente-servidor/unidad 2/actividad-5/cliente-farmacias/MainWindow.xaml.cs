using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Actividad5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DatosFarmacia datos = new DatosFarmacia();
        ClienteFarmacia cliente = new ClienteFarmacia();
        DispatcherTimer timer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = datos;
            cliente.Get();
            cliente.AlInvokar += Cliente_AlInvokar;
            cmbStock.Items.Add("DISPONIBLE");
            cmbStock.Items.Add("NO DISPONIBLE");
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += Timer_Tick;
            timer.Start();
            Editar.IsEnabled = false;
            Eliminar.IsEnabled = false;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            cliente.Get();
        }

        private void Cliente_AlInvokar()
        {
            dtgDatos.ItemsSource = cliente.model;
        }

        private void Agregar_Click(object sender, RoutedEventArgs e)
        {
            if (txtNombre.Text != "" || txtCientifico.Text != "" || txtContenido.Text != "" || txtPrecio.Text != "")
            {
                try
                {
                    datos = new DatosFarmacia();
                    datos.Nombre = txtNombre.Text;
                    datos.NombreComercial = txtCientifico.Text;
                    datos.Precio = txtPrecio.Text;
                    datos.Contenido = txtContenido.Text;
                    datos.Stock = cmbStock.Text;
                    cliente.Agregar(datos);
                    timer.Start();
                    txtNombre.Clear();
                    txtCientifico.Clear();
                    txtPrecio.Clear();
                    txtContenido.Clear();
                    cmbStock.Text = "";
                    Editar.IsEnabled = false;
                    Eliminar.IsEnabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Llene los campos para agregar un nuevo medicamento");
            }
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            if (dtgDatos.SelectedIndex != -1)
            {
                try
                {
                    datos.Nombre = txtNombre.Text;
                    datos.NombreComercial = txtCientifico.Text;
                    datos.Precio = txtPrecio.Text;
                    datos.Contenido = txtContenido.Text;
                    datos.Stock = cmbStock.Text;
                    cliente.Editar(datos);
                    cliente.Get();
                    timer.Start();
                    txtNombre.Clear();
                    txtCientifico.Clear();
                    txtPrecio.Clear();
                    txtContenido.Clear();
                    cmbStock.Text = "";
                    dtgDatos.SelectedItem = null;
                    Editar.IsEnabled = false;
                    Eliminar.IsEnabled = false;
                    txtNombre.IsEnabled = txtCientifico.IsEnabled = txtContenido.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                timer.Start();
            }
            else
            {
                MessageBox.Show("Es necesario elegir un elemento para poder editarlo.");
            }
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dtgDatos.SelectedIndex != -1)
            {
                try
                {
                    DatosFarmacia datosEliminar = dtgDatos.SelectedItem as DatosFarmacia;
                    cliente.Eliminar(datosEliminar);
                    timer.Start();
                    txtNombre.Clear();
                    txtCientifico.Clear();
                    txtPrecio.Clear();
                    txtContenido.Clear();
                    cmbStock.Text = "";
                    dtgDatos.SelectedItem = null;
                    Editar.IsEnabled = false;
                    Eliminar.IsEnabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                timer.Start();
            }
            else
            {
                MessageBox.Show("Es necesario elegir un elemento para poder eliminarlo.");
            }
        }

        private void dtgDatos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtgDatos.SelectedItem != null)
            {
                Editar.IsEnabled = true;
                Eliminar.IsEnabled = true;
                timer.Stop();
                datos = dtgDatos.SelectedItem as DatosFarmacia;
                txtNombre.Text= datos.Nombre;
                txtCientifico.Text= datos.NombreComercial;
                txtPrecio.Text= datos.Precio;
                txtContenido.Text= datos.Contenido;
                cmbStock.Text= datos.Stock;
                txtNombre.IsEnabled = txtCientifico.IsEnabled = txtContenido.IsEnabled = false;
            }
        }
    }
}
