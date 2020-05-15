using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using ClosedXML.Excel;

namespace MoneyCalc.MenuForm 
{ 
    public partial class IncomeForm : Window
    {
        private static string InPath = Directory.GetCurrentDirectory() + @"\BD\CatIn.xlsx";
        MainWindow main;

        MyCatIn myCatin;
        int CountCat;

        public class MyCatIn
        {
            public List<CatIn> CatIns { get; set; }
            public int count { get; set; }

            public MyCatIn()
            {
                CatIns = new List<CatIn>();
                count = 0;

                XLWorkbook workbook = new XLWorkbook(InPath);
                IXLWorksheet list = workbook.Worksheets.Worksheet(1);

                for (int i = 1; list.Cell("A" + i).Value.ToString() != ""; i++)
                {
                    CatIns.Add(new CatIn
                    {
                        Name = list.Cell("A" + i).Value.ToString(),
                    });
                    count++;
                }
            }
        }

        public class CatIn
        {
            public string Name { get; set; }
        }

        public IncomeForm(MainWindow mw)
        {
            InitializeComponent();
            myCatin = new MyCatIn();
            main = mw;
            IncomedataGrid.ItemsSource = myCatin.CatIns;
            CountCat = myCatin.count;
        }

        private void NoneBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            XLWorkbook workbook = new XLWorkbook(InPath);
            IXLWorksheet list = workbook.Worksheets.Worksheet(1);

            for (int i = 0; i < CountCat; i++)
            {
                list.Cell("A" + (i + 1)).Value = "";
            }

            for (int i = 0; i < myCatin.CatIns.Count; i++)
            {
                list.Cell("A" + (i + 1)).Value = myCatin.CatIns[i].Name;
            }

            workbook.Save();

            MessageBox.Show("Изменения сохранены");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            main.UpdateInCB();
        }
    }
}
