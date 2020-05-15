using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using ClosedXML.Excel;
using System.Linq;


namespace MoneyCalc
{
    public partial class ValueForm : Window
    {
        private static string MoneyPath = Directory.GetCurrentDirectory() + @"\BD\Money.xlsx";
        Money money;
        MainWindow main;

        public class Money
        {
            public List<Value> Values {get; set;}

            public Money()
            {
                Values = new List<Value>();
                XLWorkbook workbook = new XLWorkbook(MoneyPath);
                IXLWorksheet list = workbook.Worksheets.Worksheet(1);

                for (int i = 1; list.Cell("A" + i).Value.ToString() != ""; i++)
                {
                    Values.Add(new Value
                    {
                        Name = list.Cell("A" + i).Value.ToString(),
                        ID = list.Cell("B" + i).Value.ToString(),
                        Kurs = list.Cell("C" + i).Value.ToString(),
                    });
                }
            }
        }

        public class Value
        {
            public string Name { get; set; }
            public string ID { get; set; }
            public string Kurs { get; set; }
        }

        public ValueForm(MainWindow mw)
        {
            InitializeComponent();

            money = new Money();
            IncomedataGrid.ItemsSource = money.Values;
            main = mw;
        }

        private void NoneBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public Dictionary<string, string> ConvertToValue(string Cur, string New, string Sum)
        {
            Dictionary<string, string> dic= new Dictionary<string, string>();
            dic.Add("RUB", "0");

            if (Cur == "RUB")
            {
                for(int i = 0; i < money.Values.Count; i++)
                {
                    if(money.Values[i].ID == New)
                    {
                        dic.Remove(dic.Last().Key);
                        dic.Add(New, (Convert.ToDouble(Sum) / Convert.ToDouble(money.Values[i].Kurs)).ToString());
                        return dic;
                    }
                }
            }
            else if (New == "RUB")
            {
                for (int i = 0; i < money.Values.Count; i++)
                {
                    if (money.Values[i].ID == Cur)
                    {
                        dic.Remove(dic.Last().Key);
                        dic.Add(New, (Convert.ToDouble(Sum) * Convert.ToDouble(money.Values[i].Kurs)).ToString());
                        return dic;
                    }
                }
            }
            else
            {
                for (int i = 0; i < money.Values.Count; i++)
                {
                    if (money.Values[i].ID == New)
                    {
                        double SumInRub = 0;

                        for (int j = 0; j < money.Values.Count; j++)
                            if (money.Values[j].ID == Cur)
                                SumInRub = Convert.ToDouble(Sum) * Convert.ToDouble(money.Values[j].Kurs);

                        dic.Remove(dic.Last().Key);
                        dic.Add(New, (SumInRub / Convert.ToDouble(money.Values[i].Kurs)).ToString());
                        return dic;
                    }
                }
            }

            return dic;
            
        }

        public string ValueGet(string Val)
        {
            for (int i = 0; i < money.Values.Count; i++)
            {
                if (money.Values[i].ID == Val)
                    return money.Values[i].Name;
            }

            return "";
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> dic = ConvertToValue(main.personalBudget.PersonalCash.ValueID,
                money.Values[IncomedataGrid.SelectedIndex].ID,
                main.personalBudget.PersonalCash.Sum);

            main.personalBudget.PersonalCash.ValueID = dic.Last().Key;
            main.personalBudget.PersonalCash.Sum = dic.Last().Value;

            main.BudgetShowL.Content = Math.Round(Convert.ToDouble(dic.Last().Value), 2).ToString();
            main.ValueShowL.Content = ValueGet(dic.Last().Key);

            this.Close();
        }
    }
}
