using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using WMPLib;
using System.Collections.Generic;

public class CPHInline
{
    public bool Execute()
    {
        try
        {
            // Parámetros
            //Dictionary<string, object> args = new Dictionary<string, object>(); // Asumiendo que args está definido en algún lugar

            string toC = args["rawInput"].ToString();
            string voz = args["input0"].ToString();
            string user = args["user"].ToString();
            string broadcastUser = args["broadcastUser"].ToString();
            string textCommandN = toC.Replace(" ", "%20");
            string textCommand = user + "%20dice:%20" + textCommandN;
            string urlCarlos = "https://cache-a.oddcast.com/tts/genB.php?EID=2&LID=2&VID=7&TXT=";
            string urlJorge = "https://cache-a.oddcast.com/tts/genB.php?EID=2&LID=2&VID=6&TXT=";
            string urlDiego = "https://cache-a.oddcast.com/tts/genB.php?EID=2&LID=2&VID=4&TXT=";
            string urlJavier = "https://cache-a.oddcast.com/tts/genB.php?EID=4&LID=2&VID=5&TXT=";
            string urlJuan = "https://cache-a.oddcast.com/tts/genB.php?EID=2&LID=2&VID=2&TXT=";
            string urlCarmen = "https://cache-a.oddcast.com/tts/genB.php?EID=2&LID=2&VID=1&TXT=";
            string urlEsperanza = "https://cache-a.oddcast.com/tts/genB.php?EID=2&LID=2&VID=6&TXT=";
            
            if (user == broadcastUser)
            {
            	return false;
            }
            
             
            //CPH.SendMessage(textCommand, false);
            string urlStart = "";
            if (voz == "--carlos")
            {
				urlStart = urlCarlos;
				textCommand = textCommand.Replace("--carlos", "");
			}
			else if (voz == "--jorge")
			{
				urlStart = urlJorge;
				textCommand = textCommand.Replace("--jorge", "");
			}
			else if (voz == "--diego")
			{
				urlStart = urlDiego;
				textCommand = textCommand.Replace("--diego", "");
			}
			else if (voz == "--javier")
			{
				urlStart = urlJavier;
				textCommand = textCommand.Replace("--javier", "");
			}
			else if (voz == "--juan")
			{
				urlStart = urlJuan;
				textCommand = textCommand.Replace("--juan", "");
			}
            else if (voz == "--carmen")
			{
				urlStart = urlCarmen;
				textCommand = textCommand.Replace("--carmen", "");
			}
            else if (voz == "--esperanza")
			{
				urlStart = urlEsperanza;
				textCommand = textCommand.Replace("--esperanza", "");
			}
			else
			{
				var array = new List<string> { urlCarlos, urlJorge, urlDiego, urlJavier, urlJuan, urlCarmen, urlEsperanza };
				Random random = new Random();
				int i = random.Next(array.Count);
				urlStart = array[i];
			}

            string urlEnd = args["urlEnd"].ToString();
            string url = urlStart + textCommand + urlEnd;
            //CPH.SendMessage(url, false);

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    string randomFileName = Path.GetRandomFileName().Replace(".", "") + ".mp3"; // Generar un nombre de archivo aleatorio con extensión .mp3
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), randomFileName); // Ruta completa del archivo

                    using (Stream stream = response.Content.ReadAsStreamAsync().Result)
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        stream.CopyToAsync(fileStream).Wait();
                    }

                    // Obtener la duración del audio
                    WMPLib.WindowsMediaPlayer wmp = new WMPLib.WindowsMediaPlayer();
                    IWMPMedia mediaInfo = wmp.newMedia(filePath);
                    double durationd = mediaInfo.duration;
                    string durationf = durationd.ToString();
                    string duration = durationf.Replace(",", "");

                    Console.WriteLine($"Duración del audio: {duration} segundos");

                    // Enviar el mensaje con la ruta completa del archivo
                   // CPH.SendMessage(user, false);
                    
                    CPH.SetArgument("duracion", duration);

                    // Reproducir el audio
                    WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();
                    player.URL = filePath;
                    player.controls.play();
                    
                    Console.WriteLine("Archivo de audio descargado exitosamente.");
                    File.Delete(filePath);
                    return true;
                }
                else
                {
                    Console.WriteLine($"Error al descargar el archivo de audio. Código de estado: {response.StatusCode}");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al descargar el archivo de audio: {ex.Message}");
            return false;
        }
    }
}
