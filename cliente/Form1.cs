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

        string chatName;
        
        string [] nameInvitado = new string[3] ; //Ponemos vector de 3 ja que para jugar a nuestro parchis hay un maximo de 4 jugadores
        string nameInvitador;
        int jugar = 0;
        int jugadores = 0;

        string autorMensaje;
        string mensajeChat;
        string receptorMensaje;

        delegate void DelegadorParaEscribir(string mensaje);
        delegate void DelegadoGB(GroupBox mensaje);
       // delegate void DelegadaDGV(DataGridView mensaje);

        // Puerto Carla : 50015
        // Puerto Marta : 50016
        // Puerto Lucia : 50017
        //int puerto = 50017;
        int puerto = 9690;
        public Parchís()
        {
            
            InitializeComponent();
            //Es necesario para que los elementos de los formularios puedan ser accedidos desde threads diferentes a los que los crearon.
            CheckForIllegalCrossThreadCalls = false;
            ListaConectadosDG.MultiSelect = true;
            ListaConectadosDG.SelectionMode = DataGridViewSelectionMode.CellSelect;
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

            //registro.Visible = false;
            

            IPAddress direc = IPAddress.Parse("192.168.56.103");
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

        //

    
        public void RecibirChat(string mensaje)
        {
            Chat.Items.Add(chatName + ": " + mensaje);
        }
        //public void PonVisibleGB(GroupBox nombre)
        //{
        //    nombre.Visible = true;
        //}
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
                        Invoke(new Action(() =>
                        {
                            int partidasganadas = Convert.ToInt32(trozos[1]);
                            MessageBox.Show(usuario_consulta.Text + " ha ganado " + partidasganadas + " partidas");
                        }));
                        break;
                    case 2:     //TABLONES JUGADOS
                        Invoke(new Action(() =>
                        {
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
                        }));
                        break;
                    case 3:     //ID JUGADOR
                        Invoke(new Action(() =>
                        {
                            int idjugador = Convert.ToInt32(trozos[1]);
                            MessageBox.Show("La ID de " + usuario_consulta.Text + " es: " + idjugador);
                        }));
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
                                title.Visible = false;
                                menuStrip_usuario.Visible = true;
                                holaToolStripMenuItem.Text = "Hola " + usuario_ini.Text;
                                //menuStrip_usuario.("Hola %s",usuario_ini);
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
                                //int n = ListaConectadosDG.Rows.Add();
                                ListaConectadosDG.Rows[n].Cells[0].Value = trozos[n + 2];
                                n = n + 1;
                            }
                            this.ListaConectadosDG.Rows[0].Cells[0].Selected = false;

                            //ListaConectadosDG.Rows.Clear();
                            //int num = Convert.ToInt32(trozos[1]);
                            //int i;
                            //for (i = 0; i < num; i++)
                            //{
                            //    int n = ListaConectadosDG.Rows.Add();
                            //    ListaConectadosDG.Rows[n].Cells[0].Value = trozos[i + 2];

                            //}
                            //this.ListaConectadosDG.Rows[0].Cells[0].Selected = false;

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
                    //INVITACION
                    case 9:
                        var result = MessageBox.Show(trozos[1], "Invitacion", MessageBoxButtons.YesNo);
                        //Activamos la visibilidad del chat para los que hayan aceptado la invitacíón

                        Invoke(new Action(() =>
                        {

                            if (result == DialogResult.Yes)
                            {

                                // Enviamos al servidor el nombre del usuario (devuelve 10/nombre_Invitado/nombre_Invitador/Yes)       
                                string respuestaInvitacion = "10/" + usuario_ini.Text + "/" + trozos[2] + "/Yes";
                                //nameInvitado[jugadores] = usuario_ini.Text;
                                //jugadores++;

                                //Activamos la visibilidad del chat para los que hayan aceptado la invitacíón
                                //DelegadoGB delegado411 = new DelegadoGB(PonVisibleGB);

                                //groupBoxChat.Invoke(delegado411, new object[] { groupBoxChat });


                                // Enviamos al servidor el nombre tecleado
                                byte[] msg = System.Text.Encoding.ASCII.GetBytes(respuestaInvitacion);
                                server.Send(msg);
                                groupBoxChat.Visible = true;
                            }
                            if (result == DialogResult.No)
                            {
                                // Enviamos al servidor el nombre del usuario       
                                string respuestaInvitacion = "10/" + usuario_ini.Text + "/" + trozos[2] + "/No";
                                //string respuestachat = "11/" + usuario_ini.Text + "/" + trozos[2] + "/No";
                                // Enviamos al servidor el nombre tecleado
                                byte[] msg = System.Text.Encoding.ASCII.GetBytes(respuestaInvitacion);
                                server.Send(msg);
                            }
                        }));

                        break;
                    case 10:
                        if (trozos[1] == "YES")
                        { 
                        MessageBox.Show("Ha aceptado la invitación " + trozos[3], "invitacion", MessageBoxButtons.OK);
                        Invoke(new Action(() =>
                        {
                            groupBoxChat.Visible = true;
                        }));
                        }
                        else
                        {
                            MessageBox.Show(trozos[2]);
                        }
                        //DelegadoGB delegado412 = new DelegadoGB(PonVisibleGB);
                        //groupBoxChat.Invoke(delegado412, new object[] { groupBoxChat });
                        //DelegadorParaEscribir delegado441 = new DelegadorParaEscribir(PonMSN);
                        //Conversacion.Invoke(delegado441, new object[] { trozos[1] });
                        this.ListaConectadosDG.Rows[0].Cells[0].Selected = false;
                        break;
                    case 11:
                        autorMensaje = trozos[1];
                        int i = 0;
                        int encontrado = 0;
                        while(i<3 && encontrado== 0)
                        {
                            if (nameInvitado[i] == autorMensaje)
                            {
                                encontrado = 1;
                            }
                            i++;
                        }
                        if (encontrado == 0)
                        {
                            nameInvitado[jugadores] = autorMensaje;
                            jugadores++;
                        }
                                               
                        mensajeChat = trozos[2];
                        receptorMensaje = trozos[3];
                        //DelegadorParaEscribir delegado11 = new DelegadorParaEscribir(RecibirChat);
                        //this.Invoke(delegado11, new object[] { autorMensaje, mensajeChat });
                        Chat.Items.Add(trozos[1]+":"+trozos[2]);
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
            //bool isChecked1 = false;
            //bool isChecked2 = false;
            //bool isChecked3 = false;

            //isChecked1 = partidas_ganadas.Checked;
            //isChecked2 = tablones.Checked;
            //isChecked3 = id_usuario.Checked;
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
            //else if (String.IsNullOrWhiteSpace(usuario_consulta.Text) || (usuario_consulta.Text.Length > 0))
            //{
            //    MessageBox.Show("¡ATENCIÓN! \n¡Recuerda que tienes que escoger petición antes de enviar!");
            //}
            //else if(!isChecked1 && !isChecked2 && !isChecked3)
            //{
            //    MessageBox.Show("¡ATENCIÓN! \n¡Recuerda que tienes que escoger un usuario antes de enviar!");
            //}
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

                ////Recibimos la respuesta del servidor
                //byte[] msg2 = new byte[80];
                //server.Receive(msg2);
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
            //if (e.KeyCode == Keys.Enter)
            //{

                // Enviamos al servidor el nombre del usuario
                string mensaje = "5/" + usuario_ini.Text + "/" + contraseña_ini.Text;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            //}
            //else
            //{
            //    MessageBox.Show("Porfavor, haga click en Enter para Iniciar Sesión :)");
            //}
            
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

            ////Recibimos la respuesta del servidor
            //byte[] msg2 = new byte[80];
            //server.Receive(msg2);
            //mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
            ////MessageBox.Show(mensaje);
            //ListaConectadosDG.Rows.Clear();
            ////Del servidor obtenemos un resultado: "jugadoresConectados.socket/listaConectados". Separaremos el resultado segun "." y insertaremos el resultado en words[]
            //string[] words = mensaje.Split('/');
            //Int32.TryParse(words[0], out int numValue);
            //int existeConectado = 0;
            //ListaConectadosDG.ColumnCount = 1;
            ////this.ListaConectadosDG.Rows.Add();
            //int cont = 1;
            //while (numValue > 1 && existeConectado < numValue)
            //{
            //    //string[] S = words[1].Split('/');
            //    //this.ListaConectadosDG.Rows.Add();
            //    ListaConectadosDG.Rows.Add(words[cont]);


            //    existeConectado++;
            //    cont++;
            //}
            //if (numValue == 1)
            //{
            //    ListaConectadosDG.Rows.Add(words[1]);
            //}

        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        //BOTON SERVICIOS (7)

        private void button1_Click(object sender, EventArgs e)
        {
            //Pedir numero de servicios realizados
            string mensaje = "7/" + usuario_ini.Text;

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            ////Recibimos la respuesta del servidor
            //byte[] msg2 = new byte[80];
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

            //Recibimos la respuesta del servidor
            //byte[] msg2 = new byte[80];
            //server.Receive(msg2);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        //BOTON JUGAR
        private void jugar_button_Click(object sender, EventArgs e)
        {
            jugar = 1;
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
            int i = 0;
            if (jugar == 1)
            {
                if (jugadores < 3)
                {
                    nameInvitado[jugadores] = ListaConectadosDG.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

                    // Enviamos al servidor el nombre del usuario       
                    string mensaje = "9/" + usuario_ini.Text + "/" + nameInvitado[jugadores];
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

        //BOTÓN ENVIAR MENSAJE (11) 
        private void EnviarMensajebutton_Click(object sender, EventArgs e)
        {

            //// Envias el mensaje : 11/tu_nombre/mensaje
            //string mensaje = "11/" + usuario_ini.Text + "/" + MensajeTextBox.Text;
            //// Enviamos al servidor el nombre tecleado
            //byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            //server.Send(msg);
            //MensajeTextBox.Clear();
            //Chat.Items.Add(usuario_ini.Text + ":" + MensajeTextBox.Text);
            string mensaje;
            if (jugadores == 1)
            {
                // Envias el mensaje : 11/tu_nombre/mensaje/
                mensaje = "11/" + usuario_ini.Text + "/" + MensajeTextBox.Text + "/" + nameInvitado[0];
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                
                Chat.Items.Add(usuario_ini.Text + ":" + MensajeTextBox.Text);
                MensajeTextBox.Clear();
            }
            if (jugadores == 2)
            {
                mensaje = "11/" + usuario_ini.Text + "/" + MensajeTextBox.Text + "/" + nameInvitado[0] + "/" + nameInvitado[1];
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                
                Chat.Items.Add(usuario_ini.Text + ":" + MensajeTextBox.Text);
                MensajeTextBox.Clear();
            }
            if (jugadores == 3)
            {
                mensaje = "11/" + usuario_ini.Text + "/" + MensajeTextBox.Text + "/" + nameInvitado[0] + "/" + nameInvitado[1] + "/" + nameInvitado[2];
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                
                Chat.Items.Add(usuario_ini.Text + ":" + MensajeTextBox.Text);
                MensajeTextBox.Clear();
            }



        }

        private void pictureBoxStarWars_Click(object sender, EventArgs e)
        {
            // Envias el mensaje
            string mensaje = "11/SW/"+usuario_ini.Text+"/";
            // Enviamos al servidor el nombre tecleado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void pictureBoxCasaPapel_Click(object sender, EventArgs e)
        {
            // Envias el mensaje
            string mensaje = "11/CP";
            // Enviamos al servidor el nombre tecleado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void pictureBoxHarryPotter_Click(object sender, EventArgs e)
        {
            // Envias el mensaje
            string mensaje = "11/HP";
            // Enviamos al servidor el nombre tecleado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            
        }

        private void InvitarButton_Click(object sender, EventArgs e)
        {
        //    int i = 0;
        //    //var selectedCells = ListaConectadosDG.SelectedCells
        //    //    .OfType<DataGridViewCell>()
        //    //    .Where(cell => !cell.IsNewCell)
        //    //    .ToArray();

        //    //    foreach (var cell in selectedCells)
        //    //{ 
        //    foreach (DataGridViewCell cel in ListaConectadosDG.SelectedCells)
        //    {
        //        nameInvitado[i] = cel.Value.ToString();

        //            // Enviamos al servidor el nombre del usuario       
        //            string mensaje = "9/" + usuario_ini.Text + "/" + nameInvitado[i];
        //            // Enviamos al servidor el nombre tecleado
        //            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
        //            server.Send(msg);
        //            i++;
                
        //    }
        //    ListaConectadosDG.ClearSelection();
        }
    }
}

