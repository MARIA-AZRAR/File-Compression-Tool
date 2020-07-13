using System;
using System.Collections.Generic;

namespace HuffmanAlgoImplementation
{
    class Program
    {
        Dictionary<char, int> frequencyMap = new Dictionary<char, int>();        //to store frequency     A   4237

        //counting frequency and adding in map
        Dictionary<char, int> frequency(String S)
        {

            foreach (char c in S)
            {
                try
                {
                    frequencyMap.Add(c, 1);     //adding characters in the in dictionary to calculate frequecy if they arent already
                }
                catch
                {
                    frequencyMap[c] += 1; //if they are in the dictionary just plus there frequency
                }
            }
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

        static void Main(string[] args)
        {
             Program myComp = new Program(); //creating instance of the main file
             string S;       //Enter string to find the huffman code
             S = Console.ReadLine();
             Console.WriteLine("String Entered: " + S);

            Dictionary<char, int> Dic = new Dictionary<char, int>();
            Dic = myComp.frequency(S);  //frequency calculated
            myComp.printMap(Dic);



            Console.ReadKey();
        }
    }
}
