#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>
#include <pthread.h>

//---------------------------------------------------------------------------------------------------------------------------------------------------
//NOTAS:
/*Antes de ejecutar nuestro programa en c tenemos que:
- Entrar en el directorio (desde consola) donde tenemos ubicados los archivos necesarios (num_partidas_ganadas_Juan.sql).
- Introducir el comando "mysql -u root -p" y introducir contrase\ufff1a "mysql".
- Introducir el comando "source num_partidas_ganadas_Juan.sql;".
- Abrir el codigo en c y ejecutarlo.
*NOTA* : si no sigue estos pasos, no se realizara correctamente el programa ni se actualizaran los datos del fichero .sql, por tanto
cada vez que actualize dicho fichero .sql tendra que introducir el comando "source num_partidas_ganadas_Juan.sql" para actualizar
los datos.*/

//---------------------------------------------------------------------------------------------------------------------------------------------------

int contador;
int idPartida;
//Estructura necesaria para acceso excluyente
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;

int i;
int sockets [100];

//---------------------------------------------------------------------------------------------------------------------------------------------------
//Declaramos la estructuracon un nombre y un Socket
typedef struct {
	char nombre[20];
	int socket;
} Conectado;

typedef struct {
	Conectado conectados [100];
	int num;
} ListaConectados;

ListaConectados miLista;

//Declaramos la lista de Partidas
typedef struct {
	int ID;
	int numJugadores;
	char tablero[30];
	//Sockets
	char nombre1[20];
	char nombre2[20];
	char nombre3[20];
	char nombre4[20];
	int sock[4];
}Partida;

typedef struct {
	Partida Partidas [1000];
	int num;
} ListaPartidas;

ListaPartidas miListaPartidas;    

//---------------------------------------------------------------------------------------------------------------------------------------------------
//Variables SQL

MYSQL * conn;
int err;
MYSQL_RES * resultado;
MYSQL_ROW row;

//---------------------------------------------------------------------------------------------------------------------------------------------------
//Funcion que a\uffc3\uffb1ade un nuevo conectado. Retorna 0 si se ha a\uffc3\uffb1adido correctamente y -1 si no se ha podido a\uffc3\uffb1adir debido a que la lista esta llena.
int Pon (ListaConectados *lista, char nombre[20], int socket)
{
	if (lista->num == 100)
	{
		return -1;
	}
	else
	{
		strcpy(lista->conectados[lista->num].nombre, nombre);
		lista->conectados[lista->num].socket = socket;
		
		lista->num++;
		
		return 0;
	}
	
}

//---------------------------------------------------------------------------------------------------------------------------------------------------
//Funcion que devuelve la posicion o -1 si no esta en la lista de conectados.
int DameSocket (ListaConectados *lista, char nombre[20])
{
	int i = 0;
	int encontrado = 0;
	while((i < lista->num) && !encontrado)
	{
		if (strcmp(lista->conectados[i].nombre,nombre) == 0)
		{
			encontrado = 1;
		}
		if(!encontrado)
		{
			i = i+1;
		}
	}
	if (encontrado)
	{
		return lista->conectados[i].socket;
	}
	else
	{
		return -1;
	}
}
//---------------------------------------------------------------------------------------------------------------------------------------------------
//Devuelve el Socket en el caso de lo que lo encuentre, sin\uffc3\uffb3 devuelve -1
int DevuelveSocket (ListaConectados *lista, char nombre[20])
{
	int i = 0;
	int encontrado = 0;
	while((i < lista->num) && !encontrado)
	{
		if (strcmp(lista->conectados[i].nombre,nombre) == 0)
		{
			encontrado = 1;
		}
		if(!encontrado)
		{
			i = i+1;
		}
	}
	if (encontrado)
	{
		return lista->conectados[i].socket;
	}
	else
	{
		return -1;
	}
}
//---------------------------------------------------------------------------------------------------------------------------------------------------
//Funcion que retorna 0 si elimina a la persona y -1 si ese usuario no esta conectado.
int Desconectar (ListaConectados *lista, char nombre[20], char respuestaDesconectar[512])
{
	int pos = DameSocket (lista,nombre);
	if (pos == -1)
	{
		return -1;
	}
	
	
	else
	{
		int i;
		for (i = pos; i < lista->num-1; i++)
		{
			strcpy(lista->conectados[i].nombre, lista->conectados[i+1].nombre);
			lista->conectados[i].socket = lista->conectados[i+1].socket;
		}
		lista->num--;
		sprintf(respuestaDesconectar,"8/Hasta la proxima!");
		return 0;
	}
}
//---------------------------------------------------------------------------------------------------------------------------------------------------
//Funcion que pone en conectados los nombres de todos los conectados separados por barras. Primero pone el numero de conectados. Por ejemplo: "3/Lucia/Carla/Marta"
void DameConectados (ListaConectados *lista, char conectados[300])
{
	sprintf(conectados, "%d", lista->num);
	int i;
	for(i=0; i<lista->num; i++)
	{
		sprintf(conectados, "%s/%s", conectados, lista->conectados[i].nombre);
		
	}
	printf("%s\n",conectados);
}
//---------------------------------------------------------------------------------------------------------------------------------------------------
//Devuelve el numero de partidas ganadas por el usuario indicado
int PartidasGanadas (char usuario1[20],char consulta1[80],int codigo1,char respuesta1[512])
{
	sprintf(consulta1,"SELECT Jugador.partidas_ganadas FROM Jugador WHERE Jugador.username ='%s' ",usuario1);
	printf("%s",consulta1);
	err=mysql_query(conn, consulta1);
	if (err!=0)
	{
		printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn),mysql_error(conn));
		exit(1);
	}
	resultado = mysql_store_result(conn);
	row = mysql_fetch_row(resultado);
	if (row == NULL)
	{
		printf("No se ha obtenido la consulta \n");
	}
	else
	{
		printf("Codigo: %d, Nombre: %s, Partidas_ganadas: %s \n",codigo1, usuario1, row[0]);
		printf("El usuario ha ganado %s partidas \n",row[0]);
		sprintf(respuesta1,"1/%s",row[0]);
	}
}
//---------------------------------------------------------------------------------------------------------------------------------------------------
//Escribe en respuesta los tableros jugados por el usuario inicado por pantalla
int Tableros (char usuario2[20],char consulta2[80],int codigo2,char respuesta2[512])
{
	printf ("Codigo: %d, Nombre: %s \n", codigo2, usuario2);
	sprintf(consulta2,"SELECT Jugador.partidas_jugadas FROM Jugador WHERE Jugador.username = '%s'",usuario2);
	printf("%s",consulta2);
	err=mysql_query(conn, consulta2);
	if (err!=0)
	{
		printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn),mysql_error(conn));
		exit(1);
	}
	
	//Recogemos el resultado de la consulta
	resultado = mysql_store_result(conn);
	row = mysql_fetch_row(resultado);
	if (row == NULL)
	{
		printf("No se ha obtenido la consulta \n");
	}
	else
	{
		printf("Nombre: %s, Partidas_jugadas: %s \n", usuario2, row[0]);
		int pJugadas = atoi(row[0]);
		if(pJugadas==0)
		{
			printf("No ha jugado ninguna vez \n");
			sprintf(respuesta2,"2/0-No se ha jugado ninguna partida!!");
		}
		if (pJugadas!=0)
		{
			sprintf (consulta2, "SELECT Partida.tablon FROM (Partida,Jugador,Participacion) WHERE Jugador.username = '%s' AND Jugador.id=Participacion.idJ AND Participacion.idP=Partida.id",usuario2);
			err=mysql_query(conn, consulta2);
			if (err!=0)
			{
				printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn),mysql_error(conn));
				exit(1);
			}
			
			//Recogemos el resultado de la consulta
			resultado = mysql_store_result(conn);
			row = mysql_fetch_row(resultado);
			char tableros[100] = " ";
			if (row == NULL)
			{
				printf("No se ha obtenido la consulta \n");
			}
			else
			{
				while (row!=NULL)
				{
					printf("Tablero: %s \n",row[0]);
					sprintf(tableros,"%s%s,",tableros,row[0]);
					row = mysql_fetch_row(resultado);
				}
				sprintf(respuesta2,"2/1-%s",tableros);
			}
		}
	}
}
//---------------------------------------------------------------------------------------------------------------------------------------------------
//Devuelve la id del usuario indicado por pantalla
int IDUsuario(char usuario3[20],char consulta3[80],int codigo3,char respuesta3[512])
{
	printf ("Codigo: %d, Nombre: %s \n", codigo3, usuario3);
	sprintf(consulta3, "SELECT Jugador.id FROM Jugador WHERE Jugador.username='%s' ",usuario3);
	err=mysql_query(conn, consulta3);
	if (err!=0){
		printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn),mysql_error(conn));
		exit(1);
	}    
	//Recogemos el resultado de la consulta
	resultado = mysql_store_result(conn);
	row = mysql_fetch_row(resultado);
	if (row == NULL)
	{
		printf("No se ha obtenido la consulta \n");
	}
	else
	{
		printf("El id del usuario es:%s \n",row[0]);
		sprintf(respuesta3,"3/%s",row[0]);
	}
}
//---------------------------------------------------------------------------------------------------------------------------------------------------
//Comprueba que el usuario pasado por pantalla no esta en la BBDD, en el caso de que no este lo a\ufff1ade. Si esta, envia un mensaje indicando que el usuario no esta disponible.
int Registro (char usuario4[20],char consulta4[80],int codigo4,char respuesta4[512],char contrasena4[20],char consulta_id4[80])
{
	printf ("Codigo: %d, Nombre: %s, Contrase\uffc3\uffb1a: %s\n", codigo4, usuario4, contrasena4);
	sprintf(consulta4, "SELECT Jugador.username FROM Jugador WHERE Jugador.username='%s' ",usuario4);
	// hacemos la consulta
	err=mysql_query (conn, consulta4);
	if (err!=0)
	{
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	//Recogemos el resultado
	resultado = mysql_store_result (conn);
	row=mysql_fetch_row(resultado);
	if (row==NULL)
	{
		//Consultamos el numero de usuarios registrados para obtener el ultimo id
		strcpy(consulta_id4,"SELECT MAX(Jugador.id) FROM (Jugador)");
		err=mysql_query (conn, consulta_id4);
		if (err!=0)
		{
			printf ("Error al consultar datos de la base %u %s\n",
					mysql_errno(conn), mysql_error(conn));
			exit (1);
		}
		//Recogemos el resultado de la consulta de id
		resultado = mysql_store_result (conn);
		row = mysql_fetch_row (resultado);
		
		int idJugador=atoi(row[0])+1;
		char insert[150];
		sprintf(insert,"INSERT INTO Jugador(id,username,pass,partidas_ganadas,partidas_jugadas) VALUES (%d,'%s','%s',0,0)",idJugador,usuario4,contrasena4);
		
		err=mysql_query (conn, insert);
		if (err!=0)
		{
			printf ("Error al insertar datos de la base %u %s\n",
					mysql_errno(conn), mysql_error(conn));
			exit (1);
			sprintf(respuesta4,"4/NO INSERTADO");          		 
		}
		else
		{
			sprintf(respuesta4,"4/SI");
		}
	}
	else
	{
		sprintf(respuesta4,"4/NO REGISTRADO");
	}  		 
}
//---------------------------------------------------------------------------------------------------------------------------------------------------
//Comprueba que el usuario introducido por pantalla existe y si se corresponde a la contrase\ufff1a escrita e inicia sesion
int InicioSesion(char usuario5[20],char consulta5[80],int codigo5,char respuesta5[512], char contrasena5[20],char conectado5[300],int PONER5,int sock_conn5)
{
	printf ("Codigo: %d, Nombre: %s, Contrase\ufff1a: %s\n", codigo5, usuario5, contrasena5);
	sprintf(consulta5, "SELECT Jugador.username, Jugador.pass FROM Jugador WHERE (Jugador.username='%s' AND Jugador.pass='%s')",usuario5,contrasena5);
	err=mysql_query(conn, consulta5);
	if (err!=0){
		printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn),mysql_error(conn));
		exit(1);
	}
	//Recogemos el resultado de la consulta
	resultado = mysql_store_result(conn);
	row = mysql_fetch_row(resultado);
	if (row == NULL)
	{
		printf("No se ha obtenido la consulta \n");
		sprintf(respuesta5,"5/INCORRECTO");
	}
	else
	{
		
		printf("Inicio de sesion completado \n");
		sprintf(respuesta5,"5/CORRECTO");
		printf("%s,%s,%d \n",conectado5,usuario5,sock_conn5);
		pthread_mutex_lock( &mutex );
		//No hay que a\ufff1adir nuevo conectado hay que cambiar el nombre a el conectado que esta en el nombre que esta en la funcion Pon del bucle principal
		//Busco el socket (CompletarConectado) y completo con el nombre de usuario5 (Busqueda)
		CompletarConectado(&miLista,usuario5,sock_conn5);
		pthread_mutex_unlock( &mutex);
	}
}
//---------------------------------------------------------------------------------------------------------------------------------------------------
//Completa el nombre la de persona que se acaba de conectar
int CompletarConectado (ListaConectados *lista, char usuario[20],int sock_con)
{
	int i=0;
	int encontrado=0;
	printf("HOLA, %d, %s,%d\n",sock_con, usuario,lista->num);
	while(i<lista->num && encontrado==0)
	{
		printf("%d\n",lista->conectados[i].socket);
		if (lista->conectados[i].socket == sock_con)
		{
			printf("HOLA22\n");
			strcpy(lista->conectados[i].nombre,usuario);
			printf("name:%s\n",lista->conectados[i].nombre);
			encontrado=1;
		}
		else
		{
			i++;
		}
	}
}

//---------------------------------------------------------------------------------------------------------------------------------------------------
//Mostramos la lista de conectados (Funcion no usada)
int MostrarListaConectados (char respuesta6[512], char conectado6[300])
{
	sprintf(conectado6, "%d", miLista.num);
	int i;
	for(i=0; i<miLista.num; i++)
	{
		if  (miLista.conectados[i].socket == 4 && miLista.conectados[i].socket == 5)
		{
			printf("%s %d", conectado6, miLista.conectados[i].socket);
			sprintf(conectado6, "%s/%d-%s", conectado6, miLista.conectados[i].socket , miLista.conectados[i].nombre);
		}
		//Se mostrara la lista de conectados de la siguiente manera: "numero de personas conectadas.socket-nombre usuario.socket-nombre usuario ..."
		sprintf(conectado6, "%s/%d-%s", conectado6, miLista.conectados[i].socket , miLista.conectados[i].nombre);
		
	}
	printf("%s \n",conectado6);
	sprintf(respuesta6,"%s",conectado6);
}
//---------------------------------------------------------------------------------------------------------------------------------------------------
//Enviamos al cliente una invitacion para jugar
int Invitacion (ListaConectados *lista,char respuesta9[512],char nameOrigen[20],int socketOrigen, char nameDestino[20],int socketDestino, int *sock_conn9)
{
	int i = 0;
	printf("%d %d \n",socketDestino, socketOrigen);
	//Devolveremos el codigo, la respuesta y el nombre del destino de la invitacion
	sprintf(respuesta9,"9/Quieres jugar con %s?/%s",nameOrigen,nameOrigen);
	*sock_conn9 = socketDestino;
}
//---------------------------------------------------------------------------------------------------------------------------------------------------
//Enviamos al cliente la respuesta de la invitacion
int ConfirmacionInvitacion(ListaConectados *lista,ListaPartidas *listaP,char confirm[10], int socketOrigen,int socketDestino, char respuesta10 [512], char nameOrigen [20], char nameDestino[20], int *sock_conn10)
{
	listaP->Partidas[idPartida].numJugadores++;
	if (strcmp(confirm,"Yes") == 0)
	{
		strcpy(listaP->Partidas[idPartida].nombre1,nameDestino);
		listaP->Partidas[idPartida].sock[0] = DameSocket(lista,nameDestino);
		
		printf("sock1: %d",listaP->Partidas[idPartida].sock[0]);
		sprintf(respuesta10,"10/YES/%d/%s", idPartida,nameDestino);
		if (listaP->Partidas[idPartida].numJugadores == 2)
		{
			
			strcpy(listaP->Partidas[idPartida].nombre2,nameOrigen);
			listaP->Partidas[idPartida].sock[1] = DameSocket(lista,nameOrigen);
			printf("sock2: %d",listaP->Partidas[idPartida].sock[1]);
			sprintf(respuesta10, "%s/%s", respuesta10, listaP->Partidas[idPartida].nombre2);
		}
		else if (listaP->Partidas[idPartida].numJugadores == 3)
		{
			strcpy(listaP->Partidas[idPartida].nombre3,nameOrigen);
			listaP->Partidas[idPartida].sock[2] = DameSocket(lista,nameOrigen);
			sprintf(respuesta10, "%s/%s/%s", respuesta10, listaP->Partidas[idPartida].nombre2,listaP->Partidas[idPartida].nombre3);
		}
		else if (listaP->Partidas[idPartida].numJugadores == 4)
		{
			strcpy(listaP->Partidas[idPartida].nombre4,nameOrigen);
			listaP->Partidas[idPartida].sock[3] = DameSocket(lista,nameOrigen);
			sprintf(respuesta10, "%s/%s/%s/%s", respuesta10, listaP->Partidas[idPartida].nombre2,listaP->Partidas[idPartida].nombre3,listaP->Partidas[idPartida].nombre4);
		}
		
		printf("n1:%s,n2:%s\n",listaP->Partidas[idPartida].nombre1,listaP->Partidas[idPartida].nombre2);
	}
	else
	{
		sprintf(respuesta10,"10/NO/%s no quiere jugar contigo",nameOrigen);
	}
	
	*sock_conn10 = socketDestino;
}
//---------------------------------------------------------------------------------------------------------------------------------------------------
//Obtenemos los sockets de las personas que juegan en la partida
int DameSocketsJugadoresPartida(int id,ListaPartidas *listaP,ListaConectados *lista,int socketsJugadores[4])
{
	int i=0;
	while(i< listaP->Partidas[id].numJugadores)
	{
		socketsJugadores[i]= listaP->Partidas[id].sock[i];
		i++;
	}
	return listaP->Partidas[id].numJugadores;
}
//---------------------------------------------------------------------------------------------------------------------------------------------------
//Eliminar jugador (Se da de baja)
int DarseDeBaja (char nombre[20],char consulta[200],char respuesta[512])
{
	printf ("Nombre: %s \n", nombre);
	sprintf(consulta,"DELETE FROM Jugador WHERE Jugador.id = '%s' ",nombre);
	
	
	printf("%s\n",consulta);
	err = mysql_query(conn, consulta);
	
	if (err!=0)
	{
		printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn),mysql_error(conn));
		exit(1);
	}
	else
	{
		printf("%s se ha dado de baja \n",nombre);
		sprintf(respuesta,"15/");
	}
}
//---------------------------------------------------------------------------------------------------------------------------------------------------
//Funcion atender cliente
void *AtenderCliente (void *socket)
{
	//llamamos a la ListaConectados
	int sock_conn;
	int *s;
	s=(int *) socket;
	sock_conn= *s;
	int PONER;
	char conectado[300];
	int contadorSocket=0;
	
	int notificar=0;
	char notificacion [512];
	char peticion[512];
	char respuesta[512];
	char respuesta1[512];
	
	int cerrarSesion=0;
	int ret;
	
	char nameOrigen[20];
	char nameDestino[20];
	
	int terminar = 0;
	// Entramos en un bucle para atender todas las peticiones de este cliente
	//hasta que se desconecte
	while (terminar==0)
	{
		char usuario[20];
		char contrasena[20];
		int partidas_ganadas;
		char consulta[80];
		char consulta_id[80];
		//int socketInvitado;
		
		// Ahora recibimos su peticion
		ret=read(sock_conn,peticion, sizeof(peticion));
		printf ("Recibida una peticion\n");
		// Tenemos que a?adirle la marca de fin de string
		// para que no escriba lo que hay despues en el buffer
		peticion[ret]='\0';
		
		printf ("peticion: %s\n",peticion);
		//Recogemos el codigo indicado por cada peticion.
		char *p = strtok( peticion, "/");
		int codigo =  atoi (p);
		printf("%d\n",codigo);
		int cont = 0;
		//---------------------------------------------------------------------------------------------------------------------------------------------------
		//PETICIONES
		
		//DESCONECTAR
		if (codigo == 0)
		{
			write (sock_conn,"0/", strlen("0"));
			terminar=1;
			close(sock_conn);
			//strcpy(respuesta, "0/");
			
/*			pthread_mutex_lock( &mutex );*/
/*			DameConectados(&miLista,conectado);*/
/*			pthread_mutex_unlock( &mutex);*/
/*			notificar=1;*/
/*			close(sock_conn);*/
/*			 Enviamos la respuesta*/
/*			printf("Respuesta: %s \n", respuesta);*/
			//write (sock_conn,respuesta, strlen(respuesta));
		}
		//PIDEN LAS PARTIDAS GANADAS
		if (codigo ==1)
		{
			p = strtok( NULL, "/");
			strcpy(usuario,p);
			PartidasGanadas(usuario,consulta,codigo,respuesta);
			// Enviamos la respuesta
			printf("Respuesta: %s \n", respuesta);
			write (sock_conn,respuesta, strlen(respuesta));
		}
		//PIDEN LOS TABLEROS
		if (codigo == 2)
		{
			p = strtok( NULL, "/");
			strcpy (usuario, p);
			Tableros(usuario,consulta,codigo,respuesta);
			// Enviamos la respuesta
			printf("Respuesta: %s \n", respuesta);
			write (sock_conn,respuesta, strlen(respuesta));
		}
		//PIDEN EL ID DEL USUARIO
		if (codigo ==3)
		{
			p = strtok( NULL, "/");
			strcpy (usuario, p);
			IDUsuario(usuario,consulta,codigo,respuesta);
			// Enviamos la respuesta
			printf("Respuesta: %s \n", respuesta);
			write (sock_conn,respuesta, strlen(respuesta));
		}
		//INSERTA USUARIO Y CONTRASE\uffd1A EN LA BASE DE DATOS (REGISTRO)
		if (codigo == 4)
		{
			p = strtok( NULL, "/");
			strcpy (usuario, p);
			p = strtok( NULL, "/");
			strcpy(contrasena,p);
			Registro(usuario,consulta,codigo,respuesta,contrasena,consulta_id);
			notificar=0;
			// Enviamos la respuesta
			printf("Respuesta: %s \n", respuesta);
			write (sock_conn,respuesta, strlen(respuesta));
		}
		//COMPRUEBA EL NOMBRE DE USUARIO Y CONTRASE\uffd1A EN LA BASE DE DATOS (INICIO SESION)
		if (codigo == 5)
		{
			
			p = strtok( NULL, "/");
			strcpy (usuario, p);
			p = strtok( NULL, "/");
			strcpy(contrasena,p);
			if (cerrarSesion==1)
			{
				Pon(&miLista,usuario,sock_conn);
			}
			InicioSesion(usuario,consulta,codigo,respuesta,contrasena,conectado,PONER,sock_conn);
			pthread_mutex_lock( &mutex );
			DameConectados(&miLista,conectado);
			pthread_mutex_unlock( &mutex);
			//Notificamos a todos los clientes
			notificar=1;
			printf("Respuesta: %s \n", respuesta);
			write (sock_conn,respuesta, strlen(respuesta));
			cerrarSesion=0;
		}
		//CONTADOR DE SERVICIOS DEL USUARIO
		if (codigo ==7)
		{
			sprintf (respuesta,"7/%d",contador);
			printf("Respuesta: %s \n", respuesta);
			write (sock_conn,respuesta, strlen(respuesta));
		}
		//ELIMINAR DE LA LISTA DE CONECTADOS
		if (codigo==8)
		{
			cerrarSesion=1;
			pthread_mutex_lock( &mutex );
			Desconectar(&miLista,usuario,respuesta);
			DameConectados(&miLista,conectado);
			pthread_mutex_unlock( &mutex);
			notificar=1;
			printf("Respuesta: %s \n", respuesta);
			write (sock_conn,respuesta, strlen(respuesta));
		}
		//INVITACION
		if (codigo == 9)
		{
			int socketOrigen;
			int socketDestino;
			char nameOrigen[20];
			char nameDestino[20];
			
			p = strtok( NULL, "/");
			strcpy(nameOrigen,p);
			socketOrigen = DevuelveSocket(&miLista,p);
			p = strtok (NULL,"/");
			strcpy(nameDestino,p);
			socketDestino = DevuelveSocket(&miLista,p);
			
			pthread_mutex_lock( &mutex );
			Invitacion(&miLista,respuesta,nameOrigen,socketOrigen,nameDestino,socketDestino,&sock_conn);
			pthread_mutex_unlock( &mutex);
			printf("Respuesta: %s \n", respuesta);
			write (sock_conn,respuesta, strlen(respuesta));
		}
		//RECEPCION RESPUESTA DE LA INVITACION
		if (codigo == 10)
		{
			int socketOrigen;
			int socketDestino;
			char confirm[10];
			char nameOrigen[20];
			char nameDestino[20];
			
			p = strtok( NULL, "/");
			strcpy(nameOrigen,p);
			socketOrigen = DevuelveSocket(&miLista,p);
			p= strtok (NULL,"/");
			strcpy(nameDestino,p);
			socketDestino = DevuelveSocket(&miLista,p);
			printf("%d %d\n",socketDestino, socketOrigen);
			p=strtok(NULL,"/");
			strcpy(confirm,p);
			
			pthread_mutex_lock( &mutex );
			ConfirmacionInvitacion(&miLista,&miListaPartidas,confirm,socketOrigen,socketDestino,respuesta,nameOrigen,nameDestino,&sock_conn);
			DameConectados(&miLista,conectado);
			pthread_mutex_unlock( &mutex);
			printf("Respuesta: %s \n", respuesta);
			write (sock_conn,respuesta, strlen(respuesta));
		}
		//MENSAJE
		if (codigo == 11)
		{
			p= strtok(NULL, "/");
			int id = atoi(p);
			p= strtok(NULL, "/");
			char autorMsg[30];
			strcpy (autorMsg,p);
			printf("%s\n",autorMsg);
			char mensaje[100];
			p= strtok(NULL, "/");
			strcpy (mensaje,p);
			int socketsJugadores[4];
			
			int numJugadores = DameSocketsJugadoresPartida(id,&miListaPartidas,&miLista,socketsJugadores);
			
			sprintf(respuesta,"11/%d/%s/%s",id,autorMsg,mensaje);
			
			for (int i=0;i<numJugadores;i++)
			{
				if(socketsJugadores[i] != sock_conn)
				{
					printf("Respuesta: %s\n",respuesta);
					write(socketsJugadores[i],respuesta,strlen(respuesta));
				}
			}
		}
		//SELECCION TABLEROS
		if (codigo == 12)
		{
			char tablero[20];
			p= strtok(NULL, "/");
			strcpy (tablero,p);
			p=strtok(NULL,"/");
			int id = atoi(p);
			strcpy(miListaPartidas.Partidas[id].tablero,tablero);
			sprintf(respuesta,"12/%d/%s/%d",id,tablero,miListaPartidas.Partidas[id].numJugadores);
			
			sprintf(respuesta,"%s/%s", respuesta,miListaPartidas.Partidas[id].nombre1);
			if (miListaPartidas.Partidas[id].numJugadores == 2)
			{
				sprintf(respuesta, "%s/%s", respuesta, miListaPartidas.Partidas[id].nombre2);
			}
			else if (miListaPartidas.Partidas[id].numJugadores == 3)
			{
				sprintf(respuesta, "%s/%s/%s", respuesta, miListaPartidas.Partidas[id].nombre2,miListaPartidas.Partidas[id].nombre3);
			}
			else if (miListaPartidas.Partidas[id].numJugadores == 4)
			{
				sprintf(respuesta, "%s/%s/%s/%s", respuesta, miListaPartidas.Partidas[id].nombre2,miListaPartidas.Partidas[id].nombre3,miListaPartidas.Partidas[id].nombre4);
			}
			
			for (int i=0;i<miListaPartidas.Partidas[id].numJugadores;i++)
			{
				printf("Respuesta: %s\n",respuesta);
				write(miListaPartidas.Partidas[id].sock[i],respuesta,strlen(respuesta));
			}
		}
		//BOTON JUGAR
		if (codigo==14)
		{
/*			p= strtok(NULL,"/");*/
/*			int id = atoi(p);*/
			
			miListaPartidas.num = idPartida;
			miListaPartidas.num++;
			
			printf("HOLA\n");
			
			// Enviamos la respuesta
/*			char respuestaInvitados[20];*/
			
/*			int socketsJugadores[4];*/
			
			//int numJugadores = DameSocketsJugadoresPartida(id,&miListaPartidas,&miLista,socketsJugadores);
			//printf("%d\n",socketsJugadores[1]);
			sprintf(respuesta,"14/Selecciona a la persona con la que quieres jugar :)");
			//strcpy(respuestaInvitados,"14/1");
			printf("HOLA2");
			
					write (sock_conn,respuesta, strlen(respuesta));
					printf("Respuesta: %s \n", respuesta);
				
			
		}
		//DARSE DE BAJA
		if (codigo == 15)
		{
			p = strtok(NULL,"/");
			char nombreDadoBaja[20];
			strcpy(nombreDadoBaja,p);
			
			DarseDeBaja(nombreDadoBaja,consulta,respuesta);
			printf("Respuesta: %s \n", respuesta);
			
			write (sock_conn,respuesta, strlen(respuesta));
		}
		if (codigo == 16)
		{
			char turno[20];
			p = strtok(NULL,"/");
			strcpy(turno,p);
		}
		//DADO
		if (codigo == 50)
		{
			char tablero[20];
			p = strtok(NULL,"/");
			strcpy(tablero,p);
			p = strtok(NULL,"/");
			int id = atoi(p);
			//p es el nombre del jugador que tiene que tirar
			p = strtok(NULL,"/");
			char jugadorTurno[20];
			strcpy(jugadorTurno,p);
			
			p = strtok(NULL,"/");
			int valorDado= atoi(p);
			
			char notificacionTurno[20];
			
			int socketsJugadores[4];
			
			int numJugadores = DameSocketsJugadoresPartida(id,&miListaPartidas,&miLista,socketsJugadores);
			
			sprintf(notificacionTurno,"50/%s/%s/%d",tablero,jugadorTurno,valorDado);
			
			for (int i=0;i<numJugadores;i++)
			{
				write(socketsJugadores[i],notificacionTurno,strlen(notificacionTurno));
			}
		}
		
		
		if (codigo == 51)
		{
			p = strtok(NULL,"/");
			int D = atoi(p);
			char jugadorTurnoSiguiente[20];
			char respuestaTurno[20];
			int id;
			if (D == 0)
			{
				p = strtok(NULL,"/");
				strcpy(jugadorTurnoSiguiente,p);
				p = strtok(NULL,"/");
				int partidaEmpezada = atoi(p);
				p = strtok(NULL,"/");
				int turnoJ = atoi(p);
				p = strtok(NULL,"/");
				int casillaSalida= atoi(p);
				p = strtok(NULL,"/");
				int casillasOcupadas=atoi(p);
				p = strtok(NULL,"/");
				int IoD=atoi(p);
				
				
				sprintf(respuestaTurno,"51/0/%s/%d/%d/%d/%d/%d",jugadorTurnoSiguiente,partidaEmpezada,turnoJ,casillaSalida,casillasOcupadas,IoD);
			}
			if (D == 1)
			{
				printf("HA SACADO UN 5 \n");
				char tablero[10];
				p = strtok(NULL,"/");
				strcpy(tablero,p);
				
				p = strtok(NULL,"/");
				strcpy(jugadorTurnoSiguiente,p);
				printf("%s",jugadorTurnoSiguiente);
				
				
				p = strtok(NULL,"/");
				char coordenadaCasilla[2];
				strcpy(coordenadaCasilla,p);
				
				//p es el nombre del jugador que tiene que tirar
				p = strtok(NULL,"/");
				char colorFicha[20];
				strcpy(colorFicha,p);
				
				p = strtok(NULL,"/");
				int valorFicha = atoi(p);
				
				p = strtok(NULL,"/");
				id = atoi(p);
				
				p = strtok(NULL,"/");
				int partidaEmpezada = atoi(p);
				
				p = strtok(NULL,"/");
				int turnoJ= atoi(p);
				
				p = strtok(NULL,"/");
				int casillaSalida= atoi(p);
				///////
				p = strtok(NULL,"/");
				int casillasOcupadasCAct=atoi(p);
				
				p = strtok(NULL,"/");
				int IoDCAct=atoi(p);
				
				p = strtok(NULL,"/");
				int casillaActual=atoi(p);
				
				p = strtok(NULL,"/");
				int casillasOcupadasCSig=atoi(p);
				
				p = strtok(NULL,"/");
				int IoDCSig=atoi(p);
				
				p = strtok(NULL,"/");
				int casillaSiguiente=atoi(p);
				/*   		 p = strtok(NULL,"/");*/
				/*   		 int tirarFicha = atoi(p);*/
				/*   		 p = strtok(NULL,"/");*/
				/*   		 char turno[20];*/
				/*   		 strcpy(turno,p);*/
				
				//token a coordenadaCasilla
				p = strtok(coordenadaCasilla,"=");
				p = strtok(NULL,",");
				int coordenadaX = atoi(p);
				p = strtok(NULL,"=");
				p = strtok(NULL,"}");
				int coordenadaY= atoi(p);
				
				sprintf(respuestaTurno,"51/1/%s/%d/%d/%s/%d/%d/%d/%d/%d/%d/%d/%d/%d/%d",jugadorTurnoSiguiente,coordenadaX,coordenadaY,colorFicha,valorFicha,partidaEmpezada,turnoJ,casillaSalida,casillasOcupadasCAct,IoDCAct,casillaActual,casillasOcupadasCSig,IoDCSig,casillaSiguiente);
			}
			if (D == 2)
			{
				printf("HOLA\n");
				char tablero[10];
				p = strtok(NULL,"/");
				strcpy(tablero,p);
				
				p = strtok(NULL,"/");
				strcpy(jugadorTurnoSiguiente,p);
				printf("%s",jugadorTurnoSiguiente);
				
				
				p = strtok(NULL,"/");
				char coordenadaCasilla[2];
				strcpy(coordenadaCasilla,p);
				
				//p es el nombre del jugador que tiene que tirar
				p = strtok(NULL,"/");
				char colorFicha[20];
				strcpy(colorFicha,p);
				
				p = strtok(NULL,"/");
				int valorFicha = atoi(p);
				
				p = strtok(NULL,"/");
				id = atoi(p);
				
				p = strtok(NULL,"/");
				int partidaEmpezada = atoi(p);
				
				p = strtok(NULL,"/");
				int turnoJ= atoi(p);
				
				p = strtok(NULL,"/");
				int casillaSalida= atoi(p);
				
				p = strtok(NULL,"/");
				int casillasOcupadas=atoi(p);
				
				p = strtok(NULL,"/");
				int IoD=atoi(p);
				/*   		 p = strtok(NULL,"/");*/
				/*   		 int tirarFicha = atoi(p);*/
				/*   		 p = strtok(NULL,"/");*/
				/*   		 char turno[20];*/
				/*   		 strcpy(turno,p);*/
				
				//token a coordenadaCasilla
				p = strtok(coordenadaCasilla,"=");
				p = strtok(NULL,",");
				int coordenadaX = atoi(p);
				p = strtok(NULL,"=");
				p = strtok(NULL,"}");
				int coordenadaY= atoi(p);
				
				sprintf(respuestaTurno,"51/2/%s/%d/%d/%s/%d/%d/%d/%d/%d/%d",jugadorTurnoSiguiente,coordenadaX,coordenadaY,colorFicha,valorFicha,partidaEmpezada,turnoJ,casillaSalida,casillasOcupadas,IoD);
			}
			
			int socketsJugadores[4];
			
			int numJugadores = DameSocketsJugadoresPartida(id,&miListaPartidas,&miLista,socketsJugadores);
			
			
			
			for (int i=0;i<numJugadores;i++)
			{
				write(socketsJugadores[i],respuestaTurno,strlen(respuestaTurno));
			}
		}
		//CONTADOR DE FUNCIONES
		if ((codigo ==1)||(codigo==2)|| (codigo==3)||(codigo==4)|| (codigo==5)|| (codigo==6))
		{
			pthread_mutex_lock( &mutex ); //No me interrumpas ahora
			contador = contador +1;
			pthread_mutex_unlock( &mutex); //ya puedes interrumpirme
		}
		//NOTIFICACION
		if (notificar==1)
		{
			char cabecera [512] = "6/";
			strcat (cabecera, conectado);
			printf ("Notificacion: %s\n",cabecera);
			for (int j=0; j<miLista.num;j++)
			{
				write (miLista.conectados[j].socket,cabecera,strlen(cabecera));
				printf("lista: %s\n",cabecera);
			}
		}
		notificar=0;
	}
	// Se acabo el servicio para este cliente
	/*close(sock_conn);*/
	
}

int main(int argc, char *argv[])
{
	miListaPartidas.Partidas[idPartida].numJugadores=1;
	int sock_conn, sock_listen;
	struct sockaddr_in serv_adr;    
	// Puerto Carla : 50015
	// Puerto Marta : 50016
	// Puerto Lucia : 50017
	int puerto =9900;
	
	// INICIALIZACION
	// Abrimos el socket.
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creant socket");
	// Hacemos bind en el puerto.
	memset(&serv_adr, 0, sizeof(serv_adr));// inicializa en zero serv_addr.
	
	serv_adr.sin_family = AF_INET;
	// asocia el socket a cualquiera de las IP de la maquina.
	//htonl formatea el numero que recibe al formato necesario.
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	// escucharemos en el port (X).
	serv_adr.sin_port = htons(puerto);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind");
	//La cola de peticiones pendientes no podra ser superior a 4
	if (listen(sock_listen, 100) < 0)
		printf("Error en el Listen");
	contador=0;
	miLista.num = 0;
	//Creamos la conexion a MYSQL
	conn = mysql_init(NULL);
	if (conn == NULL)
	{
		printf("Error al crear la connexion: %u %s \n", mysql_errno(conn), mysql_error(conn));
		exit(1);
	}
	
	//conn = mysql_real_connect(conn,"shiva2.upc.es","root","mysql","M5_BBDDJuego",0,NULL,0);
	conn = mysql_real_connect(conn,"localhost","root","mysql","M5_BBDDJuego",0,NULL,0);
	if (conn == NULL)
	{
		printf("Error al inicializar la conexion: %u %s \n", mysql_errno(conn), mysql_error(conn));
		exit(1);
	}
	pthread_t  thread;
	i=0;
	
	//---------------------------------------------------------------------------------------------------------------------------------------------------
	//Bucle para realizar las peticiones.
	for(;;)
	{
		printf ("Escuchando\n");
		//sock_conn es el socket que usaremos para este cliente
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("He recibido conexi?n\n");
		//Problema de los clientes muertos
		//sockets[i]=sock_conn;
		
		Pon(&miLista,"nombre provisional",sock_conn);
		
		// sock_conn es el socket que utilizaremos para este cliente
		
		//Crear thread y decirle lo que tiene que hacer
		
		pthread_create(&thread, NULL, AtenderCliente, &miLista.conectados[miLista.num-1].socket);
		i++;
		
	}
}



