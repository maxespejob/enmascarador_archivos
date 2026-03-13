using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterchangeFilesMaskingApp
{
    internal class ManageFiles
    {
        public static int EnsureDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);

                return 1;
            }

            return 0;
        }

        public static void ManageMCInterpreter(InterpretFilesMC interpretador, bool onlyMask)
        {
            string inputDirectory = Defaults.InputPath;
            string outputDirectory = Defaults.OutputPath;

            // Verificar si el directorio existe
            if (Directory.Exists(inputDirectory))
            {
                // Obtener todos los archivos dentro del directorio
                string[] files = Directory.GetFiles(inputDirectory);

                // Mostrar cada archivo encontrado
                foreach (string file in files)
                {
                    // Dividir el nombre del archivo en nombre base y extensión
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                    string fileExtension = Path.GetExtension(file);

                    // Construir el nuevo nombre de archivo con el sufijo "_masked" antes de la extensión
                    string outputFile = Path.Combine(outputDirectory, $"{fileNameWithoutExtension}_masked{fileExtension}");

                    // Aplicar las operaciones sobre el archivo
                    string tempFile = Path.Combine(Path.GetDirectoryName(file), $"{fileNameWithoutExtension}_temp{fileExtension}");

                    if (onlyMask)
                    {
                        bool is_succesful = interpretador.ReadAndWriteTransactions(file, outputFile);

                        if(is_succesful)
                        {
                            Logger.SaveLog($"{file} masked successfully.");
                        }
                        else
                        {
                            Logger.SaveLog($"{file} failed the masking process in the current configuration");
                            Logger.SaveLog("Trying second configuration...");

                            UnblockFilesMC.ExtractTransactionsToFile(file, tempFile);
                            is_succesful = interpretador.ReadAndWriteTransactions(tempFile, outputFile);

                            if (is_succesful)
                            {
                                Logger.SaveLog($"{file} masked successfully.");
                            }
                            else
                            {
                                Logger.SaveLog($"{file} failed masking process.");
                            }

                            File.Delete(tempFile);
                        }
                    }

                    else
                    {
                        UnblockFilesMC.ExtractTransactionsToFile(file, tempFile);
                        bool is_succesful = interpretador.ReadAndWriteTransactions(tempFile, outputFile);

                        if (is_succesful)
                        {
                            Logger.SaveLog($"{file} masked successfully.");
                        }
                        else
                        {
                            Logger.SaveLog($"{file} failed the masking process in the current configuration");
                            Logger.SaveLog("Trying second configuration...");

                            is_succesful = interpretador.ReadAndWriteTransactions(file, outputFile);

                            if(is_succesful)
                            {
                                Logger.SaveLog($"{file} masked successfully.");
                            }
                            else
                            {
                                Logger.SaveLog($"{file} failed masking process.");
                            }                            
                        }

                        File.Delete(tempFile);
                    }
                }
            }
            else
            {
                Console.WriteLine("Input directory does not exist");
                Console.ReadLine();
            }
        }
    }
}
