using System.IO;
using System.Windows;
using System.Windows.Input;
using ClosedXML.Excel;

namespace MoneyCalc.LogRegUser
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        private static string AuthPath = Directory.GetCurrentDirectory() + @"\BD\Auth.xlsx";

        public Registration()
        {
            InitializeComponent();
        }

        private void One_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PassOneShowTB.Text = PassOneTB.Password;
            PassOneTB.Visibility = Visibility.Hidden;
            PassOneShowTB.Visibility = Visibility.Visible;
        }

        private void ShowOneBTN_MouseLeave(object sender, MouseEventArgs e)
        {
            PassOneTB.Visibility = Visibility.Visible;
            PassOneShowTB.Visibility = Visibility.Hidden;
        }

        private void Two_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PassTwoShowTB.Text = PassTwoTB.Password;
            PassTwoTB.Visibility = Visibility.Hidden;
            PassTwoShowTB.Visibility = Visibility.Visible;
        }

        private void ShowTwoBTN_MouseLeave(object sender, MouseEventArgs e)
        {
            PassTwoTB.Visibility = Visibility.Visible;
            PassTwoShowTB.Visibility = Visibility.Hidden;
        }

        private void ExitBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CreateBTN_Click(object sender, RoutedEventArgs e)
        {
            if(NameTB.Text == "")
            {
                MessageBox.Show("Заполните поле \"Имя пользователя\"!");
                return;
            } 
            else if(PassOneTB.Password == "")
            {
                MessageBox.Show("Заполните поле \"Пароль\"!");
                return;
            }
            else if (PassTwoTB.Password == "")
            {
                MessageBox.Show("Заполните поле \"Подтверждение пароля\"!");
                return;
            }
            else if(PassOneTB.Password != PassTwoTB.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            RegProcess(NameTB.Text, PassTwoTB.Password);

            this.Close();

        }

        private void RegProcess(string Login, string Pass)
        {
            XLWorkbook workbook = new XLWorkbook(AuthPath);
            IXLWorksheet list = workbook.Worksheets.Worksheet(1);

            int count = 1;
            while(list.Cell("A" + count).Value.ToString() != "")
                count++;

            list.Cell("A" + count).Value = Login;
            list.Cell("B" + count).Value = Pass;

            workbook.Save();
        }


    }
}
