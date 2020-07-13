using System;

namespace HuffmanAlgoImplementation
{
    class Program
    {
        static void Main(string[] args)
        {
             Program myComp = new Program(); //creating instance of the main file
             string S;       //Enter string to find the huffman code
             S = Console.ReadLine();
             Console.WriteLine("String Entered: " + S);

             Console.ReadKey();
        }
    }
}
