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
  protected List<cancion> lirep = new List<cancion>();//lista de reproducción
  protected HtmlGenericControl limus;//lista donde se ve la música en la vista

  protected void Page_Init(object sender, EventArgs e){
    miBase();
  }

  public void agregar(object sender, EventArgs e){
    Response.Write("<p>asdfghjgfdsasdfgh</p>");
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
        cancion a = new cancion(registros["mus_id"].ToString(),registros["mus_nombre"].ToString(),registros["mus_url"].ToString());
        mus.Add(a);

        //Esta parte hace la lista
        HtmlGenericControl li = new HtmlGenericControl("li");
        li.Attributes.Add("id", registros["mus_id"].ToString());
        LinkButton botonesA = new LinkButton();
        botonesA.Text = registros["mus_nombre"].ToString();
        botonesA.ID = "li"+registros["mus_id"].ToString();
        botonesA.Click += new System.EventHandler(agregar);
        li.Controls.Add(botonesA);
        //li.InnerHtml = "<asp:LinkButton runat='server' OnClick='agregar' id='li"+registros["mus_id"].ToString()+"'>"+registros["mus_nombre"].ToString()+"</asp:LinkButton>";
        limus.Controls.Add(li);


        //Response.Write("<h1>"+registros["mus_nombre"].ToString()+"</h1>");
      }

    }catch(MySqlException ex){
      Response.Write(ex.ToString());
    }
  }
}