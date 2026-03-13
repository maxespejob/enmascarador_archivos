using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterchangeFilesMaskingApp
{
    internal class InterpretFilesMC
    {
        public string myEncodingType;

        public InterpretFilesMC(string encoding)
        {
            myEncodingType = encoding;
        }

        public bool ReadAndWriteTransactions(string inputFile, string outputFile)
        {
            using (FileStream reader = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            using (FileStream writer = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                string encodingType = myEncodingType;
                int seekPosition = 0;

                try
                {
                    Logger.SaveLog($"Process started for file: {inputFile}");
                    byte[] buffer = new byte[20];
                    int bytesRead = reader.Read(buffer, 0, buffer.Length);
                    string hexString = BitConverter.ToString(buffer).Replace("-", "");
                    Logger.SaveLog($"Header bytes: {hexString}");

                    while (true)
                    {
                        // Reading transaction length
                        reader.Seek(seekPosition, SeekOrigin.Begin);

                        int trxLength = ReadTransactionLength(reader, writer);

                        if (trxLength == 0)
                        {
                            break;
                        }

                        // Reading the type of message
                        seekPosition += 4;
                        reader.Seek(seekPosition, SeekOrigin.Begin);

                        string mti = ReadMTI(reader, writer, encodingType);

                        // Validation step

                        bool is_mti_valid = Defaults.validMTI.Contains(mti);

                        if (trxLength > 1500 || !is_mti_valid || trxLength < 0)
                        {
                            Logger.SaveLogInfo(reader, seekPosition, trxLength);
                            return false;
                        }

                        // Reading the bit map
                        seekPosition += 4;
                        reader.Seek(seekPosition, SeekOrigin.Begin);

                        string bitMap = "";
                        try
                        {
                            bitMap = ReadBitMap(reader, writer);
                        }
                        catch
                        {
                            Logger.SaveLogInfo(reader, seekPosition, trxLength);
                        }
                        


                        // Reading transaction information
                        seekPosition += 16;

                        int nBytes = trxLength - 20;

                        try
                        {
                            ReadMaskWriteTransactionDetails(reader, writer, bitMap, seekPosition, encodingType);
                        }
                        catch (Exception ex)
                        {
                            Logger.SaveLogInfo(reader, seekPosition, trxLength);
                            Logger.SaveLog($"Error: {ex}");
                            throw new InvalidOperationException("Something went wrong. Please review logs.");
                        }



                        // Going to next transaction
                        seekPosition += nBytes;

                    }

                    return true;
                }
                catch
                {
                    return false;
                }
                
            }
        }

        int ReadTransactionLength(FileStream inputFile, FileStream outputFile)
        {
            // Creation of buffer to store bytes with transaction length information
            byte[] buffer = new byte[4];
            int bytesRead = inputFile.Read(buffer, 0, buffer.Length);
            Array.Reverse(buffer);

            // Conversion from bytes to int (transaction length)
            int trxLength = BitConverter.ToInt32(buffer, 0);

            // Writing transaction length
            Array.Reverse(buffer);
            outputFile.Write(buffer, 0, bytesRead);

            return trxLength;
        }

        string ReadMTI(FileStream inputFile, FileStream outputFile, string encodingType)
        {
            // Creation of buffer to store bytes with MTI information
            byte[] buffer = new byte[4];

            // Conversion from bytes to int (transaction length)
            int bytesRead = inputFile.Read(buffer, 0, buffer.Length);
            string typeOfMessage = ConvertBytesToText(buffer);

            // Writing MTI
            outputFile.Write(buffer, 0, bytesRead);

            return typeOfMessage;
        }

        string ReadBitMap(FileStream inputFile, FileStream outputFile)
        {
            // Create a byte array to hold the data read from the input file
            byte[] buffer = new byte[16];

            // Read up to 16 bytes from the input file into the buffer
            int bytesRead = inputFile.Read(buffer, 0, buffer.Length);

            // Initialize an empty string to store the binary representation of the bytes
            string bitMap = "";

            // Convert each byte in the buffer to its binary representation and append to bitMap
            foreach (byte b in buffer)
                bitMap += Convert.ToString(b, 2).PadLeft(8, '0');

            // Write the bytes read from the input file to the output file
            // Note: This line seems misplaced and might not produce the desired result
            outputFile.Write(buffer, 0, bytesRead);

            // Return the binary representation of the bytes read from the input file
            return bitMap;
        }


        void ReadMaskWriteTransactionDetails(FileStream inputFile, FileStream outputFile, string bitMap, int seekPosition, string encodingType)
        {
            uint dataElementIndex = 2;
            var elementDict = DataElements.ElementDict;

            foreach (char bit in bitMap.Substring(1))
            {
                if (bit == '1')
                {
                    var dataElementKey = "DE" + dataElementIndex;
                    var dataElement = elementDict[dataElementIndex];
                    byte[] buffer;

                    if (dataElement.Type == 0)
                    {
                        buffer = ReadBytes(inputFile, seekPosition, (int)dataElement.Length);
                    }
                    else
                    {
                        byte[] lengthBuffer = ReadBytes(inputFile, seekPosition, (int)dataElement.Length);
                        seekPosition += lengthBuffer.Length;
                        WriteBytes(outputFile, lengthBuffer);
                        int dataElementLength = int.Parse(ConvertBytesToText(lengthBuffer));
                        buffer = ReadBytes(inputFile, seekPosition, dataElementLength);

                        if (dataElementKey == "DE2")
                        {
                            string maskedData = MaskSensitiveData(ConvertBytesToText(buffer));
                            buffer = ConvertTextToBytes(maskedData);
                        }
                    }

                    WriteBytes(outputFile, buffer);
                    seekPosition += buffer.Length;
                }

                dataElementIndex++;
            }

        }

        // Reads a file given certain position and quantity(length) of bytes
        // Returns an array of read bytes
        byte[] ReadBytes(FileStream stream, int position, int length)
        {
            stream.Seek(position, SeekOrigin.Begin);
            byte[] buffer = new byte[length];
            int bytesRead = stream.Read(buffer, 0, length);
            if (bytesRead != length)
                throw new IOException("Error reading from file.");
            return buffer;
        }


        // Writes a buffer of bytes into an output file
        void WriteBytes(FileStream stream, byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }

        // Mask data element 2
        string MaskSensitiveData(string data)
        {
            return data.Substring(0, 9) + new string('*', data.Length - 9);
        }


        string ConvertBytesToText(byte[] buffer)
        {
            // Convert the byte array to text using specified encoding
            string bytesToText = Encoding.GetEncoding(myEncodingType).GetString(buffer);

            return bytesToText;
        }

        byte[] ConvertTextToBytes(string text)
        {
            // Convert the text to a byte array using specified encoding
            byte[] textToBytes = Encoding.GetEncoding(myEncodingType).GetBytes(text);

            return textToBytes;
        }

        /*
        static string ConvertBufferToHex(byte[] buffer)
        {
            StringBuilder hex = new StringBuilder(buffer.Length * 2);
            foreach (byte b in buffer)
            {
                hex.AppendFormat("{0:X2} ", b);
            }
            return hex.ToString().Trim();
        }

        */
    }
}
