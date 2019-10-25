using System;
using System.Collections.Generic;
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
                bool checkItemInDB = false;
                int currentItemQuantity = 0;
                SqlAccessor.Open();

                // Confirm product is in database, and get the current quantity of that item
                using (SqlDataReader reader = SqlAccessor.RunQuery("SELECT * FROM dbo.Products;"))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            if (reader["productName"].ToString().ToLower() == addProductName.Text.ToLower())
                            {
                                checkItemInDB = true;
                                currentItemQuantity = int.Parse(reader["currentQuantity"].ToString());
                            }
                        }
                    }
                }

                if (checkItemInDB == false)
                {
                    // Add new product
                    List<SqlParameter> sqlParameters = new List<SqlParameter>()
                    {
                        new SqlParameter() {ParameterName = "@name", Value = addProductName.Text},
                        new SqlParameter() {ParameterName = "@quantity", Value = addProductQuantity.Text},
                        new SqlParameter() {ParameterName = "@brandID", Value = addProductID.Text}
                    };

                    using (SqlDataReader reader = SqlAccessor.RunQuery("INSERT INTO dbo.Products (productName, currentQuantity, brandID) VALUES (@name, @quantity, @brandID)", sqlParameters))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                if (reader["productName"].ToString().ToLower() == addProductName.Text.ToLower())
                                {
                                    LoadProductsToDataGrid();
                                    edititemname.Items.Add(addProductName.Text);
                                }
                            }
                        }
                    }
                } 
                else
                {
                    // Update existing product
                    List<SqlParameter> sqlParameters = new List<SqlParameter>()
                    {
                        new SqlParameter() {ParameterName = "@id", Value = editProductQuantity.Text},
                        new SqlParameter() {ParameterName = "@curQuan", Value = currentItemQuantity + int.Parse(addProductQuantity.Text)}
                    };

                    using (SqlDataReader reader = SqlAccessor.RunQuery("UPDATE dbo.Products SET currentQuantity = @curQuan WHERE brandID = @id;", sqlParameters))
                    {
                        LoadProductsToDataGrid();
                    }
                }

                SqlAccessor.Close();
            }
        }

        private void Change_Clicked(object sender, RoutedEventArgs e)
        {
            // Update existing product
            List<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@name", Value = edititemname.Text},
                new SqlParameter() {ParameterName = "@curQuan", Value = editProductQuantity.Text}
            };


            SqlAccessor.Open();
            using (SqlDataReader reader = SqlAccessor.RunQuery("UPDATE dbo.Products SET currentQuantity = @curQuan WHERE productName = @name;", sqlParameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        if (reader["productName"].ToString().ToLower() == addProductName.Text.ToLower())
                        {
                            LoadProductsToDataGrid();
                        }
                    }
                }
            }
            SqlAccessor.Close();
        }

        // TODO: Update to reflect our current table
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
            SqlAccessor.Open();
            int i = 0;
            using (SqlDataReader reader = SqlAccessor.RunQuery("SELECT * FROM dbo.Products;"))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        edititemname.Items.Add(reader["productName"].ToString());
                        i++;
                    }
                }
            }
            SqlAccessor.Close();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadProductsToDataGrid();
        }
    }
}
