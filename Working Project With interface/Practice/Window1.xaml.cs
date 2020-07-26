using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace Practice
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        string fileName = null;  //Global  //file to be decompressed
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
                fileName = openFile.FileName;           //compressed file
                fileExt = System.IO.Path.GetExtension(openFile.FileName);
                MessageBox.Show(System.IO.Path.GetExtension(openFile.FileName));
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.FileName = "Codes"; ;
            saveFile.DefaultExt = ".txt";
            // saveFile.Filter = "Text documents (.txt)|*.txt";

            bool? response = saveFile.ShowDialog();
            if (response == true)
            {
                decomp.decmpFile = saveFile.FileName;   //file name after decompress
                //Decompressing file
                if (fileExt == ".pdf")
                {

                    //decompress
                     decomp.pdfDecompress(fileName);  //given compressed file to decompress
                }
                else
                {
                       FileStream fs2 = File.Create(decomp.decmpFile);
                       decomp.decompress(fileName, fs2);
                }

                MessageBox.Show("Decompression done");
            }
            //MessageBox.Show(myObj.CompName);
        }

        private void Decompress_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Switch_Click_1(object sender, RoutedEventArgs e)
        {
            var newForm = new MainWindow(); //create your new form.
            newForm.Show(); //show the new form.
            this.Close(); //only if you want to close the current form.
        }
    }
}
