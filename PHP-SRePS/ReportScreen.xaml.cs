using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PHP_SRePS
{
    /// <summary>
    /// Interaction logic for ReportScreen.xaml
    /// </summary>
    public partial class ReportScreen : UserControl
    {
        private readonly srepsDatabase data = new srepsDatabase();

        public int filename { get; set; }

        public ReportScreen()
        {
            InitializeComponent();
            LoadSalesToDataGrid();
        }

        public void LoadSalesToDataGrid()
        {

            // TODO: There is a UI delay / bug here, the pastSalesRange.Text does not update until it is changed a second time
            int dayRange = -1;
            if (pastSalesRange.Text == "Past week sales")
            {
                Console.WriteLine("-7");
                dayRange = -7;
            }
            else if (pastSalesRange.Text == "Past month sales")
            {
                Console.WriteLine("-28");
                dayRange = -28;
            } 
            else
            {
                Console.WriteLine("-1");
            }

            DateTime backDate = DateTime.Today.AddDays(dayRange);


            // "SELECT productID, SUM(salesQuantity) FROM Sales WHERE saleDate between '@todayDate' and '@targetPreviousDate' GROUP BY productID;"
            var query = data.Sales
            .Where(d => d.saleDate <= DateTime.Today && d.saleDate > backDate)
            .GroupBy(a => a.productID)
            .Select(a => new { TotalSales = a.Sum(b => b.salesQuantity), ProductName = a.Key })
            .OrderByDescending(a => a.TotalSales);

            if (reportDatagrid != null) reportDatagrid.ItemsSource = query.ToList();


        }

        public void SaveChanges_Clicked(object sender, RoutedEventArgs e)
        {
            
                reportDatagrid.SelectAllCells();
                reportDatagrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                ApplicationCommands.Copy.Execute(null, reportDatagrid);
                reportDatagrid.UnselectAllCells();
                String result = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
                String filePath = "D:\\"+addFileName.Text+".csv";
                File.AppendAllText(filePath, result, UnicodeEncoding.UTF8);

                MessageBox.Show("Data Saved in " + filePath);
            

        }

        private void pastSalesRange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadSalesToDataGrid();
        }

        private void CopyDataToCSV_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
