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
using Microsoft.Office.Interop.Word;
using Application = Microsoft.Office.Interop.Word.Application;

namespace huffmanInterface
{
    /// <summary>
    /// Interaction logic for compressForm.xaml
    /// </summary>
    public partial class compressForm : System.Windows.Window
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
                myObj.fileExten1 = fileExt;   //seeting up exttension name for compress.c
                MessageBox.Show(System.IO.Path.GetExtension(openFile.FileName));
            }
        }

        private void Compression_Click(object sender, RoutedEventArgs e)
        {
            string filename = fileName;
            //myObj.comleteFlag = 0;

          
            if (fileExt == ".pdf" || fileExt == ".docx")
            {
                if (fileExt == ".docx")
                {


                    try
                    {
                        // Open a doc file.
                        Application app = new Application();
                        Microsoft.Office.Interop.Word.Document document = app.Documents.Open(fileName);

                        // Select all words from word file
                        int total = document.Words.Count;
                        for (int i = 1; i <= total; i++)
                        {
                            // Reading from word document into a string
                            content += document.Words[i].Text;
                        }
                        // Close word.
                        Console.WriteLine(content);
                        app.Quit();
                    }
                    catch
                    {
                        MessageBox.Show("File is used by another Process");
                        throw new ArgumentException("File is used by another Process.");
                    }

                }
                if(fileExt == ".pdf")
                {
                    try
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
                    catch
                    {
                        MessageBox.Show("File is used by another Process");
                        throw new ArgumentException("File is used by another Process.");

                    }

                }
                //string content;

                Dictionary<char, int> Dic = new Dictionary<char, int>();
                Dic = myObj.frequencyPDF(content);
              //  myObj.printMap(Dic);

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
               // myObj.printCodes(myObj.HuffmanCode);

            }
            else
            {
                FileStream fs;
                try
                {
                    fs = File.OpenRead(fileName);
                }
                catch
                {
                    MessageBox.Show("No File has been Uploaded");
                    throw new ArgumentException("A file should be uploaded.");
                }
                //String fs = File.ReadAllText(fileName);

                Dictionary<char, int> Dic = new Dictionary<char, int>();
                //Dic = myObj.frequency(fs);
                Dic = myObj.frequency(fs);
               // myObj.printMap(Dic);

                //getting frequency
                //entering data in nodes then storing them in queue
                PQueue.PriorityQueue pQueue = new PQueue.PriorityQueue();
                pQueue = myObj.nodesinQueue(Dic);

               // pQueue.print();

                //creating encooding tree
                myObj.root = myObj.HuffmanEncoding(pQueue);

                PQueue.cNode top = myObj.root; //temporary storing the value

                // Console.WriteLine(top.getValue());
                myObj.HuffCode(top, "");
               // myObj.printCodes(myObj.HuffmanCode);

                top = myObj.root; //temporary storing the value

            }

            MessageBox.Show("Compress Done Save File.");

        }

        private void SaveCompressFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.FileName = "Compressed"; ;
            saveFile.DefaultExt = fileExt;
            bool? response = saveFile.ShowDialog();
            if (response == true)
            {
                myObj.CompName = saveFile.FileName;

                //compressing file
                if (fileExt == ".pdf" || fileExt == ".docx")
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

