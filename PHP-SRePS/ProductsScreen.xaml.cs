using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace PHP_SRePS
{
    /// <summary>
    /// Interaction logic for Products.xaml
    /// </summary>
    public partial class ProductsScreen : UserControl
    {
        // Instance of our database (product table)
        private readonly srepsDatabase data = new srepsDatabase();

        public ProductsScreen()
        {
            InitializeComponent();
            
            LoadProductsToDataGrid();
            LoadProductsToDropdown();
        }

        private void Add_Clicked(object sender, RoutedEventArgs e)
        {
            if ((sender as Button) == additembutton)
            {
                using (SqlConnection connection = new SqlConnection(@"Data Source = 'php-sreps.database.windows.net'; User ID = 'swinAdmin'; Password = '__admin12'; Initial Catalog = 'php-sreps';"))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT * FROM dbo.Products;");
                    string sql = sb.ToString();

                    bool checkItemInDB = false;
                    int currentItemQuantity = 0;
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader["productName"].ToString().ToLower() == addProductName.Text.ToLower())
                                {
                                    checkItemInDB = true;
                                    currentItemQuantity = currentItemQuantity = int.Parse(reader["currentQuantity"].ToString());
                                }
                            }
                        }
                    }
                    connection.Close();

                    if (checkItemInDB == false)
                    {
                        string query = "INSERT INTO dbo.Products (productName, currentQuantity, brandID) VALUES (@name, @quantity, @brandID)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@brandID", addProductID.Text);
                            command.Parameters.AddWithValue("@name", addProductName.Text);
                            command.Parameters.AddWithValue("@quantity", addProductQuantity.Text);
                            connection.Open();
                            int result = command.ExecuteNonQuery();
                            LoadProductsToDataGrid();
                            edititemname.Items.Add(addProductName.Text);
                            // Check error
                            if (result < 0) Console.WriteLine("Error inserting data into database!");
                        }
                    }
                    else
                    {
                        string query = "UPDATE dbo.Products SET currentQuantity = @curQuan  WHERE brandID = @id; ";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id", editProductQuantity.Text);
                            command.Parameters.AddWithValue("@curQuan", int.Parse(addProductQuantity.Text) + currentItemQuantity);
                            connection.Open();
                            int result = command.ExecuteNonQuery();
                            LoadProductsToDataGrid();
                            if (result < 0) Console.WriteLine("Error inserting data into database!");
                        }
                    }
                    connection.Close();
                }

            }
        }

        private void Change_Clicked(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(@"Data Source = 'php-sreps.database.windows.net'; User ID = 'swinAdmin'; Password = '__admin12'; Initial Catalog = 'php-sreps';"))
            {
                string query = "UPDATE dbo.Products SET currentQuantity = @curQuan  WHERE productName = @name; ";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", edititemname.Text);
                    command.Parameters.AddWithValue("@curQuan", editProductQuantity.Text);

                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    LoadProductsToDataGrid();
                    // Check Error
                    if (result < 0)
                        Console.WriteLine("Error inserting data into Database!");
                    connection.Close();
                }
            }

        }

        public void LoadProductsToDataGrid()
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

        private void LoadProductsToDropdown()
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "php-sreps.database.windows.net";
                builder.UserID = "swinAdmin";
                builder.Password = "__admin12";
                builder.InitialCatalog = "php-sreps";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT * FROM dbo.Products;");
                    string sql = sb.ToString();
                    int i = 0;
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                edititemname.Items.Add(reader["productName"].ToString());
                                i++;
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadProductsToDataGrid();
        }
    }
}
