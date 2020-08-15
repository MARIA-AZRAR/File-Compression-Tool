using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;
using Xceed.Words.NET;

namespace huffmanInterface
{
    class Decompress
    {
        PQueue.cNode FileTreeroot;                  //root of the huffman tree
        Compress program = new Compress();
        public string decmpFile;
        public string fileExten;   // file extension stoing var


        //decompress the file

        public void decompress(string fileName, FileStream fs2)
        {
            readBitByBit bit = new readBitByBit(fileName);
            FileTreeroot = ReadTreeHeader(bit);
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

        public void pdfDecompress(string fileName)
        {
            readBitByBit bit = new readBitByBit(fileName);
            FileTreeroot = ReadTreeHeader(bit);
            program.printPreorder(FileTreeroot);


            int returnbit = -1;  //initializing
            char leaf = '1'; //checking if we reached the end of file
            string content = null;  //initializing
            PQueue.cNode top = FileTreeroot;

            while (true)  //will run until we found the pseduo_EOF
            {

                if (top.leftZero == null && top.rightOne == null)  //if leaf node is reached
                {
                    leaf = top.value;
                    if (leaf == (char)program.Pseudo_EOF)   //if it is last letter close the file
                    {
                        break;
                    }
                    else
                    {
                        content = content + leaf.ToString();
                    }
                    top = FileTreeroot;   //again start from root
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

            string newC = null;
            try
            {
                newC = content.Substring(3);   // in case of empty file
            }
            catch
            {

            }

            //create pdf
            if (fileExten == ".pdf"){
            Document document = new Document(PageSize.A4, 50, 35, 50, 35);
            PdfWriter pdfWriter = PdfWriter.GetInstance(document, new FileStream(decmpFile, FileMode.Create));
            document.Open();
            Paragraph p = new Paragraph();
            p.Font = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12f, BaseColor.BLACK);
            p.Add(newC);
            document.Add(p);
            document.Close();
            }
   

            if(fileExten == ".docx"){
                var doc = DocX.Create(decmpFile);
                doc.InsertParagraph(newC).Font("Times New Roman").FontSize(12d);  //inserting paragraph
                doc.Save();
            }

        }



        public PQueue.cNode ReadTreeHeader(readBitByBit bit)
        {
            char c = bit.ByteRead();
            PQueue.cNode node = new PQueue.cNode();

            if (c == '1')
            {
                node = new PQueue.cNode();
                node.value = bit.ByteRead();
                return node;
            }

            else
            {
                PQueue.cNode leftChild = ReadTreeHeader(bit);
                PQueue.cNode rightChild = ReadTreeHeader(bit);
                node = new PQueue.cNode('0', leftChild, rightChild);
                return node;
            }
        }
    }
}
