using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using ClosedXML.Excel;

namespace MoneyCalc.MenuForm
{
    public partial class Exspense : Window
    {
        private static string OutPath = Directory.GetCurrentDirectory() + @"\BD\CatOut.xlsx";
        MainWindow main;

        MyCatOut myCatout;
        int CountCat;

        public class MyCatOut
        {
            public List<CatOut> CatOuts { get; set; }
            public int count { get; set; }

            public MyCatOut()
            {
                CatOuts = new List<CatOut>();
                count = 0;

                XLWorkbook workbook = new XLWorkbook(OutPath);
                IXLWorksheet list = workbook.Worksheets.Worksheet(1);

                for (int i = 1; list.Cell("A" + i).Value.ToString() != ""; i++)
                {
                    CatOuts.Add(new CatOut
                    {
                        Name = list.Cell("A" + i).Value.ToString(),
                    });
                    count++;
                }
            }
        }

        public class CatOut
        {
            public string Name { get; set; }
        }

        public Exspense(MainWindow mw)
        {
            InitializeComponent();
            myCatout = new MyCatOut();
            main = mw;
            ExspensedataGrid.ItemsSource = myCatout.CatOuts;
            CountCat = myCatout.count;
        }

        private void NoneBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            XLWorkbook workbook = new XLWorkbook(OutPath);
            IXLWorksheet list = workbook.Worksheets.Worksheet(1);

            for (int i = 0; i < CountCat; i++)
            {
                list.Cell("A" + (i + 1)).Value = "";
            }

            for (int i = 0; i < myCatout.CatOuts.Count; i++)
            {
                list.Cell("A" + (i + 1)).Value = myCatout.CatOuts[i].Name;
            }

            workbook.Save();

            MessageBox.Show("Изменения сохранены");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            main.UpdateOutCB();
        }
    }
}
