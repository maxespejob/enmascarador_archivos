using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace InterchangeFilesMaskingApp
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            return await CommandLineInterface.Run(args);         
        }
    }

    public class Defaults
    {
        public static readonly string InputPathVisa = Path.Combine(".", "Input", "Visa");
        public static readonly string InputPathMC = Path.Combine(".", "Input", "Mastercard");
        public static readonly string OutputPathVisa = Path.Combine(".", "Output", "Visa");
        public static readonly string OutputPathMC = Path.Combine(".", "Output", "Mastercard");
        public static readonly string LogPath = Path.Combine(".", "Logs");

        public static readonly HashSet<string> validMTI = new HashSet<string> { "1240", "1644", "1740", "1442" };
    }

    public static class Logger
    {
        public static void SaveLog(string logMessage)
        {
            string logFilePath = Path.Combine(Defaults.LogPath, "logs.txt");

            int count = 0;
            while (count < 5)
            {
                try
                {
                    using (StreamWriter Log = new StreamWriter(logFilePath, true)) // true is for appending
                    {
                        string strTmpOut = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] : {logMessage}";
                        Log.WriteLine(strTmpOut);
                    }
                    count = 5;
                }
                catch
                {
                    Thread.Sleep(250 + count * 100);
                    count += 1;
                }
            }
        }

        public static void SaveLogInfo(FileStream reader, int seekPosition, int trxLength)
        {
            int tempPosition;

            if (seekPosition >= 1500)
            {
                tempPosition = seekPosition - 1500;
            }
            else
            {
                tempPosition = 0;
            }

            reader.Seek(tempPosition, SeekOrigin.Begin);

            byte[] messageWithErrors = new byte[3000];
            reader.Read(messageWithErrors, 0, messageWithErrors.Length);
            string messageWithErrorsString = BitConverter.ToString(messageWithErrors).Replace("-", "");

            Logger.SaveLog($"Length: {trxLength}");
            Logger.SaveLog($"Message with errors: {messageWithErrorsString}");

            reader.Seek(seekPosition, SeekOrigin.Begin);
        }
    }
}
