using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.text;
using Microsoft.Win32;

namespace Practice
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string fileName = null;  //Global
        string fileExt;
        Compress myObj = new Compress();
        Decompress decomp = new Decompress();
        string content;


        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            bool? response = openFile.ShowDialog();  //this return a nulllable
            if (response == true)
            {
                fileName = openFile.FileName;
                fileExt = System.IO.Path.GetExtension(openFile.FileName);
                MessageBox.Show(System.IO.Path.GetExtension(openFile.FileName));
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
             saveFile.FileName = "Codes"; ;
             saveFile.DefaultExt = ".cmp";
            // saveFile.Filter = "Text documents (.txt)|*.txt";

            bool? response = saveFile.ShowDialog();
            if(response == true)
            {
                myObj.CompName = saveFile.FileName;
                //FileStream fs1 = File.Create(myObj.CompName);

                //compressing file
                if(fileExt == ".pdf")
                {
                    //compress file
                    myObj.PDFCompressFile(content);
                }
                else
                {
                    FileStream fs = File.OpenRead(fileName);
                    myObj.WriteCompressFile(fs);
                }

                MessageBox.Show(myObj.CompName);
            }
            
        }

        private void Compress_Click(object sender, RoutedEventArgs e)
        {
            string filename = fileName;
            //myObj.comleteFlag = 0;

            if (fileExt == ".txt" || fileExt == ".csv" || fileExt == ".cpp")
            {

                FileStream fs = File.OpenRead(fileName);

                Dictionary<char, int> Dic = new Dictionary<char, int>();
                // Dic = myObj.frequency(fs);
                Dic = myObj.frequency(fs);
                 myObj.printMap(Dic);

                //getting frequency
                //entering data in nodes then storing them in queue
                PQueue.PriorityQueue pQueue = new PQueue.PriorityQueue();
                pQueue = myObj.nodesinQueue(Dic);

                pQueue.print();

                //creating encooding tree
                myObj.root = myObj.HuffmanEncoding(pQueue);

                PQueue.cNode top = myObj.root; //temporary storing the value

               // Console.WriteLine(top.getValue());
                myObj.HuffCode(top, "");
                myObj.printCodes(myObj.HuffmanCode);


                //writing tree in file
                //top = myObj.root; //temporary storing the value
                //myObj.EncodingTreeWrite(top);
                top = myObj.root; //temporary storing the value
              //  myObj.printPreorder(top);
                //Console.WriteLine("Tree Printed");

                //fileCompress in button save
            }
          
            else if (fileExt == ".pdf")
            {

                //string content;

                StringBuilder fileContent = new StringBuilder();
                using (PdfReader pdfReader = new PdfReader(filename))
                {
                    for (int k = 1; k <= pdfReader.NumberOfPages; k++)
                    {
                        fileContent.Append(PdfTextExtractor.GetTextFromPage(pdfReader, k));
                    }
                }
                content = fileContent.ToString();


                Dictionary<char, int> Dic = new Dictionary<char, int>();
                Dic = myObj.frequencyPDF(content);
                myObj.printMap(Dic);

                //getting frequency


                //entering data in nodes then storing them in queue
                PQueue.PriorityQueue pQueue = new PQueue.PriorityQueue();
                pQueue = myObj.nodesinQueue(Dic);

              //  pQueue.print();

                //creating encooding tree
                myObj.root = myObj.HuffmanEncoding(pQueue);

                PQueue.cNode top = myObj.root; //temporary storing the value

               // Console.WriteLine(top.getValue());
                myObj.HuffCode(top, "");
               myObj.printCodes(myObj.HuffmanCode);

            }

           // myObj.comleteFlag = 1;
            MessageBox.Show("Compress Done Save File.");

        }


        private void Switch_Click_1(object sender, RoutedEventArgs e)
        {
            var newForm = new Window1(); //create your new form.
            newForm.Show(); //show the new form.
            this.Close(); //only if you want to close the current form.
        }
    }
}

/*
 
                FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                string str = " pyaar k charchy har zubaan par";
                sw.WriteLine(str);
                sw.Flush();
                sw.Close();
                fs.Close();




     */
