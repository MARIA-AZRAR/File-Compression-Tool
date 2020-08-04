using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace huffmanInterface
{
    class readBitByBit
    {
        int currentByte;
        int bitMask;
        BufferedStream inputFile;
        int flag = 0;    //flag for tree

        public readBitByBit(string fileName)
        {
            FileStream fs;
            try
            {
                 fs = File.OpenRead(fileName);  //creating an input file stream

            }
            catch
            {
                throw new ArgumentException("A file should be uploaded.");
            }

            inputFile = new BufferedStream(fs);      //opening stream in buffer to make it faster
            currentByte = inputFile.ReadByte();      //reading a byte from the file
            flag = 1;   //as we already read 1st byte
            if (currentByte == -1)
            {
                throw new Exception("File is empty");
            }
            bitMask = 128;     //setting mask to 128 which in infact 10000000  we will shift this one , and(&) it with current and get the require bit
        }

        public char ByteRead()
        {
            char Toreturn;
            if (flag == 1)
            {
                Toreturn = (char)currentByte;
                // Console.WriteLine("flag 1: " + (char)currentByte);
            }
            else
            {

                Toreturn = (char)inputFile.ReadByte();
                //  Console.WriteLine("flag 0: " + (char)currentByte);

            }
            flag = 0;
            return Toreturn;
        }


        public int bitRead()
        {
            int bitReturn = -1;  //initializing
            if ((bitMask & currentByte) == 0)  //we took both & so if our specific bit is 0 then its and with 1 will be 0 and if not well know our bit is 1 
                bitReturn = 0;
            else
                bitReturn = 1;

            bitMask = bitMask >> 1;  //this will move the position of 1 in our 8 bits  e.g 00100000 will become 00010000
            if (bitMask == 0) //means if we have traversed through whole byte
            {
                bitMask = 128;  //again it becomes 10000000 to traverse a byte
                currentByte = inputFile.ReadByte();  //read a byte again
            }

            return bitReturn;
        }

        public void close()
        {
            inputFile.Close();
        }
    }
}
