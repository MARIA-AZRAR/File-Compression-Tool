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
        PQueue.cNode root;
        int Pseudo_EOF = 254; //root of the huffman tree

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


        //decompress the file

        void decompress(string fileName, FileStream fs2)
        {
            readBitByBit bit = new readBitByBit(fileName);
            var output = new StreamWriter(fs2);
            int returnbit = -1;
            char leaf = '1'; //checking if we reached the end of file
            PQueue.cNode top = root;
            while (true)  //will run until we found the pseduo_EOF
            {
                if (top.leftZero == null && top.rightOne == null)  //if leaf node is reached
                {
                    leaf = top.value;
                    if (leaf == (char)Pseudo_EOF)   //if it is last letter close the file
                    {
                        output.Close();
                        break;
                    }
                    else
                    {
                        output.Write(leaf);  //else write in file
                        top = root;   //again start from root
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

            bit.close();

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

       
            Console.ReadKey();
        }
    }
}
