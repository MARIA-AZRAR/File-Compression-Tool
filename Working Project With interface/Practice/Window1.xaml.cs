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

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            bool? response = openFile.ShowDialog();  //this return a nulllable
            if (response == true)
            {
                string filePath = openFile.FileName;
                MessageBox.Show(filePath);

            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.FileName = "Codes"; ;
            saveFile.DefaultExt = ".cmp";
            // saveFile.Filter = "Text documents (.txt)|*.txt";

            bool? response = saveFile.ShowDialog();
            if (response == true)
            {
                string filename = saveFile.FileName;
                FileStream fs = File.Create(filename);
                MessageBox.Show(filename);
            }
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
