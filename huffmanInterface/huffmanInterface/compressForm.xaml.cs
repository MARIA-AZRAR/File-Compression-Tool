using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.text;
using Microsoft.Win32;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace huffmanInterface
{
    /// <summary>
    /// Interaction logic for compressForm.xaml
    /// </summary>
    public partial class compressForm : Window
    {
        public compressForm()
        {
            InitializeComponent();
        }

        //global variables
        string fileName = null;  //Global
        string fileExt;
        Compress myObj = new Compress();
        Decompress decomp = new Decompress();
        string content;

        private void ListViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            //Set tooltip visibility
            if (Tb_Btn.IsChecked == true)
            {
                tt_home.Visibility = Visibility.Collapsed;
                tt_compress.Visibility = Visibility.Collapsed;
                tt_decompress.Visibility = Visibility.Collapsed;
            }
            else
            {
                tt_home.Visibility = Visibility.Visible;
                tt_compress.Visibility = Visibility.Visible;
                tt_decompress.Visibility = Visibility.Visible;
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
      

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var form2 = new MainWindow(); //create an instance of form 3
            Hide();             //hide me currentForfm
            form2.Show();       //show form2
            Close();
        }

        private void Image_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            var form2 = new compressForm(); //create an instance of form 3
            Hide();             //hide me (currentForm)
            form2.Show();       //show form2
            Close();
        }

        private void Image_MouseUp_2(object sender, MouseButtonEventArgs e)
        {
            var form2 = new decompressForm(); //create an instance of form 3
            Hide();             //hide me (currentForm)
            form2.Show();       //show form2
            Close();
        }

        private void Upload_Click(object sender, RoutedEventArgs e)
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

        private void Compression_Click(object sender, RoutedEventArgs e)
        {
            string filename = fileName;
            //myObj.comleteFlag = 0;

          
            if (fileExt == ".pdf" || fileExt == ".docx" || fileExt == ".doc")
            {
                if (fileExt == ".docx" || fileExt == ".doc")
                {


                    // Open a WordprocessingDocument based on a filepath.
                    using (WordprocessingDocument wordDocument =
                        WordprocessingDocument.Open(fileName, false))
                    {
                        // Assign a reference to the existing document body.  
                        Body body = wordDocument.MainDocumentPart.Document.Body;

                        //Saving the text of Docx file in a strin content
                        content =  body.InnerText.ToString();
                    }
                }
                if(fileExt == ".pdf")
                {
                    StringBuilder fileContent = new StringBuilder();
                    using (PdfReader pdfReader = new PdfReader(filename))
                    {
                        for (int k = 1; k <= pdfReader.NumberOfPages; k++)
                        {
                            fileContent.Append(PdfTextExtractor.GetTextFromPage(pdfReader, k));
                        }
                    }
                    content = fileContent.ToString();
                }
                //string content;

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
            else
            {

                FileStream fs = File.OpenRead(fileName);
                //String fs = File.ReadAllText(fileName);

                Dictionary<char, int> Dic = new Dictionary<char, int>();
                //Dic = myObj.frequency(fs);
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

            // myObj.comleteFlag = 1;
            MessageBox.Show("Compress Done Save File.");

        }

        private void SaveCompressFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.FileName = "Codes"; ;
            saveFile.DefaultExt = fileExt;
            // saveFile.Filter = "Text documents (.txt)|*.txt";

            bool? response = saveFile.ShowDialog();
            if (response == true)
            {
                myObj.CompName = saveFile.FileName;
                //FileStream fs1 = File.Create(myObj.CompName);

                //compressing file
                if (fileExt == ".pdf" || fileExt == ".docx" || fileExt == ".doc")
                {
                    //compress file
                    myObj.PDFCompressFile(content);
                }
                else
                {
                    FileStream fs;
                    try
                    {
                        fs = File.OpenRead(fileName);
                        myObj.WriteCompressFile(fs);
                    }
                    catch
                    {
                        MessageBox.Show("You have not uploaded any file.");
                        throw new ArgumentException("A file should be uploaded.");
                    }
                    
                }

                MessageBox.Show(myObj.CompName);
                MessageBox.Show("File Saved");
            }
        }
    }
}

