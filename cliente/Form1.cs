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

namespace cliente
{
    public partial class Parchís : Form
    {

        Socket server;
        Thread Atender;

        
        
        string [] nameJugadores = new string[4] ; //Ponemos vector de 3 ja que para jugar a nuestro parchis hay un maximo de 4 jugadores
        int jugar = 0;
        int jugadores = 0;
        int idPartida;
        

        string autorMensaje;
        string mensajeChat;
        

        delegate void DelegadorParaEscribir(string mensaje);
        delegate void DelegadoGB(GroupBox mensaje);
        // delegate void DelegadaDGV(DataGridView mensaje);

        // Puerto Carla : 50015
        // Puerto Marta : 50016
        // Puerto Lucia : 50017
        //int puerto = 50017;
        int puerto = 9980;
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
            TableroHarryPotter.Visible = false;
            TableroCasaDePapel.Visible = false;
            groupBoxSeleccionTableros.Visible = false;
            EmpezarPartidaButton.Visible = false;


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
        //FUNCIÓN TENDER SERVIDOR
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
                    case 5:     //INICIO SESION

                        Invoke(new Action(() =>
                        {

                            if (trozos[1] == "INCORRECTO")
                            {
                                MessageBox.Show("Nombre de usuario o contraseña incorrecta.");
                                this.BackColor = Color.LightCoral;
                            }

                            else
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
                                // Enviamos al servidor el nombre tecleado
                                byte[] msg = System.Text.Encoding.ASCII.GetBytes(respuestaInvitacion);
                                server.Send(msg);
                                //Ocultamos para esperar a que empiece la partida. 
                                PeticionesGroupBox.Visible = false;
                                jugar_button.Visible = false;
                            }
                            if (result == DialogResult.No)
                            {
                                // Enviamos al servidor el nombre del usuario       
                                string respuestaInvitacion = "10/" + usuario_ini.Text + "/" + trozos[2] + "/No";
                                // Enviamos al servidor el nombre tecleado
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
                                nameJugadores[1] = trozos[4];  
                            }

                            if (trozos.Length == 6)
                            {
                                MessageBox.Show("Ha aceptado la invitación " + trozos[5], "invitacion", MessageBoxButtons.OK);
                                nameJugadores[2] = trozos[5];
                            }
                            if (trozos.Length == 7)
                            {
                                MessageBox.Show("Ha aceptado la invitación " + trozos[6], "invitacion", MessageBoxButtons.OK);
                                nameJugadores[3] = trozos[6];
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
                        Chat.Items.Add(autorMensaje+":"+mensajeChat);
                        break;
                    case 12:      //TABLEROS
                        if (trozos[2]=="SW")
                        {
                            Invoke(new Action(() =>
                            {
                                TableroStarWars.Visible = true;
                                groupBoxChat.Visible = true;
                                groupBoxSeleccionTableros.Visible = false;
                                EmpezarPartidaButton.Visible = false;
                            }));   
                        }
                        if (trozos[2] == "HP")
                        {
                            Invoke(new Action(() =>
                            {
                                TableroHarryPotter.Visible = true;
                                groupBoxChat.Visible = true;
                                groupBoxSeleccionTableros.Visible = false;
                                EmpezarPartidaButton.Visible = false;
                            }));
                        }
                        if (trozos[2] == "CP")
                        {
                             Invoke(new Action(() =>
                             {
                                 TableroCasaDePapel.Visible = true;
                                 groupBoxChat.Visible = true;
                                 groupBoxSeleccionTableros.Visible = false;
                                 EmpezarPartidaButton.Visible = false;
                                 //peticiones.BackgroundImage = Properties.Resources.FondoCasaPapel;
                                 //this.BackgroundImage = Properties.Resources.FondoCasaPapel;
                             }));
                        }
                        break;
                    case 14:      //BOTON JUGAR
                        MessageBox.Show(trozos[1]);
                        break;
                }
            }
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

            // Nos desconectamos
            this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);
            server.Close();
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

                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
            }

            //Petición de tablones jugados por el usuario (2)
            else if (tablones.Checked && (usuario_consulta.Text.Length > 0))
            {
                string mensaje = "2/" + usuario_consulta.Text;
                // Enviamos al servidor el nombre del usuario
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
            }
            //Petición ID del usuario (3)
            else if (id_usuario.Checked && (usuario_consulta.Text.Length > 0))
            {
                // Enviamos al servidor el nombre del usuario
                string mensaje = "3/" + usuario_consulta.Text;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
            }

            //Aviso petición no seleccionada.
            else if (String.IsNullOrWhiteSpace(usuario_consulta.Text)|| (usuario_consulta.Text.Length > 0) && !partidas_ganadas.Checked && !tablones.Checked && !id_usuario.Checked)
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
                // Enviamos al servidor el nombre tecleado
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
                // Enviamos al servidor el nombre del usuario
                string mensaje = "5/" + usuario_ini.Text + "/" + contraseña_ini.Text;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        //ENTRAR CON "ENTER" (5)
        private void entrar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                // Enviamos al servidor el nombre del usuario
                string mensaje = "5/" + usuario_ini.Text + "/" + contraseña_ini.Text;
                // Enviamos al servidor el nombre tecleado
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

            // Enviamos al servidor el nombre del usuario
            string mensaje = "6/" + usuario_ini.Text + "/" + contraseña_ini.Text;
            // Enviamos al servidor el nombre tecleado
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
            // Enviamos al servidor el nombre del usuario
            string mensaje = "8/" + usuario_ini.Text;
            // Enviamos al servidor el nombre tecleado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        //BOTON JUGAR
        private void jugar_button_Click(object sender, EventArgs e)
        {
            jugar = 1;
            // Enviamos al servidor el nombre del usuario
            string mensaje = "14/" + usuario_ini.Text;
            // Enviamos al servidor el nombre tecleado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            PeticionesGroupBox.Visible = false;
            jugar_button.Visible = false;
            groupBoxSeleccionTableros.Visible=false;
        }
        //-----------------------------------------------------------------------------------------------------------------------------------
        //BOTON DESCONECTARSE DEL SERVIDOR MENU SUPERIOR (0)
        private void desconectarseDelServidorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Mensaje de desconexión
            string mensaje = "0/";

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // Nos desconectamos
            this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);
            server.Close();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        //BOTON SALIR DE LISTA CONECTADOS (8)
        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Enviamos al servidor el nombre del usuario
            string mensaje = "8/" + usuario_ini.Text;
            // Enviamos al servidor el nombre tecleado
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

                    // Enviamos al servidor el nombre del usuario       
                    string mensaje = "9/" + usuario_ini.Text + "/" + nameJugadores[jugadores];
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                    jugadores++;
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
            string mensaje = "11/" + idPartida + "/" + usuario_ini.Text + "/" + MensajeTextBox.Text ;
            // Enviamos al servidor el nombre tecleado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            Chat.Items.Add(usuario_ini.Text + ":" + MensajeTextBox.Text);
        }
        //-----------------------------------------------------------------------------------------------------------------------------------
        //SELECCION DE TABLERO (12)
        private void pictureBoxStarWars_Click(object sender, EventArgs e)
        {
            // Envias el mensaje
            string mensaje = "12/SW/"+idPartida;
            // Enviamos al servidor el nombre tecleado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void pictureBoxCasaPapel_Click(object sender, EventArgs e)
        {
            // Envias el mensaje
            string mensaje = "12/CP"+"/"+idPartida;
            // Enviamos al servidor el nombre tecleado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void pictureBoxHarryPotter_Click(object sender, EventArgs e)
        {
            // Envias el mensaje
            string mensaje = "12/HP"+"/"+idPartida;
            // Enviamos al servidor el nombre tecleado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            
        }
        //-----------------------------------------------------------------------------------------------------------------------------------
        //BOTON EMPEZAR PARTIDA
        private void EmpezarPartidaButton_Click(object sender, EventArgs e)
        {
            groupBoxSeleccionTableros.Visible = true;
        }
    }
}

