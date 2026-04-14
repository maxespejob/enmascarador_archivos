using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterchangeFilesMaskingApp
{
    internal class InterpretFilesVisa
    {
        string inputDirectory;
        string outputDirectory;
        string[] visa_files;

        HashSet<string> valid_transaction_codes = new HashSet<string> { "05", "06", "07", "15", "16", "17", "25", "26", "27", "35", "36", "37" };
        HashSet<string> valid_transaction_codes_sms = new HashSet<string> { "33" };
        HashSet<string> exceptions_transaction_codes = new HashSet<string> { "50" };

        // Constructor
        public InterpretFilesVisa()
        {
            inputDirectory = Defaults.InputPathVisa;
            outputDirectory = Defaults.OutputPathVisa;
            visa_files = Directory.GetFiles(inputDirectory);
        }

        // Examine if a line of the visa file contains a valid transaction code
        public bool IsTransactionCodeValid(string line)
        {
            if (line.Length >= 2)
            {
                return valid_transaction_codes.Contains(line.Substring(0, 2));
            }

            return false;
        }

        // Examine if a line of the visa file contains a valid transaction code
        public bool IsAnSMSTransactionValid(string line)
        {
            if (line.Length < 2)
            {
                return false;
            }

            bool isSMSTC = valid_transaction_codes_sms.Contains(line.Substring(0, 2));

            int report_identifier_start_position = PositionWithoutSpaces(line, 16);

            if (report_identifier_start_position + 10 > line.Length)
            {
                return false;
            }

            string report_identifier = line.Substring(report_identifier_start_position, 10);

            bool isReportIdentifierValid = (report_identifier == "SMSRAWDATA");

            int record_type_start_position = PositionWithoutSpaces(line, 34);

            if (record_type_start_position + 6 > line.Length)
            {
                return false;
            }

            string record_type = line.Substring(record_type_start_position, 6);

            bool isRecordTypeValid = (record_type == "V22200");

            return isSMSTC && isReportIdentifierValid && isRecordTypeValid;
        }

        // Examine if a line of the visa file contains a valid transaction component sequence number
        public bool IsTransactionComponentSequenceNumberValid (string line)
        {
            string temp_line = line.Replace(" ", "");

            if (temp_line.Length > 3 && temp_line.Substring(3, 1) == "0")
            {
                return true;
            }

            return false;
        }

        public int PositionWithoutSpaces(string line, int position)
        {
            int relative_position = 0;

            for (int current_position = 0; current_position < line.Length; current_position++)
            {
                if (!char.IsWhiteSpace(line[current_position]))
                {
                    relative_position++;
                }

                if (relative_position == position)
                {
                    return current_position + 1;
                }
            }
            return line.Length;
        }
        //Forzando que el salto de línea del archivo final siempre sea el de Windows - Esto solo aplica para BTRLRO - 2026-04-14
        // Implement logic for masking transactions
    public void MaskVisaTransactions()
    {
        foreach (string path in visa_files)
        {
            Console.WriteLine(path);

            string lineWithErrors = "";
            int? referenceLength = null;

            try
            {
                bool skipFile = false;

                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
                string fileExtension = Path.GetExtension(path);

                ManageFiles.EnsureDirectoryExists(outputDirectory);

                string outputFile = Path.Combine(outputDirectory, $"{fileNameWithoutExtension}{fileExtension}");

                using (var writer = new StreamWriter(outputFile, false))
                {
                    // 🔥 Forzar salto de línea Windows
                    writer.NewLine = "\r\n";

                    foreach (string line in File.ReadLines(path))
                    {
                        // Validación de longitud de línea
                        if (!referenceLength.HasValue)
                        {
                            if (line.Length == 168 || line.Length == 170)
                            {
                                referenceLength = line.Length;
                            }
                            else
                            {
                                Logger.SaveLog($"Initial line length error in file {path}. Line length is {line.Length}, but expected 168 or 170.");
                                Logger.SaveLog("Error in line: " + line);
                                skipFile = true;
                                break;
                            }
                        }
                        else if (line.Length != referenceLength.Value)
                        {
                            if (line.Length >= 2 && valid_transaction_codes.Contains(line.Substring(0, 2)))
                            {
                                Logger.SaveLog($"Line length mismatch in file {path}. Expected length {referenceLength.Value}, but got {line.Length}.");
                                Logger.SaveLog("Error in line: " + line);
                                skipFile = true;
                                break;
                            }
                        }

                        string processedLine;

                        if (IsAnSMSTransactionValid(line))
                        {
                            int report_text_initial_position = PositionWithoutSpaces(line, 34);
                            int account_number_initial_position = report_text_initial_position + 96;

                            processedLine =
                                line.Substring(0, account_number_initial_position + 9) +
                                new string('*', 10) +
                                line.Substring(account_number_initial_position + 19);
                        }
                        else if (IsTransactionCodeValid(line) && IsTransactionComponentSequenceNumberValid(line))
                        {
                            int account_number_initial_position = PositionWithoutSpaces(line, 4);

                            processedLine =
                                line.Substring(0, account_number_initial_position + 9) +
                                new string('*', 7) +
                                line.Substring(account_number_initial_position + 16);
                        }
                        else
                        {
                            processedLine = line;
                        }

                        // ✅ Escribir con salto de línea Windows
                        writer.WriteLine(processedLine);

                        lineWithErrors = line;
                    }
                }

                if (skipFile)
                {
                    Logger.SaveLog($"{path} skipped due to line length errors.");
                    continue;
                }

                Logger.SaveLog($"{path} masked successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Logger.SaveLog($"{path} failed masking process.");

                if (lineWithErrors != "")
                {
                    Logger.SaveLog("Error in line: " + lineWithErrors);
                }
            }
        }
    }
    }
}
