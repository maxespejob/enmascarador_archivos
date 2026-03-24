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
            string inputDirectory = Defaults.InputPathMC;
            string outputDirectory = Defaults.OutputPathMC;

            if (Directory.Exists(inputDirectory))
            {
                string[] files = Directory.GetFiles(inputDirectory);

                foreach (string file in files)
                {
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                    string fileExtension = Path.GetExtension(file);

                    // ✅ ÚNICO OUTPUT FINAL
                    string finalFile = Path.Combine(outputDirectory, $"{fileNameWithoutExtension}_final_masked{fileExtension}");

                    // Archivos temporales internos
                    string tempFile = Path.Combine(outputDirectory, $"{fileNameWithoutExtension}_temp{fileExtension}");
                    string intermediateMasked = Path.Combine(outputDirectory, $"{fileNameWithoutExtension}_masked{fileExtension}");

                    if (onlyMask)
                    {
                        bool is_successful = false;
                        try
                        {
                            // Intento directo → guardar como final
                            is_successful = interpretador.ReadAndWriteTransactions(file, finalFile);

                            if (is_successful)
                            {
                                Logger.SaveLog($"{file} masked successfully (direct).");
                            }
                            else
                            {
                                Logger.SaveLog($"{file} failed direct masking. Trying with unblock...");

                                UnblockFilesMC.ExtractTransactionsToFile(file, tempFile);
                                is_successful = interpretador.ReadAndWriteTransactions(tempFile, intermediateMasked);

                                if (is_successful)
                                {
                                    // Reblock hacia archivo final
                                    byte[] controlBytes = UnblockFilesMC.ReadControlBytes(file);
                                    UnblockFilesMC.ReblockFile(intermediateMasked, finalFile, controlBytes);

                                    Logger.SaveLog($"{file} masked and reblocked successfully.");
                                }
                                else
                                {
                                    Logger.SaveLog($"{file} failed masking process in both configurations.");
                                }
                            }
                        }
                        finally
                        {
                            if (File.Exists(tempFile))
                                File.Delete(tempFile);

                            if (File.Exists(intermediateMasked))
                                File.Delete(intermediateMasked);
                        }
                    }
                    else
                    {
                        bool is_successful = false;
                        try
                        {
                            // Flujo principal: unblock → mask → reblock → final
                            UnblockFilesMC.ExtractTransactionsToFile(file, tempFile);
                            is_successful = interpretador.ReadAndWriteTransactions(tempFile, intermediateMasked);

                            if (is_successful)
                            {
                                byte[] controlBytes = UnblockFilesMC.ReadControlBytes(file);
                                UnblockFilesMC.ReblockFile(intermediateMasked, finalFile,controlBytes);

                                Logger.SaveLog($"{file} masked and reblocked successfully.");
                            }
                            else
                            {
                                Logger.SaveLog($"{file} failed after unblock. Trying direct masking...");

                                // fallback directo → final
                                is_successful = interpretador.ReadAndWriteTransactions(file, finalFile);

                                if (is_successful)
                                {
                                    Logger.SaveLog($"{file} masked successfully (direct, no unblock needed).");
                                }
                                else
                                {
                                    Logger.SaveLog($"{file} failed masking process in both configurations.");
                                }
                            }
                        }
                        finally
                        {
                            if (File.Exists(tempFile))
                                File.Delete(tempFile);

                            if (File.Exists(intermediateMasked))
                                File.Delete(intermediateMasked);
                        }
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