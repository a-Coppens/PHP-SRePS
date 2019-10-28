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

            DateTime backDate = DateTime.Today;
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

            backDate = DateTime.Today.AddDays(dayRange);


            // "SELECT productID, SUM(salesQuantity) FROM Sales WHERE saleDate between '@todayDate' and '@targetPreviousDate' GROUP BY productID;"
            var query = data.Sales
            .Where(d => d.saleDate <= DateTime.Today && d.saleDate > backDate)
            .GroupBy(a => a.productID)
            .Select(a => new { TotalSales = a.Sum(b => b.salesQuantity), ProductID = a.Key })
            .OrderByDescending(a => a.TotalSales);

         
            if (reportDatagrid != null)
                reportDatagrid.ItemsSource = query.ToList();

            
            


        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadSalesToDataGrid();
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
            if(reportDatagrid!=null)
                reportDatagrid.Items.Refresh();
            LoadSalesToDataGrid();
           
        }

        private void CopyDataToCSV_Click(object sender, System.Windows.RoutedEventArgs e)
        {

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
            int total = 0;
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

                // Output Monthly
                foreach (var row in query)
                {
                    forecastDescriptor.Text = "Monthly Sales Forecast for:" + row.ToString();
                }
            }

            //Weekly
            else if (requestedPeriod.Text == "Weekly Forecast")
            {
                SqlAccessor.Open();
                int i = 0;

                using (SqlDataReader reader = SqlAccessor.RunQuery("SELECT productID, DATEADD(week, DATEDIFF(week, 0, saleDate), 0) AS WeekStart, SUM(salesQuantity) as WeeklySales FROM Sales WHERE productID =" + x + "GROUP BY DATEADD(week, DATEDIFF(week, 0, saleDate), 0), productID;"))

                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {

                            //pass the reader variables to a string
                            string readerToString = "ID: " + reader["productID"].ToString() + " DateTime: " + Convert.ToString(reader["WeekStart"]) + " Sales: " + reader["WeeklySales"].ToString();
                            //calculate total
                            total = reader.GetInt32(2) + total;
                            // forecastDescriptor.Text = read;
                            i++;

                        }
                    }
                }
                // i being the number of entries in reader
                int average = total / i;

                SqlAccessor.Close();

                // Output Weekly
                forecastDescriptor.Text = "Weekly Sales Prediction for product " + x + " is " + average;

            }


        }
    }
}

