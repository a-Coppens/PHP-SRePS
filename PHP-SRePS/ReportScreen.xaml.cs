using System;
using System.Data.SqlClient;
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
            storeItemsInComboBox();
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
            String filePath = "D:\\" + addFileName.Text + ".csv";
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

        public void Tester()
        {
            DateTime month = new DateTime();

            month = data.Sales.Select(d => d.saleDate).SingleOrDefault();
        }

        private void storeItemsInComboBox()
        {
            //TODO: Weird results
            //var itemNameQuery =
            //from product in data.Products
            //select new {product.productName};

            //itemName.ItemsSource = itemNameQuery.ToList();


            //Copied from Products page as above query was not working as intended
            SqlAccessor.Open();
            int i = 0;
            using (SqlDataReader reader = SqlAccessor.RunQuery("SELECT DISTINCT productID FROM dbo.Sales ORDER BY(productID);"))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        itemName.Items.Add(reader["productID"]);
                        i++;
                    }
                }
            }
            SqlAccessor.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IQueryable query = null;
            int x = int.Parse(itemName.SelectedItem.ToString());

            //Monthly
            if (requestedPeriod.Text == "Monthly Forecast")
            {

                query =
                from s in data.Sales
                where s.productID == x
                group s by s.productID into sales
                select new
                {
                    itemID = sales.Key,
                    sales = sales.Sum(a => a.salesQuantity)
                };
            }

            //Weekly
            else if (requestedPeriod.Text == "Weekly Forecast")
            {

            }

            //Output
            foreach (var row in query)
            {
                forecastDescriptor.Text = "Monthly Sales Forecast for:" + row.ToString();
            }

            //foreach (var row in query)
            //{
            //    MessageBox.Show(row.ToString());
            //}

            int productsSold;

        }
    }
}

