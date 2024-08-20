using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;

namespace LaRottaO.CSharp.AsignaDateTimeSegunFilename
{
    internal class Program
    {
        private static CultureInfo culture = CultureInfo.InvariantCulture;

        static bool move = false;
        static bool renameWithDate = true;
        private static void Main(string[] args)
        {
            string rootDirectory = "C:\\Users\\Pantufla\\Desktop\\EXPORT PHOTOS\\Takeout\\Google Photos\\Photos from 2024";
         
            string[] filePaths = Directory.GetFiles(rootDirectory, "*", SearchOption.AllDirectories);

            List<string> posiblesFormatos = new List<string>
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

                bool exitoExif = corregirDatosUsandoEXIF(filePath);

                if (exitoExif)
                {
                    continue;
                }

                string fileName = Path.GetFileNameWithoutExtension(filePath);

                // Clean the filename
                fileName = fileName.Replace("IMG_", "")
                                  .Replace("IMG-", "")
                                  .Replace(".IMG_", "")
                                  .Replace("XRecorder_", "")
                                  .Replace("WhatsApp Video ", "")
                                  .Replace("WhatsApp Image ", "")
                                  .Replace(" at ", " ")
                                  .Replace(".VID_", "")
                                  .Replace("1-edited", "")
                                  .Replace("(1)", "")
                                  .Replace("(2)", "")
                                  .Replace("VID_", "")
                                  .Replace("VID-", "")
                                  .Replace("PM1", "")
                                  .Replace("AM1", "")
                                  .Replace("Captura de pantalla ", "")
                                  .Replace("Screenshot_", "")
                                  .Replace("_com.whatsapp", "")
                                  .Replace("_WhatsApp", "");

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

                foreach (string posibleFormato in posiblesFormatos)
                {
                    if (DateTime.TryParseExact(fileName, posibleFormato, culture, DateTimeStyles.None, out DateTime result))
                    {
                        if (move)
                        {
                            MoveFileToDateFolder(filePath, result);
                        }
                        else if (renameWithDate)
                        {
                            RenameFileWithDate(filePath, result);
                        }
                        break;
                    }
                }
            }

            Console.WriteLine("Presione una tecla para salir...");
            Console.ReadLine();
        }

        public static bool corregirDatosUsandoEXIF(string filePath)
        {
            string dateTaken = null;

            using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    Image image = Image.FromStream(fs);
                    PropertyItem propItem = image.GetPropertyItem(36867);
                    dateTaken = Encoding.UTF8.GetString(propItem.Value);
                    image.Dispose();
                }
                catch (Exception ex)
                {
                    fallido(filePath, ex.Message);
                    return false;
                }
            }

            if (dateTaken == null)
            {
                fallido(filePath, "EXIF no encontrado.");
                return false;
            }

            dateTaken = dateTaken.Replace("\0", "").Trim();

            if (DateTime.TryParseExact(dateTaken, "yyyy:MM:dd HH:mm:ss", culture, DateTimeStyles.None, out DateTime result))
            {
                if (move)
                {
                    MoveFileToDateFolder(filePath, result);
                }
                else if (renameWithDate)
                {
                    RenameFileWithDate(filePath, result);
                }
                return true;
            }
            else
            {
                fallido(filePath, "Se encontro EXIF, pero no se pudo hacer parsing.");
                return false;
            }
        }

        public static void fallido(string filePath, string motivoFallo)
        {
            Console.WriteLine(filePath + " no se pudo corregir. " + motivoFallo);
        }

        public static void MoveFileToDateFolder(string filePath, DateTime result)
        {
            string carpeta = Path.GetDirectoryName(filePath) + @"\" + result.ToString("yyyy-MM-dd");

            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }

            File.Move(filePath, carpeta + @"\" + Path.GetFileName(filePath));
            Console.WriteLine(filePath + " Movido a: " + carpeta);
        }

        public static void RenameFileWithDate(string filePath, DateTime result)
        {
            string directory = Path.GetDirectoryName(filePath);
            string newFileName = result.ToString("yyyy-MM-dd") + " " + Path.GetFileName(filePath);
            string newFilePath = Path.Combine(directory, newFileName);

            File.Move(filePath, newFilePath);
            Console.WriteLine(filePath + " Renombrado a: " + newFileName);
        }
    }
}
