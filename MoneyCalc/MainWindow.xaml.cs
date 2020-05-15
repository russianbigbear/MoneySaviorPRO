using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MoneyCalc.MenuForm;
using System.IO;
using ClosedXML.Excel;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using Novacode;

namespace MoneyCalc
{
    public partial class MainWindow : Window
    {
        private static string BillPath = Directory.GetCurrentDirectory() + @"\BD\Bill.xlsx";
        private static string InPath = Directory.GetCurrentDirectory() + @"\BD\CatIn.xlsx";
        private static string OutPath = Directory.GetCurrentDirectory() + @"\BD\CatOut.xlsx";
        private static string UserInfo = Directory.GetCurrentDirectory() + @"\BD\UserInfo\";

        private string Username;
        private string Pass;
        private string Report;
        private List<string> CategoryIncomeCB;
        private List<string> CategoryExpenseCB;
        private List<string> Bills;

        public PersonalBudget personalBudget;

        /// <summary>
        /// "Класс" Личный бюджет.
        /// Вся информация о бюджете пользователя.
        /// PersonalCash - сумма бюджета;
        /// Incomes - доходы;
        /// Expenses - расходы;
        /// </summary>
        public class PersonalBudget
        {
            public Cash PersonalCash { get; set; }
            public ObservableCollection<Income> Incomes { get; set; }
            public ObservableCollection<Expense> Expenses { get; set; }
            public ObservableCollection<Goal> Goals { get; set; }

            /*Конструктор бкз параметров*/
            public PersonalBudget()
            {
                PersonalCash = new Cash();
                Incomes = new ObservableCollection<Income>();           
                Expenses = new ObservableCollection<Expense>();
                Goals = new ObservableCollection<Goal>();
            }
        }

        /// <summary>
        /// "Класс" Общая сумма бюджета.
        /// ( Sum - Сумма; ValueID - ID валюты)
        /// </summary>
        public class Cash
        {
            public string Sum { get; set; }
            public string ValueID { get; set; }

            public Cash()
            {
                Sum = "0";
                ValueID = "RUB";
            }
        }

        /// <summary>
        /// "Класс" Доход.
        /// (IncomeID - иденнтификатор;
        /// CategoryIncome - категория;
        /// NameIncome - название;
        /// BillIncome - счет;
        /// SumIncome - сумма;
        /// CommentIncome - комментарий;
        /// DateIncome - дата создания)
        /// </summary>
        public class Income
        {
            public string IncomeID { get; set; }
            public DateTime DateIncome { get; set; }
            public string CategoryIncome { get; set; }
            public string NameIncome { get; set; }
            public string BillIncome { get; set; }
            public string SumIncome { get; set; }
            public string CommentIncome { get; set; }

            public Income()
            {
                IncomeID = "";
                DateIncome = DateTime.Now;
                CategoryIncome = "";
                NameIncome = "";
                BillIncome = "";
                SumIncome = "";
                CommentIncome = "";
            }
        }

        /// <summary>
        /// "Класс" Расход.
        /// (ExpenseID - иденнтификатор;
        /// CategoryExpense - категория;
        /// NameExpense - название;
        /// BillExpense - счет;
        /// SumExpense - сумма;
        /// CommentExpense - комментарий;
        /// DateExpense - дата создания)
        /// </summary>
        public class Expense
        {
            public string ExpenseID { get; set; }
            public DateTime DateExpense { get; set; }
            public string CategoryExpense { get; set; }
            public string NameExpense { get; set; }
            public string BillExpense { get; set; }
            public string SumExpense { get; set; }
            public string CommentExpense { get; set; }

            public Expense()
            {
                ExpenseID = "";
                DateExpense = DateTime.Now;
                CategoryExpense = "";
                NameExpense = "";
                BillExpense = "";
                SumExpense = "";
                CommentExpense = "";
            }
        }

        /// <summary>
        /// "Класс" Цель.
        /// (GoalID - иденнтификатор;
        /// DateCreateGoal - дата создания;
        /// NameGoal - название;
        /// SumGoal - сумма цели;
        /// SumAlredy - текущая сумма;
        /// SumLeft - осталось;
        /// DateFinalGoal - дата окончания цели;
        /// </summary>
        public class Goal
        {
            public string GoalID { get; set; }
            public DateTime DateCreateGoal { get; set; }
            public string NameGoal { get; set; }
            public string SumGoal { get; set; }
            public string SumAlredy { get; set; }
            public string SumLeft { get; set; }
            public DateTime DateFinalGoal { get; set; }

            public Goal()
            {
                GoalID = "";
                DateCreateGoal = DateTime.Now;            
                NameGoal = "";
                SumGoal = "";
                SumAlredy = "";
                SumLeft = "";
                DateFinalGoal = DateTime.Now;
            }
        }

        /// <summary>
        /// Конструктор главного окна
        /// </summary>
        /// <param name="user">Данные о пользователе</param>
        public MainWindow(Dictionary<string, string> user)
        {
            Username = user.Last().Key;
            Pass = user.Last().Value;
            Report = "";
            personalBudget = new PersonalBudget();

            InitializeComponent();

            UsernameL.Header = "Пользователь: " + Username;
            BudgetShowL.Content = Math.Round(Convert.ToDouble(personalBudget.PersonalCash.Sum), 2).ToString();

            ValueForm value_form = new ValueForm(this);
            ValueShowL.Content = value_form.ValueGet(personalBudget.PersonalCash.ValueID);
            value_form.Close();

            UpdateBillCB();
            BillInCB.SelectedIndex = 0;
            BillOutCB.SelectedIndex = 0;
           
            UpdateInCB();
            CategoryInCB.SelectedIndex = 0;

            UpdateOutCB();
            CategoryOutCB.SelectedIndex = 0;

            INdataGrid.ItemsSource = personalBudget.Incomes;
            EXdataGrid.ItemsSource = personalBudget.Expenses;
            TAdataGrid.ItemsSource = personalBudget.Goals;

            LoadUserInfo();

            INdataGrid.Items.Refresh();
            EXdataGrid.Items.Refresh();
            TAdataGrid.Items.Refresh();

        }

        /// <summary>
        /// Вывод элементов формы в зависимости от выбранной вкладки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Print_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Print.SelectedItem == IncomeTab)
            {
                AddInGB.Visibility = Visibility.Visible;
                AddOutGB.Visibility = Visibility.Hidden;
                AddGoalGB.Visibility = Visibility.Hidden;

                ClsExpense();
                ClsGoal();
                ClsReport();
            }

            if (Print.SelectedItem == ExpensesTab)
            {
                AddInGB.Visibility = Visibility.Hidden;
                AddOutGB.Visibility = Visibility.Visible;
                AddGoalGB.Visibility = Visibility.Hidden;

                ClsIncome();
                ClsGoal();
                ClsReport();
            }

            if(Print.SelectedItem == TargetTab)
            {
                AddInGB.Visibility = Visibility.Hidden;
                AddOutGB.Visibility = Visibility.Hidden;
                AddGoalGB.Visibility = Visibility.Visible;

                ClsIncome();
                ClsExpense();
                ClsReport();
            }

            if (Print.SelectedItem == ReportTab)
            {
                AddInGB.Visibility = Visibility.Hidden;
                AddOutGB.Visibility = Visibility.Hidden;
                AddGoalGB.Visibility = Visibility.Hidden;


                ClsIncome();
                ClsExpense();
                ClsGoal();
            }
        }

        /// <summary>
        /// Нажатие на кнопку "Сменить пользователя"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            LoginForm winL = new LoginForm();
            winL.Show();
            this.Close();
        }

        /// <summary>
        /// Нажатие на кнопку "Мои счета"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BillsItem_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            BillForm winB = new BillForm(this);
            winB.Closed += Window_Closed;
            winB.Show();
        }

        /// <summary>
        /// Нажатие на кнопку "Категории доходов"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IncomeItem_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            IncomeForm winI = new IncomeForm(this);
            winI.Closed += Window_Closed;
            winI.Show();
        }

        /// <summary>
        /// Нажатие на кнопку "к=Категории расходов"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExspenseItem_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Exspense winE = new Exspense(this);
            winE.Closed += Window_Closed;
            winE.Show();
        }

        /// <summary>
        /// Нажатие на кнопку "Сменить валюту"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BudgetBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            ValueForm winV = new ValueForm(this);
            winV.Closed += Window_Closed;
            winV.Show();
        }

        /// <summary>
        /// Событие при закртыии главного окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                Window win = ((Window)sender);
                win.Closed -= Window_Closed;
                this.Show();
            }
            catch (Exception) {
                SaveUserInfo();
                this.Close(); 
            }
        }

        /// <summary>
        /// Метод обновления ComboBox счетов
        /// </summary>
        public void UpdateBillCB()
        {
            XLWorkbook workbook = new XLWorkbook(BillPath);
            IXLWorksheet list = workbook.Worksheets.Worksheet(1);
            Bills = new List<string>();

            BillInCB.Items.Clear();
            BillOutCB.Items.Clear();
            Bills.Clear();

            for (int i = 1; list.Cell("A" + i).Value.ToString() != ""; i++)
            {
                Bills.Add(list.Cell("A" + i).Value.ToString());

                BillInCB.Items.Add(Bills[i - 1]);
                BillOutCB.Items.Add(Bills[i - 1]);
    
            }
        }

        /// <summary>
        /// Метод обновления ComboBox доходов
        /// </summary>
        public void UpdateInCB()
        {
            XLWorkbook workbook = new XLWorkbook(InPath);
            IXLWorksheet list = workbook.Worksheets.Worksheet(1);
            CategoryIncomeCB = new List<string>();

            CategoryInCB.Items.Clear();
            CategoryIncomeCB.Clear();

            for (int i = 1; list.Cell("A" + i).Value.ToString() != ""; i++)
            {
                CategoryIncomeCB.Add(list.Cell("A" + i).Value.ToString());
                CategoryInCB.Items.Add(CategoryIncomeCB[i - 1]);
            }
        }

        /// <summary>
        /// Метод обновления ComboBox расходов
        /// </summary>
        public void UpdateOutCB()
        {
            XLWorkbook workbook = new XLWorkbook(OutPath);
            IXLWorksheet list = workbook.Worksheets.Worksheet(1);
            CategoryExpenseCB = new List<string>();

            CategoryOutCB.Items.Clear();
            CategoryExpenseCB.Clear();

            for (int i = 1; list.Cell("A" + i).Value.ToString() != ""; i++)
            {
                CategoryExpenseCB.Add(list.Cell("A" + i).Value.ToString());
                CategoryOutCB.Items.Add(CategoryExpenseCB[i - 1]);
            }
        }

        /// <summary>
        /// Метод очистки форм связанных с доходами
        /// </summary>
        private void ClsIncome()
        {
            CategoryInCB.SelectedIndex = 0;
            NameAddInTB.Text = "";
            BillInCB.SelectedIndex = 0;
            MoneyInTB.Text = "";
            CommentInTB.Text = "";

            INAllRB.IsChecked = true;
        }

        /// <summary>
        /// Метод очистки форм связанных с расходами
        /// </summary>
        private void ClsExpense()
        {
            CategoryOutCB.SelectedIndex = 0;
            NameAddOutTB.Text = "";
            BillOutCB.SelectedIndex = 0;
            MoneyOutTB.Text = "";
            CommentOutTB.Text = "";

            EXAllRB.IsChecked = true;
        }

        /// <summary>
        /// Метод очистки форм связанных с целями
        /// </summary>
        private void ClsGoal()
        {
            NameAddGoalTB.Text = "";
            MoneyGoalTB.Text = "";
            MoneyGoalHaveTB.Text = "";
            DateGoalDP.Text = "";

            GOAllRB.IsChecked = true;
        }

        /// <summary>
        /// Метод очистки форм связанных с отчетами
        /// </summary>
        private void ClsReport()
        {
            Report = "";

            BudgetBOX.IsChecked = false;
            CatInBOX.IsChecked = false;
            CatExBOX.IsChecked = false;
            InBOX.IsChecked = false;
            ExBOX.IsChecked = false;

            FromDP.Text = "";
            ToDP.Text = "";
            RepTB.Text = "";
        }

        /// <summary>
        /// Добавление дохода
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddInBTN_Click(object sender, RoutedEventArgs e)
        {
            if (CategoryInCB.Text == "")
                MessageBox.Show("Выберите категорию дохода!");
            else if (NameAddInTB.Text == "")
                MessageBox.Show("Введите название дохода!");
            else if (BillInCB.Text == "")
                MessageBox.Show("Выберите счёт дохода!");
            else if (MoneyInTB.Text == "")
                MessageBox.Show("Введите сумму дохода!");
            else
            {
                personalBudget.Incomes.Add(new Income
                {
                    IncomeID = personalBudget.Incomes.Count.ToString(),
                    DateIncome = DateTime.Now,
                    CategoryIncome = CategoryInCB.Text,
                    NameIncome = NameAddInTB.Text,
                    BillIncome = BillInCB.Text,
                    SumIncome = MoneyInTB.Text,
                    CommentIncome = CommentInTB.Text,
                });

                ClsIncome();
            }
            CalculateBudget();
        }

        /// <summary>
        /// Добавление расхода
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddOutBTN_Click(object sender, RoutedEventArgs e)
        {
            if (CategoryOutCB.Text == "")
                MessageBox.Show("Выберите категорию расхода!");
            else if (NameAddOutTB.Text == "")
                MessageBox.Show("Введите название расхода!");
            else if (BillOutCB.Text == "")
                MessageBox.Show("Выберите счёт расхода!");
            else if (MoneyOutTB.Text == "")
                MessageBox.Show("Введите сумму расхода!");
            else
            {
                personalBudget.Expenses.Add(new Expense
                {
                    ExpenseID = personalBudget.Expenses.Count.ToString(),
                    DateExpense = DateTime.Now,
                    CategoryExpense = CategoryOutCB.Text,
                    NameExpense = NameAddOutTB.Text,
                    BillExpense = BillOutCB.Text,
                    SumExpense = MoneyOutTB.Text,
                    CommentExpense = CommentOutTB.Text,
                });

                ClsExpense();
            }
            CalculateBudget();
        }

        /// <summary>
        /// Добавление цели
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddGoalBTN_Click(object sender, RoutedEventArgs e)
        {
            if (NameAddGoalTB.Text == "")
                MessageBox.Show("Введите название цели!");
            else if (MoneyGoalTB.Text == "")
                MessageBox.Show("Введите сумму цели!");
            else if (MoneyGoalHaveTB.Text == "")
                MessageBox.Show("Введите собранную сумму (0, если нет)!");
            else if (DateGoalDP.Text == "")
                MessageBox.Show("Введите дату окончания цели!");
            else
            {
                string leftS = "Сумма собрана";
                double leftD = Convert.ToDouble(MoneyGoalTB.Text) - Convert.ToDouble(MoneyGoalHaveTB.Text);

                if (leftD > 0)
                    leftS = leftD.ToString();

                personalBudget.Goals.Add(new Goal
                {
                    GoalID = personalBudget.Goals.Count.ToString(),
                    DateCreateGoal = DateTime.Now,
                    NameGoal = NameAddGoalTB.Text,
                    SumGoal = MoneyGoalTB.Text,
                    SumAlredy = MoneyGoalHaveTB.Text,
                    SumLeft = leftS,
                    DateFinalGoal = Convert.ToDateTime(DateGoalDP.Text), 
                });

                ClsGoal();
            }
            CalculateBudget();
        }

        /// <summary>
        /// Нажатие на кнопку Сохранить изменения доходы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void INBTN_Click(object sender, RoutedEventArgs e)
        {
            TAdataGrid.Items.Refresh();
            CalculateBudget();
        }

        /// <summary>
        /// Нажатие на кнопку Сохранить изменения расходы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EXBTN_Click(object sender, RoutedEventArgs e)
        {
            TAdataGrid.Items.Refresh();
            CalculateBudget();
        }

        /// <summary>
        /// Нажатие на кнопку Сохранить изменения цели
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TABTN_Click(object sender, RoutedEventArgs e)
        {

            for (int i = 0; i < personalBudget.Goals.Count; i++)
            {
                string leftS = "Сумма собрана";
                double leftD = Convert.ToDouble(personalBudget.Goals[i].SumGoal) - Convert.ToDouble(personalBudget.Goals[i].SumAlredy);

                if (leftD > 0)
                    leftS = leftD.ToString();

                personalBudget.Goals[i].SumLeft = leftS;
            }

            TAdataGrid.Items.Refresh();
            CalculateBudget();
        }

        /// <summary>
        /// Метод подсчета бюджета
        /// </summary>
        public void CalculateBudget()
        {
            double AllIncome = 0;
            double AllExpense = 0;
            double AllGoal = 0;

            for(int i = 0; i < personalBudget.Incomes.Count; i++)
                AllIncome += Convert.ToDouble(personalBudget.Incomes[i].SumIncome);

            for (int i = 0; i < personalBudget.Expenses.Count; i++)
                AllExpense += Convert.ToDouble(personalBudget.Expenses[i].SumExpense);

            for (int i = 0; i < personalBudget.Goals.Count; i++)
                AllGoal += Convert.ToDouble(personalBudget.Goals[i].SumAlredy);

            personalBudget.PersonalCash.Sum = (AllIncome - AllExpense - AllGoal).ToString();
            personalBudget.PersonalCash.ValueID = "RUB";

            BudgetShowL.Content = personalBudget.PersonalCash.Sum;
            ValueForm value_form = new ValueForm(this);
            ValueShowL.Content = value_form.ValueGet(personalBudget.PersonalCash.ValueID);
            value_form.Close();
        }

        private void INdataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            CalculateBudget();
        }

        private void INdataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            CalculateBudget();
        }

        private void EXdataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            CalculateBudget();
        }

        private void EXdataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            CalculateBudget();
        }

        private void TAdataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            CalculateBudget();
        }

        private void TAdataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            CalculateBudget();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CalculateBudget();
        }

        /// <summary>
        /// Сохранение всех данных пользователя
        /// </summary>
        public void SaveUserInfo()
        {
            string UnicName = "";
            char[] Name = (Username + "Info" + Pass).ToCharArray();

            for (int i = 0; i < Name.Length; i++)
                UnicName += Convert.ToInt32(Name[i]).ToString();    

            XLWorkbook workbook = new XLWorkbook(UserInfo + UnicName + ".xlsx");
            IXLWorksheet list = workbook.Worksheets.Worksheet(1);

            for (int i = 1; list.Cell("A" + i).Value.ToString() != ""; i++) 
            {
                list.Cell("A" + i).Value = "";
                list.Cell("B" + i).Value = "";
                list.Cell("C" + i).Value = "";
                list.Cell("D" + i).Value = "";
                list.Cell("E" + i).Value = "";
                list.Cell("F" + i).Value = "";
            }

            for (int i = 0; i < personalBudget.Incomes.Count; i++)
            {
                list.Cell("A" + (i + 1)).Value = personalBudget.Incomes[i].DateIncome.ToString("dd:MM:yyyy");
                list.Cell("B" + (i + 1)).Value = personalBudget.Incomes[i].CategoryIncome;
                list.Cell("C" + (i + 1)).Value = personalBudget.Incomes[i].NameIncome;
                list.Cell("D" + (i + 1)).Value = personalBudget.Incomes[i].BillIncome;
                list.Cell("E" + (i + 1)).Value = personalBudget.Incomes[i].SumIncome;

                if (personalBudget.Incomes[i].CommentIncome == "")
                    list.Cell("F" + (i + 1)).Value = "";
                else
                    list.Cell("F" + (i + 1)).Value = personalBudget.Incomes[i].CommentIncome.ToString();
            }

            for (int i = 1; list.Cell("G" + i).Value.ToString() != ""; i++)
            {
                list.Cell("G" + i).Value = "";
                list.Cell("H" + i).Value = "";
                list.Cell("I" + i).Value = "";
                list.Cell("J" + i).Value = "";
                list.Cell("K" + i).Value = "";
                list.Cell("L" + i).Value = "";
            }

            for (int i = 0; i < personalBudget.Expenses.Count; i++)
            {
                list.Cell("G" + (i + 1)).Value = personalBudget.Expenses[i].DateExpense.ToString("dd:MM:yyyy");
                list.Cell("H" + (i + 1)).Value = personalBudget.Expenses[i].CategoryExpense;
                list.Cell("I" + (i + 1)).Value = personalBudget.Expenses[i].NameExpense;
                list.Cell("J" + (i + 1)).Value = personalBudget.Expenses[i].BillExpense;
                list.Cell("K" + (i + 1)).Value = personalBudget.Expenses[i].SumExpense;

                if (personalBudget.Expenses[i].CommentExpense == "")
                    list.Cell("L" + (i + 1)).Value = "";
                else
                    list.Cell("L" + (i + 1)).Value = personalBudget.Expenses[i].CommentExpense.ToString();
            }

            for (int i = 1; list.Cell("M" + i).Value.ToString() != ""; i++)
            {
                list.Cell("M" + i).Value = "";
                list.Cell("N" + i).Value = "";
                list.Cell("O" + i).Value = "";
                list.Cell("P" + i).Value = "";
                list.Cell("Q" + i).Value = "";
                list.Cell("R" + i).Value = "";
            }

            for (int i = 0; i < personalBudget.Goals.Count; i++)
            {
                list.Cell("M" + (i + 1)).Value = personalBudget.Goals[i].DateCreateGoal.ToString("dd:MM:yyyy");
                list.Cell("N" + (i + 1)).Value = personalBudget.Goals[i].NameGoal;
                list.Cell("O" + (i + 1)).Value = personalBudget.Goals[i].SumGoal;
                list.Cell("P" + (i + 1)).Value = personalBudget.Goals[i].SumAlredy;
                list.Cell("Q" + (i + 1)).Value = personalBudget.Goals[i].SumLeft;
                list.Cell("R" + (i + 1)).Value = personalBudget.Goals[i].DateFinalGoal.ToString("dd:MM:yyyy");
            }

            workbook.Save();
        }

        /// <summary>
        /// Загрузка данных пользователя
        /// </summary>
        public void LoadUserInfo()
        {
            string UnicName = "";
            char[] Name = (Username + "Info" + Pass).ToCharArray();

            for (int i = 0; i < Name.Length; i++)
                UnicName += Convert.ToInt32(Name[i]).ToString();

            try
            {
                new XLWorkbook(UserInfo + UnicName + ".xlsx");
            }
            catch (Exception)
            {
                XLWorkbook New = new XLWorkbook();
                New.Worksheets.Add();
                New.SaveAs(UserInfo + UnicName + ".xlsx");
            }

            XLWorkbook workbook = new XLWorkbook(UserInfo + UnicName + ".xlsx");
            IXLWorksheet list = workbook.Worksheets.Worksheet(1);

            for (int i = 1; list.Cell("A" + i).Value.ToString() != ""; i++)
            {
                personalBudget.Incomes.Add(new Income
                {
                    IncomeID = (i - 1).ToString(),
                    DateIncome = new DateTime(
                                Int32.Parse(list.Cell("A" + i).Value.ToString().Substring(6, 4)),
                                Int32.Parse(list.Cell("A" + i).Value.ToString().Substring(3, 2)),
                                Int32.Parse(list.Cell("A" + i).Value.ToString().Substring(0, 2))),
                    CategoryIncome = list.Cell("B" + i).Value.ToString(),
                    NameIncome = list.Cell("C" + i).Value.ToString(),
                    BillIncome = list.Cell("D" + i).Value.ToString(),
                    SumIncome = list.Cell("E" + i).Value.ToString(),
                    CommentIncome = list.Cell("F" + i).Value.ToString(),
                }); ;
            }

            for (int i = 1; list.Cell("G" + i).Value.ToString() != ""; i++)
            {
                personalBudget.Expenses.Add(new Expense
                {
                    ExpenseID = (i - 1).ToString(),
                    DateExpense = new DateTime(
                                Int32.Parse(list.Cell("G" + i).Value.ToString().Substring(6, 4)),
                                Int32.Parse(list.Cell("G" + i).Value.ToString().Substring(3, 2)),
                                Int32.Parse(list.Cell("G" + i).Value.ToString().Substring(0, 2))),
                    CategoryExpense = list.Cell("H" + i).Value.ToString(),
                    NameExpense = list.Cell("I" + i).Value.ToString(),
                    BillExpense = list.Cell("J" + i).Value.ToString(),
                    SumExpense = list.Cell("K" + i).Value.ToString(),
                    CommentExpense = list.Cell("L" + i).Value.ToString(),
                });
            }

            for (int i = 1; list.Cell("M" + i).Value.ToString() != ""; i++)
            {
                personalBudget.Goals.Add(new Goal
                {
                    GoalID = (i - 1).ToString(),
                    DateCreateGoal = new DateTime(
                                Int32.Parse(list.Cell("M" + i).Value.ToString().Substring(6, 4)),
                                Int32.Parse(list.Cell("M" + i).Value.ToString().Substring(3, 2)),
                                Int32.Parse(list.Cell("M" + i).Value.ToString().Substring(0, 2))),
                    NameGoal = list.Cell("N" + i).Value.ToString(),
                    SumGoal = list.Cell("O" + i).Value.ToString(),
                    SumAlredy = list.Cell("P" + i).Value.ToString(),
                    SumLeft = list.Cell("Q" + i).Value.ToString(),
                    DateFinalGoal = new DateTime(
                                Int32.Parse(list.Cell("R" + i).Value.ToString().Substring(6, 4)),
                                Int32.Parse(list.Cell("R" + i).Value.ToString().Substring(3, 2)),
                                Int32.Parse(list.Cell("R" + i).Value.ToString().Substring(0, 2))),
                });
            }
        }

        /// <summary>
        /// Создание и вывод отчета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateReportBRB_Click(object sender, RoutedEventArgs e)
        {
            Report += "<ОТЧЁТ>\n";

            if(BudgetBOX.IsChecked == true)
            {
                Report += "Сумма бюджета в рублях: " + personalBudget.PersonalCash.Sum + "\n";
            }

            if(CatInBOX.IsChecked == true)
            {
                for(int i = 0; i < CategoryIncomeCB.Count; i++)
                {
                    string catS = "\n//Доход с категорией - " + CategoryIncomeCB[i] + "//\n";
                    string repSub = "";
                    bool flag = false;

                    for (int j = 0; j < personalBudget.Incomes.Count; j++)
                    {
                        if (CategoryIncomeCB[i] == personalBudget.Incomes[j].CategoryIncome)
                        {
                            flag = true;
                            repSub += personalBudget.Incomes[j].NameIncome + ": ";
                            repSub += personalBudget.Incomes[j].SumIncome + "\n";
                        }
                    }

                    if (flag)
                        Report += catS + repSub;
                }
            }

            if (CatExBOX.IsChecked == true)
            {
                for (int i = 0; i < CategoryExpenseCB.Count; i++)
                {
                    string catS = "\n//Расход с категорией - " + CategoryExpenseCB[i] + "//\n";
                    string repSub = "";
                    bool flag = false;

                    for (int j = 0; j < personalBudget.Expenses.Count; j++)
                    {
                        if (CategoryExpenseCB[i] == personalBudget.Expenses[j].CategoryExpense)
                        {
                            flag = true;
                            repSub += personalBudget.Expenses[j].NameExpense + ": ";
                            repSub += personalBudget.Expenses[j].SumExpense + "\n";
                        }
                    }

                    if (flag)
                        Report += catS + repSub;
                }
            }

            if (InBOX.IsChecked == true)
            {
                for(int i = 0; i < personalBudget.Incomes.Count; i++)
                    if(personalBudget.Incomes[i].DateIncome >= FromDP.SelectedDate && personalBudget.Incomes[i].DateIncome <= ToDP.SelectedDate)
                    {
                        Report += "\n//Доход " + (i + 1) + "//\n";
                        Report += "Дата создания: " + personalBudget.Incomes[i].DateIncome.ToString("dd:MM:yyyy") + "\n";
                        Report += "Категория: " + personalBudget.Incomes[i].CategoryIncome + "\n";
                        Report += "Название: " + personalBudget.Incomes[i].NameIncome + "\n";
                        Report += "Счет: " + personalBudget.Incomes[i].BillIncome + "\n";
                        Report += "Сумма: " + personalBudget.Incomes[i].SumIncome + "\n";
                        Report += "Комментарий: " + personalBudget.Incomes[i].CommentIncome + "\n\n";
                    }
            }

            if (ExBOX.IsChecked == true)
            {
                for (int i = 0; i < personalBudget.Expenses.Count; i++)
                    if (personalBudget.Expenses[i].DateExpense >= FromDP.SelectedDate && personalBudget.Expenses[i].DateExpense <= ToDP.SelectedDate)
                    {
                        Report += "\n//Расход " + (i + 1) + "//\n";
                        Report += "Дата создания: " + personalBudget.Expenses[i].DateExpense.ToString("dd:MM:yyyy") + "\n";
                        Report += "Категория: " + personalBudget.Expenses[i].CategoryExpense + "\n";
                        Report += "Название: " + personalBudget.Expenses[i].NameExpense + "\n";
                        Report += "Счет: " + personalBudget.Expenses[i].BillExpense + "\n";
                        Report += "Сумма: " + personalBudget.Expenses[i].SumExpense + "\n";
                        Report += "Комментарий: " + personalBudget.Expenses[i].CommentExpense + "\n\n";
                    }
            }

            Report += "<КОНЕЦ ОТЧЁТА>";
            RepTB.Text = Report;

            Report = "";
        }

        /// <summary>
        /// Сохранение отчета в файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SavePrintBTn_Click(object sender, RoutedEventArgs e)
        {
            // Сохранение файла
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Word files|*.doc",
                Title = "Save an Word File",
                FileName = "Отчет-" + Username + "-" + DateTime.Now.ToString("dd.MM.yyyy") + ".doc",
            };

            if (saveFileDialog.ShowDialog() == true && !String.IsNullOrWhiteSpace(saveFileDialog.FileName))
            {
                DocX doc = DocX.Create(saveFileDialog.FileName);

                doc.InsertParagraph();
                doc.Paragraphs[0].InsertText(RepTB.Text);
                doc.SaveAs(saveFileDialog.FileName);
            }
        }

        private void INdataGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            INAllRB.IsChecked = true;
        }


        private void INdataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            INAllRB.IsChecked = true;
        }

        private void INAllRB_Checked(object sender, RoutedEventArgs e)
        {
            INdataGrid.IsReadOnly = false;
            INdataGrid.ItemsSource = personalBudget.Incomes;
        }

        private void INYearRB_Checked(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Income> InFilterDate= new ObservableCollection<Income>(from income in personalBudget.Incomes
                                                            where income.DateIncome.Year == DateTime.Now.Year
                                                            select income);
            INdataGrid.ItemsSource = InFilterDate;;
            INdataGrid.IsReadOnly = true;
        }

        private void INMounthRB_Checked(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Income> InFilterDate = new ObservableCollection<Income>(from income in personalBudget.Incomes
                                                            where income.DateIncome.Month == DateTime.Now.Month
                                                            && income.DateIncome.Year == DateTime.Now.Year
                                                            select income);
            INdataGrid.ItemsSource = InFilterDate;
            INdataGrid.IsReadOnly = true;
        }

        private void INWeekRB_Checked(object sender, RoutedEventArgs e)
        {     
            ObservableCollection<Income> InFilterDate = new ObservableCollection<Income>(from income in personalBudget.Incomes
                                                            where income.DateIncome.Day >= (DateTime.Now.Day - 3)
                                                            && income.DateIncome.Day <= (DateTime.Now.Day + 4)
                                                            && income.DateIncome.Month == DateTime.Now.Month
                                                            && income.DateIncome.Year == DateTime.Now.Year
                                                            select income);
            INdataGrid.ItemsSource = InFilterDate;
            INdataGrid.IsReadOnly = true;
        }

        private void INTodayRB_Checked(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Income> InFilterDate = new ObservableCollection<Income>(from income in personalBudget.Incomes
                                                            where income.DateIncome.Day == DateTime.Now.Day
                                                            && income.DateIncome.Month == DateTime.Now.Month
                                                            && income.DateIncome.Year == DateTime.Now.Year
                                                                                         select income);
            INdataGrid.ItemsSource = InFilterDate;
            INdataGrid.IsReadOnly = true;
        }

        private void EXdataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            EXAllRB.IsChecked = true;
        }

        private void EXdataGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EXAllRB.IsChecked = true;
        }

        private void EXAllRB_Checked(object sender, RoutedEventArgs e)
        {
            EXdataGrid.IsReadOnly = false;
            EXdataGrid.ItemsSource = personalBudget.Expenses;
        }

        private void EXYearRB_Checked(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Expense> ExFilterDate = new ObservableCollection<Expense>(from expense in personalBudget.Expenses
                                                                                         where expense.DateExpense.Year == DateTime.Now.Year
                                                                                         select expense);
            EXdataGrid.ItemsSource = ExFilterDate;
            EXdataGrid.IsReadOnly = true;
        }

        private void EXMounthRB_Checked(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Expense> ExFilterDate = new ObservableCollection<Expense>(from expense in personalBudget.Expenses
                                                                                           where expense.DateExpense.Month == DateTime.Now.Month
                                                                                         && expense.DateExpense.Year == DateTime.Now.Year
                                                                                         select expense);
            INdataGrid.ItemsSource = ExFilterDate;
            INdataGrid.IsReadOnly = true;
        }

        private void EXWeekRB_Checked(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Expense> ExFilterDate = new ObservableCollection<Expense>(from expense in personalBudget.Expenses
                                                                                           where expense.DateExpense.Day >= (DateTime.Now.Day - 3)
                                                                                             && expense.DateExpense.Day <= (DateTime.Now.Day + 4)
                                                                                             && expense.DateExpense.Month == DateTime.Now.Month
                                                                                             && expense.DateExpense.Year == DateTime.Now.Year
                                                                                           select expense);
            INdataGrid.ItemsSource = ExFilterDate;
            INdataGrid.IsReadOnly = true;
        }

        private void EXTodayRB_Checked(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Expense> ExFilterDate = new ObservableCollection<Expense>(from expense in personalBudget.Expenses
                                                                                           where expense.DateExpense.Day == DateTime.Now.Day
                                                                                            && expense.DateExpense.Month == DateTime.Now.Month
                                                                                            && expense.DateExpense.Year == DateTime.Now.Year
                                                                                           select expense);
            INdataGrid.ItemsSource = ExFilterDate;
            INdataGrid.IsReadOnly = true;
        }

        private void TAdataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            GOAllRB.IsChecked = true;
        }

        private void TAdataGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GOAllRB.IsChecked = true;
        }

        private void GOAllRB_Checked(object sender, RoutedEventArgs e)
        {
            TAdataGrid.IsReadOnly = false;
            TAdataGrid.ItemsSource = personalBudget.Goals;
        }

        private void GOYearRB_Checked(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Goal> GoFilterDate = new ObservableCollection<Goal>(from goal in personalBudget.Goals
                                                                                           where goal.DateCreateGoal.Year == DateTime.Now.Year
                                                                                           select goal);
            TAdataGrid.ItemsSource = GoFilterDate;
            TAdataGrid.IsReadOnly = true;
        }

        private void GOMounthRB_Checked(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Goal> GoFilterDate = new ObservableCollection<Goal>(from goal in personalBudget.Goals
                                                                                     where goal.DateCreateGoal.Month == DateTime.Now.Month
                                                                                        && goal.DateCreateGoal.Year == DateTime.Now.Year
                                                                                     select goal);
            TAdataGrid.ItemsSource = GoFilterDate;
            TAdataGrid.IsReadOnly = true;
        }

        private void GOWeekRB_Checked(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Goal> GoFilterDate = new ObservableCollection<Goal>(from goal in personalBudget.Goals
                                                                                     where goal.DateCreateGoal.Day >= (DateTime.Now.Day - 3)
                                                                                       && goal.DateCreateGoal.Day <= (DateTime.Now.Day + 4)
                                                                                       && goal.DateCreateGoal.Month == DateTime.Now.Month
                                                                                       && goal.DateCreateGoal.Year == DateTime.Now.Year
                                                                                     select goal);
            TAdataGrid.ItemsSource = GoFilterDate;
            TAdataGrid.IsReadOnly = true;
        }

        private void GOTodayRB_Checked(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Goal> GoFilterDate = new ObservableCollection<Goal>(from goal in personalBudget.Goals
                                                                                     where goal.DateCreateGoal.Day == DateTime.Now.Day
                                                                                            && goal.DateCreateGoal.Month == DateTime.Now.Month
                                                                                            && goal.DateCreateGoal.Year == DateTime.Now.Year
                                                                                     select goal);
            TAdataGrid.ItemsSource = GoFilterDate;
            TAdataGrid.IsReadOnly = true;
        }
    }
}
