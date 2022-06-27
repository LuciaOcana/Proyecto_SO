using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Media;

namespace cliente
{
    public partial class Parchís : Form
    {

        Socket server;
        Thread Atender;


        //VARIABLES CARACTERISTICAS PARTIDA
        string[] nameJugadores = new string[4]; //Vector con los nombres de los 4 jugadores 
        int jugar = 0;
        int jugadores = 0;
        int idPartida;
        int numJugadoresP = 0;
        int musica;
        string tablero;
        string fichacolor;
        int jugador;
        int casillasFichas;

        //VARIABLES MENSAJES
        string autorMensaje;
        string mensajeChat;

        //Vectorcontador
        int[] contadorAmarillo = new int[4];
        int[] contadorVerde = new int[4];
        int[] contadorNegro = new int[4];
        int[] contadorAzul = new int[4];

        //VARIABLES JUEGO
        //valor del dado
        int valorDado;
       
        
        //Almacenamos el numero de tiradas de cada jugador
        int[] tirada = new int[4];

        //VARIABLES FICHAS
        int tirarFicha;
        //Guardamos las posiciones de las fichas del J1
        int[] posicionFichasJ1 = new int[4];

        //VARIABLES TURNOS
        int turnoActual;
        int turnoJ = 0;

        //VARIABLES CASILLAS CASA
        //Flag para entrar a casa
        int[] AzulFlag = new int[4];
        int[] NegraFlag = new int[4];
        int[] AmarillaFlag = new int[4];
        int[] VerdeFlag = new int[4];
        //Contamos las fichas que ya han entrado a casa
        // TABLERO SW
        int contCAzul = 0;
        int contCAmarillo = 0;
        int contCVerde = 0;
        int contCNegro = 0;
        //
        int[] casa = new int[4];
        int casillaCasa;
        
        //Este int se encarga de comprobar si el jugador ha iniciado la partida, es decir, si ha sacado ficha con dado=5.
        int[] partidaEmpezada = new int[4];
        
        int casilla = 0;

        //Indicamos si no hay mas fichas en casa
        int[] casaFin = new int[4];

        int sumaFCasa;

        //VARIABLES CASILLAS
        int[] IoD = new int[68];    //si [i] = 1->izquierda, 2->derecha, 3->lleno
        int[] casillaSalida = new int[4];
        int casillasComprobar;
        int comprobarAnterior;
        //almacenamos la informacion de la casilla en un vector de int (casa posicion para cada casilla de la ficha)
        int[] casillaJ1 = new int[4];
        int[] casillaJ2 = new int[4];
        int[] casillaJ3 = new int[4];
        int[] casillaJ4 = new int[4];

        //VARIABLES FICHAS
        string colorFicha5;

        //fichas de los jugadores (4 fichas (en formato imagen) por jugador).
        PictureBox[] J1 = new PictureBox[4];
        PictureBox[] J2 = new PictureBox[4];
        PictureBox[] J3 = new PictureBox[4];
        PictureBox[] J4 = new PictureBox[4];

        //Vector de fichas de cada jugador
        int[] fichaJ1 = new int[4];
        int[] fichaJ2 = new int[4];
        int[] fichaJ3 = new int[4];
        int[] fichaJ4 = new int[4];

        //VARIABLES CASILLAS
        //Casillas ocupadas
        int[] casillasOcupadas = new int[68];

        string[] colorFichaCasilla = new string[68];

        //Casillas de las casas
        List<Point> casillasCasa = new List<Point>()
        {
            //lista de todas las casillas del tablero. Se escribiran las posiciones una vez se selecciones el tablero.
        };

        //LISTA POSICIONES CASILLAS
        //hay que sumar x= 364 e y=61 para cuadrar la posición
        List<Point> casillas = new List<Point>()
        {
            //lista de todas las casillas del tablero. Se escribiran las posiciones una vez se selecciones el tablero.
        };


        //------------------------------------------------------------------------------------------------------------------------
        delegate void DelegadorParaEscribir(string mensaje);
        delegate void DelegadoGB(GroupBox mensaje);

        // Puerto Carla : 50015
        // Puerto Marta : 50016
        // Puerto Lucia : 50017
        int puerto = 9900;
        public Parchís()
        {
            InitializeComponent();
            //Es necesario para que los elementos de los formularios puedan ser accedidos desde threads diferentes a los que los crearon.
            CheckForIllegalCrossThreadCalls = false;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        //FUNCIÓN CARGAR FORMS
        private void Form1_Load(object sender, EventArgs e)
        {
            //------------------ FUNCIONES OCULTAS ------------------------------------
            //Boton conectar
            conectar.Visible = false;
            //Boton desconectar
            desconectar.Visible = false;
            //-------------------------------------------------------------------------

            registro.Visible = false;
            iniciar_sesion.Visible = true;
            //Hace grande la ventana de forms.
            int lx, ly;
            int sw, sh;

            lx = this.Location.X;
            ly = this.Location.Y;
            sw = this.Size.Width;
            sh = this.Size.Height;
            this.Size = Screen.PrimaryScreen.WorkingArea.Size;
            this.Location = Screen.PrimaryScreen.WorkingArea.Location;
            iniciar_sesion.Visible = true;
            peticiones.Visible = false;
            menuStrip_usuario.Visible = false;
            //Visibilidad chat inactiva hasta que no acepten la invitación
            groupBoxChat.Visible = false;
            TableroStarWars.Visible = false;
            groupBoxSeleccionTableros.Visible = false;
            EmpezarPartidaButton.Visible = false;
            DadoPixel.Visible = false;
            pictureBoxReglas.Visible = false;

            //Fichas SW
            FichaAmarilloSW1.Visible = false;
            FichaAmarilloSW2.Visible = false;
            FichaAmarilloSW3.Visible = false;
            FichaAmarilloSW4.Visible = false;
            FichaVerdeSW1.Visible = false;
            FichaVerdeSW2.Visible = false;
            FichaVerdeSW3.Visible = false;
            FichaVerdeSW4.Visible = false;
            FichaAzulSW1.Visible = false;
            FichaAzulSW2.Visible = false;
            FichaAzulSW3.Visible = false;
            FichaAzulSW4.Visible = false;
            FichaNegraSW1.Visible = false;
            FichaNegraSW2.Visible = false;
            FichaNegraSW3.Visible = false;
            FichaNegraSW4.Visible = false;
            //Visibilidad inactiva del turno hasta abrir el tablero
            pictureBox5.Visible = false;
            turno.Visible = false;
            

            

            //IP servidor
            IPAddress direc = IPAddress.Parse("192.168.56.101"); 
            
            //IP shiva:
            //IPAddress direc = IPAddress.Parse("147.83.117.22");
            IPEndPoint ipep = new IPEndPoint(direc, puerto);

            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
                this.BackColor = Color.LightGreen;
                //pongo en marcha el thread que atendera los mensajes del servidor
                ThreadStart ts = delegate { AtenderServidor(); };
                Atender = new Thread(ts);
                Atender.Start();
                MessageBox.Show("Se ha conectado con el servidor");

            }
            catch (SocketException)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }

        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        //FUNCIÓN ATENDER SERVIDOR
        private void AtenderServidor()
        {
            while (true)
            {
                //Recibimos mensaje del servidor
                byte[] msg2 = new byte[200];
                server.Receive(msg2);
                string a = Encoding.ASCII.GetString(msg2);

                // Limpiamos el mensaje
                string mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];

                string[] trozos = mensaje.Split('/');
                
                int codigo = Convert.ToInt32(trozos[0]);

                switch (codigo)
                {
                    case 0:
                        
                        Invoke(new Action(() =>
                            {
                                // DESCONEXIÓN DEL SERVIDOR
                                this.BackColor = Color.Gray;
                                server.Shutdown(SocketShutdown.Both);
                                server.Close();

                                MessageBox.Show("Te has desconectado del servidor, para volver a conectarte reinicia el programa o haz click en el boton conectarse");
                                conectar.Visible = true;
                                peticiones.Visible = false;
                                conectar.Location = new Point(175, 85);
                                
                            }));
                        break;
                    case 1:     //PARTIDAS GANADAS

                        int partidasganadas = Convert.ToInt32(trozos[1]);
                        MessageBox.Show(usuario_consulta.Text + " ha ganado " + partidasganadas + " partidas");

                        break;
                    case 2:     //TABLONES JUGADOS

                        string[] words1 = trozos[1].Split('-');
                        //a = words[1];
                        Int32.TryParse(words1[0], out int numValue);

                        if (numValue == 0)
                        {
                            MessageBox.Show(words1[1]);
                        }
                        else
                        {
                            MessageBox.Show(usuario_consulta.Text + " ha jugado los tablones: " + words1[1]);
                        }

                        break;
                    case 3:     //ID JUGADOR
                        int idjugador = Convert.ToInt32(trozos[1]);
                        MessageBox.Show("La ID de " + usuario_consulta.Text + " es: " + idjugador);
                        break;
                    case 4:     //REGISTRO

                        if (trozos[1] == "SI")
                        {
                            MessageBox.Show("¡Enhorabuena, ya estas registrado!");
                            Invoke(new Action(() =>
                            {

                                iniciar_sesion.Visible = true;
                                registro.Visible = false;
                            }));
                        }
                        else
                        {
                            MessageBox.Show("Lo sentimos el nombre de usuario ya se esta utilizando:( \n ¡Porfavor selecciona otro!");
                        }

                        break;
                    case 5:     //INICIO SESIÓN

                        Invoke(new Action(() =>
                        {

                            if (trozos[1] == "INCORRECTO")
                            {
                                MessageBox.Show("Nombre de usuario o contraseña incorrecta.");
                                this.BackColor = Color.LightCoral;
                            }

                            else //damos la bienvenida al juego
                            {
                                MessageBox.Show("bienvenid@ " + usuario_ini.Text);
                                this.BackColor = Color.Lavender;
                                iniciar_sesion.Visible = false;
                                registro.Visible = false;
                                peticiones.Visible = true;
                                PeticionesGroupBox.Visible = true;
                                title.Visible = false;
                                menuStrip_usuario.Visible = true;
                                holaToolStripMenuItem.Text = "Hola " + usuario_ini.Text;
                            }
                        }));
                        break;
                    case 6:     //LISTA CONECTADOS
                        Invoke(new Action(() =>
                        {
                            ListaConectadosDG.Rows.Clear();
                            int num = Convert.ToInt32(trozos[1]);
                            ListaConectadosDG.RowCount = num;
                            int n = 0;
                            for (int i = 0; i < num; i++)
                            {
                                ListaConectadosDG.Rows[n].Cells[0].Value = trozos[n + 2];
                                n = n + 1;
                            }
                            this.ListaConectadosDG.Rows[0].Cells[0].Selected = false;
                        }));
                        break;
                    case 7:     //SERVICIOS
                        contLbl.Text = trozos[1];
                        break;
                    case 8:     //DESCONEXION
                        Invoke(new Action(() =>
                        {
                            MessageBox.Show(trozos[1]);
                            iniciar_sesion.Visible = true;
                            usuario_ini.Clear();
                            contraseña_ini.Clear();
                            title.Visible = true;
                            registro.Visible = false;
                            peticiones.Visible = false;
                        }));
                        break;
                    case 9:      //INVITACION
                        var result = MessageBox.Show(trozos[1], "Invitacion", MessageBoxButtons.YesNo);
                        //Activamos la visibilidad del chat para los que hayan aceptado la invitacíón

                        Invoke(new Action(() =>
                        {
                            if (result == DialogResult.Yes)
                            {
                                // Enviamos al servidor el nombre del usuario (devuelve 10/nombre_Invitado/nombre_Invitador/Yes)       
                                string respuestaInvitacion = "10/" + usuario_ini.Text + "/" + trozos[2] + "/Yes";
                                byte[] msg = System.Text.Encoding.ASCII.GetBytes(respuestaInvitacion);
                                server.Send(msg);
                                //Ocultamos para esperar a que empiece la partida. 
                                PeticionesGroupBox.Visible = false;
                                jugar_button.Visible = false;
                                pictureBoxReglas.Visible = true;
                                pictureBoxReglas.Image = Properties.Resources.Reglas_del_parchís_invitados_1;
                            }
                            if (result == DialogResult.No)
                            {
                                string respuestaInvitacion = "10/" + usuario_ini.Text + "/" + trozos[2] + "/No";
                                byte[] msg = System.Text.Encoding.ASCII.GetBytes(respuestaInvitacion);
                                server.Send(msg);
                            }
                        }));
                        break;
                    case 10:      //RECEPCION INVITACION     
                        if (trozos[1] == "YES")
                        {
                            idPartida = Convert.ToInt32(trozos[2]);
                            EmpezarPartidaButton.Visible = true;

                            if (trozos.Length == 5) 
                            {
                                MessageBox.Show("Ha aceptado la invitación " + trozos[4], "invitacion", MessageBoxButtons.OK);
                                nameJugadores[0] = trozos[3];//El jugador que invita 
                                tirada[0] = 0;
                                casa[0] = 4;
                                casillaSalida[0] = 2;
                                nameJugadores[1] = trozos[4]; //Segundo Jugador
                                tirada[1] = 0;
                                casa[1] = 4;
                                casillaSalida[1] = 2;
                            }
                            if (trozos.Length == 6)
                            {
                                MessageBox.Show("Ha aceptado la invitación " + trozos[5], "invitacion", MessageBoxButtons.OK);
                                nameJugadores[2] = trozos[5]; //Tercer Jugador
                                tirada[2] = 0;
                                casa[2] = 4;
                                casillaSalida[2] = 2;

                            }
                            if (trozos.Length == 7)
                            {
                                MessageBox.Show("Ha aceptado la invitación " + trozos[6], "invitacion", MessageBoxButtons.OK);
                                nameJugadores[3] = trozos[6]; //Cuarto Jugador
                                tirada[3] = 0;
                                casa[3] = 4;
                                casillaSalida[3] = 2;
                            }
                        }
                        else
                        {
                            MessageBox.Show(trozos[2]);
                        }

                        this.ListaConectadosDG.Rows[0].Cells[0].Selected = false;
                        break;

                    case 11:     //MENSAJE
                        autorMensaje = trozos[2];
                        mensajeChat = trozos[3];
                        Chat.Items.Add(autorMensaje + ":" + mensajeChat);
                        break;
                    case 12:      //TABLEROS
                        tablero = trozos[2];
                        if (trozos.Length == 6) //Caso en el que juegan 2
                        {
                            nameJugadores[0] = trozos[4];//El jugador que invita 
                            tirada[0] = 0;
                            casa[0] = 4;
                            casillaSalida[0] = 2;
                            partidaEmpezada[0] = 0;

                            nameJugadores[1] = trozos[5]; //Segundo jugador
                            tirada[1] = 0;
                            casa[1] = 4;
                            casillaSalida[1] = 2;
                            //Numeros de jugadores de la partida.
                            numJugadoresP = Convert.ToInt32(trozos[3]);
                        }
                        if (trozos.Length == 7) //Caso en el que juegan 3
                        {
                            nameJugadores[0] = trozos[4];//El jugador que invita 
                            tirada[0] = 0;
                            casa[0] = 4;
                            casillaSalida[0] = 2;
                            partidaEmpezada[0] = 0;

                            nameJugadores[1] = trozos[5]; //Segundo jugador
                            tirada[1] = 0;
                            casa[1] = 4;
                            casillaSalida[1] = 2;
                            //Numeros de jugadores de la partida.
                            numJugadoresP = Convert.ToInt32(trozos[3]);

                            nameJugadores[2] = trozos[6]; //Tercer jugador
                            tirada[2] = 0;
                            casa[2] = 4;
                            casillaSalida[2] = 2;
                            numJugadoresP = Convert.ToInt32(trozos[3]);

                        }
                        if (trozos.Length == 8) //Caso en el que juegan 4
                        {
                            nameJugadores[0] = trozos[4];//El jugador que invita 
                            tirada[0] = 0;
                            casa[0] = 4;
                            casillaSalida[0] = 2;
                            partidaEmpezada[0] = 0;

                            nameJugadores[1] = trozos[5]; //Segundo jugador
                            tirada[1] = 0;
                            casa[1] = 4;
                            casillaSalida[1] = 2;
                            //Numeros de jugadores de la partida.
                            numJugadoresP = Convert.ToInt32(trozos[3]);

                            nameJugadores[2] = trozos[6]; //Tercer jugador
                            tirada[2] = 0;
                            casa[2] = 4;
                            casillaSalida[2] = 2;
                            numJugadoresP = Convert.ToInt32(trozos[3]);

                            nameJugadores[3] = trozos[7]; //Cuarto jugador
                            tirada[3] = 0;
                            casa[3] = 4;
                            casillaSalida[3] = 2;
                            numJugadoresP = Convert.ToInt32(trozos[3]);
                        }

                        if (tablero == "SW")
                        {
                           Invoke(new Action(() =>
                            {
                                //Empieza el juego, hacemos visible el tablero y lo demás necesario
                                TableroStarWars.Visible = true;
                                groupBoxChat.Visible = true;
                                pictureBox5.Visible = true;
                                turno.Visible = true;
                                DadoPixel.Visible = true;
                                groupBoxSeleccionTableros.Visible = false;
                                EmpezarPartidaButton.Visible = false;
                                pictureBoxReglas.Visible = true;
                                pictureBoxReglas.Visible = false;

                                //Fichas y sus posiciones

                                FichaAmarilloSW1.Visible = true;
                                /*1*/
                                casillasCasa.Add(new Point(440, 600));
                                /*2*/
                                casillasCasa.Add(new Point(555, 600));
                                /*3*/
                                casillasCasa.Add(new Point(440, 760));
                                /*4*/
                                casillasCasa.Add(new Point(555, 760));
                                FichaAmarilloSW1.Location = new Point(440, 600);
                                FichaAmarilloSW2.Visible = true;
                                FichaAmarilloSW2.Location = new Point(555, 600);
                                FichaAmarilloSW3.Visible = true;
                                FichaAmarilloSW3.Location = new Point(440, 760);
                                FichaAmarilloSW4.Visible = true;
                                FichaAmarilloSW4.Location = new Point(555, 760);
                                J3[0] = FichaAmarilloSW1;
                                J3[1] = FichaAmarilloSW2;
                                J3[2] = FichaAmarilloSW3;
                                J3[3] = FichaAmarilloSW4;

                                FichaVerdeSW1.Visible = true;
                                /*1*/
                                casillasCasa.Add(new Point(910, 600));
                                /*2*/
                                casillasCasa.Add(new Point(1030, 600));
                                /*3*/
                                casillasCasa.Add(new Point(910, 750));
                                /*4*/
                                casillasCasa.Add(new Point(1050, 750));
                                FichaVerdeSW1.Location = new Point(910, 600);
                                FichaVerdeSW2.Visible = true;
                                FichaVerdeSW2.Location = new Point(1030, 600);
                                FichaVerdeSW3.Visible = true;
                                FichaVerdeSW3.Location = new Point(910, 750);
                                FichaVerdeSW4.Visible = true;
                                FichaVerdeSW4.Location = new Point(1050, 750);
                                J2[0] = FichaNegraSW1;
                                J2[1] = FichaNegraSW2;
                                J2[2] = FichaNegraSW3;
                                J2[3] = FichaNegraSW4;

                                FichaAzulSW1.Visible = true;
                                /*1*/
                                casillasCasa.Add(new Point(440, 120));
                                /*2*/
                                casillasCasa.Add(new Point(555, 120));
                                /*3*/
                                casillasCasa.Add(new Point(440, 280));
                                /*4*/
                                casillasCasa.Add(new Point(555, 280));
                                FichaAzulSW1.Location = new Point(440, 120);
                                FichaAzulSW2.Visible = true;
                                FichaAzulSW2.Location = new Point(555, 120);
                                FichaAzulSW3.Visible = true;
                                FichaAzulSW3.Location = new Point(440, 280);
                                FichaAzulSW4.Visible = true;
                                FichaAzulSW4.Location = new Point(555, 280);
                                J1[0] = FichaAzulSW1;
                                J1[1] = FichaAzulSW2;
                                J1[2] = FichaAzulSW3;
                                J1[3] = FichaAzulSW4;

                                FichaNegraSW1.Visible = true;
                                /*1*/
                                casillasCasa.Add(new Point(910, 120));
                                /*2*/
                                casillasCasa.Add(new Point(1050, 120));
                                /*3*/
                                casillasCasa.Add(new Point(910, 280));
                                /*4*/
                                casillasCasa.Add(new Point(1050, 280));
                                FichaNegraSW1.Location = new Point(910, 120);
                                FichaNegraSW2.Visible = true;
                                FichaNegraSW2.Location = new Point(1050, 120);
                                FichaNegraSW3.Visible = true;
                                FichaNegraSW3.Location = new Point(910, 280);
                                FichaNegraSW4.Visible = true;
                                FichaNegraSW4.Location = new Point(1050, 280);
                                J4[0] = FichaVerdeSW1;
                                J4[1] = FichaVerdeSW2;
                                J4[2] = FichaVerdeSW3;
                                J4[3] = FichaVerdeSW4;

                                //Escribimos en el label del turno el nombre del primer jugador
                                turno.Text = nameJugadores[0];

                                /*1*/
                                casillas.Add(new Point(810, 767));
                                /*2*/
                                casillas.Add(new Point(810, 737));
                                /*3*/
                                casillas.Add(new Point(810, 707));
                                /*4*/
                                casillas.Add(new Point(810, 677));
                                /*5*/
                                casillas.Add(new Point(810, 647));
                                /*6*/
                                casillas.Add(new Point(810, 617));
                                /*7*/
                                casillas.Add(new Point(810, 587));
                                /*8*/
                                casillas.Add(new Point(810, 557));
                                /*9*/
                                casillas.Add(new Point(856, 505));
                                /*10*/
                                casillas.Add(new Point(890, 505));
                                /*11*/
                                casillas.Add(new Point(920, 505));
                                /*12*/
                                casillas.Add(new Point(950, 505));
                                /*13*/
                                casillas.Add(new Point(980, 505));
                                /*14*/
                                casillas.Add(new Point(1010, 505));
                                /*15*/
                                casillas.Add(new Point(1040, 505));
                                /*16*/
                                casillas.Add(new Point(1070, 505));
                                /*17*/
                                casillas.Add(new Point(1070, 460));
                                /*18*/
                                casillas.Add(new Point(1070, 375));
                                /*19*/
                                casillas.Add(new Point(1040, 375));
                                /*20*/
                                casillas.Add(new Point(1010, 375));
                                /*21*/
                                casillas.Add(new Point(980, 375));
                                /*22*/
                                casillas.Add(new Point(950, 375));
                                /*23*/
                                casillas.Add(new Point(920, 375));
                                /*24*/
                                casillas.Add(new Point(890, 375));
                                /*25*/
                                casillas.Add(new Point(860, 375));
                                /*26*/
                                casillas.Add(new Point(800, 315));
                                /*27*/
                                casillas.Add(new Point(800, 285));
                                /*28*/
                                casillas.Add(new Point(800, 255));
                                /*29*/
                                casillas.Add(new Point(800, 225));
                                /*30*/
                                casillas.Add(new Point(800, 195));
                                /*31*/
                                casillas.Add(new Point(800, 165));
                                /*32*/
                                casillas.Add(new Point(800, 135));
                                /*33*/
                                casillas.Add(new Point(800, 105));
                                /*34*/
                                casillas.Add(new Point(760, 105));
                                /*35*/
                                casillas.Add(new Point(671, 105));
                                /*36*/
                                casillas.Add(new Point(671, 135));
                                /*37*/
                                casillas.Add(new Point(671, 165));
                                /*38*/
                                casillas.Add(new Point(671, 195));
                                /*39*/
                                casillas.Add(new Point(671, 225));
                                /*40*/
                                casillas.Add(new Point(671, 255));
                                /*41*/
                                casillas.Add(new Point(671, 285));
                                /*42*/
                                casillas.Add(new Point(671, 315));
                                /*43*/
                                casillas.Add(new Point(620, 370));
                                /*44*/
                                casillas.Add(new Point(590, 370));
                                /*45*/
                                casillas.Add(new Point(560, 370));
                                /*46*/
                                casillas.Add(new Point(530, 370));
                                /*47*/
                                casillas.Add(new Point(500, 370));
                                /*48*/
                                casillas.Add(new Point(470, 370));
                                /*49*/
                                casillas.Add(new Point(440, 370));
                                /*50*/
                                casillas.Add(new Point(410, 370));
                                /*51*/
                                casillas.Add(new Point(410, 420));
                                /*52*/
                                casillas.Add(new Point(410, 500));
                                /*53*/
                                casillas.Add(new Point(440, 500));
                                /*54*/
                                casillas.Add(new Point(470, 500));
                                /*55*/
                                casillas.Add(new Point(500, 500));
                                /*56*/
                                casillas.Add(new Point(530, 500));
                                /*57*/
                                casillas.Add(new Point(560, 500));
                                /*58*/
                                casillas.Add(new Point(590, 500));
                                /*59*/
                                casillas.Add(new Point(620, 500));
                                /*60*/
                                casillas.Add(new Point(675, 557));
                                /*61*/
                                casillas.Add(new Point(675, 587));
                                /*62*/
                                casillas.Add(new Point(675, 617));
                                /*63*/
                                casillas.Add(new Point(675, 647));
                                /*64*/
                                casillas.Add(new Point(675, 677));
                                /*65*/
                                casillas.Add(new Point(675, 707));
                                /*66*/
                                casillas.Add(new Point(675, 737));
                                /*67*/
                                casillas.Add(new Point(675, 767));
                                /*68*/
                                casillas.Add(new Point(720, 767));

                                //SEGUNDA POSICIÓN FICHAS TABLERO
                                /*1*/
                                casillas.Add(new Point(840, 767));
                                /*2*/
                                casillas.Add(new Point(840, 737));
                                /*3*/
                                casillas.Add(new Point(840, 707));
                                /*4*/
                                casillas.Add(new Point(840, 677));
                                /*5*/
                                casillas.Add(new Point(840, 647));
                                /*6*/
                                casillas.Add(new Point(840, 617));
                                /*7*/
                                casillas.Add(new Point(840, 587));
                                /*8*/
                                casillas.Add(new Point(840, 557));
                                /*9*/
                                casillas.Add(new Point(856, 535));
                                /*10*/
                                casillas.Add(new Point(890, 535));
                                /*11*/
                                casillas.Add(new Point(920, 535));
                                /*12*/
                                casillas.Add(new Point(950, 535));
                                /*13*/
                                casillas.Add(new Point(980, 535));
                                /*14*/
                                casillas.Add(new Point(1010, 535));
                                /*15*/
                                casillas.Add(new Point(1040, 535));
                                /*16*/
                                casillas.Add(new Point(1070, 535));
                                /*17*/
                                casillas.Add(new Point(1070, 420));
                                /*18*/
                                casillas.Add(new Point(1070, 335));
                                /*19*/
                                casillas.Add(new Point(1040, 335));
                                /*20*/
                                casillas.Add(new Point(1010, 335));
                                /*21*/
                                casillas.Add(new Point(980, 335));
                                /*22*/
                                casillas.Add(new Point(950, 335));
                                /*23*/
                                casillas.Add(new Point(920, 335));
                                /*24*/
                                casillas.Add(new Point(890, 335));
                                /*25*/
                                casillas.Add(new Point(860, 335));
                                /*26*/
                                casillas.Add(new Point(840, 315));
                                /*27*/
                                casillas.Add(new Point(840, 285));
                                /*28*/
                                casillas.Add(new Point(840, 255));
                                /*29*/
                                casillas.Add(new Point(840, 225));
                                /*30*/
                                casillas.Add(new Point(840, 195));
                                /*31*/
                                casillas.Add(new Point(840, 165));
                                /*32*/
                                casillas.Add(new Point(840, 135));
                                /*33*/
                                casillas.Add(new Point(840, 105));
                                /*34*/
                                casillas.Add(new Point(720, 105));
                                /*35*/
                                casillas.Add(new Point(640, 105));
                                /*36*/
                                casillas.Add(new Point(640, 135));
                                /*37*/
                                casillas.Add(new Point(640, 165));
                                /*38*/
                                casillas.Add(new Point(640, 195));
                                /*39*/
                                casillas.Add(new Point(640, 225));
                                /*40*/
                                casillas.Add(new Point(640, 255));
                                /*41*/
                                casillas.Add(new Point(640, 285));
                                /*42*/
                                casillas.Add(new Point(640, 315));
                                /*43*/
                                casillas.Add(new Point(620, 335));
                                /*44*/
                                casillas.Add(new Point(590, 335));
                                /*45*/
                                casillas.Add(new Point(560, 335));
                                /*46*/
                                casillas.Add(new Point(530, 335));
                                /*47*/
                                casillas.Add(new Point(500, 335));
                                /*48*/
                                casillas.Add(new Point(470, 335));
                                /*49*/
                                casillas.Add(new Point(440, 335));
                                /*50*/
                                casillas.Add(new Point(410, 335));
                                /*51*/
                                casillas.Add(new Point(410, 455));
                                /*52*/
                                casillas.Add(new Point(410, 540));
                                /*53*/
                                casillas.Add(new Point(440, 540));
                                /*54*/
                                casillas.Add(new Point(470, 540));
                                /*55*/
                                casillas.Add(new Point(500, 540));
                                /*56*/
                                casillas.Add(new Point(530, 540));
                                /*57*/
                                casillas.Add(new Point(560, 540));
                                /*58*/
                                casillas.Add(new Point(590, 540));
                                /*59*/
                                casillas.Add(new Point(620, 540));
                                /*60*/
                                casillas.Add(new Point(635, 557));
                                /*61*/
                                casillas.Add(new Point(635, 587));
                                /*62*/
                                casillas.Add(new Point(635, 617));
                                /*63*/
                                casillas.Add(new Point(635, 647));
                                /*64*/
                                casillas.Add(new Point(635, 677));
                                /*65*/
                                casillas.Add(new Point(635, 707));
                                /*66*/
                                casillas.Add(new Point(635, 737));
                                /*67*/
                                casillas.Add(new Point(635, 767));
                                /*68*/
                                casillas.Add(new Point(755, 767));

                                //CASA VERDE
                                //Primera posición
                                /*136*/
                                casillas.Add(new Point(720, 737));
                                /*137*/
                                casillas.Add(new Point(720, 707));
                                /*138*/
                                casillas.Add(new Point(720, 677));
                                /*139*/
                                casillas.Add(new Point(720, 647));
                                /*140*/
                                casillas.Add(new Point(720, 617));
                                /*141*/
                                casillas.Add(new Point(720, 587));
                                /*142*/
                                casillas.Add(new Point(720, 557));

                                //Segunda posición
                                /*143*/
                                casillas.Add(new Point(755, 737));
                                /*144*/
                                casillas.Add(new Point(755, 707));
                                /*145*/
                                casillas.Add(new Point(755, 677));
                                /*146*/
                                casillas.Add(new Point(755, 647));
                                /*147*/
                                casillas.Add(new Point(755, 617));
                                /*148*/
                                casillas.Add(new Point(755, 587));
                                /*149*/
                                casillas.Add(new Point(755, 557));

                                //CASA NEGRA

                                //Primera posición
                                /*150*/
                                casillas.Add(new Point(1040, 460));
                                /*151*/
                                casillas.Add(new Point(1010, 460));
                                /*152*/
                                casillas.Add(new Point(980, 460));
                                /*153*/
                                casillas.Add(new Point(950, 460));
                                /*154*/
                                casillas.Add(new Point(920, 460));
                                /*155*/
                                casillas.Add(new Point(890, 460));
                                /*156*/
                                casillas.Add(new Point(860, 460));

                                //Segunda posición
                                /*157*/
                                casillas.Add(new Point(1040, 420));
                                /*158*/
                                casillas.Add(new Point(1010, 420));
                                /*159*/
                                casillas.Add(new Point(980, 420));
                                /*160*/
                                casillas.Add(new Point(950, 420));
                                /*161*/
                                casillas.Add(new Point(920, 420));
                                /*162*/
                                casillas.Add(new Point(890, 420));
                                /*163*/
                                casillas.Add(new Point(860, 420));

                                //CASA AMARILLA

                                //Primera posición

                                /*164*/
                                casillas.Add(new Point(440, 420));
                                /*165*/
                                casillas.Add(new Point(470, 420));
                                /*166*/
                                casillas.Add(new Point(500, 420));
                                /*167*/
                                casillas.Add(new Point(530, 420));
                                /*168*/
                                casillas.Add(new Point(560, 420));
                                /*169*/
                                casillas.Add(new Point(590, 420));
                                /*170*/
                                casillas.Add(new Point(620, 420));

                                //Segunda posición

                                /*171*/
                                casillas.Add(new Point(440, 455));
                                /*172*/
                                casillas.Add(new Point(470, 455));
                                /*173*/
                                casillas.Add(new Point(500, 455));
                                /*174*/
                                casillas.Add(new Point(530, 455));
                                /*175*/
                                casillas.Add(new Point(560, 455));
                                /*176*/
                                casillas.Add(new Point(590, 455));
                                /*177*/
                                casillas.Add(new Point(620, 455));

                                //CASA AZUL

                                //Primera posición
                                /*178*/
                                casillas.Add(new Point(760, 135));
                                /*179*/
                                casillas.Add(new Point(760, 165));
                                /*180*/
                                casillas.Add(new Point(760, 195));
                                /*181*/
                                casillas.Add(new Point(760, 225));
                                /*182*/
                                casillas.Add(new Point(760, 255));
                                /*183*/
                                casillas.Add(new Point(760, 285));
                                /*184*/
                                casillas.Add(new Point(760, 315));

                                //Segunda posición
                                /*185*/
                                casillas.Add(new Point(640, 135));
                                /*186*/
                                casillas.Add(new Point(640, 165));
                                /*187*/
                                casillas.Add(new Point(640, 195));
                                /*188*/
                                casillas.Add(new Point(640, 225));
                                /*189*/
                                casillas.Add(new Point(640, 255));
                                /*190*/
                                casillas.Add(new Point(640, 285));
                                /*191*/
                                casillas.Add(new Point(640, 315));

                                //FICHAS DENTRO DE CASA
                                //AZULES
                                /*192*/
                                casillas.Add(new Point(800, 345));
                                /*193*/
                                casillas.Add(new Point(760, 345));
                                /*194*/
                                casillas.Add(new Point(720, 345));
                                /*195*/
                                casillas.Add(new Point(680, 345));

                                //NEGRAS
                                /*196*/
                                casillas.Add(new Point(825, 375));
                                /*197*/
                                casillas.Add(new Point(825, 335));
                                /*198*/
                                casillas.Add(new Point(825, 295));
                                /*199*/
                                casillas.Add(new Point(825, 255));

                                //VERDES
                                /*200*/
                                casillas.Add(new Point(675, 525));
                                /*201*/
                                casillas.Add(new Point(705, 525));
                                /*202*/
                                casillas.Add(new Point(745, 525));
                                /*203*/
                                casillas.Add(new Point(785, 525));

                                //AMARILLAS
                                /*204*/
                                casillas.Add(new Point(650, 380));
                                /*205*/
                                casillas.Add(new Point(650, 340));
                                /*206*/
                                casillas.Add(new Point(650, 300));
                                /*207*/
                                casillas.Add(new Point(650, 260));

                                int i = 0;
                                while (i < 68)
                                {
                                    casillasOcupadas[i] = 0;
                                    i++;
                                }
                            }));
                        }

                        int i = 0;
                        while (i < 4)
                        {
                            //Labels para relacionar el color con el nombre del jugador correspondiente
                            string jugador = nameJugadores[i];
                            if (i == 0)
                            {
                                JugadoresLabel.Text = "· Azul: " + jugador;
                            }
                            if (i == 1)
                            {
                                label10.Text = "· Negro: " + jugador;
                            }
                            if (i == 2)
                            {
                                label6.Text = "· Amarillo: " + jugador;
                            }
                            if (i == 3)
                            {
                                label12.Text = "· Verde: " + jugador;
                            }
                            i++;
                        }
                        break;

                    case 14:      //BOTÓN JUGAR
                        
                        Invoke(new Action(() =>
                        {
                            MessageBox.Show(trozos[1]);
                            pictureBoxReglas.Visible = true;
                            pictureBoxReglas.Image = Properties.Resources.Reglas_del_parchís_1;
                        }));
                        
                        break;
                    case 15:
                        //Caso en el que se ha dado de baja
                        Invoke(new Action(() =>
                        {
                            registro.Visible = false;
                            iniciar_sesion.Visible = true;
                            iniciar_sesion.Visible = true;
                            peticiones.Visible = false;
                            menuStrip_usuario.Visible = false;
                            //Visibilidad chat inactiva hasta que no acepten la invitación
                            groupBoxChat.Visible = false;
                            TableroStarWars.Visible = false;
                            groupBoxSeleccionTableros.Visible = false;
                            EmpezarPartidaButton.Visible = false;
                            DadoPixel.Visible = false;
                            //Fichas SW
                            FichaAmarilloSW1.Visible = false;
                            FichaAmarilloSW2.Visible = false;
                            FichaAmarilloSW3.Visible = false;
                            FichaAmarilloSW4.Visible = false;
                            FichaVerdeSW1.Visible = false;
                            FichaVerdeSW2.Visible = false;
                            FichaVerdeSW3.Visible = false;
                            FichaVerdeSW4.Visible = false;
                            FichaAzulSW1.Visible = false;
                            FichaAzulSW2.Visible = false;
                            FichaAzulSW3.Visible = false;
                            FichaAzulSW4.Visible = false;
                            FichaNegraSW1.Visible = false;
                            FichaNegraSW2.Visible = false;
                            FichaNegraSW3.Visible = false;
                            FichaNegraSW4.Visible = false;
                            MessageBox.Show("Se ha dado de baja");


                        }));

                        break;
                    case 50: //TIRAMOS EL DADO
                        string mensajeFicha;
                        int casilla = 0;
                        int ficha = 0;
                        int casillaCont = 0;
                        //Mensaje de desconexión
                        if (trozos[1] == "SW")
                        {
                            //DADO = 1
                            if (trozos[3] == "1")
                            {
                                DadoPixel.Image = Properties.Resources.DadoP1;
                                if (tirada[turnoJ] == 1 || tirada[turnoJ] > 1 && partidaEmpezada[turnoJ] == 0)
                                {
                                    labelIndicaciones.Text = "Has de obtener un 5 \n para empezar la partida " + usuario_ini.Text;

                                }

                                if (tirada[turnoJ] > 1 && partidaEmpezada[turnoJ] == 1)
                                {
                                    labelIndicaciones.Text = "Te toca tirar " + usuario_ini.Text;
                                }
                            }
                            //DADO = 2
                            if (trozos[3] == "2")
                            {
                                DadoPixel.Image = Properties.Resources.DadoP2;
                                if (tirada[turnoJ] == 1 || tirada[turnoJ] > 1 && partidaEmpezada[turnoJ] == 0)
                                {
                                    labelIndicaciones.Text = "Has de obtener un 5 \n para empezar la partida " + usuario_ini.Text;
                                }
                                if (tirada[turnoJ] > 1 && partidaEmpezada[turnoJ] == 1)
                                {
                                    labelIndicaciones.Text = "Te toca tirar " + usuario_ini.Text;
                                }
                            }
                            //DADO = 3
                            if (trozos[3] == "3")
                            {
                                DadoPixel.Image = Properties.Resources.DadoP3;
                                if (tirada[turnoJ] == 1 || tirada[turnoJ] > 1 && partidaEmpezada[turnoJ] == 0)
                                {
                                    labelIndicaciones.Text = "Has de obtener un 5 \n para empezar la partida " + usuario_ini.Text;
                                }
                                if (tirada[turnoJ] > 1 && partidaEmpezada[turnoJ] == 1)
                                {
                                    labelIndicaciones.Text = "Te toca tirar " + usuario_ini.Text;
                                }
                            }
                            //DADO = 4
                            if (trozos[3] == "4")
                            {
                                DadoPixel.Image = Properties.Resources.DadoP4;
                                if (tirada[turnoJ] == 1 || tirada[turnoJ] > 1 && partidaEmpezada[turnoJ] == 0)
                                {
                                    labelIndicaciones.Text = "Has de obtener un 5 \n para empezar la partida " + usuario_ini.Text;
                                }
                                if (tirada[turnoJ] > 1 && partidaEmpezada[turnoJ] == 1)
                                {
                                    labelIndicaciones.Text = "Te toca tirar " + usuario_ini.Text;
                                }
                            }

                            //DADO = 5
                            if (trozos[3] == "5")
                            {
                                DadoPixel.Image = Properties.Resources.DadoP5;

                                //PARTIDA EMPEZADA
                                //si no es la primera tirada y tenemos casilla libre en la salida
                                if (tirada[turnoJ] > 1 && partidaEmpezada[turnoJ] == 1)
                                {
                                    if (turnoJ == 0)
                                    {
                                        //Hay que poner la 2a  
                                        if (casa[0] == 3)
                                        {
                                            if (casillasOcupadas[38] == 2)
                                            {
                                                labelIndicaciones.Text = "No se puede sacar otra ficha \n Abre barrera!";
                                                casillaCont = 38;
                                                casillaJ1[1] = 9;
                                                casilla = casillaJ1[1];
                                            }
                                            else
                                            {
                                                if (casillaSalida[turnoJ] == 1 && IoD[38] == 1)
                                                {
                                                    FichaAzulSW2.Location = casillas[106];
                                                    casillaJ1[1] = 106;
                                                    IoD[38] = 3;
                                                    casillasOcupadas[38] = 2;
                                                    casilla = casillaJ1[1];
                                                    casaFin[turnoJ] = 1;

                                                }
                                                else if (casillaSalida[turnoJ] == 1 && IoD[38] == 2)
                                                {
                                                    FichaAzulSW2.Location = casillas[38];
                                                    casillaJ1[1] = 38;
                                                    IoD[38] = 3;
                                                    casillasOcupadas[38] = 2;
                                                    casilla = casillaJ1[1];
                                                    casaFin[turnoJ] = 1;
                                                }
                                                else if (casillaSalida[turnoJ] == 2)
                                                {
                                                    FichaAzulSW2.Location = casillas[38];
                                                    casillaJ1[1] = 38;
                                                    IoD[38] = 1;
                                                    casillasOcupadas[38] = 1;
                                                    casilla = casillaJ1[1];
                                                    casaFin[turnoJ] = 1;
                                                }

                                            }
                                            colorFicha5 = "Azul";
                                            ficha = 2;

                                        }
                                        if (casa[0] == 2) //Hay que poner la 3a
                                        {
                                            if (casillasOcupadas[38] == 2)
                                            {
                                                labelIndicaciones.Text = "No se puede sacar otra ficha \n Abre barrera!";
                                                casillaCont = 38;

                                                casillaJ1[2] = 10;
                                                casilla = casillaJ1[2];
                                            }
                                            else
                                            {
                                                if (casillaSalida[turnoJ] == 1 && IoD[38] == 1)
                                                {
                                                    FichaAzulSW3.Location = casillas[106];
                                                    casillaJ1[2] = 106;
                                                    IoD[38] = 3;
                                                    casillasOcupadas[38] = 2;
                                                    casilla = casillaJ1[2];
                                                    casaFin[turnoJ] = 1;
                                                }
                                                else if (casillaSalida[turnoJ] == 1 && IoD[38] == 2)
                                                {
                                                    FichaAzulSW3.Location = casillas[38];
                                                    casillaJ1[2] = 38;
                                                    IoD[38] = 3;
                                                    casillasOcupadas[38] = 2;
                                                    casilla = casillaJ1[2];
                                                    casaFin[turnoJ] = 1;
                                                }

                                                else
                                                {
                                                    FichaAzulSW3.Location = casillas[38];
                                                    casillaJ1[2] = 38;
                                                    IoD[38] = 1;
                                                    casillasOcupadas[38] = 1;
                                                    casilla = casillaJ1[2];
                                                    casaFin[turnoJ] = 1;
                                                }
                                            }
                                            
                                            colorFicha5 = "Azul";
                                            ficha = 3;
                                        }
                                        if (casa[0] == 1)
                                        {
                                            if (casillasOcupadas[38] == 2)
                                            {
                                                labelIndicaciones.Text = "No se puede sacar otra ficha \n Abre barrera!";
                                                casillaCont = 11;

                                                casillaJ1[3] = 3;
                                                casilla = casillaJ1[3];
                                            }
                                            else
                                            {
                                                if (casillaSalida[turnoJ] == 1 && IoD[38] == 1)
                                                {
                                                    FichaAzulSW4.Location = casillas[106];
                                                    casillaJ1[3] = 106;
                                                    IoD[38] = 3;
                                                    casillasOcupadas[38] = 2;
                                                    casilla = casillaJ1[3];
                                                    casaFin[turnoJ] = 1;
                                                }
                                                else if (casillaSalida[turnoJ] == 1 && IoD[38] == 2)
                                                {
                                                    FichaAzulSW4.Location = casillas[38];
                                                    casillaJ1[3] = 38;
                                                    IoD[38] = 3;
                                                    casillasOcupadas[38] = 2;
                                                    casilla = casillaJ1[3];
                                                    casaFin[turnoJ] = 1;
                                                }

                                                else
                                                {
                                                    FichaAzulSW4.Location = casillas[38];
                                                    casillaJ1[3] = 38;
                                                    IoD[38] = 1;
                                                    casillasOcupadas[38] = 1;
                                                    casilla = casillaJ1[3];
                                                    casaFin[turnoJ] = 1;
                                                }
                                            }
                                            
                                            colorFicha5 = "Azul";
                                            ficha = 4;
                                        }
                                        if (casa[turnoJ] > 0) //si queda alguna ficha en casa
                                        {
                                            //Indicamos que tenemos una ficha menos en "casa"
                                            casa[turnoJ]--;
                                        }
                                        //Ahora ponemos que tenemos unicamente un espacio libre en la casilla de salida
                                        casillaSalida[turnoJ]--;
                                        if (casillaSalida[turnoJ] < 0)
                                        {
                                            casillaSalida[turnoJ] = 0;
                                        }
                                    }
                                    if (turnoJ == 1)
                                    {
                                        //Hay que poner la 2a  
                                        if (casa[1] == 3)
                                        {
                                            if (casillasOcupadas[21] == 2)
                                            {
                                                labelIndicaciones.Text = "No se puede sacar otra ficha \n Abre barrera!";
                                                casillaCont = 21;

                                                casillaJ2[1] = 13;
                                                casilla = casillaJ2[1];
                                            }
                                            else
                                            {
                                                if (casillaSalida[turnoJ] == 1 && IoD[21] == 1)
                                                {
                                                    FichaNegraSW2.Location = casillas[89];
                                                    casillaJ2[1] = 89;
                                                    IoD[21] = 3;
                                                    casillasOcupadas[21] = 2;
                                                    casilla = casillaJ2[1];
                                                    casaFin[turnoJ] = 1;
                                                }
                                                else if (casillaSalida[turnoJ] == 1 && IoD[21] == 2)
                                                {
                                                    FichaNegraSW2.Location = casillas[21];
                                                    casillaJ2[1] = 21;
                                                    IoD[21] = 3;
                                                    casillasOcupadas[21] = 2;
                                                    casilla = casillaJ2[1];
                                                    casaFin[turnoJ] = 1;
                                                }

                                                else
                                                {
                                                    FichaNegraSW2.Location = casillas[21];
                                                    casillaJ2[1] = 21;
                                                    IoD[21] = 1;

                                                    casillasOcupadas[21] = 1;
                                                    casilla = casillaJ2[1];
                                                    casaFin[turnoJ] = 1;

                                                }
                                            }
                                            
                                            colorFicha5 = "Negra";
                                            ficha = 2;
                                        }
                                        if (casa[1] == 2)
                                        {
                                            if (casillasOcupadas[21] == 2)
                                            {
                                                labelIndicaciones.Text = "No se puede sacar otra ficha \n Abre barrera!";
                                                casillaCont = 21;

                                                casillaJ2[2] = 14;
                                                casilla = casillaJ2[2];
                                            }
                                            else
                                            {
                                                if (casillaSalida[turnoJ] == 1 && IoD[21] == 1)
                                                {
                                                    FichaNegraSW3.Location = casillas[89];
                                                    casillaJ2[2] = 89;
                                                    IoD[21] = 3;

                                                    casillasOcupadas[21] = 2;
                                                    casilla = casillaJ2[2];
                                                    casaFin[turnoJ] = 1;
                                                }
                                                else if (casillaSalida[turnoJ] == 1 && IoD[21] == 2)
                                                {
                                                    FichaNegraSW3.Location = casillas[21];
                                                    casillaJ2[2] = 21;
                                                    IoD[21] = 3;

                                                    casillasOcupadas[21] = 2;
                                                    casilla = casillaJ2[2];
                                                    casaFin[turnoJ] = 1;
                                                }

                                                else
                                                {
                                                    FichaNegraSW3.Location = casillas[21];
                                                    casillaJ2[2] = 21;
                                                    IoD[21] = 1;

                                                    casillasOcupadas[21] = 1;
                                                    casilla = casillaJ2[2];
                                                    casaFin[turnoJ] = 1;
                                                }
                                            }
                                            
                                            colorFicha5 = "Negra";
                                            ficha = 3;
                                        }
                                        if (casa[1] == 1)
                                        {
                                            if (casillasOcupadas[21] == 2)
                                            {
                                                labelIndicaciones.Text = "No se puede sacar otra ficha \n Abre barrera!";
                                                casillaCont = 21;

                                                casillaJ2[3] = 15;
                                                casilla = casillaJ2[3];
                                            }
                                            else
                                            {
                                                if (casillaSalida[turnoJ] == 1 && IoD[21] == 1)
                                                {
                                                    FichaNegraSW4.Location = casillas[89];
                                                    casillaJ2[3] = 89;
                                                    IoD[21] = 3;

                                                    casillasOcupadas[21] = 2;
                                                    casilla = casillaJ2[3];
                                                    casaFin[turnoJ] = 1;
                                                }
                                                else if (casillaSalida[turnoJ] == 1 && IoD[21] == 2)
                                                {
                                                    FichaNegraSW4.Location = casillas[21];
                                                    casillaJ2[3] = 21;
                                                    IoD[21] = 3;

                                                    casillasOcupadas[21] = 2;
                                                    casilla = casillaJ2[3];
                                                    casaFin[turnoJ] = 1;
                                                }

                                                else
                                                {
                                                    FichaNegraSW4.Location = casillas[21];
                                                    casillaJ2[3] = 21;
                                                    IoD[21] = 1;

                                                    casillasOcupadas[21] = 1;
                                                    casilla = casillaJ2[3];
                                                    casaFin[turnoJ] = 1;
                                                }
                                            }
                                            
                                            colorFicha5 = "Negra";
                                            ficha = 4;
                                        }
                                        if (casa[turnoJ] > 0) //si queda alguna ficha en casa
                                        {
                                            //Indicamos que tenemos una ficha menos en "casa"
                                            casa[turnoJ]--;
                                        }
                                        //Ahora ponemos que tenemos unicamente un espacio libre en la casilla de salida
                                        casillaSalida[turnoJ]--;
                                        if (casillaSalida[turnoJ] < 0)
                                        {
                                            casillaSalida[turnoJ] = 0;
                                        }
                                    }
                                    if (turnoJ == 2)
                                    {
                                        //Hay que poner la 2a  
                                        if (casa[2] == 3)
                                        {
                                            if (casillasOcupadas[55] == 2)
                                            {
                                                labelIndicaciones.Text = "No se puede sacar otra ficha \n Abre barrera!";
                                                casillaCont = 55;

                                                casillaJ3[1] = 1;
                                                casilla = casillaJ3[1];
                                            }
                                            else
                                            {
                                                if (casillaSalida[turnoJ] == 1 && IoD[55] == 1)
                                                {
                                                    FichaAmarilloSW2.Location = casillas[123];
                                                    casillaJ3[1] = 123;
                                                    IoD[55] = 3;

                                                    casillasOcupadas[55] = 2;
                                                    casilla = casillaJ3[1];
                                                }
                                                else if (casillaSalida[turnoJ] == 1 && IoD[55] == 2)
                                                {
                                                    FichaAmarilloSW2.Location = casillas[55];
                                                    casillaJ3[1] = 55;
                                                    IoD[55] = 3;

                                                    casillasOcupadas[55] = 2;
                                                    casilla = casillaJ3[1];
                                                }

                                                else
                                                {
                                                    FichaAmarilloSW2.Location = casillas[55];
                                                    casillaJ3[1] = 55;
                                                    IoD[55] = 1;

                                                    casillasOcupadas[55] = 1;
                                                    casilla = casillaJ3[1];
                                                }
                                            }

                                            colorFicha5 = "Amarilla";
                                            ficha = 2;
                                        }
                                        if (casa[2] == 2)
                                        {
                                            if (casillasOcupadas[55] == 2)
                                            {
                                                labelIndicaciones.Text = "No se puede sacar otra ficha \n Abre barrera!";
                                                casillaCont = 55;

                                                casillaJ3[2] = 2;
                                                casilla = casillaJ3[2];
                                            }
                                            else
                                            {
                                                if (casillaSalida[turnoJ] == 1 && IoD[55] == 1)
                                                {
                                                    FichaAmarilloSW3.Location = casillas[123];
                                                    casillaJ3[2] = 123;
                                                    IoD[55] = 3;
                                                    casillasOcupadas[55] = 2;
                                                    casilla = casillaJ3[2];

                                                }
                                                else if (casillaSalida[turnoJ] == 1 && IoD[55] == 2)
                                                {
                                                    FichaAmarilloSW3.Location = casillas[55];
                                                    casillaJ3[2] = 55;
                                                    IoD[55] = 3;
                                                    casillasOcupadas[55] = 2;
                                                    casilla = casillaJ3[2];

                                                }

                                                else
                                                {
                                                    FichaAmarilloSW3.Location = casillas[55];
                                                    casillaJ3[2] = 55;
                                                    IoD[55] = 1;
                                                    casillasOcupadas[55] = 1;
                                                    casilla = casillaJ3[2];

                                                }
                                            }
                                            
                                            colorFicha5 = "Amarilla";
                                            ficha = 3;
                                        }
                                        if (casa[2] == 1)
                                        {
                                            if (casillasOcupadas[55] == 2)
                                            {
                                                labelIndicaciones.Text = "No se puede sacar otra ficha \n Abre barrera!";
                                                casillaCont = 55;

                                                casillaJ3[3] = 3;
                                                casilla = casillaJ3[3];
                                            }
                                            else
                                            {

                                                if (casillaSalida[turnoJ] == 1 && IoD[55] == 1)
                                                {
                                                    FichaAmarilloSW4.Location = casillas[123];
                                                    casillaJ3[3] = 123;
                                                    IoD[55] = 3;
                                                    casillasOcupadas[55] = 2;
                                                    casilla = casillaJ3[3];
                                                    casaFin[turnoJ]=1;
                                                }
                                                else if (casillaSalida[turnoJ] == 1 && IoD[55] == 2)
                                                {
                                                    FichaAmarilloSW4.Location = casillas[55];
                                                    casillaJ3[3] = 55;
                                                    IoD[55] = 3;
                                                    casillasOcupadas[55] = 2;

                                                    casilla = casillaJ3[3];
                                                    casaFin[turnoJ] = 1;

                                                }

                                                else
                                                {
                                                    FichaAmarilloSW4.Location = casillas[55];
                                                    casillaJ3[3] = 55;
                                                    IoD[55] = 1;
                                                    casillasOcupadas[55] = 1;

                                                    casilla = casillaJ3[3];
                                                    casaFin[turnoJ] = 1;

                                                }
                                            }
                                            
                                            colorFicha5 = "Amarilla";
                                            ficha = 4;
                                        }
                                        //Indicamos que tenemos una ficha menos en "casa"
                                        casa[turnoJ]--;
                                        //Ahora ponemos que tenemos unicamente un espacio libre en la casilla de salida
                                        casillaSalida[turnoJ]--;
                                        if (casillaSalida[turnoJ] < 0)
                                        {
                                            casillaSalida[turnoJ] = 0;
                                        }
                                    }
                                    if (turnoJ == 3)
                                    {
                                        //Hay que poner la 2a  
                                        if (casa[3] == 3)
                                        {
                                            if (casillasOcupadas[78] == 2)
                                            {
                                                labelIndicaciones.Text = "No se puede sacar otra ficha \n Abre barrera!";
                                                casillaCont = 78;

                                                casillaJ4[1] = 5;
                                                casilla = casillaJ4[1];
                                            }
                                            else
                                            {
                                                if (casillaSalida[turnoJ] == 1 && IoD[78] == 1)
                                                {
                                                    FichaVerdeSW2.Location = casillas[146];
                                                    casillaJ4[1] = 146;
                                                    IoD[78] = 3;
                                                    casillasOcupadas[10] = 2;
                                                    casilla = casillaJ4[1];
                                                }
                                                else if (casillaSalida[turnoJ] == 1 && IoD[78] == 2)
                                                {
                                                    FichaVerdeSW2.Location = casillas[78];
                                                    casillaJ4[1] = 78;
                                                    IoD[78] = 3;
                                                    casillasOcupadas[10] = 2;
                                                    casilla = casillaJ4[1];
                                                }

                                                else
                                                {
                                                    FichaVerdeSW2.Location = casillas[78];
                                                    casillaJ4[1] = 78;
                                                    IoD[78] = 1;
                                                    casillasOcupadas[10] = 1;
                                                    casilla = casillaJ4[1];
                                                }
                                            }
                                            
                                            colorFicha5 = "Verde";
                                            ficha = 2;
                                        }
                                        if (casa[3] == 2)
                                        {
                                            if (casillasOcupadas[78] == 2)
                                            {
                                                labelIndicaciones.Text = "No se puede sacar otra ficha \n Abre barrera!";
                                                casillaCont = 78;

                                                casillaJ3[2] = 6;
                                                casilla = casillaJ4[2];
                                            }
                                            else
                                            {
                                                if (casillaSalida[turnoJ] == 1 && IoD[78] == 1)
                                                {
                                                    FichaVerdeSW3.Location = casillas[146];
                                                    casillaJ4[2] = 146;
                                                    IoD[78] = 3;
                                                    casillasOcupadas[10] = 2;
                                                    casilla = casillaJ4[2];
                                                }
                                                else if (casillaSalida[turnoJ] == 1 && IoD[78] == 2)
                                                {
                                                    FichaVerdeSW3.Location = casillas[78];
                                                    casillaJ4[2] = 78;
                                                    IoD[78] = 3;
                                                    casillasOcupadas[10] = 2;
                                                    casilla = casillaJ4[2];
                                                }

                                                else
                                                {
                                                    FichaVerdeSW3.Location = casillas[78];
                                                    casillaJ4[2] = 78;
                                                    IoD[78] = 1;
                                                    casillasOcupadas[10] = 1;
                                                    casilla = casillaJ4[2];
                                                }
                                            }
                                            
                                            colorFicha5 = "Verde";
                                            ficha = 3;
                                        }
                                        if (casa[3] == 1)
                                        {
                                            if (casillasOcupadas[55] == 2)
                                            {
                                                labelIndicaciones.Text = "No se puede sacar otra ficha \n Abre barrera!";
                                                casillaCont = 7;

                                                casillaJ4[3] = 15;
                                                casilla = casillaJ4[3];
                                            }
                                            else
                                            {
                                                if (casillaSalida[turnoJ] == 1 && IoD[78] == 1)
                                                {
                                                    FichaVerdeSW4.Location = casillas[146];
                                                    casillaJ4[3] = 146;
                                                    IoD[78] = 3;
                                                    casillasOcupadas[10] = 2;

                                                    casilla = casillaJ4[3];
                                                    casaFin[turnoJ] = 1;

                                                }
                                                else if (casillaSalida[turnoJ] == 1 && IoD[78] == 2)
                                                {
                                                    FichaVerdeSW4.Location = casillas[78];
                                                    casillaJ4[3] = 78;
                                                    IoD[78] = 3;
                                                    casillasOcupadas[10] = 2;

                                                    casilla = casillaJ4[3];
                                                    casaFin[turnoJ] = 1;

                                                }

                                                else
                                                {
                                                    FichaVerdeSW4.Location = casillas[78];
                                                    casillaJ4[3] = 78;
                                                    IoD[78] = 1;
                                                    casillasOcupadas[10] = 1;

                                                    casilla = casillaJ4[3];
                                                    casaFin[turnoJ] = 1;

                                                }
                                            }
                                            
                                            colorFicha5 = "Verde";
                                            ficha = 4;

                                        }
                                        //Indicamos que tenemos una ficha menos en "casa"
                                        casa[turnoJ]--;
                                        //Ahora ponemos que tenemos unicamente un espacio libre en la casilla de salida
                                        casillaSalida[turnoJ]--;
                                    }

                                }


                                //PARTIDA NO EMPEZADA
                                //Primera salida
                                if (tirada[turnoJ] == 1 || tirada[turnoJ] > 1 && partidaEmpezada[turnoJ] == 0)
                                {
                                    if (turnoJ == 0)
                                    {
                                        //Casilla = 39
                                        FichaAzulSW1.Location = casillas[38];
                                        casillaJ1[0] = 38;
                                        casilla = casillaJ1[0];
                                        colorFicha5 = "Azul";
                                        ficha = 1;

                                        IoD[38] = 1;

                                        casillasOcupadas[38]++;
                                    }
                                    if (turnoJ == 1)
                                    {
                                        //Casilla = 22
                                        FichaNegraSW1.Location = casillas[21];
                                        casillaJ2[0] = 21;
                                        casilla = casillaJ2[0];
                                        ficha = 1;
                                        colorFicha5 = "Negra";

                                        IoD[21] = 1;

                                        casillasOcupadas[21]++;
                                    }
                                    if (turnoJ == 2)
                                    {
                                        //Casilla = 56
                                        FichaAmarilloSW1.Location = casillas[55];
                                        casillaJ3[0] = 55;
                                        casilla = casillaJ3[0];
                                        colorFicha5 = "Amarilla";
                                        ficha = 1;

                                        IoD[55] = 1;

                                        casillasOcupadas[55]++;
                                    }
                                    if (turnoJ == 3)
                                    {
                                        //Casilla = 5
                                        FichaVerdeSW1.Location = casillas[4];
                                        casillaJ4[0] = 4;
                                        casilla = casillaJ4[0];
                                        colorFicha5 = "Verde";
                                        ficha = 1;

                                        IoD[4] = 1;

                                        casillasOcupadas[4]++;

                                    }
                                    //Indicamos que tenemos una ficha menos en "casa"
                                    casa[turnoJ]--;
                                    //Ahora ponemos que tenemos unicamente un espacio libre en la casilla de salida
                                    casillaSalida[turnoJ]--;
                                    partidaEmpezada[turnoJ] = 1;

                                }
                                if (casillaSalida[turnoJ] == 0)
                                {
                                    labelIndicaciones.Text = "Te toca tirar " + usuario_ini.Text;
                                }
                            }
                            //DADO = 6
                            if (trozos[3] == "6")
                            {
                                DadoPixel.Image = Properties.Resources.DadoP6;
                                if (tirada[turnoJ] == 1 || tirada[turnoJ] > 1 && partidaEmpezada[turnoJ] == 0)
                                {
                                    labelIndicaciones.Text = "Has de obtener un 5 \n para empezar la partida " + usuario_ini.Text;
                                }

                                int e = 0;
                                int contadorCasillasO = 0;

                                //Comprobamos si hay alguna barrera que se necesita abrir
                                while (e < 68)
                                {
                                    if (casillasOcupadas[e] == 2)
                                    {
                                        contadorCasillasO++;
                                    }
                                    e++;
                                }
                                if (contadorCasillasO > 0) //Obliga a abrir barrera
                                {
                                    labelIndicaciones.Text = "Tienes que abrir la barrera " + usuario_ini.Text;
                                }
                                else if (tirada[turnoJ] > 1 && partidaEmpezada[turnoJ] == 1) //Tirada normal
                                {
                                    labelIndicaciones.Text = "Te toca tirar " + usuario_ini.Text;
                                }
                            }

                            //Pasamos de turno
                            turnoActual = turnoJ;

                            string nombreturno1 = nameJugadores[turnoJ];
                            int casillaActual;
                            int casillaSiguiente;

                            if (casilla > 67 && casilla < 135) //Caso en el que se trata de una ficha en la 2a posición
                            {
                                casillasFichas = casilla + valorDado; //Donde va a ir la ficha
                                casillasComprobar = casillasFichas - 68; //Casilla que tiene que comprobar
                                casillaSiguiente = casillasComprobar;
                                casillaActual = casilla - 68;
                                //Si no hay nadie que la ponga a la izquierda
                                if (casillasOcupadas[casillasComprobar] == 0)
                                {
                                    casillasFichas = casillasComprobar;
                                }
                            }
                            else //Caso en el que se trata de una ficha en la 1a posición
                            {
                                if (casilla == 67 && colorFicha5 != "Verde") //Caso en el que está en la 68 y tiene que volver a la 1, cuando hagamos la subida a casa habrá que poner una condición de color
                                {
                                    casillasFichas = valorDado - 1;
                                }
                                else
                                {
                                    casillasFichas = casilla + valorDado; //Donde va a ir la ficha
                                }

                                casillaSiguiente = casillasFichas; //Casilla que tiene que comprobar
                                casillaActual = casilla;
                            }

                            //jugadorturno
                            if (trozos[3] == "5" && casa[turnoJ] != 0 && partidaEmpezada[turnoJ] == 1 && trozos[2] == usuario_ini.Text || trozos[3] == "5" && casa[turnoJ] == 0 && casaFin[turnoJ] == 1 && partidaEmpezada[turnoJ] == 1 && trozos[2] == usuario_ini.Text)
                            {
                                //CASO EN QUE SE HA SACADO UN 5 Y LA PARTIDA ESTÁ EMPEZADA, SOLO ACCEDE EL QUE LE TOCA TIRAR

                                turnoJ++; //Salta de turno

                                if (turnoJ >= numJugadoresP)
                                {
                                    turnoJ = 0; //Para reiniciar el turno y volver al primer jugador
                                }
                                
                            
                                if (casillasOcupadas[casillaCont] == 2) 
                                {
                                    mensajeFicha = "51/2/" + tablero + "/" + nameJugadores[turnoJ] + "/" + casillasCasa[casilla] + "/" + colorFicha5 + "/" + ficha + "/" + idPartida + "/" + partidaEmpezada[turnoActual] + "/" + turnoJ + "/" + casillaSalida[turnoActual] + "/" + casillasOcupadas[comprobarAnterior] + "/" + IoD[comprobarAnterior] + "/" + comprobarAnterior + "/" + casillasOcupadas[casillasComprobar] + "/" + IoD[casillasComprobar] + "/" + casillasComprobar;
                                    byte[] msg1 = System.Text.Encoding.ASCII.GetBytes(mensajeFicha);
                                    server.Send(msg1);
                                }
                                else
                                {
                                    mensajeFicha = "51/1/" + tablero + "/" + nameJugadores[turnoJ] + "/" + casillas[casilla] + "/" + colorFicha5 + "/" + ficha + "/" + idPartida + "/" + partidaEmpezada[turnoActual] + "/" + turnoJ + "/" + casillaSalida[turnoActual] + "/" + casillasOcupadas[casillaActual] + "/" + IoD[casillaActual] + "/" + casillaActual + "/" + casillasOcupadas[casillaSiguiente] + "/" + IoD[casillaSiguiente] + "/" + casillaSiguiente;
                                    byte[] msg1 = System.Text.Encoding.ASCII.GetBytes(mensajeFicha);
                                    server.Send(msg1);
                                }
                            }
                            
                            else if (trozos[3] != "5" && trozos[2] == usuario_ini.Text)
                            {
                                //CASO DE UNA TIRADA NORMAL, SOLO ACCEDE EL QUE LE TOCA TIRAR

                                turnoJ++; //Salta de turno
                                if (turnoJ >= numJugadoresP)
                                {
                                    turnoJ = 0; //Para reiniciar el turno y volver al primer jugador
                                }
                                mensajeFicha = "51/0/" + nameJugadores[turnoJ] + "/" + partidaEmpezada[turnoActual] + "/" + turnoJ + "/" + casillaSalida[turnoActual] + "/" + casillasOcupadas[casillasComprobar] + "/" + IoD[casillasComprobar];
                                byte[] msg1 = System.Text.Encoding.ASCII.GetBytes(mensajeFicha);
                                server.Send(msg1);
                            }
                        }
                        break;

                    case 51: 
                        if (tablero == "SW")
                        {
                            if (trozos[1] == "0") //Troceamos el mensaje
                            {
                                turno.Text = trozos[2];
                                partidaEmpezada[turnoActual] = Convert.ToInt32(trozos[3]);
                                turnoJ = Convert.ToInt32(trozos[4]);
                                casillaSalida[turnoActual] = Convert.ToInt32(trozos[5]);
                                casillasOcupadas[casillasComprobar] = Convert.ToInt32(trozos[6]);
                                IoD[casillasComprobar] = Convert.ToInt32(trozos[7]);
                            }
                            else if (trozos[1] == "1") //Troceamos el mensaje, caso en el que se mueve una ficha
                            {
                                int coordenadaX = Convert.ToInt32(trozos[3]);
                                int coordenadaY = Convert.ToInt32(trozos[4]);
                                string colorFichaMovida = trozos[5];
                                int valorFicha = Convert.ToInt32(trozos[6]);
                                
                                partidaEmpezada[turnoActual] = Convert.ToInt32(trozos[7]);
                                turnoJ = Convert.ToInt32(trozos[8]);
                                casillaSalida[turnoActual] = Convert.ToInt32(trozos[9]);
                                int casillaAct = Convert.ToInt32( trozos[12]);
                                casillasOcupadas[casillaAct] = Convert.ToInt32(trozos[10]);
                                IoD[casillaAct] = Convert.ToInt32(trozos[11]);

                                colorFichaCasilla[casillaAct] = null;

                                int casillaSig = Convert.ToInt32(trozos[15]);
                                casillasOcupadas[casillaSig] = Convert.ToInt32(trozos[13]);
                                IoD[casillaSig] = Convert.ToInt32(trozos[14]);
                                colorFichaCasilla[casillaSig] = colorFichaMovida;

                                //Asignamos localizaciones
                                if (colorFichaMovida == "Amarilla")
                                {
                                    if (valorFicha == 1)
                                    {
                                        FichaAmarilloSW1.Location = new Point(coordenadaX, coordenadaY);
                                        casillaJ3[0] = casillaSig;
                                    }
                                    if (valorFicha == 2)
                                    {
                                        FichaAmarilloSW2.Location = new Point(coordenadaX, coordenadaY);
                                        casillaJ3[1] = casillaSig;
                                    }
                                    if (valorFicha == 3)
                                    {
                                        FichaAmarilloSW3.Location = new Point(coordenadaX, coordenadaY);
                                        casillaJ3[2] = casillaSig;
                                    }
                                    if (valorFicha == 4)
                                    {
                                        FichaAmarilloSW4.Location = new Point(coordenadaX, coordenadaY);
                                        casillaJ3[3] = casillaSig;
                                    }
                                }
                                if (colorFichaMovida == "Negra")
                                {
                                    if (valorFicha == 1)
                                    {
                                        FichaNegraSW1.Location = new Point(coordenadaX, coordenadaY);
                                        casillaJ2[0] = casillaSig;
                                    }
                                    if (valorFicha == 2)
                                    {
                                        FichaNegraSW2.Location = new Point(coordenadaX, coordenadaY);
                                        casillaJ2[1] = casillaSig;
                                    }
                                    if (valorFicha == 3)
                                    {
                                        FichaNegraSW3.Location = new Point(coordenadaX, coordenadaY);
                                        casillaJ2[2] = casillaSig;
                                    }
                                    if (valorFicha == 4)
                                    {
                                        FichaNegraSW4.Location = new Point(coordenadaX, coordenadaY);
                                        casillaJ2[3] = casillaSig;
                                    }
                                }
                                if (colorFichaMovida == "Azul")
                                {
                                    if (valorFicha == 1)
                                    {
                                        FichaAzulSW1.Location = new Point(coordenadaX, coordenadaY);
                                        casillaJ1[0] = casillaSig;
                                    }
                                    if (valorFicha == 2)
                                    {
                                        FichaAzulSW2.Location = new Point(coordenadaX, coordenadaY);
                                        casillaJ1[1] = casillaSig;
                                    }
                                    if (valorFicha == 3)
                                    {
                                        FichaAzulSW3.Location = new Point(coordenadaX, coordenadaY);
                                        casillaJ1[2] = casillaSig;
                                    }
                                    if (valorFicha == 4)
                                    {
                                        FichaAzulSW4.Location = new Point(coordenadaX, coordenadaY);
                                        casillaJ1[3] = casillaSig;
                                    }
                                }
                                if (colorFichaMovida == "Verde")
                                {
                                    if (valorFicha == 1)
                                    {
                                        FichaVerdeSW1.Location = new Point(coordenadaX, coordenadaY);
                                        casillaJ4[0] = casillaSig;
                                    }
                                    if (valorFicha == 2)
                                    {
                                        FichaVerdeSW2.Location = new Point(coordenadaX, coordenadaY);
                                        casillaJ4[1] = casillaSig;
                                    }
                                    if (valorFicha == 3)
                                    {
                                        FichaVerdeSW3.Location = new Point(coordenadaX, coordenadaY);
                                        casillaJ4[2] = casillaSig;
                                    }
                                    if (valorFicha == 4)
                                    {
                                        FichaVerdeSW4.Location = new Point(coordenadaX, coordenadaY);
                                        casillaJ4[3] = casillaSig;
                                    }
                                }
                                //Actualizamos el label del turno
                                turno.Text = trozos[2];
                            }
                            else if (trozos[1] == "2") //Troceamos el mensaje, caso en el que no se puede sacar otra ficha
                            {
                                int coordenadaX = Convert.ToInt32(trozos[3]);
                                int coordenadaY = Convert.ToInt32(trozos[4]);
                                string colorFichaMovida = trozos[5];
                                int valorFicha = Convert.ToInt32(trozos[6]);
                                partidaEmpezada[turnoActual] = Convert.ToInt32(trozos[7]);
                                turnoJ = Convert.ToInt32(trozos[8]);
                                casillasOcupadas[casillasComprobar] = Convert.ToInt32(trozos[10]);
                                IoD[casillasComprobar] = Convert.ToInt32(trozos[11]);

                                //Asignamos localizaciones
                                if (colorFichaMovida == "Amarilla")
                                {
                                    if (valorFicha == 1)
                                    {
                                        FichaAmarilloSW1.Location = new Point(coordenadaX, coordenadaY);
                                    }
                                    if (valorFicha == 2)
                                    {
                                        FichaAmarilloSW2.Location = new Point(coordenadaX, coordenadaY);
                                    }
                                    if (valorFicha == 3)
                                    {
                                        FichaAmarilloSW3.Location = new Point(coordenadaX, coordenadaY);
                                    }
                                    if (valorFicha == 4)
                                    {
                                        FichaAmarilloSW4.Location = new Point(coordenadaX, coordenadaY);
                                    }
                                }
                                if (colorFichaMovida == "Negra")
                                {
                                    if (valorFicha == 1)
                                    {
                                        FichaNegraSW1.Location = new Point(coordenadaX, coordenadaY);
                                    }
                                    if (valorFicha == 2)
                                    {
                                        FichaNegraSW2.Location = new Point(coordenadaX, coordenadaY);
                                    }
                                    if (valorFicha == 3)
                                    {
                                        FichaNegraSW3.Location = new Point(coordenadaX, coordenadaY);
                                    }
                                    if (valorFicha == 4)
                                    {
                                        FichaNegraSW4.Location = new Point(coordenadaX, coordenadaY);
                                    }
                                }
                                if (colorFichaMovida == "Azul")
                                {
                                    if (valorFicha == 1)
                                    {
                                        FichaAzulSW1.Location = new Point(coordenadaX, coordenadaY);
                                    }
                                    if (valorFicha == 2)
                                    {
                                        FichaAzulSW2.Location = new Point(coordenadaX, coordenadaY);
                                    }
                                    if (valorFicha == 3)
                                    {
                                        FichaAzulSW3.Location = new Point(coordenadaX, coordenadaY);
                                    }
                                    if (valorFicha == 4)
                                    {
                                        FichaAzulSW4.Location = new Point(coordenadaX, coordenadaY);
                                    }
                                }
                                if (colorFichaMovida == "Verde")
                                {
                                    if (valorFicha == 1)
                                    {
                                        FichaVerdeSW1.Location = new Point(coordenadaX, coordenadaY);
                                    }
                                    if (valorFicha == 2)
                                    {
                                        FichaVerdeSW2.Location = new Point(coordenadaX, coordenadaY);
                                    }
                                    if (valorFicha == 3)
                                    {
                                        FichaVerdeSW3.Location = new Point(coordenadaX, coordenadaY);
                                    }
                                    if (valorFicha == 4)
                                    {
                                        FichaVerdeSW4.Location = new Point(coordenadaX, coordenadaY);
                                    }
                                }
                                //Actualizamos el label de turno
                                turno.Text = trozos[2];
                                labelIndicaciones.Text = "No se puede sacar otra ficha \n Abre barrera!";
                            }

                        }
                        
                        break;


                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------
        //FUNCIÓN POSICION DE LAS FICHAS
        public void PosicionFicha(PictureBox ficha, int casilla)
        {
            ficha.Location = casillas[40];
        }


        //------------------------------------------------------------------------------------------------------------------------------------
        //FUNCIÓN DETECCION DE LOCATION
        private void peticiones_MouseMove(object sender, MouseEventArgs e)
        {
            //label3.Text = e.X.ToString() + "," + e.Y.ToString();
        }


        //------------------------------------------------------------------------------------------------------------------------------------
        //FUNCIÓN BOTÓN CONECTAR (Oculto)
        private void conectar_Click(object sender, EventArgs e)
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("192.168.56.101");
            IPEndPoint ipep = new IPEndPoint(direc, puerto);

            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
                this.BackColor = Color.LightGreen;

            }
            catch (SocketException)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }

            //pongo en marcha el thread que atendera los mensajes del servidor
            ThreadStart ts = delegate { AtenderServidor(); };
            Atender = new Thread(ts);
            Atender.Start();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        //FUNCIÓN BOTÓN DESCONECTAR (0)
        private void desconectar_Click(object sender, EventArgs e)
        {
            //Mensaje de desconexión
            string mensaje = "0/";

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            //// Nos desconectamos
            //this.BackColor = Color.Gray;
            //server.Shutdown(SocketShutdown.Both);
            //server.Close();
        }
        //-----------------------------------------------------------------------------------------------------------------------------------
        //BOTON DESCONECTARSE DEL SERVIDOR MENU SUPERIOR (0)
        private void desconectarseDelServidorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Mensaje de desconexión
            string mensaje = "0/";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg); 
        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        //FUNCIÓN BOTÓN ENVIAR PETICIONES (1,2,3)
        private void enviar_Click(object sender, EventArgs e)
        {
            //Petición partidas ganadas por el usuario seleccionado (1)
            if (partidas_ganadas.Checked && (usuario_consulta.Text.Length > 0))
            {
                string mensaje = "1/" + usuario_consulta.Text;
                // Enviamos al servidor el nombre del usuario
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }

            //Petición de tablones jugados por el usuario (2)
            else if (tablones.Checked && (usuario_consulta.Text.Length > 0))
            {
                string mensaje = "2/" + usuario_consulta.Text;
                // Enviamos al servidor el nombre del usuario
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

            }
            //Petición ID del usuario (3)
            else if (id_usuario.Checked && (usuario_consulta.Text.Length > 0))
            {
                string mensaje = "3/" + usuario_consulta.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }

            //Aviso petición no seleccionada.
            else if (String.IsNullOrWhiteSpace(usuario_consulta.Text) || (usuario_consulta.Text.Length > 0) && !partidas_ganadas.Checked && !tablones.Checked && !id_usuario.Checked)
            {
                MessageBox.Show("¡ATENCIÓN! \n¡Recuerda que tienes que escoger petición y indicar un usuario antes de enviar!");
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        //FUNCIÓN BOTÓN ENTRAR AL REGISTRO
        private void entrar_reg_Click(object sender, EventArgs e)
        {
            registro.Visible = true;
            iniciar_sesion.Visible = false;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        //FUNCIÓN BOTÓN REGISTRARSE (4)
        private void envia_reg_Click(object sender, EventArgs e)
        {

            if (((usuario_reg.Text.Length > 1) && (contraseña_reg.Text.Length > 1)) && ((usuario_reg.Text != "") && (contraseña_reg.Text != "")))
            {
                string mensaje = "4/" + usuario_reg.Text + "/" + contraseña_reg.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            else
            {
                MessageBox.Show("El nombre debe tener más de un caracter");
            }

        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        //FUNCIÓN BOTÓN INICIAR SESIÓN (5)
        private void entrar_Click(object sender, EventArgs e)
        {
            string mensaje = "5/" + usuario_ini.Text + "/" + contraseña_ini.Text;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        //ENTRAR CON "ENTER" (5)
        private void entrar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string mensaje = "5/" + usuario_ini.Text + "/" + contraseña_ini.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            else
            {
                MessageBox.Show("Porfavor, haga click en Enter para Iniciar Sesión :)");
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        //BOTON LISTA DE CONECTADOS (6)
        public void ListaConectados()
        {
            string mensaje = "6/" + usuario_ini.Text + "/" + contraseña_ini.Text;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        //BOTON SERVICIOS (7)

        private void button1_Click(object sender, EventArgs e)
        {
            //Pedir numero de servicios realizados
            string mensaje = "7/" + usuario_ini.Text;

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        //BOTON SALIR DE LISTA CONECTADOS (8)
        private void SalirButton_Click(object sender, EventArgs e)
        {
            string mensaje = "8/" + usuario_ini.Text;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        //BOTON SALIR DE LISTA CONECTADOS (8)
        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string mensaje = "8/" + usuario_ini.Text;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        //SELECCION DE INVITADOS (9)
        private void ListaConectadosDG_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (jugar == 1)
            {
                if (jugadores < 3)
                {
                    nameJugadores[jugadores] = ListaConectadosDG.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

                    if (usuario_ini.Text == nameJugadores[jugadores])
                    {
                        MessageBox.Show("ATENCIÓN!! No puedes seleccionarte a ti mismo!!");
                    }
                    else
                    {
                        string mensaje = "9/" + usuario_ini.Text + "/" + nameJugadores[jugadores];
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                        jugadores++;
                    }
                }

            }
            else
            {
                MessageBox.Show("Para poder invitar hace falta pulsar el botón de 'Jugar' :)");
            }
        }
        //-----------------------------------------------------------------------------------------------------------------------------------
        //BOTÓN ENVIAR MENSAJE (11) 
        private void EnviarMensajebutton_Click(object sender, EventArgs e)
        {
            string mensaje = "11/" + idPartida + "/" + usuario_ini.Text + "/" + MensajeTextBox.Text;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            Chat.Items.Add(usuario_ini.Text + ":" + MensajeTextBox.Text);
            MensajeTextBox.Clear();
        }
        //-----------------------------------------------------------------------------------------------------------------------------------
        //SELECCION DE TABLERO (12)
        private void pictureBoxStarWars_Click(object sender, EventArgs e)
        {
            string mensaje = "12/SW/" + idPartida;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }
        private void pictureBoxCasaPapel_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Tablero disponible en pròximas actualizaciones. \n Disculpa las molestias:(");
            //string mensaje = "12/CP" + "/" + idPartida;
            //byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            //server.Send(msg);
        }
        private void pictureBoxHarryPotter_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Tablero disponible en pròximas actualizaciones. \n Disculpa las molestias:(");
            //string mensaje = "12/HP" + "/" + idPartida;
            //byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            //server.Send(msg);

        }
        //-----------------------------------------------------------------------------------------------------------------------------------
        //BOTON EMPEZAR PARTIDA
        private void EmpezarPartidaButton_Click(object sender, EventArgs e)
        {
            groupBoxSeleccionTableros.Visible = true;

            //  groupBoxSeleccionTableros.Location = new System.Drawing.Point(50, 19);

        }
        //-----------------------------------------------------------------------------------------------------------------------------------
        //BOTON JUGAR (14)
        private void jugar_button_Click(object sender, EventArgs e)
        {
            jugar = 1;
            string mensaje = "14/" + idPartida;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            PeticionesGroupBox.Visible = false;
            jugar_button.Visible = false;
            groupBoxSeleccionTableros.Visible = false;
        }
        //-----------------------------------------------------------------------------------------------------------------------------------
        //BOTON DARME DE BAJA (15)
        private void darmeDeBajaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string mensaje = "15/" + usuario_ini.Text;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }
        //-----------------------------------------------------------------------------------------------------------------------------------
        //DADO PIXEL STARWARS TIRADA
        private void DadoPixel_Click(object sender, EventArgs e)
        {
            if (usuario_ini.Text != turno.Text || tirarFicha == 1)
            {
                MessageBox.Show("Porfavor espera a tu turno");
            }
            else
            {
                timer.Enabled = true;

                //Cogemos un numero random entre 1 y 6  
                Random dado = new Random();
                int numDado = dado.Next(1, 7);

                //Sumamos +1 a la tirada
                tirada[turnoJ]++;
                if (numDado == 1)
                {
                    valorDado = 1;
                }
                if (numDado == 2)
                {
                    valorDado = 2;
                }
                if (numDado == 3)
                {
                    valorDado = 3;
                }
                if (numDado == 4)
                {
                    valorDado = 4;
                }
                if (numDado == 5)
                {
                    valorDado = 5;
                }
                if (numDado == 6)
                {
                    valorDado = 6;

                }
                
                //Hay que enviar este nombre al servidor para que se ponga en todos los Formularios de los jugadores
                string mensaje = "50/" + tablero + "/" + idPartida + "/" + usuario_ini.Text + "/" + valorDado;
                byte[] msg1 = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg1);
                timer.Stop();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            PosicionFicha(J1[0], posicionFichasJ1[0]);
        }

        private void Parchís_MouseMove(object sender, MouseEventArgs e)
        {
            //label3.Text = e.X.ToString() + "," + e.Y.ToString();
        }
        //FICHAS TABLERO SW, funciones Click para todas las fichas
        private void FichaAzulSW1_Click(object sender, EventArgs e)
        {

            if (colorFicha5 == "Azul")
            {
                fichaJ1[0] = 1;
                for (int i = 0; i < 136; i++)
                {
                    if (FichaAzulSW1.Location == casillas[i])
                    {
                        casillaJ1[0] = i;
                    }
                }
                moverFicha(FichaAzulSW1, casillaJ1[0], fichaJ1[0]);
                tirarFicha = 0;
            }
            else
            {
                labelIndicaciones.Text = "No puedes mover una ficha que no es tuya!";

            }
        }

        private void FichaAzulSW2_Click(object sender, EventArgs e)
        {

            if (colorFicha5 == "Azul")
            {
                fichaJ1[1] = 2;
                for (int i = 0; i < 136; i++)
                {
                    if (FichaAzulSW2.Location == casillas[i])
                    {
                        casillaJ1[1] = i;
                    }
                }
                moverFicha(FichaAzulSW2, casillaJ1[1], fichaJ1[1]);
                tirarFicha = 0;
            }
            else
            {
                labelIndicaciones.Text = "No puedes mover una ficha que no es tuya!";

            }
        }

        private void FichaAzulSW3_Click(object sender, EventArgs e)
        {

            if (colorFicha5 == "Azul")
            {

                fichaJ1[2] = 3;
                for (int i = 0; i < 136; i++)
                {
                    if (FichaAzulSW3.Location == casillas[i])
                    {
                        casillaJ1[2] = i;
                    }
                }
                moverFicha(FichaAzulSW3, casillaJ1[2], fichaJ1[2]);
                tirarFicha = 0;
            }
            else
            {
                labelIndicaciones.Text = "No puedes mover una ficha que no es tuya!";

            }
        }
        private void FichaAzulSW4_Click(object sender, EventArgs e)
        {

            if (colorFicha5 == "Azul")
            {
                fichaJ1[3] = 4;
                for (int i = 0; i < 136; i++)
                {
                    if (FichaAzulSW4.Location == casillas[i])
                    {
                        casillaJ1[3] = i;
                    }
                }
                moverFicha(FichaAzulSW4, casillaJ1[3], fichaJ1[3]);
                tirarFicha = 0;
            }
            else
            {
                labelIndicaciones.Text = "No puedes mover una ficha que no es tuya!";

            }
        }

        private void FichaNegraSW1_Click(object sender, EventArgs e)
        {
            if (colorFicha5 == "Negra")
            {
                fichaJ2[0] = 1;
                for (int i = 0; i < 136; i++)
                {
                    if (FichaNegraSW1.Location == casillas[i])
                    {
                        casillaJ2[0] = i;
                    }
                }
                moverFicha(FichaNegraSW1, casillaJ2[0], fichaJ2[0]);
                tirarFicha = 0;
            }
            else
            {
                labelIndicaciones.Text = "No puedes mover una ficha que no es tuya!";

            }
        }
        private void FichaNegraSW2_Click(object sender, EventArgs e)
        {
            if (colorFicha5 == "Negra")
            {

                fichaJ2[1] = 2;
                for (int i = 0; i < 136; i++)
                {
                    if (FichaNegraSW2.Location == casillas[i])
                    {
                        casillaJ2[1] = i;
                    }
                }
                moverFicha(FichaNegraSW2, casillaJ2[1], fichaJ2[1]);
                tirarFicha = 0;
            }
            else
            {
                labelIndicaciones.Text = "No puedes mover una ficha que no es tuya!";

            }
        }
        private void FichaNegraSW3_Click(object sender, EventArgs e)
        {
            if (colorFicha5 == "Negra")
            {
                fichaJ2[2] = 3;
                for (int i = 0; i < 136; i++)
                {
                    if (FichaNegraSW3.Location == casillas[i])
                    {
                        casillaJ2[2] = i;
                    }
                }
                moverFicha(FichaNegraSW3, casillaJ2[2], fichaJ2[2]);
                tirarFicha = 0;
            }
            else
            {
                labelIndicaciones.Text = "No puedes mover una ficha que no es tuya!";

            }
        }
        private void FichaNegraSW4_Click(object sender, EventArgs e)
        {
            if (colorFicha5 == "Negra")
            {
                fichaJ2[3] = 1;
                for (int i = 0; i < 136; i++)
                {
                    if (FichaNegraSW4.Location == casillas[i])
                    {
                        casillaJ2[3] = i;
                    }
                }
                moverFicha(FichaNegraSW4, casillaJ2[3], fichaJ2[3]);
                tirarFicha = 0;
            }
            else
            {
                labelIndicaciones.Text = "No puedes mover una ficha que no es tuya!";

            }
        }
        private void FichaAmarilloSW1_Click(object sender, EventArgs e)
        {

            if (colorFicha5 == "Amarilla")
            {
                fichaJ3[0] = 1;
                for (int i = 0; i < 136; i++)
                {
                    if (FichaAmarilloSW1.Location == casillas[i])
                    {
                        casillaJ3[0] = i;
                    }
                }
                moverFicha(FichaAmarilloSW1, casillaJ3[0], fichaJ3[0]);
                tirarFicha = 0;
            }
            else
            {
                labelIndicaciones.Text = "No puedes mover una ficha que no es tuya!";

            }
        }

        private void FichaAmarilloSW2_Click(object sender, EventArgs e)
        {

            if (colorFicha5 == "Amarilla")
            {
                fichaJ3[1] = 2;
                for (int i = 0; i < 136; i++)
                {
                    if (FichaAmarilloSW2.Location == casillas[i])
                    {
                        casillaJ3[1] = i;
                    }
                }
                moverFicha(FichaAmarilloSW2, casillaJ3[1], fichaJ3[1]);
                tirarFicha = 0;
            }
            else
            {
                labelIndicaciones.Text = "No puedes mover una ficha que no es tuya!";

            }
        }
        private void FichaAmarilloSW3_Click(object sender, EventArgs e)
        {

            if (colorFicha5 == "Amarilla")
            {
                fichaJ3[2] = 3;
                for (int i = 0; i < 136; i++)
                {
                    if (FichaAmarilloSW3.Location == casillas[i])
                    {
                        casillaJ3[2] = i;
                    }
                }
                moverFicha(FichaAmarilloSW3, casillaJ3[2], fichaJ3[2]);
                tirarFicha = 0;
            }
            else
            {
                labelIndicaciones.Text = "No puedes mover una ficha que no es tuya!";

            }
        }

        private void FichaAmarilloSW4_Click(object sender, EventArgs e)
        {
            if (colorFicha5 == "Amarilla")
            {
                fichaJ3[3] = 4;
                for (int i = 0; i < 136; i++)
                {
                    if (FichaAmarilloSW4.Location == casillas[i])
                    {
                        casillaJ3[3] = i;
                    }
                }
                moverFicha(FichaAmarilloSW4, casillaJ3[3], fichaJ3[3]);
                tirarFicha = 0;
            }
            else
            {
                labelIndicaciones.Text = "No puedes mover una ficha que no es tuya!";
            }
        }

        private void FichaVerdeSW1_Click(object sender, EventArgs e)
        {
            if (colorFicha5 == "Verde")
            {

                fichaJ4[0] = 1;
                for (int i = 0; i < 136; i++)
                {
                    if (FichaVerdeSW1.Location == casillas[i])
                    {
                        casillaJ4[0] = i;
                    }
                }
                moverFicha(FichaVerdeSW1, casillaJ4[0], fichaJ4[0]);
                tirarFicha = 0;
            }
            else
            {
                labelIndicaciones.Text = "No puedes mover una ficha que no es tuya!";
            }

        }

        private void FichaVerdeSW2_Click(object sender, EventArgs e)
        {
            if (colorFicha5 == "Verde")
            {
                fichaJ4[1] = 2;
                for (int i = 0; i < 136; i++)
                {
                    if (FichaVerdeSW2.Location == casillas[i])
                    {
                        casillaJ4[1] = i;
                    }
                }
                moverFicha(FichaVerdeSW2, casillaJ4[1], fichaJ4[1]);
                tirarFicha = 0;
            }
            else
            {
                labelIndicaciones.Text = "No puedes mover una ficha que no es tuya!";
            }

        }
        private void FichaVerdeSW3_Click(object sender, EventArgs e)
        {
            if (colorFicha5 == "Verde")
            {
                fichaJ4[2] = 3;
                for (int i = 0; i < 136; i++)
                {
                    if (FichaVerdeSW3.Location == casillas[i])
                    {
                        casillaJ4[2] = i;
                    }
                }
                moverFicha(FichaVerdeSW3, casillaJ4[2], fichaJ4[2]);
                tirarFicha = 0;
            }
            else
            {
                labelIndicaciones.Text = "No puedes mover una ficha que no es tuya!";
            }
        }
        private void FichaVerdeSW4_Click(object sender, EventArgs e)
        {
            if (colorFicha5 == "Verde")
            {
                fichaJ4[3] = 4;
                for (int i = 0; i < 136; i++)
                {
                    if (FichaVerdeSW4.Location == casillas[i])
                    {
                        casillaJ4[3] = i;
                    }
                }
                moverFicha(FichaVerdeSW4, casillaJ4[3], fichaJ4[3]);
                tirarFicha = 0;
            }
            else
            {
                labelIndicaciones.Text = "No puedes mover una ficha que no es tuya!";
            }

        }

        //FUNCIÓN PARA COMERTE UNA FICHA
        public void ComerFicha(PictureBox ficha, int casillaS)
        {
            if (tablero == "SW")
            {
                if (colorFichaCasilla[casillaS] == "Azul")
                {
                    if(casillaJ1[0] == casillaS)
                    {
                        FichaAzulSW1.Location = new Point(440, 120);
                        J1[0].Location = new Point(440, 120);
                    }
                    if (casillaJ1[1] == casillaS)
                    {
                        J1[1].Location = new Point(555, 120);
                    }
                    if (casillaJ1[2] == casillaS)
                    {
                        J1[2].Location = new Point(440, 280);
                    }
                    if (casillaJ1[3] == casillaS)
                    {
                        J1[3].Location = new Point(555, 280);
                    }

                }
                if (colorFichaCasilla[casillaS] == "Negra")
                {
                    if (casillaJ2[0] == casillaS)
                    {
                        FichaNegraSW1.Location = new Point(910, 600);
                        J2[0].Location = new Point(910, 600);
                    }
                    if (casillaJ2[1] == casillaS)
                    {
                        J2[1].Location = new Point(1030, 600);
                    }
                    if (casillaJ2[2] == casillaS)
                    {
                        J2[2].Location = new Point(910, 750);
                    }
                    if (casillaJ2[3] == casillaS)
                    {
                        J2[3].Location = new Point(1050, 750);
                    }

                }

                if (ficha == FichaAmarilloSW1)
                {
                    J3[0].Location = new Point(440, 600);
                }
                if (ficha == FichaAmarilloSW2)
                {
                    J3[1].Location = new Point(555, 600);
                }
                if (ficha == FichaAmarilloSW3)
                {
                    J3[2].Location = new Point(440, 760);
                }
                if (ficha == FichaAmarilloSW4)
                {
                    J3[3].Location = new Point(555, 760);
                }
                if (ficha == FichaVerdeSW1)
                {
                    J3[3].Location = new Point(910, 120);
                }
                if (ficha == FichaVerdeSW2)
                {
                    J3[3].Location = new Point(1050, 120);
                }
                if (ficha == FichaVerdeSW3)
                {
                    J3[3].Location = new Point(910, 280);
                }
                if (ficha == FichaVerdeSW4)
                {
                    J3[3].Location = new Point(1050, 280);
                }
            } 
        }

        //FUNCION PARA SUMAR PUNTOS CUANDO MATAS A UNA FICHA
        public void sumarPuntos(PictureBox ficha, int casilla)
        {
            ficha.Location = casillas[casilla + 20];
        }

        //FUNCION CONSULTAR FICHA QUE ESTA EN "X" CASILLA
        public string MirarFicha(int casilla, PictureBox ficha)
        {
            string color = "null";
            int i = 0;
            int e = 0;
            while (i < 4)
            {
                if (i == 0) //Comprobamos las fichas que hay almacenadas en cada casilla 
                {
                    while (e < 4)
                    {
                        if (e == 0)
                        {
                            if (casillaJ1[e] == casilla)
                            {
                                if (tablero == "SW")
                                {
                                    ficha = FichaAzulSW1;
                                    color = "Azul";
                                }
                                
                            }

                        }
                        if (e == 1)
                        {
                            if (casillaJ1[e] == casilla)
                            {
                                if (tablero == "SW")
                                {
                                    ficha = FichaAzulSW2;
                                    color = "Azul";
                                }
                                
                            }

                        }
                        if (e == 2)
                        {
                            if (casillaJ1[e] == casilla)
                            {
                                if (tablero == "SW")
                                {
                                    ficha = FichaAzulSW3;
                                    color = "Azul";
                                }
                               
                            }

                        }
                        if (e == 3)
                        {
                            if (casillaJ1[e] == casilla)
                            {
                                if (tablero == "SW")
                                {
                                    ficha = FichaAzulSW4;
                                    color = "Azul";
                                }
                                
                            }

                        }
                        e++;
                    }

                }
                if (i == 1)
                {
                    e = 0;
                    while (e < 4)
                    {

                        if (e == 0)
                        {
                            if (casillaJ1[e] == casilla)
                            {
                                if (tablero == "SW")
                                {
                                    ficha = FichaNegraSW1;
                                    color = "Negra";
                                }
                                
                            }

                        }
                        if (e == 1)
                        {
                            if (casillaJ1[e] == casilla)
                            {
                                if (tablero == "SW")
                                {
                                    ficha = FichaNegraSW2;
                                    color = "Negra";
                                }
                                
                            }

                        }
                        if (e == 2)
                        {
                            if (casillaJ1[e] == casilla)
                            {
                                if (tablero == "SW")
                                {
                                    ficha = FichaNegraSW3;
                                    color = "Negra";
                                }
                                
                            }

                        }
                        if (e == 3)
                        {
                            if (casillaJ1[e] == casilla)
                            {
                                if (tablero == "SW")
                                {
                                    ficha = FichaNegraSW4;
                                    color = "Negra";
                                }
                                
                            }

                        }

                        e++;
                    }
                }
                if (i == 2)
                {
                    e = 0;
                    while (e < 4)
                    {

                        if (e == 0)
                        {
                            if (casillaJ1[e] == casilla)
                            {
                                if (tablero == "SW")
                                {
                                    ficha = FichaAmarilloSW1;
                                    color = "Amarilla";
                                }
                                
                            }

                        }
                        if (e == 1)
                        {
                            if (casillaJ1[e] == casilla)
                            {
                                if (tablero == "SW")
                                {
                                    ficha = FichaAmarilloSW2;
                                    color = "Amarilla";
                                }
                               
                            }

                        }
                        if (e == 2)
                        {
                            if (casillaJ1[e] == casilla)
                            {
                                if (tablero == "SW")
                                {
                                    ficha = FichaAmarilloSW3;
                                    color = "Amarilla";
                                }
                                
                            }

                        }
                        if (e == 3)
                        {
                            if (casillaJ1[e] == casilla)
                            {
                                if (tablero == "SW")
                                {
                                    ficha = FichaAmarilloSW4;
                                    color = "Amarilla";
                                }
                                
                            }

                        }

                        e++;
                    }
                }
                if (i == 3)
                {
                    e = 0;
                    while (e < 4)
                    {
                        if (e == 0)
                        {
                            if (casillaJ1[e] == casilla)
                            {
                                if (tablero == "SW")
                                {
                                    ficha = FichaVerdeSW1;
                                    color = "Verde";
                                }
                                
                            }

                        }
                        if (e == 1)
                        {
                            if (casillaJ1[e] == casilla)
                            {
                                if (tablero == "SW")
                                {
                                    ficha = FichaVerdeSW2;
                                    color = "Verde";
                                }
                                
                            }

                        }
                        if (e == 2)
                        {
                            if (casillaJ1[e] == casilla)
                            {
                                if (tablero == "SW")
                                {
                                    ficha = FichaVerdeSW3;
                                    color = "Verde";
                                }
                                
                            }

                        }
                        if (e == 3)
                        {
                            if (casillaJ1[e] == casilla)
                            {
                                if (tablero == "SW")
                                {
                                    ficha = FichaVerdeSW4;
                                    color = "Verde";
                                }
                                
                            }

                        }
                        e++;
                    }
                }

                i++;
            }
            return color;
        }
        public int EntrarFichasCasa(int casilla, PictureBox ficha, int valorFicha, int casillaC)
        {
            if (tablero == "SW")
            {
                //Entramos las fichas a casa
                if ((casilla > 50 && colorFicha5 == "Amarillo") || (casilla > 118 && colorFicha5 == "Amarillo")) //CASA DE LAS AMARILLAS
                {
                    if (AmarillaFlag[valorFicha] == 0)
                    {
                        if (casilla == 51)
                        {
                            ficha.Location = casillas[164];
                            casillaC = 164;
                            contadorAmarillo[valorFicha] = 1;
                        }
                        if (casilla == 119)
                        {
                            ficha.Location = casillas[171];
                            casillaC = 171;
                            contadorAmarillo[valorFicha] = 1;
                        }
                        if (casilla == 52)
                        {
                            ficha.Location = casillas[165];
                            casillaC = 165;
                            contadorAmarillo[valorFicha] = 1;
                        }
                        if (casilla == 120)
                        {
                            ficha.Location = casillas[172];
                            casillaC = 172;
                            contadorAmarillo[valorFicha] = 1;
                        }
                        if (casilla == 53)
                        {
                            ficha.Location = casillas[166];
                            casillaC = 166;
                            contadorAmarillo[valorFicha] = 1;
                        }

                        if (casilla == 121)
                        {
                            ficha.Location = casillas[173];
                            casillaC = 173;
                            contadorAmarillo[valorFicha] = 1;
                        }
                        if (casilla == 54)
                        {
                            ficha.Location = casillas[167];
                            casillaC = 167;
                            contadorAmarillo[valorFicha] = 1;
                        }
                        if (casilla == 122)
                        {
                            ficha.Location = casillas[174];
                            casillaC = 174;
                            contadorAmarillo[valorFicha] = 1;
                        }
                        if (casilla == 55)
                        {
                            ficha.Location = casillas[168];
                            casillaC = 168;
                            contadorAmarillo[valorFicha] = 1;
                        }
                        if (casilla == 123)
                        {
                            ficha.Location = casillas[175];
                            casillaC = 175;
                            contadorAmarillo[valorFicha] = 1;
                        }
                        if (casilla == 56)
                        {
                            ficha.Location = casillas[169];
                            casillaC = 169;
                            contadorAmarillo[valorFicha] = 1;

                        }
                        if (casilla == 124)
                        {
                            ficha.Location = casillas[176];
                            casillaC = 176;
                            contadorAmarillo[valorFicha] = 1;
                        }
                        if (casilla == 57)
                        {
                            ficha.Location = casillas[170];
                            casillaC = 170;
                            contadorAmarillo[valorFicha] = 1;
                        }
                        if (casilla == 125)
                        {
                            ficha.Location = casillas[177];
                            casillaC = 177;
                            contadorAmarillo[valorFicha] = 1;

                        }
                        if (casilla == 58)
                        {
                            if (valorFicha == 1)
                            {
                                ficha.Location = casillas[204];
                            }
                            if (valorFicha == 2)
                            {
                                ficha.Location = casillas[205];
                            }
                            if (valorFicha == 3)
                            {
                                ficha.Location = casillas[206];
                            }
                            if (valorFicha == 4)
                            {
                                ficha.Location = casillas[207];
                            }
                            contCAmarillo++;
                        }
                        if (casilla == 126)
                        {
                            if (valorFicha == 1)
                            {
                                ficha.Location = casillas[204];
                            }
                            if (valorFicha == 2)
                            {
                                ficha.Location = casillas[205];
                            }
                            if (valorFicha == 3)
                            {
                                ficha.Location = casillas[206];
                            }
                            if (valorFicha == 4)
                            {
                                ficha.Location = casillas[207];
                            }
                            contCAmarillo++;
                        }
                        AmarillaFlag[valorFicha] = 1;
                    }
                    else
                    {
                        if (casilla > 58 || casilla > 126)
                        {
                            MessageBox.Show("Has sacado un número muy alto, no puedes avanzar");
                        }
                        else
                        {
                            ficha.Location = casillas[casilla];
                        }
                    }

                }
                if ((casilla > 67 && colorFicha5 == "Verde") || (casilla > 135 && colorFicha5 == "Verde")) //CASA DE LAS VERDES
                {
                    if (VerdeFlag[valorFicha] == 0)
                    {
                        if (casilla == 68)
                        {
                            ficha.Location = casillas[136];
                            casillaC = 136;
                            contadorVerde[valorFicha] = 1;
                        }
                        if (casilla == 136)
                        {
                            ficha.Location = casillas[143];
                            casillaC = 143;
                            contadorVerde[valorFicha] = 1;
                        }
                        if (casilla == 69)
                        {
                            ficha.Location = casillas[137];
                            casillaC = 137;
                            contadorVerde[valorFicha] = 1;

                        }
                        if (casilla == 137)
                        {
                            ficha.Location = casillas[144];
                            casillaC = 144;
                            contadorVerde[valorFicha] = 1;
                        }
                        if (casilla == 70)
                        {
                            ficha.Location = casillas[138];
                            casillaC = 138;
                            contadorVerde[valorFicha] = 1;
                        }
                        if (casilla == 138)
                        {
                            ficha.Location = casillas[145];
                            casillaC = 145;
                            contadorVerde[valorFicha] = 1;
                        }
                        if (casilla == 71)
                        {
                            ficha.Location = casillas[139];
                            casillaC = 139;
                            contadorVerde[valorFicha] = 1;
                        }
                        if (casilla == 139)
                        {
                            ficha.Location = casillas[146];
                            casillaC = 146;
                            contadorVerde[valorFicha] = 1;
                        }
                        if (casilla == 72)
                        {
                            ficha.Location = casillas[140];
                            casillaC = 140;
                            contadorVerde[valorFicha] = 1;
                        }
                        if (casilla == 140)
                        {
                            ficha.Location = casillas[147];
                            casillaC = 147;
                            contadorVerde[valorFicha] = 1;
                        }
                        if (casilla == 73)
                        {
                            ficha.Location = casillas[141];
                            casillaC = 141;
                            contadorVerde[valorFicha] = 1;
                        }
                        if (casilla == 141)
                        {
                            ficha.Location = casillas[148];
                            casillaC = 148;
                            contadorVerde[valorFicha] = 1;
                        }
                        if (casilla == 74)
                        {
                            ficha.Location = casillas[142];
                            casillaC = 142;
                            contadorVerde[valorFicha] = 1;
                        }
                        if (casilla == 142)
                        {
                            ficha.Location = casillas[149];
                            casillaC = 149;
                            contadorVerde[valorFicha] = 1;
                        }
                        if (casilla == 75)
                        {
                            if (valorFicha == 1)
                            {
                                ficha.Location = casillas[200];
                            }
                            if (valorFicha == 2)
                            {
                                ficha.Location = casillas[201];
                            }
                            if (valorFicha == 3)
                            {
                                ficha.Location = casillas[202];
                            }
                            if (valorFicha == 4)
                            {
                                ficha.Location = casillas[203];
                            }
                            
                            contCVerde++;

                        }
                        if (casilla == 143)
                        {
                            if (valorFicha == 1)
                            {
                                ficha.Location = casillas[200];
                            }
                            if (valorFicha == 2)
                            {
                                ficha.Location = casillas[201];
                            }
                            if (valorFicha == 3)
                            {
                                ficha.Location = casillas[202];
                            }
                            if (valorFicha == 4)
                            {
                                ficha.Location = casillas[203];
                            }
                            contCVerde++;
                        }
                        VerdeFlag[valorFicha] = 1;
                    }
                    else
                    {
                        if (casilla > 75 || casilla > 143)
                        {
                            MessageBox.Show("Has sacado un número muy alto, no puedes avanzar");
                        }
                        else
                        {
                            ficha.Location = casillas[casilla];
                        }
                    }
                }
                if ((casilla > 16 && colorFicha5 == "Negra") || (casilla > 84 && colorFicha5 == "Negra")) //CASA DE LAS NEGRAS
                {
                    if (NegraFlag[valorFicha] == 0)
                    {
                        if (casilla == 17)
                        {
                            ficha.Location = casillas[150];
                            casillaC = 150;
                            contadorNegro[valorFicha] = 1;

                        }
                        if (casilla == 85)
                        {
                            ficha.Location = casillas[157];
                            casillaC = 157;
                            contadorNegro[valorFicha] = 1;

                        }
                        if (casilla == 18)
                        {
                            ficha.Location = casillas[151];
                            casillaC = 151;
                            contadorNegro[valorFicha] = 1;

                        }
                        if (casilla == 86)
                        {
                            ficha.Location = casillas[158];
                            casillaC = 158;
                            contadorNegro[valorFicha] = 1;

                        }
                        if (casilla == 19)
                        {
                            ficha.Location = casillas[152];
                            casillaC = 152;
                            contadorNegro[valorFicha] = 1;

                        }
                        if (casilla == 87)
                        {
                            ficha.Location = casillas[159];
                            casillaC = 159;
                            contadorNegro[valorFicha] = 1;
                        }
                        if (casilla == 20)
                        {
                            ficha.Location = casillas[153];
                            casillaC = 153;
                            contadorNegro[valorFicha] = 1;
                        }
                        if (casilla == 88)
                        {
                            ficha.Location = casillas[160];
                            casillaC = 160;
                            contadorNegro[valorFicha] = 1;
                        }
                        if (casilla == 21)
                        {
                            ficha.Location = casillas[154];
                            casillaC = 154;
                            contadorNegro[valorFicha] = 1;
                        }
                        if (casilla == 89)
                        {
                            ficha.Location = casillas[161];
                            casillaC = 161;
                            contadorNegro[valorFicha] = 1;
                        }
                        if (casilla == 22)
                        {
                            ficha.Location = casillas[155];
                            casillaC = 155;
                            contadorNegro[valorFicha] = 1;
                        }
                        if (casilla == 90)
                        {
                            ficha.Location = casillas[162];
                            casillaC = 162;
                            contadorNegro[valorFicha] = 1;
                        }
                        if (casilla == 23)
                        {
                            ficha.Location = casillas[156];
                            casillaC = 156;
                            contadorNegro[valorFicha] = 1;
                        }
                        if (casilla == 91)
                        {
                            ficha.Location = casillas[163];
                            casillaC = 163;
                            contadorNegro[valorFicha] = 1;
                        }
                        if (casilla == 24)
                        {
                            if (valorFicha == 1)
                            {
                                ficha.Location = casillas[196];
                            }
                            if (valorFicha == 2)
                            {
                                ficha.Location = casillas[197];
                            }
                            if (valorFicha == 3)
                            {
                                ficha.Location = casillas[198];
                            }

                            if (valorFicha == 4)
                            {
                                ficha.Location = casillas[199];
                            }
                            contCNegro++;
                        }
                        if (casilla == 92)
                        {
                            if (valorFicha == 1)
                            {
                                ficha.Location = casillas[196];
                            }
                            if (valorFicha == 2)
                            {
                                ficha.Location = casillas[197];
                            }
                            if (valorFicha == 3)
                            {
                                ficha.Location = casillas[198];
                            }

                            if (valorFicha == 4)
                            {
                                ficha.Location = casillas[199];
                            }
                            contCNegro++;
                        }
                        NegraFlag[valorFicha] = 1;
                    }
                    else
                    {
                        if (casilla > 24 || casilla > 92)
                        {
                            MessageBox.Show("Has sacado un número muy alto, no puedes avanzar");
                        }
                        else
                        {
                            ficha.Location = casillas[casilla];
                        }
                    }
                }
                if ((casilla > 33 && colorFicha5 == "Azul") || (casilla > 101 && colorFicha5 == "Azul")) //CASA DE LAS AZULES
                {
                    if (AzulFlag[valorFicha] == 0)
                    {
                        if (casilla == 34)
                        {
                            ficha.Location = casillas[178];
                            casillaC = 178;
                            contadorAzul[valorFicha] = 1;
                            sumaFCasa = 1;
                        }
                        if (casilla == 102)
                        {
                            ficha.Location = casillas[185];
                            casillaC = 185;
                            contadorAzul[valorFicha] = 1;
                            sumaFCasa = 1;
                        }
                        if (casilla == 35)
                        {
                            ficha.Location = casillas[179];
                            casillaC = 179;
                            contadorAzul[valorFicha] = 1;
                            sumaFCasa = 2;
                        }
                        if (casilla == 103)
                        {
                            ficha.Location = casillas[186];
                            casillaC = 186;
                            contadorAzul[valorFicha] = 1;
                            sumaFCasa = 2;
                        }
                        if (casilla == 36)
                        {
                            ficha.Location = casillas[180];
                            casillaC = 180;
                            contadorAzul[valorFicha] = 1;
                            sumaFCasa = 3;
                        }
                        if (casilla == 104)
                        {
                            ficha.Location = casillas[187];
                            casillaC = 187;
                            contadorAzul[valorFicha] = 1;
                            sumaFCasa = 3;
                        }
                        if (casilla == 37)
                        {
                            ficha.Location = casillas[181];
                            casillaC = 181;
                            contadorAzul[valorFicha] = 1;
                            sumaFCasa = 4;
                        }
                        if (casilla == 105)
                        {
                            ficha.Location = casillas[188];
                            casillaC = 188;
                            contadorAzul[valorFicha] = 1;
                            sumaFCasa = 4;
                        }
                        if (casilla == 38)
                        {
                            ficha.Location = casillas[182];
                            casillaC = 182;
                            contadorAzul[valorFicha] = 1;
                            sumaFCasa = 5;
                        }
                        if (casilla == 106)
                        {
                            ficha.Location = casillas[189];
                            casillaC = 189;
                            contadorAzul[valorFicha] = 1;
                            sumaFCasa = 5;
                        }
                        if (casilla == 39)
                        {
                            ficha.Location = casillas[183];
                            casillaC = 183;
                            contadorAzul[valorFicha] = 1;
                            sumaFCasa = 6;
                        }
                        if (casilla == 107)
                        {
                            ficha.Location = casillas[190];
                            casillaC = 190;
                            contadorAzul[valorFicha] = 1;
                            sumaFCasa = 6;
                        }
                        if (casilla == 40)
                        {
                            ficha.Location = casillas[184];
                            casillaC = 184;
                            contadorAzul[valorFicha] = 1;
                            sumaFCasa = 7;
                        }
                        if (casilla == 108)
                        {
                            ficha.Location = casillas[191];
                            casillaC = 191;
                            contadorAzul[valorFicha] = 1;
                            sumaFCasa = 7;
                        }
                        ////CASA (TRIANGULO)
                        //if (casilla == 41)
                        //{
                        //    if (valorFicha == 1)
                        //    {
                        //        ficha.Location = casillas[192];
                        //    }
                        //    if (valorFicha == 2)
                        //    {
                        //        ficha.Location = casillas[193];
                        //    }
                        //    if (valorFicha == 3)
                        //    {
                        //        ficha.Location = casillas[194];
                        //    }

                        //    if (valorFicha == 4)
                        //    {
                        //        ficha.Location = casillas[192];
                        //    }

                        //    contadorAzul[valorFicha] = 1;
                        //}
                        //if (casilla == 109)
                        //{
                        //    if (valorFicha == 1)
                        //    {
                        //        ficha.Location = casillas[192];
                        //    }
                        //    if (valorFicha == 2)
                        //    {
                        //        ficha.Location = casillas[193];
                        //    }
                        //    if (valorFicha == 3)
                        //    {
                        //        ficha.Location = casillas[194];
                        //    }
                        //    if (valorFicha == 4)
                        //    {
                        //        ficha.Location = casillas[192];
                        //    }
                        //    contadorAzul[valorFicha] = 1;

                        //}
                        AzulFlag[valorFicha] = 1;
                    }
                    else
                    {
                        int GSum = sumaFCasa;
                        sumaFCasa = sumaFCasa + valorDado;
                        if (sumaFCasa > 8)
                        {
                            MessageBox.Show("Has sacado un número muy alto, no puedes avanzar");
                            ficha.Location = casillas[casilla - valorDado];
                            casillaC = casilla - valorDado;
                            sumaFCasa = GSum;
                        }
                        else
                        {
                            if (sumaFCasa == 8)
                            {
                                if (valorFicha == 1)
                                {
                                    ficha.Location = casillas[192];
                                    casilla = 192;
                                }
                                if (valorFicha == 2)
                                {
                                    ficha.Location = casillas[193];
                                    casilla = 193;
                                }
                                if (valorFicha == 3)
                                {
                                    ficha.Location = casillas[194];
                                    casilla = 194;
                                }

                                if (valorFicha == 4)
                                {
                                    ficha.Location = casillas[195];
                                    casilla = 195;
                                }
                            }
                            else
                            {
                                ficha.Location = casillas[casilla];
                            }
                            casillaC = casilla;
                        }
                    }
                }

            }
            return casillaC;
        }
        public void moverFicha(PictureBox ficha, int casilla, int valor)
        {
            int casillaActual;
            int casillaSiguiente;

            if (tablero == "SW")
            {
                if (ficha == FichaAmarilloSW1 || ficha == FichaAmarilloSW2 || ficha == FichaAmarilloSW3 || ficha == FichaAmarilloSW4)
                {
                    fichacolor = "Amarilla";
                    jugador = 3;
                    
                        casillaJ3[valor-1] = casilla+valorDado;
                    

                }
                else if (ficha == FichaNegraSW1 || ficha == FichaNegraSW2 || ficha == FichaNegraSW3 || ficha == FichaNegraSW4)
                {
                    fichacolor = "Negra";
                    jugador = 2;
                    casillaJ2[valor - 1] = casilla+valorDado;
                }
                else if (ficha == FichaAzulSW1 || ficha == FichaAzulSW2 || ficha == FichaAzulSW3 || ficha == FichaAzulSW4)
                {
                    fichacolor = "Azul";
                    jugador = 1;
                    casillaJ1[valor - 1] = casilla+valorDado;
                }
                else if (ficha == FichaVerdeSW1 || ficha == FichaVerdeSW2 || ficha == FichaVerdeSW3 || ficha == FichaVerdeSW4)
                {
                    fichacolor = "Verde";
                    jugador = 4;
                    casillaJ4[valor - 1] = casilla+valorDado;
                }
            }
            
            //Casilla a la que avanzamos
            casillasFichas = casilla + valorDado;
                        
            if (casilla > 67 && casilla < 135) //Caso en el que se trata de una ficha en la 2a posición
            {
                casillaSiguiente = casillasFichas - 68; //Casilla que tiene que comprobar, a la que va
                casillaActual = casilla - 68;
                casillasOcupadas[casillaActual]--; //Dejamos libre la casilla anterior

                //Si no hay nadie que la ponga a la izquierda
                if (casillasOcupadas[casillaSiguiente] == 0)
                {
                    casillasFichas = casillaSiguiente;
                }

            }
            else //Caso en el que se trata de una ficha en la 1a posición
            {
                if (casilla == 67 && colorFicha5 != "Verde") //Caso en el que está en la 68 y tiene que volver a la 1
                {
                    casillasFichas = valorDado - 1;
                }
                else
                {
                    casillasFichas = casilla + valorDado; //Donde va a ir la ficha
                }                
                casillaActual = casilla;

                casillasOcupadas[casilla]--; //Dejamos libre la casilla anterior
                casillaSiguiente = casillasFichas;

            }
            //Indicar que la ficha se va de una casilla de salida
            if (casilla == 21 || casilla == 89 || casilla == 38 || casilla == 106 || casilla == 55 || casilla == 123 || casilla == 4 || casilla == 72)
            {
                casillaSalida[turnoActual]++;
            }
            //Caso en el que habían dos fichas
            if ((casilla == 21 || casilla == 89 || casilla == 38 || casilla == 106 || casilla == 55 || casilla == 123 || casilla == 4 || casilla == 72) && casillasOcupadas[comprobarAnterior] == 1) //1 porque solo lo hace si antes había 2 fichas, no ponemos 2 porque ya le hemos restado
            {
                if (casilla == 21 || casilla == 38 || casilla == 55 || casilla == 4)
                {
                    //mueve la izquierda, se queda derecha
                    IoD[casilla] = 2;
                }
                else
                {
                    //mueve la derecha, se queda izquierda
                    IoD[casillaActual] = 1;
                }
            }
            else
            {
                if (casilla < 68) //Modificar IoD en 1a posición al irse de la casilla
                {
                    if (IoD[casilla] == 3)
                    {
                        IoD[casilla] = 2;
                    }
                    else if (IoD[casilla] == 1)
                    {
                        IoD[casilla] = 0;
                    }

                }
                else if (casilla > 67 && casilla < 135) //Modificar IoD en 2a posición al irse de la casilla
                {
                    if (IoD[casilla - 68] == 3)
                    {
                        IoD[casilla - 68] = 1;
                    }
                    else if (IoD[casilla - 68] == 2)
                    {
                        IoD[casilla - 68] = 0;
                    }
                }

            }

            //Barrera
            if (casillasOcupadas[casillaSiguiente] == 2)
            {
                MessageBox.Show("No puedes mover esa ficha porque hay una barrera, intenta mover otra");
            }

            //Te puedes comer a la ficha que sea de color diferente al tuyo o hacer barrera moviendote a la 2a posición

            if (casillasOcupadas[casillaSiguiente] == 1)
            {                
                //string color = MirarFicha(casillasFichas, ficha); //Miramos el color para saber si hay que ponerse a su lado o matarla

                if (colorFichaCasilla[casillaSiguiente] != null)
                {
                    //Comentada porque falta repasar la función
                    MessageBox.Show("¡Has matado a una ficha del oponente!");
                    //if (casillasFichas != 4 && fichacolor != colorFichaCasilla[casillaSiguiente] || casillasFichas != 11 && fichacolor != colorFichaCasilla[casillaSiguiente] || casillasFichas != 16 && fichacolor != colorFichaCasilla[casillaSiguiente] ||
                    //   casillasFichas != 21 && fichacolor != colorFichaCasilla[casillaSiguiente] || casillasFichas != 28 && fichacolor != colorFichaCasilla[casillaSiguiente] || casillasFichas != 33 && fichacolor != colorFichaCasilla[casillaSiguiente] ||
                    //   casillasFichas != 38 && fichacolor != colorFichaCasilla[casillaSiguiente] || casillasFichas != 45 && fichacolor != colorFichaCasilla[casillaSiguiente] || casillasFichas != 50 && fichacolor != colorFichaCasilla[casillaSiguiente] ||
                    //   casillasFichas != 55 && fichacolor != colorFichaCasilla[casillaSiguiente] || casillasFichas != 62 && fichacolor != colorFichaCasilla[casillaSiguiente] || casillasFichas != 67 && fichacolor != colorFichaCasilla[casillaSiguiente])
                    //{
                    //    //Se come la ficha

                    //    ComerFicha(ficha,casillaSiguiente);
                    //    labelIndicaciones.Text = "Te has comido una ficha, \n haz click en la ficha que quieres mover!";
                    //    sumarPuntos(ficha, casilla);
                    //}
                }
                else //Consultamos si nos tenemos que colocar a la izquierda o a la derecha en la nueva casilla
                {
                    if (IoD[casillaSiguiente] == 1)
                    {
                        //Nos ponemos a su derecha
                        casillasFichas = casillaSiguiente + 68;
                    }
                    else if (IoD[casillaSiguiente] == 2)
                    {
                        //Nos ponemos a su izquierda
                        casillasFichas = casillaSiguiente;
                    }
                }
                
            }
            
            if (contadorAzul[valor] == 0 || contadorNegro[valor] == 0 || contadorAmarillo[valor] == 0 || contadorVerde[valor] == 0)
            {

                ficha.Location = casillas[casillasFichas]; //Movemos
                if (colorFicha5 == "Azul")
                {
                    if (casillasFichas == 34 || casillasFichas == 35 || casillasFichas == 36 || casillasFichas == 37 || casillasFichas == 38)
                    {
                        contadorAzul[valor] = 1;
                    }
                    if (casillasFichas == 17 || casillasFichas == 18 || casillasFichas == 19 || casillasFichas == 20 || casillasFichas == 21)
                    {
                        contadorNegro[valor] = 1;
                    }
                    if (casillasFichas == 51 || casillasFichas == 52 || casillasFichas == 53 || casillasFichas == 54 || casillasFichas == 55)
                    {
                        contadorAmarillo[valor] = 1;
                    }
                    if (casillasFichas == 0 || casillasFichas == 1 || casillasFichas == 2 || casillasFichas == 3 || casillasFichas == 4)
                    {
                        contadorVerde[valor] = 1;
                    }
                }
            }
            if (fichacolor == "Amarillo" && casillasFichas > 50 && contadorAmarillo[valor] == 1 || (casillasFichas > 118 && fichacolor == "Amarillo") && contadorAmarillo[valor] == 1 || (casillasFichas > 67 && fichacolor == "Verde") && contadorVerde[valor] == 0 || (casillasFichas > 135 && fichacolor == "Verde") && contadorVerde[valor] == 1 || (casillasFichas > 33 && fichacolor == "Azul" && contadorAzul[valor] == 1) || (casillasFichas > 101 && fichacolor == "Azul" && contadorAzul[valor] == 1) || (casillasFichas > 16 && fichacolor == "Negra") && contadorNegro[valor] == 1 || (casillasFichas > 84 && fichacolor == "Negra" && contadorNegro[valor] == 1))
            {

                casilla = EntrarFichasCasa(casillasFichas, ficha, valor, casillaCasa);
                if (colorFicha5 == "Azul")
                {
                    if (valor == 1)
                    {
                        casillaJ1[0] = casilla;
                    }
                    if (valor == 2)
                    {
                        casillaJ1[1] = casilla;
                    }
                    if (valor == 3)
                    {
                        casillaJ1[2] = casilla;
                    }
                    if (valor == 4)
                    {
                        casillaJ1[3] = casilla;
                    }
                }
                if (colorFicha5 == "Negra")
                {
                    if (valor == 1)
                    {
                        casillaJ2[0] = casilla;
                    }
                    if (valor == 2)
                    {
                        casillaJ2[1] = casilla;
                    }
                    if (valor == 3)
                    {
                        casillaJ2[2] = casilla;
                    }
                    if (valor == 4)
                    {
                        casillaJ2[3] = casilla;
                    }
                }
                if (colorFicha5 == "Amarilla")
                {
                    if (valor == 1)
                    {
                        casillaJ3[0] = casilla;
                    }
                    if (valor == 2)
                    {
                        casillaJ3[1] = casilla;
                    }
                    if (valor == 3)
                    {
                        casillaJ3[2] = casilla;
                    }
                    if (valor == 4)
                    {
                        casillaJ3[3] = casilla;
                    }
                }
                if (colorFicha5 == "Verde")
                {
                    if (valor == 1)
                    {
                        casillaJ4[0] = casilla;
                    }
                    if (valor == 2)
                    {
                        casillaJ4[1] = casilla;
                    }
                    if (valor == 3)
                    {
                        casillaJ4[2] = casilla;
                    }
                    if (valor == 4)
                    {
                        casillaJ4[3] = casilla;
                    }
                }

            }

            casillasOcupadas[casillaSiguiente]++; //Añadimos la ficha al vector de ocupados
                                                  //MODIFICAMOS IoD DE LA CASILLA A LA QUE VAMOS

            if (IoD[casillaSiguiente] == 0) //No hay ninguna
            {
                IoD[casillaSiguiente] = 1;
            }
            else if (IoD[casillaSiguiente] == 1) //Hay una ficha en la izquierda
            {
                IoD[casillaSiguiente] = 3;
            }
            else if (IoD[casillaSiguiente] == 2) //Hay una ficha en la derecha
            {
                IoD[casillaSiguiente] = 3;
            }
            else
            {
                //nada ya que IoD=3, ya hay 2 fichas en esa casilla
            }

            //Si ganamos la partida
            if (contCAzul == 4)
            {
                MessageBox.Show("Has ganado esta partida");
            }
            if (contCNegro == 4)
            {
                MessageBox.Show("Has ganado esta partida");
            }
            if (contCAmarillo == 4)
            {
                MessageBox.Show("Has ganado esta partida");
            }
            if (contCVerde == 4)
            {
                MessageBox.Show("Has ganado esta partida");
            }


            string mensaje = "51/1/" + tablero + "/" + nameJugadores[turnoJ] + "/" + ficha.Location + "/" + fichacolor + "/" + valor + "/" + idPartida + "/" + partidaEmpezada[turnoActual] + "/" + turnoJ + "/" + casillaSalida[turnoActual] + "/" + casillasOcupadas[casillaActual] + "/" + IoD[casillaActual] + "/" + casillaActual + "/" + casillasOcupadas[casillaSiguiente] + "/" + IoD[casillaSiguiente] + "/" + casillaSiguiente;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

        }     
        private void pictureBoxStarWars_MouseHover(object sender, EventArgs e)
        {
            pictureBoxStarWars.Image = cliente.Properties.Resources.TableroSW;
            pictureBoxStarWars.Size = new System.Drawing.Size(200, 200);
        }

        private void pictureBoxStarWars_MouseLeave(object sender, EventArgs e)
        {
            pictureBoxStarWars.Image = cliente.Properties.Resources.star_wars_logo_PNG20;
            pictureBoxStarWars.Size = new System.Drawing.Size(196, 90);
        }

        private void pictureBoxCasaPapel_MouseHover(object sender, EventArgs e)
        {
            pictureBoxCasaPapel.Image = cliente.Properties.Resources.image_6483441;
            pictureBoxCasaPapel.Size = new System.Drawing.Size(200, 200);
        }

        private void pictureBoxCasaPapel_MouseLeave(object sender, EventArgs e)
        {
            pictureBoxCasaPapel.Image = cliente.Properties.Resources.logo;
            pictureBoxCasaPapel.Size= new System.Drawing.Size(297, 44);
        }

        private void pictureBoxHarryPotter_MouseHover(object sender, EventArgs e)
        {
            pictureBoxHarryPotter.Image = cliente.Properties.Resources.image_6483441__1_;
                pictureBoxHarryPotter.Size = new System.Drawing.Size(200, 200);
        }

        private void pictureBoxHarryPotter_MouseLeave(object sender, EventArgs e)
        {
            pictureBoxHarryPotter.Image = cliente.Properties.Resources._142_1428900_harry_potter_collection_image_lego_harry_potter_logo;
            pictureBoxHarryPotter.Size = new System.Drawing.Size(177, 101);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            string mensaje = "0/";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // Nos desconectamos
            this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);
            server.Close();
        }
    }
}


