<%@ Page Language="C#" CodeBehind="Spotify.dll" Inherits="Spotify" %>
<!DOCTYPE html>
<html>
	<head>
		<title>Spotify pero no</title>
		<meta charset="utf-8">
		<script language="javascript" type="text/javascript">
			function funcion_inicial(){
				var audio = document.getElementById("reproductor");
				//ejecuta el botón cuanto termina una canción para pasar a la siguiente  desde el code Behind
				audio.addEventListener("ended", function() {
					botonOcultoEjecutar();
				});
			}
			function botonOcultoEjecutar(){
				var boton = document.getElementById('<%=btnEjecutar.ClientID%>');
				boton.click(); 
			}
			function errorAdd(){
				alert("Esta cancion ya se encuentra en la playlist");
			}
			function addCancion(){
				alert("Cancion agregada con exito");
			}
		</script>
	</head>
	<body onload="funcion_inicial()">
		<form action=? runat="server" method="post">
			
			<div>
				<h3>Musica</h3>
				<ul runat="server" id="limus">
				</ul>
			</div>
			<div>
				<h3>Lista de reproduccion</h3>
				<audio runat="server" id="reproductor" controls autoplay>
				</audio>
				<ul runat="server" id="lirep"></ul>
			</div>
			<div id="botones"><!--Este es un botón oculto para poder manejar la música desde el code Behind-->
				<asp:Button id="btnEjecutar" runat="server" Text="Get" Style="display: none" OnClick="tocarSiguiente"/>
			</div>
		</form>
	</body>
</html> 