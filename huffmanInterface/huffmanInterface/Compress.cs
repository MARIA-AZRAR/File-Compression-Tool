using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace huffmanInterface
{
    class Compress
    {
        public Dictionary<char, string> HuffmanCode = new Dictionary<char, string>();   // to store huffman code   A 100001011
        public Dictionary<char, int> frequencyMap = new Dictionary<char, int>();        //to store frequency     A   4237
        public int Pseudo_EOF = 254;                                                    //special char to be added at the last
        public PQueue.cNode root;                  //root of the huffman tree
        public StreamWriter treeFile;         //file to store tree
        public StreamWriter compressFile;         //file to store CompreesFiles\\
        public string fileExten1;   // file extension stoing var

        public string CompName;   //compressed file name
        public int comleteFlag;



        //counting frequency and adding in map
        public Dictionary<char, int> frequency(FileStream fs)
        {
            if (fileExten1 == ".csv")   //code for csv is different because that code cause problem with csv files
            {
                var sr = new StreamReader(fs);  //opening file
                int c;
                while ((c = sr.Read()) != -1)               //reading character 8 bits from file as int and converting it in char
                {
                    if (frequencyMap.ContainsKey((char)c))
                    {
                        frequencyMap[(char)c] += 1; //if they are in the dictionary just plus there frequency

                    }
                    else
                    {
                        frequencyMap.Add((char)c, 1);     //adding characters in the in dictionary to calculate frequecy if they arent already
                        HuffmanCode.Add((char)c, "");     //initializing the code dictionary       
                    }
                }

                frequencyMap.Add((char)Pseudo_EOF, 1);
                sr.Close();
                fs.Close();
            }
            else
            {
                using (var sr = new StreamReader(fs))
                {
                    while (!sr.EndOfStream)
                    {
                        string s = sr.ReadLine();
                        //Debug.WriteLine(s); //to check string read from file
                        foreach (char c in s)
                        {
                            if (frequencyMap.ContainsKey(c))
                                frequencyMap[c]++;
                            else
                                frequencyMap.Add(c, 1);
                        }
                    }
                }

                try
                {
                    frequencyMap.Add((char)Pseudo_EOF, 1);
                }
                catch
                {
                    frequencyMap.Remove((char)Pseudo_EOF);
                    frequencyMap.Add((char)Pseudo_EOF, 1);
                }

                fs.Close();
            }
            return frequencyMap;
        }

        //printing map
        public void printMap(Dictionary<char, int> Dic)
        {
            foreach (KeyValuePair<char, int> kvp in Dic)
            {
                Console.WriteLine("Key = {0}, Value = {1}",
                 kvp.Key, kvp.Value);
            }
            Console.ReadLine();

        }

        //printing map
        public void printCodes(Dictionary<char, string> Dic)
        {
            foreach (KeyValuePair<char, string> kvp in Dic)
            {
                Console.WriteLine("Value       = {0}, Codes            = {1}",
                 kvp.Key, kvp.Value);
            }
            Console.ReadLine();

        }

        //entering values in map inside nodes and arranging those nodes in Priority queue with smallest frequencies in front

        public PQueue.PriorityQueue nodesinQueue(Dictionary<char, int> Dic)
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

        //calculating codes making tree

        public PQueue.cNode HuffmanEncoding(PQueue.PriorityQueue pQueue)
        {
            int n = pQueue.count;
            while (n != 1)
            {
                PQueue.cNode node = new PQueue.cNode();

                node.leftZero = pQueue.remove();
                node.rightOne = pQueue.remove();
                node.frequency = node.leftZero.frequency + node.rightOne.frequency;
                node.value = '~';                               //just adding a temp value to middle nodes
                pQueue.insertWithPriority(node);
                n = pQueue.count;
            }
            return pQueue.Top();
        }



        //to calculate code from tree
        public void HuffCode(PQueue.cNode root, string str)
        {
            if (root == null)   //base case
                return;

            if (root.leftZero == null && root.rightOne == null)    //if we reached at leaf node 
            {
                try
                {
                    HuffmanCode.Add(root.value, str);     //if it is not already in the Dictionary As i haven't added pseduEOF in Code before it will be added here
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


        //Writng Compress File
        public void WriteCompressFile(FileStream fs)
        {

            // Create a new file
            writeBitByBit bit = new writeBitByBit(CompName);  //creating an instance of bit write

            //writing tree info
            PQueue.cNode top = root;
            printHeaderTree(top, bit);
            string code = "0";

            if (fileExten1 == ".csv")  //code for csv is different because that code cause problem with csv files
            {  
                var sr = new StreamReader(fs);   //opeining input file to be compreesed so to read from it and compare and store
                int c;
                while ((c = sr.Read()) != -1)   //reading char from input file
                {
                    if (frequencyMap.ContainsKey((char)c)) //while calculating frequencies we don't calculate 10 which is LF so we use try to avoid exception when 10 comes as it is not present in the HuffmanCode directory
                    {
                        code = HuffmanCode[(char)c];   //reading character's huffman code from th Code table
                    }

                    for (int i = 0; i < code.Length; ++i)    //reading each character from the Huffman code  like if code for A = 011 the first 0 then 1 and then 1
                    {
                        if (code[i] == '0')
                            bit.BitWrite(0);                //calling writing function with 0 
                        else
                            bit.BitWrite(1);               //calling writing function with 1

                    }
                }
            }

            else   //code for .txt and .cpp .c .cs
            {
                using (var sr1 = new StreamReader(fs))
                {
                    while (!sr1.EndOfStream)
                    {
                        string s = sr1.ReadLine();
                        foreach (char ch in s)
                        {
                            if (frequencyMap.ContainsKey(ch))
                                code = HuffmanCode[ch];   //reading character's huffman code from th Code table

                            for (int i = 0; i < code.Length; ++i)    //reading each character from the Huffman code  like if code for A = 011 the first 0 then 1 and then 1
                            {
                                if (code[i] == '0')
                                    bit.BitWrite(0);                //calling writing function with 0 
                                else
                                    bit.BitWrite(1);               //calling writing function with 1

                            }

                        }
                    }
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
            fs.Close();
        }


        //writing tree
        public void printHeaderTree(PQueue.cNode top, writeBitByBit bit)
        {
            if (top == null)
                return;
            if (top.leftZero == null && top.rightOne == null)
            {
                bit.ByteWrite('1');
                bit.ByteWrite(top.value);
            }
            else
            {
                bit.ByteWrite('0');
                printHeaderTree(top.leftZero, bit);
                printHeaderTree(top.rightOne, bit);
            }
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


        //calculate frequency for pdf
        public Dictionary<char, int> frequencyPDF(string s)
        {
            foreach (char c in s)
            {
                if (frequencyMap.ContainsKey(c))
                    frequencyMap[c]++;
                else
                {
                    frequencyMap.Add(c, 1);     //adding characters in the in dictionary to calculate frequecy if they arent already
                    HuffmanCode.Add(c, "");     //initializing the code dictionary       
                }
            }
        
            frequencyMap.Add((char)Pseudo_EOF, 1);
            return frequencyMap;
            
        }

        //compressing pdf file

        //Writng Compress File
        public void PDFCompressFile(string content)
        {
            //string fileName = "compressed.cmu";

            // Create a new file
            writeBitByBit bit = new writeBitByBit(CompName);  //creating an instance of bit write

            //writing tree info
            PQueue.cNode top = root;
            printHeaderTree(top, bit);

            int c;
            string code = "0";   //initializing
            for (int i = 0; i < content.Length; ++i)   //reading char from string containg input
            {
                Console.WriteLine(content[i]);
                if (HuffmanCode.ContainsKey(content[i]))
                {
                    code = HuffmanCode[content[i]];   //reading character's huffman code from th Code table
                }
                else
                {
                    //if not fount in table do nothing
                }
                for (int j = 0; j < code.Length; ++j)    //reading each character from the Huffman code  like if code for A = 011 the first 0 then 1 and then 1
                {
                    if (code[j] == '0')
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

    }
}
