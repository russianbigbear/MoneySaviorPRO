using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using MoneyCalc.LogRegUser;
using ClosedXML.Excel;

namespace MoneyCalc
{
    public partial class LoginForm : Window
    {
        private static string AuthPath = Directory.GetCurrentDirectory() + @"\BD\Auth.xlsx";
        private Dictionary<string, string> User = new Dictionary<string, string>();

        public LoginForm()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                Window win = ((Window)sender);
                win.Closed -= Window_Closed;
                this.Show();
            }
            catch (Exception) { this.Close(); }

        }

        private void RegBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Registration winR = new Registration();
            winR.Closed += Window_Closed;
            winR.Show();
        }

        private void LoginBTN_Click(object sender, RoutedEventArgs e)
        {
            if(AuthProcess(NameTB.Text, PassTB.Password))
            {
                MainWindow winM = new MainWindow(User);
                winM.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Имя пользователя или пароль не существует!");
            }
        }

        private void ExitBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Window win = ((Window)sender);
                win.Closed -= Window_Closed;
                this.Show();
            }
            catch (Exception) { this.Close(); }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PassShowTB.Text = PassTB.Password;
            PassTB.Visibility = Visibility.Hidden;
            PassShowTB.Visibility = Visibility.Visible;
        }

        private void ShowBTN_MouseLeave(object sender, MouseEventArgs e)
        {
            PassTB.Visibility = Visibility.Visible;
            PassShowTB.Visibility = Visibility.Hidden;
        }

        private bool AuthProcess(string Login, string Pass)
        {
            XLWorkbook workbook = new XLWorkbook(AuthPath);
            IXLWorksheet list = workbook.Worksheets.Worksheet(1);

            for(int i = 1; list.Cell("A" + i).Value.ToString() != ""; i++)
            {
                string login = list.Cell("A" + i).Value.ToString();
                string pass = list.Cell("B" + i).Value.ToString();

                if(login == Login && pass == Pass)
                {
                    User.Add(Login, pass);
                    return true;
                }
            }

            return false;
        }
       
    }
}
