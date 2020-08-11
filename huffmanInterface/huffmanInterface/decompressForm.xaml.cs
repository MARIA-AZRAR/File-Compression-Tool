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

namespace huffmanInterface
{
    /// <summary>
    /// Interaction logic for decompressForm.xaml
    /// </summary>
    public partial class decompressForm : Window
    {
        public decompressForm()
        {
            InitializeComponent();
        }

        //global

        string fileName = null;  //Global  //file to be decompressed
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
            Hide();             //hide me (currentForm)
            form2.Show();       //show form3
            Close();
        }

        private void Image_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            var form2 = new compressForm(); //create an instance of form 3
            Hide();             //hide me (currentForm)
            form2.Show();       //show form3
            Close();
        }

        private void Image_MouseUp_2(object sender, MouseButtonEventArgs e)
        {
            var form2 = new decompressForm(); //create an instance of form 3
            Hide();             //hide me (currentForm)
            form2.Show();       //show form3
            Close();
        }

        private void SaveDecompressFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.FileName = "Decompressed"; ;
            saveFile.DefaultExt = fileExt;

            bool? response = saveFile.ShowDialog();
            if (response == true)
            {
                decomp.decmpFile = saveFile.FileName;   //file name after decompress
                //Decompressing file
                if (fileExt == ".pdf" || fileExt == ".docx")
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

        private void DeUpload_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFile = new OpenFileDialog();
            bool? response = openFile.ShowDialog();  //this return a nulllable
            if (response == true)
            {
                fileName = openFile.FileName;           //compressed file
                fileExt = System.IO.Path.GetExtension(openFile.FileName);
                decomp.fileExten = fileExt;
                MessageBox.Show(System.IO.Path.GetExtension(openFile.FileName));
            }
        }
    }
}

