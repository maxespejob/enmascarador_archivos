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
            inputDirectory = Defaults.InputPath;
            outputDirectory = Defaults.OutputPath;
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

        // Implement logic for masking transactions
        public void MaskVisaTransactions ()
        {
            foreach (string path in visa_files)
            {
                Console.WriteLine(path);
                List<string> linesToWrite = new List<string>();

                string lineWithErrors = "";
                int? referenceLength = null; // Holds the reference line length (168 or 170)

                try
                {
                    bool skipFile = false;

                    foreach (string line in File.ReadLines(path))
                    {
                        // Check and set the reference line length
                        if (!referenceLength.HasValue)
                        {
                            if (line.Length == 168 || line.Length == 170)
                            {
                                referenceLength = line.Length; // Set the reference length
                            }
                            else
                            {
                                Logger.SaveLog($"Initial line length error in file {path}. Line length is {line.Length}, but expected 168 or 170.");
                                Logger.SaveLog("Error in line: " + line);
                                skipFile = true;
                                break; // Stop processing lines for this file
                            }
                        }
                        else if (line.Length != referenceLength.Value)
                        {
                            if (line.Length >= 2 && valid_transaction_codes.Contains(line.Substring(0, 2)))
                            {
                                // Check if subsequent lines match the reference length
                                Logger.SaveLog($"Line length mismatch in file {path}. Expected length {referenceLength.Value}, but got {line.Length}.");
                                Logger.SaveLog("Error in line: " + line);
                                skipFile = true;
                                break; // Stop processing lines for this file
                            }
                            
                        }

                        if (IsAnSMSTransactionValid(line))
                        {
                            int report_text_initial_position = PositionWithoutSpaces(line, 34);

                            int account_number_initial_position = report_text_initial_position + 96;

                            string line_with_account_number_masked = line.Substring(0, account_number_initial_position + 9) + new string('*', 10) + line.Substring(account_number_initial_position + 19);

                            linesToWrite.Add(line_with_account_number_masked);
                        }

                        else if (IsTransactionCodeValid(line) && IsTransactionComponentSequenceNumberValid(line))
                        {
                            int account_number_initial_position = PositionWithoutSpaces(line, 4);

                            string line_with_account_number_masked = line.Substring(0, account_number_initial_position + 9) + new string('*', 7) + line.Substring(account_number_initial_position + 16);

                            linesToWrite.Add(line_with_account_number_masked);
                        }
                        else
                        {
                            linesToWrite.Add(line);
                        }

                        lineWithErrors = line;
                    }

                    if (skipFile)
                    {
                        // Skip processing this file and move to the next one
                        Logger.SaveLog($"{path} skipped due to line length errors.");
                        continue;
                    }

                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
                    string fileExtension = Path.GetExtension(path);

                    ManageFiles.EnsureDirectoryExists(outputDirectory);

                    // Construir el nuevo nombre de archivo con el sufijo "_masked" antes de la extensión
                    string outputFile = Path.Combine(outputDirectory, $"{fileNameWithoutExtension}_masked{fileExtension}");

                    File.WriteAllLines(outputFile, linesToWrite);

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
