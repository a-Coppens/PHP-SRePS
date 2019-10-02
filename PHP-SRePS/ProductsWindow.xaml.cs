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
        srepsDatabase data = new srepsDatabase();

        public ProductsWindow()
        {
            InitializeComponent();
        }

        private void Add_Clicked(object sender, RoutedEventArgs e)
        {
            if ((sender as Button) == additembutton)
            {
                using (SqlConnection connection = new SqlConnection(@"Data Source = 'php-sreps.database.windows.net'; User ID = 'swinAdmin'; Password = '__admin12'; Initial Catalog = 'php-sreps';"))
                {
                    String query = "INSERT INTO dbo.Products (productName, currentQuantity) VALUES (@name, @quantity)";

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

