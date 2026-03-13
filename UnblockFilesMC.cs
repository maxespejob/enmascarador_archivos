using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterchangeFilesMaskingApp
{
    internal class UnblockFilesMC
    {
        public static void ExtractTransactionsToFile(string inputFile, string outputFile)
        {
            using (FileStream fEntrada = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            using (FileStream fSalida = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                int seekPositionTemp = 0;
                while (true)
                {
                    fEntrada.Seek(seekPositionTemp, SeekOrigin.Begin);
                    byte[] bytesWithInfo = new byte[1012];
                    int bytesRead = fEntrada.Read(bytesWithInfo, 0, 1012);
                    if (bytesRead == 0)
                        break;
                    fSalida.Write(bytesWithInfo, 0, bytesRead);
                    seekPositionTemp += 1014;
                }
            }
        }

        // AGREGAR REBLOCK POST ENMASCARAMIENTO
    }
}
