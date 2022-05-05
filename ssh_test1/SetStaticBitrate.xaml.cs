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

namespace ssh_test1
{
    /// <summary>
    /// Interaction logic for SetStaticBitrate.xaml
    /// </summary>
    public partial class SetStaticBitrate : UserControl
    {
        OptionsBase options;
        public SetStaticBitrate(OptionsBase opt)
        {
            this.options = opt;
            InitializeComponent();
            populateListData();
        }
        private void populateListData() 
        {
            try 
            {
                var lines = File.ReadAllLines(@"Config\Bitrate.txt");
                foreach (string line in lines)
                {
                    string[] parts = line.Split(';');
                    lw.Items.Add(new BitrateInfo { name = parts[0], upbitr = Convert.ToDouble(parts[1]), downbitr = Convert.ToDouble(parts[2]) });
                }
            }
            catch 
            {
                MessageBox.Show("File you are trying to read located at: " + System.IO.Path.GetFullPath(@"Config\Bitrate.txt").ToString() + " does not have the right format");
            }
        }

        private void plusBut_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                double upbr = Convert.ToDouble(upbitrText.Text);
                double downbr = Convert.ToDouble(downbitrText.Text);
                File.AppendAllText(@"Config\Bitrate.txt", $"{annexText.Text};{upbitrText.Text};{downbitrText.Text}\n");
                lw.Items.Clear();
                populateListData();
            }
            catch 
            {
                MessageBox.Show("Please provide bitrate input with numbers only.");
            }

        }

        private void minusBut_Click(object sender, RoutedEventArgs e)
        {

            var lines = File.ReadAllLines(@"Config\Bitrate.txt").Where(line => line.Trim() != $"{((BitrateInfo)lw.SelectedItem).name};{((BitrateInfo)lw.SelectedItem).upbitr};{((BitrateInfo)lw.SelectedItem).downbitr}");
            File.WriteAllLines(@"Config\Bitrate.txt", lines);
            lw.Items.Clear();
            populateListData();
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            options.Close();
        }
    }
}
