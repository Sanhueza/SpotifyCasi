<%@ Page Language="C#" CodeBehind="Spotify.dll" Inherits="Spotify" %>
<!DOCTYPE html>
<html>
	<head>
		<title>Spotify pero no</title>
		<meta charset="utf-8">
	</head>
	<body>
		<form action=? runat="server" method="post">
			
			<div>
				<h3>Musica</h3>
				<ul runat="server" id="limus"> </ul>
			</div>
			<div>
				<audio controls>
  					<source src="musica\01-I.mp3" type="audio/ogg">
					Tu navegador no puede reproducir esta música.
				</audio>
				<h3>Lista de reproduccion</h3>
				<ul runat="server" id="lirep"></ul>
			</div>
		</form>
	</body>
</html> 