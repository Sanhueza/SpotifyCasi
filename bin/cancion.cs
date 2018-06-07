public class cancion{
	public string id;
	public string nombre;
	public string url;

	public cancion(){}

	public cancion(string i,string nom,string ur){
		this.id=i;
		this.nombre=nom;
		this.url=ur;
	}

	public void setNombre(string nom){
		this.nombre=nom;
	}
}