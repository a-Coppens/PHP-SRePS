using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PHP_SRePS
{
    /// <summary>
    /// Interaction logic for Products.xaml
    /// </summary>
    public partial class Products : UserControl
    {
        // Instance of our database (product table)
        private readonly srepsDatabase data = new srepsDatabase();

        private DataGrid _products;

        public Products()
        {
            InitializeComponent();
            LoadData();
        }

        private void Add_Clicked(object sender, RoutedEventArgs e)
        {
            if ((sender as Button) == additembutton)
            {
                using (SqlConnection connection = new SqlConnection(@"Data Source = 'php-sreps.database.windows.net'; User ID = 'swinAdmin'; Password = '__admin12'; Initial Catalog = 'php-sreps';"))
                {
                    string query = "INSERT INTO dbo.Products (productName, currentQuantity) VALUES (@name, @quantity)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", additemname.Text);
                        command.Parameters.AddWithValue("@quantity", additemquantity.Text);

                        connection.Open();
                        int result = command.ExecuteNonQuery();

                        // Check Error
                        if (result < 0)
                            Console.WriteLine("Error inserting data into Database!");
                    }
                }

            }
        }

        private void LoadData()
        {
            // Defines a query to get all products in 
            // Our product table
            var query =
            from product in data.Products
            orderby product.productID
            select new { product.productID, product.productName, product.currentQuantity };

            // Stores our Query in our grid as a list
            myDataGrid.ItemsSource = query.ToList();
        }
    }
}
