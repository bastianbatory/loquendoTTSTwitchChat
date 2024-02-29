using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using WMPLib;
using System.Collections.Generic;
using Newtonsoft.Json;

public class CPHInline
{
    public bool Execute()
    {

        try
        {

            string toC = args["rawInput"].ToString();
            string voz = args["input0"].ToString();
            string user = args["user"].ToString();
            string userColor = args["color"].ToString();
            string broadcastUser = args["broadcastUser"].ToString();
            string textCommandN = toC.Replace(" ", "%20");
            string textCommand = user + "%20dice:%20" + textCommandN;
            string urlEnd = "&EXT=mp3&FNAME=&ACC=15679&SceneID=2701949&HTTP_ERR=&cache_flag=3";
            string urlCarlos = "https://cache-a.oddcast.com/tts/genB.php?EID=2&LID=2&VID=7&TXT=";
            string urlJorge = "https://cache-a.oddcast.com/tts/genB.php?EID=2&LID=2&VID=6&TXT=";
            string urlDiego = "https://cache-a.oddcast.com/tts/genB.php?EID=2&LID=2&VID=4&TXT=";
            string urlJavier = "https://cache-a.oddcast.com/tts/genB.php?EID=4&LID=2&VID=5&TXT=";
            string urlJuan = "https://cache-a.oddcast.com/tts/genB.php?EID=2&LID=2&VID=2&TXT=";
            string urlCarmen = "https://cache-a.oddcast.com/tts/genB.php?EID=2&LID=2&VID=1&TXT=";
            string urlEsperanza = "https://cache-a.oddcast.com/tts/genB.php?EID=2&LID=2&VID=5&TXT=";
            string urlStart = "";

            if (user == broadcastUser || user == "StreamElements" || user == "Nightbot")
            {
                return false;
            }

/*
			Colores
			
            ["SpringGreen", "#00FF7F"];  Carlos
            ["Green", "#00FF00"],    Carlos
            ["FireBrick", "#B22222"],   Carlos
            ["Red", "#FF0000"],   Jorge
            ["CadetBlue", "#5F9EA0"],	Jorge            
            ["GoldenRod", "#DAA520"], 	Diego
            ["Chocolate", "#D2691E"],	Diego
            ["Blue", "#0000FF"],   Javier
            ["DodgerBlue", "#1E90FF"],  Javier
            ["OrangeRed", "#FF4500"],   Juan
            ["SeaGreen", "#2E8B57"],	Juan
            ["YellowGreen", "#9ACD32"],  Carmen
            ["BlueViolet", "#8A2BE2"],  Carmen
            ["Coral", "#FF7F50"],		Esperanza
            ["HotPink", "#FF69B4"],		Esperanza
*/
			if (voz == "--carlos" || userColor == "#00FF00" || userColor == "#B22222")
			{
				urlStart = urlCarlos;
				textCommand = textCommand.Replace("--carlos", "");
			}
            else if (voz == "--jorge" || userColor == "#FF0000" || userColor == "#5F9EA0")
            {
                urlStart = urlJorge;
                textCommand = textCommand.Replace("--jorge", "");
            }
            else if (voz == "--diego" || userColor == "#DAA520" || userColor == "#D2691E")
            {
                urlStart = urlDiego;
                textCommand = textCommand.Replace("--diego", "");
            }
            else if (voz == "--javier" || userColor == "#0000FF" || userColor == "#1E90FF")
            {
                urlStart = urlJavier;
                textCommand = textCommand.Replace("--javier", "");
            }
            else if (voz == "--juan" || userColor == "#FF4500" || userColor == "#2E8B57")
            {
                urlStart = urlJuan;
                textCommand = textCommand.Replace("--juan", "");
            }
            else if (voz == "--carmen" || userColor == "#9ACD32" || userColor == "#8A2BE2")
            {
                urlStart = urlCarmen;
                textCommand = textCommand.Replace("--carmen", "");
            }
            else if (voz == "--esperanza" || userColor == "#FF7F50" || userColor == "#FF69B4")
            {
                urlStart = urlEsperanza;
                textCommand = textCommand.Replace("--esperanza", "");
            }
            else
            {
                var array = new List<string>
                {
                    urlCarlos,
                    urlJorge,
                    urlDiego,
                    urlJavier,
                    urlJuan,
                    urlCarmen,
                    urlEsperanza
                };
                Random random = new Random();
                int i = random.Next(array.Count);
                urlStart = array[i];
            }
   

            string url = urlStart + textCommand + urlEnd;
          //  CPH.SendMessage(url, false);
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    string randomFileName = Path.GetRandomFileName().Replace(".", "") + ".mp3"; // Generar un nombre de archivo aleatorio con extensión .mp3
                    string audioFilePath = Path.Combine(Directory.GetCurrentDirectory(), randomFileName); // Ruta completa del archivo
                    using (Stream stream = response.Content.ReadAsStreamAsync().Result)
                    using (FileStream fileStream = new FileStream(audioFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        stream.CopyToAsync(fileStream).Wait();
                    }

                    // Obtener la duración del audio
                    WMPLib.WindowsMediaPlayer wmp = new WMPLib.WindowsMediaPlayer();
                    IWMPMedia mediaInfo = wmp.newMedia(audioFilePath);
                    double duration = mediaInfo.duration;
                    string durationString = duration.ToString().Replace(",", "");
                    CPH.SetArgument("duracion", durationString); // En streamer.bot agregar una sub-action delay, y el delay dejarlo en %duracion%
                    WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();
                    player.URL = audioFilePath;
                    player.controls.play(); // Reproduce el archivo
                    Console.WriteLine("Archivo de audio descargado exitosamente.");
                    File.Delete(audioFilePath); // Borra el archivo creado
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
