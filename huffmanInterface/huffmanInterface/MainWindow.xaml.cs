using Squirrel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace huffmanInterface
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
        //Function to view different list items 
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
    
        //Function that is event for close button 
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var form3 = new decompressForm(); //create an instance of form 3
            Hide();             //hide me (currentForm)
            form3.Show();       //show form3
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
            var form3 = new MainWindow(); //create an instance of form 3
            Hide();             //hide me (currentForm)
            form3.Show();       //show form3
            Close();
        }
    }
}
