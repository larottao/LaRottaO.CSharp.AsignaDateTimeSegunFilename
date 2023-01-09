using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Net.Http.Headers;
using System.Text;

namespace LaRottaO.CSharp.AsignaDateTimeSegunFilename
{
    internal class Program
    {
        private static CultureInfo culture = CultureInfo.InvariantCulture;

        private static void Main(string[] args)
        {
            string rootDirectory = "C:\\Multimedia\\Takeout2\\";

            string[] filePaths = Directory.GetFiles(rootDirectory, "*", SearchOption.AllDirectories);

            List<String> posiblesFormatos = new List<String>
            {
                "ddMMyyyy_hhmmss",
                "yyyy-MM-dd h.mm.ss tt",
                "yyyy-MM-dd h.mm.ss",
                "yyyy-MM-dd HH.mm.fff",
                "yyyy-MM-dd hh.mm.ss",
                "yyyy-MM-dd HH.mm.ss",
                "yyyyMMdd_hhmmss",
                "yyyyMMdd_HHmmss",
                "yyyyMMdd_HHmmss_fff",
                "yyyyMMdd-HHmmss",
                "yyyy-MM-dd-HH-mm-ss-fff",
                "yyyyMMdd",
                "yyyyMMdd_HHmmss_fff",
                "yyyy-MM-dd-HH-mm-ss-fff"
            };

            foreach (string filePath in filePaths)
            {
                if (Path.GetExtension(filePath).Equals(".rar", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                Console.WriteLine(filePath + "--------------------------------------------");

                string fileName = Path.GetFileNameWithoutExtension(filePath);

                fileName = fileName.Replace("IMG_", "");
                fileName = fileName.Replace("IMG-", "");
                fileName = fileName.Replace(".IMG_", "");
                fileName = fileName.Replace("XRecorder_", "");
                fileName = fileName.Replace("WhatsApp Video ", "");
                fileName = fileName.Replace("WhatsApp Image ", "");
                fileName = fileName.Replace(" at ", " ");
                fileName = fileName.Replace(".VID_", "");
                fileName = fileName.Replace("1-edited", "");
                fileName = fileName.Replace("(1)", "");
                fileName = fileName.Replace("(2)", "");
                fileName = fileName.Replace("VID_", "");
                fileName = fileName.Replace("VID-", "");
                fileName = fileName.Replace("PM1", "");
                fileName = fileName.Replace("AM1", "");
                fileName = fileName.Replace("Captura de pantalla ", "");
                fileName = fileName.Replace("Screenshot_", "");
                fileName = fileName.Replace("_com.whatsapp", "");
                fileName = fileName.Replace("_WhatsApp", "");

                fileName = fileName.Trim();

                if (fileName.Contains("-WA"))
                {
                    fileName = fileName.Substring(0, fileName.IndexOf("-WA"));
                }

                if (fileName.Contains("~"))
                {
                    fileName = fileName.Substring(0, fileName.IndexOf("~"));
                }

                if (fileName.StartsWith("_"))
                {
                    fileName = fileName.Substring(1);
                }

                if (fileName.StartsWith("."))
                {
                    fileName = fileName.Substring(1);
                }

                Boolean parseoExitoso = false;

                foreach (String posibleFormato in posiblesFormatos)
                {
                    if (DateTime.TryParseExact(fileName, posibleFormato, culture, DateTimeStyles.None, out DateTime result))
                    {
                        corregirFecha(filePath, result, "filename");
                        parseoExitoso = true;
                        break;
                    }
                }

                if (!parseoExitoso)
                {
                    obtenerExif(filePath);
                }
            }

            Console.ReadLine();
        }

        public static void corregirFecha(String filePath, DateTime result, String mecanismoUsado)
        {
            File.SetCreationTime(filePath, result);
            File.SetLastWriteTime(filePath, result);
            Console.WriteLine(filePath + " Corregido a: " + result.ToString() + " usando " + mecanismoUsado);
        }

        public static void obtenerExif(String filePath)
        {
            String dateTaken = null;

            using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    Image image = Image.FromStream(fs);

                    PropertyItem propItem = image.GetPropertyItem(36867);
                    dateTaken = Encoding.UTF8.GetString(propItem.Value);
                    image.Dispose();
                }
                catch
                {
                    return;
                }
            }

            if (dateTaken == null)
            {
                return;
            }

            dateTaken = dateTaken.Replace("\0", "");
            dateTaken = dateTaken.Trim();

            if (DateTime.TryParseExact(dateTaken, "yyyy:MM:dd HH:mm:ss", culture, DateTimeStyles.None, out DateTime result))
            {
                corregirFecha(filePath, result, "EXIF");
            }
        }
    }
}