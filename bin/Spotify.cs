using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;

public class Spotify: Page{

  protected List<cancion> mus = new List<cancion>();
  protected List<cancion> lirep = new List<cancion>();
  protected HtmlGenericControl limus;
  protected cancion cncn=new cancion();

  protected void Page_Load(){
    if(IsPostBack){
      miBase();
    }
  }

  public void miBase(){
    try{
      string @base="Server=127.0.0.1;Database=prufi;Uid=root;Pwd='mariadb';sslmode=none";
      MySqlConnection conexion = new MySqlConnection(@base);

      conexion.Open();

      string miSql = "obtener_musica";

      MySqlCommand miComando = new MySqlCommand(miSql,conexion);
      miComando.CommandType = CommandType.StoredProcedure;
      MySqlDataReader registros = miComando.ExecuteReader();
      //miComando.ExecuteNonQuery();

      while(registros.Read()){
        cancion a = new cancion(registros["mus_id"].ToString(),registros["mus_nombre"].ToString(),registros["mus_url"].ToString());
        mus.Add(a);
        HtmlGenericControl li = new HtmlGenericControl("li");
        li.Attributes.Add("class", registros["mus_id"].ToString());
        li.InnerText = registros["mus_nombre"].ToString();
        limus.Controls.Add(li);

        //Response.Write("<h1>"+registros["mus_nombre"].ToString()+"</h1>");
      }
    }catch(MySqlException ex){
      Response.Write(ex.ToString());
    }
  }
}