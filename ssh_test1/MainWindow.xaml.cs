﻿using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ssh_test1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ipv4.Foreground = new SolidColorBrush(Colors.Gray);
            Name.Foreground = new SolidColorBrush(Colors.Gray);
            password.Foreground = new SolidColorBrush(Colors.Gray);
        }

        public void ErrorMessage(string errorText)
        {
            MessageBox.Show(errorText);
        }

        private void ipv4_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Name_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {

            Connect connect = new Connect(ipv4.Text.ToString(), Name.Text.ToString(), password.Text.ToString());
            if (connect.con() == true)
            {
                ConData.ipv4 = ipv4.Text.ToString();
                ConData.name = Name.Text.ToString();
                ConData.password = password.Text.ToString();
                Window1 wind = new Window1();
                wind.Show();
                this.Close();
            }
        }

        private void pass_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ipv4_GotFocus(object sender, RoutedEventArgs e)
        {
            if(ipv4.Text == "IP Address") 
            {
                ipv4.Foreground = new SolidColorBrush(Colors.Black);
                ipv4.Text = "";
            }
        }

        private void ipv4_LostFocus(object sender, RoutedEventArgs e)
        {
            if(ipv4.Text == "") 
            {
                ipv4.Foreground = new SolidColorBrush(Colors.Gray);
                ipv4.Text = "IP Address";
            }
        }

        private void Name_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Name.Text == "Name of User")
            {
                Name.Foreground = new SolidColorBrush(Colors.Black);
                Name.Text = "";
            }
        }

        private void Name_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Name.Text == "")
            {
                Name.Foreground = new SolidColorBrush(Colors.Gray);
                Name.Text = "Name of User";
            }
        }

        private void password_GotFocus(object sender, RoutedEventArgs e)
        {
            if (password.Text == "Password")
            {
                password.Foreground = new SolidColorBrush(Colors.Black);
                password.Text = "";
            }
        }

        private void password_LostFocus(object sender, RoutedEventArgs e)
        {
            if (password.Text == "")
            {
                password.Foreground = new SolidColorBrush(Colors.Gray);
                password.Text = "Password";
            }
        }
    }
}