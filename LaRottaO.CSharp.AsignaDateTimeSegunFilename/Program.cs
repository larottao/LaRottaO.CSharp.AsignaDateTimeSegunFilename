using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace LaRottaO.CSharp.AsignaDateTimeSegunFilename
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string rootDirectory = "C:\\Multimedia\\Takeout2\\";

            string[] filePaths = Directory.GetFiles(rootDirectory, "*", SearchOption.AllDirectories);

            CultureInfo culture = CultureInfo.InvariantCulture;

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
                        File.SetCreationTime(filePath, result);
                        File.SetLastWriteTime(filePath, result);
                        //Console.WriteLine(filePath + " Corregido a: " + result.ToString());
                        parseoExitoso = true;
                        break;
                    }
                }

                if (!parseoExitoso)
                {
                    Console.WriteLine("<" + fileName + "> no se pudo parsear.");
                }
            }

            Console.ReadLine();
        }
    }
}