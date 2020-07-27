using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace huffmanInterface
{
    class writeBitByBit
    {
        byte current;   //current byte
        byte totalbits; //total no of bits in cuurent Byte
        BufferedStream outputFile;
        public writeBitByBit(String fileName)  //constructor
        {
            current = 0;
            totalbits = 0;

            // Check if file already exists. If yes, delete it.     

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            FileStream fs = File.OpenWrite(fileName);   //opening output file
            outputFile = new BufferedStream(fs); //buffered stream improves the file stream
        }

        public void ByteWrite(char c)
        {
            outputFile.WriteByte((byte)c);
        }

        public void BitWrite(int i)
        {
            totalbits++;
            int bit = i << (8 - totalbits);
            current |= (byte)bit;
            if (totalbits == 8)
            {
                outputFile.WriteByte(current);
                current = 0;      //ressetting everything after write
                totalbits = 0;
            }
        }

        public void close()
        {
            outputFile.WriteByte(current);   ///writing all left out bytes in memory
            outputFile.Close();
        }
    }
}
