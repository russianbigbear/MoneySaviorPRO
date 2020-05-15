using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using ClosedXML.Excel;
using System.Linq;

namespace MoneyCalc.MenuForm
{
    public partial class BillForm : Window
    {
        private static string BillPath = Directory.GetCurrentDirectory() + @"\BD\Bill.xlsx";
        MainWindow main;

        MyBills bill;
        int CountBill;


        public class MyBills
        {
            public List<Bill> Bills {get; set;}
            public int count { get; set; }

            public MyBills()
            {
                Bills = new List<Bill>();
                count = 0;

                XLWorkbook workbook = new XLWorkbook(BillPath);
                IXLWorksheet list = workbook.Worksheets.Worksheet(1);

                for (int i = 1; list.Cell("A" + i).Value.ToString() != ""; i++)
                {
                    Bills.Add(new Bill
                    {
                        Name = list.Cell("A" + i).Value.ToString(),
                    });
                    count++;
                }
            }
        }

        public class Bill
        {
            public string Name { get; set; }
        }

        public BillForm(MainWindow mw)
        {
            InitializeComponent();
            bill = new MyBills();
            main = mw;
            BilldataGrid.ItemsSource = bill.Bills;
            CountBill = bill.count;
        }

        private void NoneBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            XLWorkbook workbook = new XLWorkbook(BillPath);
            IXLWorksheet list = workbook.Worksheets.Worksheet(1);

            for (int i = 0; i < CountBill; i++)
            {
                list.Cell("A" + (i + 1)).Value = "";
            }

            for (int i = 0; i < bill.Bills.Count ; i++)
            {
                list.Cell("A" + (i + 1)).Value = bill.Bills[i].Name;
            }

            workbook.Save();

            MessageBox.Show("Изменения сохранены");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            main.UpdateBillCB();
        }
    }
}
