using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media;
using System.Collections.Generic;

namespace Gato
{
    public enum Ficha { O, X }
    public enum Comando { UsuarioEnviado, JugadaEnviada }
    public class JuegoGato : INotifyPropertyChanged
    {
        public string NombreJugador1 { get; set; } = "Jugador";
        public string NombreJugador2 { get; set; }

        public string IP { get; set; } = "localhost";
        public bool VentanaPrincipalVisible { get; set; } = true;
        public byte PuntosJugador1 { get; set; }
        public byte PuntosJugador2 { get; set; }

        public Ficha? FichaJugador1 { get; set; } = Ficha.X;
        public Ficha? FichaJugador2 { get; set; } = Ficha.O;

        public string Mensaje { get; set; }

        private bool puedeJugar;
        public bool PuedeJugar
        {
            get { return puedeJugar; }
            set
            {
                puedeJugar = value;
                Actualizar("PuedeJugar");
            }
        }

        public ICommand JugarCommand { get; set; }
        public ICommand ConfirmarCommand { get; set; }
        public ICommand VaciarCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        Juego juego;
        Lobby lobby;
        HttpListener servidor;
        ClientWebSocket cliente;
        Dispatcher currentDispatcher;
        WebSocket webSocket;
        public JuegoGato()
        {
            currentDispatcher = Dispatcher.CurrentDispatcher;
            ConfirmarCommand = new RelayCommand<bool>(IniciarPartida);
            JugarCommand = new RelayCommand<string>(Jugar);

        }

        private void Jugar(string obj)
        {
            string casilla = juego.txtCasilla.Text.ToUpper();
            if (cliente != null)
            {
                ComprobarSeleccion(casilla, FichaJugador2);
                EnviarComando(new DatoEnviado { Comando = Comando.JugadaEnviada, Dato = casilla });
            }
            else
            {
                ComprobarSeleccion(casilla, FichaJugador1);
                EnviarComando(new DatoEnviado { Comando = Comando.JugadaEnviada, Dato = casilla });
            }
            PuedeJugar = false;
            CambioMensaje("Esperando la jugada del contrincante.");
            _ = VerificarGanadorAsync();
        }

        public void ComprobarSeleccion(string casilla, Ficha? ficha)
        {
            switch (casilla)
            {
                case "A1":
                    juego.txtA1.Text = ficha.ToString();
                    CambioMensaje("Esperando la jugada del otro jugador.");
                    break;
                case "A2":
                    juego.txtA2.Text = ficha.ToString();
                    CambioMensaje("Esperando la jugada del otro jugador.");
                    break;
                case "A3":
                    juego.txtA3.Text = ficha.ToString();
                    CambioMensaje("Esperando la jugada del otro jugador.");
                    break;

                case "B1":
                    juego.txtB1.Text = ficha.ToString();
                    CambioMensaje("Esperando la jugada del otro jugador.");
                    break;
                case "B2":
                    juego.txtB2.Text = ficha.ToString();
                    CambioMensaje("Esperando la jugada del otro jugador.");
                    break;
                case "B3":
                    juego.txtB3.Text = ficha.ToString();
                    CambioMensaje("Esperando la jugada del otro jugador.");
                    break;

                case "C1":
                    juego.txtC1.Text = ficha.ToString();
                    CambioMensaje("Esperando la jugada del otro jugador.");
                    break;
                case "C2":
                    juego.txtC2.Text = ficha.ToString();
                    CambioMensaje("Esperando la jugada del otro jugador.");
                    break;
                case "C3":
                    juego.txtC3.Text = ficha.ToString();
                    CambioMensaje("Esperando la jugada del otro jugador.");
                    break;
            }
        }

        private void Lobby_Closing(object sender, CancelEventArgs e)
        {
            VentanaPrincipalVisible = true;
            Actualizar("VentanaPrincipalVisible");
            if (servidor != null)
            {
                servidor.Stop();
                servidor = null;
            }
        }
        private async void IniciarPartida(bool partida)
        {
            try
            {
                VentanaPrincipalVisible = false;
                lobby = new Lobby();
                lobby.Closing += Lobby_Closing;
                lobby.DataContext = this;
                lobby.Show();
                Actualizar();
                if (partida == true)
                {
                    CrearPartida();
                }
                else
                {
                    NombreJugador2 = NombreJugador1;
                    NombreJugador1 = null;
                    Mensaje = "Intentando conectar con el servidor en " + IP;
                    Actualizar("Mensaje");
                    await ConectarPartida();
                }
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                Actualizar();
            }
        }
        public void CrearPartida()
        {
            servidor = new HttpListener();
            servidor.Prefixes.Add("http://*:1000/gato/");
            servidor.Start();
            servidor.BeginGetContext(OnContext, null);
            Mensaje = "Esperando a conectar con un contrincante...";
            Actualizar();
        }
        public async Task ConectarPartida()
        {
            cliente = new ClientWebSocket();
            await cliente.ConnectAsync(new Uri($"ws://{IP}:1000/gato/"), CancellationToken.None);
            webSocket = cliente;
            RecibirComando();
        }
        private async void OnContext(IAsyncResult ar)
        {
            var context = servidor.EndGetContext(ar);
            if (context.Request.IsWebSocketRequest)
            {
                var listener = await context.AcceptWebSocketAsync(null);
                webSocket = listener.WebSocket;
                CambioMensaje("Cliente aceptado. Esperando la información del contrincante.");
                EnviarComando(new DatoEnviado { Comando = Comando.UsuarioEnviado, Dato = NombreJugador1 });
                RecibirComando();
            }
            else
            {
                context.Response.StatusCode = 404;
                context.Response.Close();
                servidor.BeginGetContext(OnContext, null);
            }
        }
        private async void EnviarComando(DatoEnviado datos)
        {
            byte[] buffer;
            var json = JsonConvert.SerializeObject(datos);
            buffer = Encoding.UTF8.GetBytes(json);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        private async void RecibirComando()
        {
            try
            {
                byte[] buffer = new byte[1024];
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        juego.Close();
                        return;
                    }
                    string datosRecibidos = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var comando = JsonConvert.DeserializeObject<DatoEnviado>(datosRecibidos);
                    // Cliente
                    if (cliente != null)
                    {
                        switch (comando.Comando)
                        {
                            case Comando.UsuarioEnviado:
                                NombreJugador1 = (string)comando.Dato;
                                CambioMensaje("Conectando con el jugador " + NombreJugador1);
                                _ = currentDispatcher.BeginInvoke(new Action(() =>
                                {
                                    EnviarComando(new DatoEnviado { Comando = Comando.UsuarioEnviado, Dato = NombreJugador2 });
                                    lobby.Hide();
                                    juego = new Juego();
                                    juego.Title = "Cliente";
                                    juego.DataContext = this;
                                    CambioMensaje("Escriba el renglón y la columna donde poner su símbolo.");
                                    juego.ShowDialog();
                                    lobby.Show();
                                }));
                                break;
                            case Comando.JugadaEnviada:
                                currentDispatcher.Invoke(new Action(() =>
                                {
                                    ComprobarSeleccion(comando.Dato.ToString(), FichaJugador1);
                                    CambioMensaje($"{NombreJugador1} ha hecho su movimiento.");
                                    PuedeJugar = true;
                                }));
                                _ = VerificarGanadorAsync();
                                break;
                        }
                    }
                    // Servidor
                    else
                    {
                        switch (comando.Comando)
                        {
                            case Comando.UsuarioEnviado:
                                NombreJugador2 = (string)comando.Dato;
                                CambioMensaje("Conectando con el jugador " + NombreJugador2);
                                _ = currentDispatcher.BeginInvoke(new Action(() =>
                                {
                                    lobby.Hide();
                                    juego = new Juego();
                                    juego.Title = "Servidor";
                                    juego.DataContext = this;
                                    PuedeJugar = true;
                                    CambioMensaje("Escriba el renglón y la columna donde poner su símbolo.");
                                    juego.ShowDialog();
                                    lobby.Show();
                                }));
                                break;
                            case Comando.JugadaEnviada:
                                currentDispatcher.Invoke(new Action(() =>
                                {
                                    ComprobarSeleccion(comando.Dato.ToString(), FichaJugador2);
                                    CambioMensaje($"{NombreJugador2} ha hecho su movimiento.");
                                    PuedeJugar = true;
                                }));
                                _ = VerificarGanadorAsync();
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (webSocket.State == WebSocketState.Aborted)
                {
                    juego.Close();
                    lobby.Close();
                    VentanaPrincipalVisible = true;
                    Actualizar("MainWindowVisible");
                }
                else
                    CambioMensaje(ex.Message);
            }

        }

        async Task VerificarGanadorAsync()
        {
            bool ganaJugador1 =
                (
                (juego.txtA1.Text == "X" && juego.txtA2.Text == "X" && juego.txtA3.Text == "X")
                || (juego.txtA1.Text == "X" && juego.txtB1.Text == "X" && juego.txtC1.Text == "X")
                || (juego.txtA1.Text == "X" && juego.txtB2.Text == "X" && juego.txtC3.Text == "X")
                || (juego.txtB1.Text == "X" && juego.txtB2.Text == "X" && juego.txtB3.Text == "X")
                || (juego.txtC1.Text == "X" && juego.txtB2.Text == "X" && juego.txtA3.Text == "X")
                || (juego.txtC1.Text == "X" && juego.txtB2.Text == "X" && juego.txtA3.Text == "X")
                || (juego.txtA2.Text == "X" && juego.txtB2.Text == "X" && juego.txtC2.Text == "X")
                || (juego.txtA1.Text == "X" && juego.txtA2.Text == "X" && juego.txtA3.Text == "X")
                );
            bool ganaJugador2 =
                (
                (juego.txtA1.Text == "O" && juego.txtA2.Text == "O" && juego.txtA3.Text == "O")
                || (juego.txtA1.Text == "O" && juego.txtB1.Text == "O" && juego.txtC1.Text == "O")
                || (juego.txtA1.Text == "O" && juego.txtB2.Text == "O" && juego.txtC3.Text == "O")
                || (juego.txtB1.Text == "O" && juego.txtB2.Text == "O" && juego.txtB3.Text == "O")
                || (juego.txtC1.Text == "O" && juego.txtB2.Text == "O" && juego.txtA3.Text == "O")
                || (juego.txtC1.Text == "O" && juego.txtB2.Text == "O" && juego.txtA3.Text == "O")
                || (juego.txtA2.Text == "O" && juego.txtB2.Text == "O" && juego.txtC2.Text == "O")
                || (juego.txtA1.Text == "O" && juego.txtA2.Text == "O" && juego.txtA3.Text == "O")
                );
            bool Empate =
                (
                (juego.txtA1.Text != "" &&
                juego.txtA2.Text != "" &&
                juego.txtA3.Text != "" &&
                juego.txtB1.Text != "" &&
                juego.txtB2.Text != "" &&
                juego.txtB3.Text != "" &&
                juego.txtC1.Text != "" &&
                juego.txtC2.Text != "" &&
                juego.txtC3.Text != "")
                );

            if (ganaJugador1)
            {
                CambioMensaje($"GANÓ {NombreJugador1}");
                PuntosJugador1++;

                if (PuntosJugador1 < 3 && PuntosJugador2 < 3)
                {
                    await Task.Delay(3000);
                    CambioMensaje("Reiniciando el tablero...");
                    await Task.Delay(3000);
                    juego.txtA1.Clear();
                    juego.txtA2.Clear();
                    juego.txtA3.Clear();
                    juego.txtB1.Clear();
                    juego.txtB2.Clear();
                    juego.txtB3.Clear();
                    juego.txtC1.Clear();
                    juego.txtC2.Clear();
                    juego.txtC3.Clear();
                    PuedeJugar = true;
                    CambioMensaje("Escriba el renglón y la columna donde poner su símbolo.");
                }
                else
                {
                    await Task.Delay(3000);
                    CambioMensaje($"El juego ha terminado. Ganó {((PuntosJugador1 > PuntosJugador2) ? NombreJugador1 : NombreJugador2)}");
                    juego.Close();
                }
            }
            else if (ganaJugador2)
            {
                CambioMensaje($"GANÓ {NombreJugador2}");
                PuntosJugador2++;

                if (PuntosJugador1 < 3 && PuntosJugador2 < 3)
                {
                    await Task.Delay(3000);
                    CambioMensaje("Reiniciando el tablero...");
                    await Task.Delay(3000);
                    juego.txtA1.Clear();
                    juego.txtA2.Clear();
                    juego.txtA3.Clear();
                    juego.txtB1.Clear();
                    juego.txtB2.Clear();
                    juego.txtB3.Clear();
                    juego.txtC1.Clear();
                    juego.txtC2.Clear();
                    juego.txtC3.Clear();
                    PuedeJugar = true;
                    CambioMensaje("Escriba el renglón y la columna donde poner su símbolo.");
                }
                else
                {
                    await Task.Delay(3000);
                    CambioMensaje($"El juego ha terminado. Ganó {((PuntosJugador1 > PuntosJugador2) ? NombreJugador1 : NombreJugador2)}");
                    juego.Close();
                }
            }
            else if (Empate)
            {
                CambioMensaje("EMPATE.");
                if (PuntosJugador1 < 3 && PuntosJugador2 < 3)
                {
                    await Task.Delay(3000);
                    CambioMensaje("Reiniciando el tablero...");
                    await Task.Delay(3000);
                    juego.txtA1.Clear();
                    juego.txtA2.Clear();
                    juego.txtA3.Clear();
                    juego.txtB1.Clear();
                    juego.txtB2.Clear();
                    juego.txtB3.Clear();
                    juego.txtC1.Clear();
                    juego.txtC2.Clear();
                    juego.txtC3.Clear();
                    PuedeJugar = true;
                    CambioMensaje("Escriba el renglón y la columna donde poner su símbolo.");
                }
                else
                {
                    await Task.Delay(3000);
                    CambioMensaje($"El juego ha terminado. Ganó {((PuntosJugador1 > PuntosJugador2) ? NombreJugador1 : NombreJugador2)}");
                    juego.Close();
                }
            }
        }

        void CambioMensaje(string mensaje)
        {
            currentDispatcher.Invoke(new Action(() =>
            {
                Mensaje = mensaje;
                Actualizar();
            }));
        }
        void Actualizar(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public class DatoEnviado
        {
            public Comando Comando { get; set; }
            public object Dato { get; set; }
        }
    }
}