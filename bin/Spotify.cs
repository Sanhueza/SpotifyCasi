using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;

public class Spotify: Page{

  protected List<cancion> mus = new List<cancion>(); //lista de música
  protected List<cancion> rep = new List<cancion>();//lista de reproducción
  protected HtmlGenericControl limus;//lista donde se ve la música para escoger en la vista
  protected HtmlGenericControl lirep;//lista donde se ve la música a reproducir en la vista
  protected HtmlGenericControl reproductor;//este es el reproductor de audio
  protected HtmlGenericControl lilirep;//es para todos los li de la lista de reproducción
  protected HtmlGenericControl lilimus;//es para todos los li de la lista de musica

  protected void Page_Init(object sender, EventArgs e){
    if(!IsPostBack){//limpia la lista dereproducción cuando la página está recien cargada
      conectar("limpiar","no");
    }
    conectar("paramus","no");//llena mus desde la base de datos
    conectar("pararep","no");//llena rep desde la base de datos
    cargarReproductor();//carga el reproductor
  }

  //agrega la canción seleccionada a la lista de reproducción
  public void agregar(object sender, EventArgs e){
    try{
      LinkButton idcan=sender as LinkButton;//idcan es la canción seleccionada
      foreach (cancion canci in mus){//recorremos la lista para buscar la canción
        if(canci.id==idcan.ID){
          conectar("agrega",canci.id);//se agrega a la base de datos
          cargarReproductor();//carga el reproductor con la canción
        }
      }
    }catch(MySqlException ex){
      Response.Write(ex.ToString());
    }
  }

  public void cargarReproductor(){//carga las canciones de la lista en el reporductor
    reproductor.Controls.Clear();//limpia el reproductor
    foreach(cancion tocar in rep){
      HtmlGenericControl source=new HtmlGenericControl("source");
      source.Attributes.Add("src",tocar.url);
      reproductor.Controls.Add(source);
    }
  }

  public void tocarSiguiente(object sender, EventArgs e){//elimina la primera canción de la lista para que se toque la siguiente
    //esto es para mantener la integridad de la list, está explicado en el método de abajo
    List<cancion> listAux = new List<cancion>(); 
    listAux.Add(rep[0]);
    rep.Remove(listAux[0]);
    conectar("quita",listAux[0].id);
  }
    
  //quita la canción seleccionada de la lista de reproducción 
  public void quitar(object sender,EventArgs e){
    LinkButton idcan=sender as LinkButton;//idcan es la canción seleccionada
    string siId=idcan.ID.Trim(new Char[]{'c'});//Es para sacarle la c del principio al id

    //esto es para mantener la integridad. En List no se puede solo borrar
    List<cancion> listAux = new List<cancion>(); 
    foreach(cancion canci in rep){//recorremos la lista para buscar la canción
      if(canci.id==siId){
        listAux.Add(canci);//se guarda la que se va a borrar en este list auxiliar
        break;
      }
    }
    foreach (cancion item in listAux){
        rep.Remove(item);//ahora si permite removerlo
        conectar("quita",item.id);//lo quitamos también de la base de datos
    }
  }
  
  //carga la lista de reporducción en la vista
  public void cargarRep(){
    lirep.Controls.Clear();//limpiamos para que no se dupliquen
    foreach (cancion canci in rep){//recorremos la lista para cargar la lista de reproducción entera
      HtmlGenericControl li = new HtmlGenericControl("li");
      LinkButton tocar = new LinkButton();
      tocar.Text = canci.nombre;
      tocar.ID = "c"+canci.id;
      tocar.Click += new System.EventHandler(quitar);
      li.Controls.Add(tocar);
      lirep.Controls.Add(li);
    }
    cargarReproductor();
  }

  public void cargarMus(){//carga la lista para escoger la música
    foreach(cancion canci in mus){
      HtmlGenericControl li = new HtmlGenericControl("li");
      LinkButton botonesA = new LinkButton();
      botonesA.Text = canci.nombre;
      botonesA.ID = canci.id;
      botonesA.Click += new System.EventHandler(agregar);
      li.Controls.Add(botonesA);
      limus.Controls.Add(li);
    }
  }

//esta función concentra todas las actividades relacionadas con la base de datos
  public void conectar(string op, string no){
    try{//inicair conexión
      string @base="Server=127.0.0.1;Database=prufi;Uid=root;Pwd='';sslmode=none";
      MySqlConnection conexion = new MySqlConnection(@base);
      conexion.Open();
      MySqlCommand comando;
      MySqlDataReader datos;
      string miSql;

    switch (op){

        case "paramus"://obtener música
          miSql = "obtener_musica";
          comando = new MySqlCommand(miSql,conexion);
          comando.CommandType = CommandType.StoredProcedure;
          datos = comando.ExecuteReader();
          while(datos.Read()){//Esto guarda los datos de las canciones en la lista de musica (mus)
            cancion ca = new cancion(datos["mus_id"].ToString(),datos["mus_nombre"].ToString(),datos["mus_url"].ToString());
            mus.Add(ca);
          }
          datos.Close();
          cargarMus();
          break;

        case "pararep"://obtener la lista de reproducción de la base de datos
          rep.Clear();
          miSql = "select mus_id, mus_nombre, mus_url from musica left join music_play on mus_id = musica_id where playlist_id = 1 group by mus_id";
          comando = new MySqlCommand(miSql,conexion);
          datos = comando.ExecuteReader();
          while(datos.Read()){//carga las canciones en la listade reproducción (rep)
            cancion c = new cancion(datos["mus_id"].ToString(),datos["mus_nombre"].ToString(),datos["mus_url"].ToString());
            rep.Add(c);
          }
          datos.Close();
          cargarRep();
          break;

        case "agrega"://agrega una canción a la playlist
          miSql = "select * from music_play where musica_id = "+no+" and playlist_id = 1";//no es el id
          comando = new MySqlCommand(miSql, conexion); 
          datos = comando.ExecuteReader();  
          bool noExiste = true;
          // Se busca si la cancion ya se encuentra registrada en la playlist
          while(datos.Read()){
            noExiste = false;
          }
          datos.Close();
          // En caso de que no se encuentre registrada se agrega a la playlist
          if(noExiste){
            miSql = "insert into music_play (musica_id, playlist_id) values ("+no+",1)"; 
            comando = new MySqlCommand(miSql, conexion); 
            datos = comando.ExecuteReader();  
            datos.Close();
            Page.ClientScript.RegisterStartupScript(GetType(),"errorAdd","addCancion()",true); //Ejecuta la funcion js addCancion() en el archivo vistaSpotify.aspx
          }else{
            Page.ClientScript.RegisterStartupScript(GetType(),"errorAdd","errorAdd()",true); // Ejecuta la funcion js errorAdd() en el archivo vistaSpotify.aspx
          }
          conectar("pararep","no");
          break;

        case "quita"://quita la canción de la playlist
          miSql = "DELETE FROM `prufi`.`music_play` WHERE  `musica_id`="+no+" AND `playlist_id`=1";
          comando = new MySqlCommand(miSql,conexion);
          comando.ExecuteNonQuery();
          cargarRep();
          break;

        case "limpiar"://limpia la base de datos al cargar la página, podemos usarla o no
          miSql ="DELETE FROM `prufi`.`music_play` WHERE `playlist_id`=1";
          comando = new MySqlCommand(miSql,conexion);
          comando.ExecuteNonQuery();
          break;

        default:
          break;
      }
      conexion.Close();
    }catch(MySqlException ex){
      Response.Write(ex.ToString());
    }
  }
}