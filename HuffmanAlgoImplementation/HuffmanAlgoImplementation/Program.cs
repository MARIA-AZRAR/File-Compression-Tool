using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace HuffmanAlgoImplementation
{
    class Program
    {
        Dictionary<char, string> HuffmanCode = new Dictionary<char, string>();   // to store huffman code   A 100001011
        Dictionary<char, int> frequencyMap = new Dictionary<char, int>();        //to store frequency     A   4237
        public int Pseudo_EOF = 254;                                                    //special char to be added at the last
        PQueue.cNode root;                  //root of the huffman tree
        StreamWriter treeFile;         //file to store tree
        StreamWriter compressFile;         //file to store CompreesFiles

        //counting frequency and adding in map
        Dictionary<char, int> frequency(string fileName)
        {
            FileStream fs = File.OpenRead(fileName);  //opening file

            var sr = new StreamReader(fs);  //opening file
            int c;
            while ((c = sr.Read()) != -1)               //reading character 8 bits from file as int and converting it in char
            {
                try
                {
                    frequencyMap.Add((char)c, 1);     //adding characters in the in dictionary to calculate frequecy if they arent already
                    HuffmanCode.Add((char)c, "");     //initializing the code dictionary       
                }
                catch
                {
                    frequencyMap[(char)c] += 1; //if they are in the dictionary just plus there frequency
                }

            }
            frequencyMap.Add((char)Pseudo_EOF, 1);
            sr.Close();
            fs.Close();
            return frequencyMap;
        }

        //printing map
        void printMap(Dictionary<char, int> Dic)
        {
            foreach (KeyValuePair<char, int> kvp in Dic)
            {
                Console.WriteLine("Key = {0}, Value = {1}",
                 kvp.Key, kvp.Value);
                //  Console.WriteLine("{0} , {1} ", kvp.Key, kvp.Value);
            }
            Console.ReadLine();

        }


        //entering values in map inside nodes and arranging those nodes in Priority queue with smallest frequencies in front

        PQueue.PriorityQueue nodesinQueue(Dictionary<char, int> Dic)
        {
            PQueue.cNode node = new PQueue.cNode(); //node created     
            PQueue.PriorityQueue pQueue = new PQueue.PriorityQueue();  //queue created
            foreach (KeyValuePair<char, int> kvp in Dic)   //reading from frequency dictionary
            {
                node.value = kvp.Key;
                node.frequency = kvp.Value;    //setting the values of node
                pQueue.insertWithPriority(node);   //entering the node
                node = new PQueue.cNode();
            }
            return pQueue;
        }

        //building the tree
        PQueue.cNode HuffmanEncoding(PQueue.PriorityQueue pQueue)
        {
            int n = pQueue.count;
            while (n != 1)
            {
                PQueue.cNode node = new PQueue.cNode();

                node.leftZero = pQueue.remove();
                node.rightOne = pQueue.remove();
                node.frequency = node.leftZero.frequency + node.rightOne.frequency;
                node.value = 'a';
                pQueue.insertWithPriority(node);
                pQueue.print();
                Console.WriteLine("Inserted");
                n = pQueue.count;
            }
            return pQueue.Top();
        }
        
        
        
        void HuffCode(PQueue.cNode root, string str)
        {
            if (root == null)   //base case
                return;

            if (root.leftZero == null && root.rightOne == null)    //if we reached at leaf node 
            {
                try
                {
                    HuffmanCode.Add(root.value, str);     //if it is not already in the Dictionary
                }
                catch
                {
                    HuffmanCode[root.value] = str;   //if it is in the dictionary just edit 2nd value
                }
            }
            else
            {
                HuffCode(root.leftZero, str + "0");   //concatination to form the code
                HuffCode(root.rightOne, str + "1");
            }
        }



        //printing map
        void printCodes(Dictionary<char, string> Dic)
        {
            foreach (KeyValuePair<char, string> kvp in Dic)
            {
                Console.WriteLine("Value       = {0}, Codes            = {1}",
                 kvp.Key, kvp.Value);
                //  Console.WriteLine("{0} , {1} ", kvp.Key, kvp.Value);
            }
            Console.ReadLine();

        }

        //Writng decoding tree
        void EncodingTreeWrite(PQueue.cNode r)
        {
            string fileName = "tree.cmu";

            try
            {
                // Check if file already exists. If yes, delete it.     

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                // Create a new file     
                treeFile = new StreamWriter(fileName, true, Encoding.ASCII);
                Write_tree(r);
                treeFile.Write(254);
                treeFile.Close();

            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }

        }
        //writing tree at the top of decoding file
        void Write_tree(PQueue.cNode r)
        {
            if (r.leftZero == null && r.rightOne == null)
            {
                treeFile.Write("0");
                treeFile.Write(r.value);
            }
            else
            {
                treeFile.Write("1");
                Write_tree(r.leftZero);
                Write_tree(r.rightOne);
            }
        }


        //Writng Compress File
        void WriteCompressFile(FileStream fs)
        {
            string fileName = "compress.cmu";

            // Create a new file
            writeBitByBit bit = new writeBitByBit(fileName);  //creating an instance of bit write
            var sr = new StreamReader(fs);   //opeining input file to be compreesed so to read from it and compare and store
            int c;
            string code = "0";
            while ((c = sr.Read()) != -1)   //reading char from input file
            {
                //Console.WriteLine("char: " + (char)c  + "int: " + c);
                try   //while calculating frequencies we don't calculate 10 which is LF so we use try to avoid exception when 10 comes as it is not present in the HuffmanCode directory
                {
                    code = HuffmanCode[(char)c];   //reading character's huffman code from th Code table
                }
                catch
                {

                }
                for (int i = 0; i < code.Length; ++i)    //reading each character from the Huffman code  like if code for A = 011 the first 0 then 1 and then 1
                {
                    if (code[i] == '0')
                        bit.BitWrite(0);                //calling writing function with 0 
                    else
                        bit.BitWrite(1);               //calling writing function with 1

                }
            }
            //now we need to write psedu_EOF so that we don't reed extra bytes from the file
            code = HuffmanCode[(char)Pseudo_EOF];
            for (int i = 0; i < code.Length; ++i)    //reading each character from the Huffman code  like if code for A = 011 the first 0 then 1 and then 1
            {
                if (code[i] == '0')
                    bit.BitWrite(0);                //calling writing function with 0 
                else
                    bit.BitWrite(1);               //calling writing function with 1

            }
            bit.close();
        }


        public void printPreorder(PQueue.cNode node)
        {
            if (node == null)
                return;

            /* first print data of node */
            if (node.leftZero == null && node.rightOne == null)
            {
                Console.Write(node.value + " ");

            }

            /* then recur on left sutree */
            printPreorder(node.leftZero);

            /* now recur on right subtree */
            printPreorder(node.rightOne);
        }



        static void Main(string[] args)
        {
             Program myComp = new Program(); //creating instance of the main file
                                             //string S;       //Enter string to find the huffman code
                                             //S = Console.ReadLine();
                                             //  Console.WriteLine("String Entered: " + S);

            Console.WriteLine("Enter File Name: ");
            var fileName = Console.ReadLine();
            Console.WriteLine(fileName);


            Dictionary<char, int> Dic = new Dictionary<char, int>();
            Dic = myComp.frequency(fileName);  //frequency calculated
            myComp.printMap(Dic);

            //entering data in nodes then storing them in queue
            PQueue.PriorityQueue pQueue = new PQueue.PriorityQueue();
            pQueue = myComp.nodesinQueue(Dic);


            //creating encooding tree
            myComp.root = myComp.HuffmanEncoding(pQueue);
            
            
            PQueue.cNode top = myComp.root; //temporary storing the value

            Console.WriteLine(top.getValue());
            myComp.HuffCode(top, "");
            myComp.printCodes(myComp.HuffmanCode);

            //writing tree in file
            // top = myComp.root; //temporary storing the value
            //myComp.EncodingTreeWrite(top);

            //compressing file
            FileStream fs = File.OpenRead(fileName);
            myComp.WriteCompressFile(fs);


            Console.ReadKey();
        }
    }
}
