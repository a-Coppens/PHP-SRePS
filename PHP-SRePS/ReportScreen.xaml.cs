using System;
using System.Linq;
using System.Windows.Controls;

namespace PHP_SRePS
{
    /// <summary>
    /// Interaction logic for ReportScreen.xaml
    /// </summary>
    public partial class ReportScreen : UserControl
    {
        private readonly srepsDatabase data = new srepsDatabase();

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

        private void pastSalesRange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadSalesToDataGrid();
        }
    }
}
