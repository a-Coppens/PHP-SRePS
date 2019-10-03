using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.SqlClient;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace PHP_SRePS
{
    /// <summary>
    /// Interaction logic for ProductsWindow.xaml
    /// Includes Button handling, Data handling
    /// </summary>
    public partial class ProductsWindow : Window
    {
        // Instance of our database (product table)
        private srepsDatabase data = new srepsDatabase();

        private DataGrid _products;

        public ProductsWindow()
        {
            InitializeComponent();
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
                    String sql = sb.ToString();
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

        private void Add_Clicked(object sender, RoutedEventArgs e)
        {
            if ((sender as Button) == additembutton)
            {
                using (SqlConnection connection = new SqlConnection(@"Data Source = 'php-sreps.database.windows.net'; User ID = 'swinAdmin'; Password = '__admin12'; Initial Catalog = 'php-sreps';"))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT * FROM dbo.Products;");
                    String sql = sb.ToString();
                    Boolean checkitemindb = false;
                    int currentitemquantity = 0;
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if ((reader["productName"].ToString()==additemname.Text)){
                                    checkitemindb = true;
                                    currentitemquantity = int.Parse((reader["currentQuantity"].ToString()));
                                }
                                
                            }
                        }
                    }
                    connection.Close();
                    if (checkitemindb == false) {
                        String query = "INSERT INTO dbo.Products (productName, currentQuantity) VALUES (@name, @quantity)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@name", additemname.Text);
                            command.Parameters.AddWithValue("@quantity", additemquantity.Text);

                        connection.Open();
                        int result = command.ExecuteNonQuery();
                        ProductsWindow productWindow = new ProductsWindow();

                            // Check Error
                            if (result < 0)
                                Console.WriteLine("Error inserting data into Database!");
                        }
                    }
                    else
                    {
                        String query = "UPDATE dbo.Products SET currentQuantity = @curQuan  WHERE productName = @name; ";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@name", additemname.Text);
                            command.Parameters.AddWithValue("@curQuan", int.Parse(additemquantity.Text)+currentitemquantity);

                            connection.Open();
                            int result = command.ExecuteNonQuery();

                            // Check Error
                            if (result < 0)
                                Console.WriteLine("Error inserting data into Database!");
                        }
                    }
                    connection.Close();
                }

            }
        }

        private void Close_Clicked(object sender, RoutedEventArgs e)
        {
            Window_Closed();
        }

        private void Apply_Clicked(object sender, RoutedEventArgs e)
        {
            Window_Closed();
        }

        // When apply or closed is clicked, this
        // window gets closed and our main window 
        // is re-opened
        private void Window_Closed()
        {
            MainWindow main = new MainWindow();
            App.Current.MainWindow = main;
            this.Close();
            main.Show();
        }

        // On this window load 
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {       
            // Defines a query to get all products in 
            // Our product table
            var query =
            from product in data.Products
            orderby product.productID
            select new { product.productID, product.productName, product.currentQuantity};

            // Stores our Query in our grid as a list
            myDataGrid.ItemsSource = query.ToList();
        }
    }
}

