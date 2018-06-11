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

  protected void Page_Init(object sender, EventArgs e){
    miBase();//carga la lista de música
  }

  //Solo llega agregar una canción porque cuando se vuelve a seleccionar algo carga toda la página de nuevo, por lo demás funciona
  public void agregar(object sender, EventArgs e){
    try{
      LinkButton idcan=sender as LinkButton;//idcan es la canción seleccionada
      foreach (cancion canci in mus){//recorremos la lista para buscar la canción
        if(canci.id==idcan.ID){
          rep.Add(canci);//se agrega a la lista reproducción
          cambiarCancion(canci);//se cambia la canción en el reproductor
          // Ahora se procede a insertar la cancion en la playlist
          string datab = "Server=127.0.0.1;Database=prufi;Uid=root;Pwd='';sslmode=none";
          MySqlConnection dba = new MySqlConnection(datab);
          dba.Open();
          string validar = "select * from music_play where musica_id = "+canci.id+" and playlist_id = 1";
          MySqlCommand comando = new MySqlCommand(validar, dba); 
          MySqlDataReader readerValidar = comando.ExecuteReader();  
          bool noExiste = true;
          // Se busca si la cancion ya se encuentra registrada en la playlist
          while(readerValidar.Read()){
            noExiste = false;
          }
          readerValidar.Close();
          // En caso de que no se encuentre registrada se agrega a la playlist
          if(noExiste){
            string insertar = "insert into music_play (musica_id, playlist_id) values ("+canci.id+",1)"; 
            comando = new MySqlCommand(insertar, dba); 
            MySqlDataReader readerInsert = comando.ExecuteReader();  
            readerInsert.Close();
            cargarReproduccion();//se agrega a la lista visual de reproducción
            Page.ClientScript.RegisterStartupScript(GetType(),"errorAdd","addCancion()",true); //Ejecuta la funcion js addCancion() en el archivo vistaSpotify.aspx
          }else{
            Page.ClientScript.RegisterStartupScript(GetType(),"errorAdd","errorAdd()",true); // Ejecuta la funcion js errorAdd() en el archivo vistaSpotify.aspx
          }
          dba.Close();
        }
      }
    }catch(MySqlException ex){
      Response.Write(ex.ToString());
    }
  }

  public void cambiarCancion(cancion tocando){//cambia la canción que está puesta en el reproductor
    HtmlGenericControl source=new HtmlGenericControl("source");
    source.Attributes.Add("src",tocando.url);
    reproductor.Controls.Add(source);
  }

  //Esta debería quitar la cancion seleccionada de la lista de reproducción, pero no llega a ejecutarse, no entiendo porqué 
  public void quitar(object sender,EventArgs e){
    Response.Write("Holaaaaaaaaaaaaaaaaaaa");
    LinkButton idcan=sender as LinkButton;//idcan es la canción seleccionada
    foreach (cancion canci in rep){//recorremos la lista para buscar la canción
      if(canci.id==idcan.ID){
        rep.Remove(canci);//se quita la canción de la lista

        //Esto no debería ir, pero lo dejo porque hay que tener en cuenta en qué quedará el reproductor:
        cambiarCancion(canci);
      }
    }
    cargarReproduccion();//Vuelve a cargar para que cargue la lista sin la canción que se acaba de sacar
  }
  
  //como la página carga cada de nuevo cada vez despues de clickear solo guarda una canción por vez
  public void cargarReproduccion(){
    foreach (cancion canci in rep){//recorremos la lista para cargar la lista de reproducción entera
      HtmlGenericControl li = new HtmlGenericControl("li");
      LinkButton tocar = new LinkButton();
      tocar.Text = canci.nombre;
      tocar.ID = canci.id;
      tocar.Click += new System.EventHandler(quitar);
      li.Controls.Add(tocar);
      lirep.Controls.Add(li);
    }
  }

  public void miBase(){
    try{
      string @base="Server=127.0.0.1;Database=prufi;Uid=root;Pwd='';sslmode=none";
      MySqlConnection conexion = new MySqlConnection(@base);

      conexion.Open();

      string miSql = "obtener_musica";

      MySqlCommand miComando = new MySqlCommand(miSql,conexion);
      miComando.CommandType = CommandType.StoredProcedure;
      MySqlDataReader registros = miComando.ExecuteReader();
      //miComando.ExecuteNonQuery();

      while(registros.Read()){
        //Esto guarda los datos de las canciones en la lista de musica
        cancion ca = new cancion(registros["mus_id"].ToString(),registros["mus_nombre"].ToString(),registros["mus_url"].ToString());
        mus.Add(ca);

        //Esta parte hace la lista
        HtmlGenericControl li = new HtmlGenericControl("li");
        LinkButton botonesA = new LinkButton();
        botonesA.Text = registros["mus_nombre"].ToString();
        botonesA.ID = registros["mus_id"].ToString();
        botonesA.Click += new System.EventHandler(agregar);
        li.Controls.Add(botonesA);
        limus.Controls.Add(li);
      }
      registros.Close();
      miSql = "select mus_id, mus_nombre, mus_url from musica left join music_play on mus_id = musica_id where playlist_id = 1 group by mus_id";
      MySqlCommand comando = new MySqlCommand(miSql,conexion);
      MySqlDataReader playlist = comando.ExecuteReader();
      while(playlist.Read()){
        //Esto busca todas la canciones de la lista principal
        cancion c = new cancion(playlist["mus_id"].ToString(),playlist["mus_nombre"].ToString(),playlist["mus_url"].ToString());
        HtmlGenericControl li = new HtmlGenericControl("li");
        LinkButton tocar = new LinkButton();
        tocar.Text = c.nombre;
        tocar.ID = "cancion"+c.id;
        tocar.Click += new System.EventHandler(quitar);
        li.Controls.Add(tocar);
        lirep.Controls.Add(li);
      }
      conexion.Close();
    }catch(MySqlException ex){
      Response.Write(ex.ToString());
    }
  }
}