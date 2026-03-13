using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace InterchangeFilesMaskingApp
{
    public static class CommandLineInterface
    {
        public static async Task<int> Run(string[] args)
        {
            Console.WriteLine("Welcome to Intelica's Masker CLI Application");
            Console.WriteLine("Please drag and drop the files to be masked into a folder named 'Input' located next to the program.");
            Console.WriteLine();
            Console.WriteLine("No arguments provided. Use '--help' to see usage instructions.");
            Console.WriteLine("Please enter arguments (or type 'exit' to quit):");

            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                var visaCommand = new Command("visa", "Mask VISA files");

                visaCommand.Handler = CommandHandler.Create(() =>
                {
                    EjecutarPasoVI();
                });

                var mastercardCommand = new Command("mastercard", "Mask Mastercard files")
                {
                    new Option<bool>(new string[] { "--blocked1012", "-b" }, "Unblock files"),
                    new Option<string>(new string[] {"--encoding", "-e"}, "Encoding to use for masking (e.g., ebcdic, latin-1)")
                };

                mastercardCommand.Handler = CommandHandler.Create<bool, string>((blocked1012, encoding) =>
                {
                    EjecutarPasoMC(blocked1012, encoding);
                });


                var rootCommand = new RootCommand("Masker CLI Application")
                {
                    visaCommand,
                    mastercardCommand
                };

                var helpCommand = new Command("--help", "Show usage instructions")
                {
                    Handler = CommandHandler.Create(() =>
                    {
                        Console.WriteLine();
                        Console.WriteLine("Usage Instructions:");
                        Console.WriteLine(" [command] [options] [parameters]");
                        Console.WriteLine();
                        Console.WriteLine("Visa File Masking");
                        Console.WriteLine("     Commands:");
                        Console.WriteLine("         visa                    Mask VISA files");
                        Console.WriteLine();
                        Console.WriteLine("Mastercard File Masking");
                        Console.WriteLine("     Commands:");
                        Console.WriteLine("         mastercard              Mask Mastercard files");
                        Console.WriteLine();
                        Console.WriteLine("     Options:");
                        Console.WriteLine("         --blocked1012, -b       (Default: Unblocked) Unblock mastercard files");
                        Console.WriteLine("         --encoding, -e          (Default: EBCDIC) Encoding to use for masking mastercard files (e.g., ebcdic, latin-1)");
                        Console.WriteLine();
                        Console.WriteLine("     Parameters:");
                        Console.WriteLine("         ebcdic                  Use EBCDIC encoding");
                        Console.WriteLine("         latin-1                 Use ISO 8859-1 (Latin-1) encoding");
                        Console.WriteLine();
                        Console.WriteLine("Application Control");
                        Console.WriteLine("     Commands:");
                        Console.WriteLine("         exit                    Exit the application.");
                        Console.WriteLine();
                        Console.WriteLine("Example:");
                        Console.WriteLine(" mastercard --blocked1012 --encoding ebcdic");
                        Console.WriteLine("  This will unblock Mastercard files and mask them using EBCDIC encoding.");
                        
                    })
                };

                var mastercardHelpCommand = new Command("--help", "Show usage instructions for Mastercard command")
                {
                    Handler = CommandHandler.Create(() =>
                    {
                        Console.WriteLine();
                        Console.WriteLine("Usage Instructions:");
                        Console.WriteLine(" [command] [options] [parameters]");
                        Console.WriteLine();
                        Console.WriteLine("Commands:");
                        Console.WriteLine("  mastercard             Mask Mastercard files");
                        Console.WriteLine();
                        Console.WriteLine("Options:");
                        Console.WriteLine("  --blocked1012, -b      (Default: Unblocked) Unblock mastercard files");
                        Console.WriteLine("  --encoding, -e         (Default: EBCDIC) Encoding to use for masking mastercard files (e.g., ebcdic, latin-1)");
                        Console.WriteLine("  --help                 Mask using EBCDIC encoding");
                        Console.WriteLine();
                        Console.WriteLine("Parameters:");
                        Console.WriteLine("  ebcdic                 Use EBCDIC encoding");
                        Console.WriteLine("  latin-1                Use ISO 8859-1 (Latin-1) encoding");
                        Console.WriteLine();
                        Console.WriteLine("Application Control");
                        Console.WriteLine("  Commands:");
                        Console.WriteLine("    exit                 Exit the application.");
                        Console.WriteLine();
                        Console.WriteLine("Example:");
                        Console.WriteLine(" mastercard --blocked1012 --encoding ebcdic");
                        Console.WriteLine("  This will unblock Mastercard files and mask them using EBCDIC encoding.");
                    })
                };

                var visaHelpCommand = new Command("--help", "Show usage instructions for Mastercard command")
                {
                    Handler = CommandHandler.Create(() =>
                    {
                        Console.WriteLine();
                        Console.WriteLine("Usage Instructions:");
                        Console.WriteLine(" [command]");
                        Console.WriteLine();
                        Console.WriteLine("Commands:");
                        Console.WriteLine("  visa                   Mask Visa files");
                        Console.WriteLine();
                        Console.WriteLine("Application Control");
                        Console.WriteLine("  Commands:");
                        Console.WriteLine("    exit                 Exit the application.");
                        Console.WriteLine();
                        Console.WriteLine("Example:");
                        Console.WriteLine(" visa ");
                        Console.WriteLine("  This will mask visa files.");
                    })
                };

                // Add aliases for help command
                helpCommand.AddAlias("-h");
                mastercardHelpCommand.AddAlias("-h");
                visaHelpCommand.AddAlias("-h");

                mastercardCommand.AddCommand(mastercardHelpCommand);
                visaCommand.AddCommand(visaHelpCommand);
                rootCommand.AddCommand(helpCommand);


                if (args.Length == 0)
                {
                    args = new string[] { "--help" };
                }

                while (true)
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine();
                        string input = Console.ReadLine();

                        if (input.Trim().ToLower() == "exit")
                        {
                            return 0;
                        }

                        args = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        if (args.Contains("visa") && args.Length == 1)
                        {
                            args = new string[] { "visa" };
                        }

                        else if (args.Contains("mastercard") && args.Length == 1)
                        {
                            args = new string[] { "mastercard" };
                        }

                        if (args.Length == 0)
                        {
                            continue;
                        }
                    }

                    // Execute the root command with given args
                    var result = await rootCommand.InvokeAsync(args);

                    // Restart args
                    args = Array.Empty<string>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return 1;
            }
        }

        static void EjecutarPasoVI()
        {
            ManageFiles.EnsureDirectoryExists(Defaults.OutputPath);
            ManageFiles.EnsureDirectoryExists(Defaults.LogPath);

            InterpretFilesVisa visaInterpreter = new InterpretFilesVisa();
            visaInterpreter.MaskVisaTransactions();

            Console.WriteLine("Process completed. Exiting program.");
            Task.Delay(2000).Wait();
            Environment.Exit(0);
        }

        static void EjecutarPasoMC(bool blocked1012, string encoding)
        {
            bool onlyMask = true;

            if (blocked1012)
            {
                onlyMask = false;
            }

            if (encoding != null)
            {
                if (encoding.Equals("latin-1", StringComparison.OrdinalIgnoreCase))
                {
                    MaskFiles("ISO-8859-1", onlyMask);
                    
                }
                else if (encoding.Equals("ebcdic", StringComparison.OrdinalIgnoreCase))
                {
                    MaskFiles("EBCDIC-CP-BE", onlyMask);
                }
                else
                {
                    string errorMessage = "Unsupported encoding. Please use 'latin-1' or 'ebcdic'.";
                    Logger.SaveLog(errorMessage);
                    Console.WriteLine(errorMessage);
                    Console.WriteLine("Exiting program.");

                    Task.Delay(2000).Wait();
                    Environment.Exit(0);
                }
            }
            else
            {
                MaskFiles("EBCDIC-CP-BE", onlyMask);
            }
        }

        static void MaskFiles(string encoding, bool onlyMask)
        {
            try
            {
                InterpretFilesMC interpretador = new InterpretFilesMC(encoding);
                ManageFiles.EnsureDirectoryExists(Defaults.OutputPath);
                ManageFiles.EnsureDirectoryExists(Defaults.LogPath);
                if (ManageFiles.EnsureDirectoryExists(Defaults.InputPath) == 1)
                {
                    string errorMessage = "Input directory does not exist. Exiting program.";
                    Logger.SaveLog(errorMessage);
                    Directory.Delete(Defaults.InputPath);
                    Directory.Delete(Defaults.OutputPath);
                    Console.WriteLine(errorMessage);
                    Task.Delay(2000).Wait();
                    Environment.Exit(0);
                }
                else
                {
                    ManageFiles.ManageMCInterpreter(interpretador, onlyMask);
                    Console.WriteLine();
                }
                Console.WriteLine("Process completed. Exiting program.");
                Task.Delay(2000).Wait();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while masking files: {ex.Message}");
            }
        }
    }
}
