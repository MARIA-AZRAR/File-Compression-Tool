using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace HuffmanAlgoImplementation
{
    class Decompress
    {

        PQueue.cNode FileTreeroot;                  //root of the huffman tree
        Program program = new Program();


        //decompress the file

        public void decompress(string fileName, FileStream fs2)
        {
            readBitByBit bit = new readBitByBit(fileName);
            FileTreeroot = ReadTreeHeader(bit);
            Console.WriteLine("insode decompress");
            program.printPreorder(FileTreeroot);
            var output = new StreamWriter(fs2);
            int returnbit = -1;
            char leaf = '1'; //checking if we reached the end of file

            // PQueue.cNode top = root;
            PQueue.cNode top = FileTreeroot;

            while (true)  //will run until we found the pseduo_EOF
            {
                if (top.leftZero == null && top.rightOne == null)  //if leaf node is reached
                {
                    leaf = top.value;
                    if (leaf == (char)program.Pseudo_EOF)   //if it is last letter close the file
                    {
                        output.Close();
                        break;
                    }
                    else
                    {
                        output.Write(leaf);  //else write in file
                        top = FileTreeroot;   //again start from root
                    }
                }
                returnbit = bit.bitRead();
                if (returnbit == 0)  //if not leaf keep on reading the file
                {
                    top = top.leftZero;
                }
                else if (returnbit == 1)
                {
                    top = top.rightOne;
                }
            }
            output.Close();
            bit.close();

        }


        //decompress the file

    



        public PQueue.cNode ReadTreeHeader(readBitByBit bit)
        {
            char c = bit.();
            PQueue.cNode node = new PQueue.cNode();

            if (c == '1')
            {
                Console.WriteLine("inside 1 : ");
                node = new PQueue.cNode();
                node.value = bit.ByteRead();
                Console.WriteLine("inside 1 value: " + node.value);
                return node;
            }

            else
            {
                Console.WriteLine("inside 0");
                PQueue.cNode leftChild = ReadTreeHeader(bit);
                PQueue.cNode rightChild = ReadTreeHeader(bit);
                node = new PQueue.cNode('0', leftChild, rightChild);
                return node;
            }
        }
    }
}

    }
}
