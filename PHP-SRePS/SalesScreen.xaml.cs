using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PHP_SRePS
{
    /// <summary>
    /// Interaction logic for Sales.xaml
    /// </summary>
    /// 
    public partial class SalesScreen : UserControl
    {
        private readonly List<InventoryItem> _inventoryItems = new List<InventoryItem>();

        private srepsDatabase data = new srepsDatabase();

        public SalesScreen()
        {
            InitializeComponent();
            InitDataGrid(dataGrid);

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
                                itemnamebox.Items.Add(reader["productName"].ToString());
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InventoryItem newInvItem;
            if ((sender as Button) == salesadditem)
            {
                using (SqlConnection connection = new SqlConnection(@"Data Source = 'php-sreps.database.windows.net'; User ID = 'swinAdmin'; Password = '__admin12'; Initial Catalog = 'php-sreps';"))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT * FROM dbo.Products;");
                    string sql = sb.ToString();
                    int currentItemQuantity = 0;
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if ((reader["productName"].ToString().ToLower() == itemnamebox.Text.ToLower()))
                                {
                                    currentItemQuantity = int.Parse((reader["currentQuantity"].ToString()));
                                }
                            }
                        }
                    }
                    connection.Close();

                    string query = "UPDATE dbo.Products SET currentQuantity = @curQuan  WHERE productName = @name; ";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", itemnamebox.Text);
                        command.Parameters.AddWithValue("@curQuan", currentItemQuantity - int.Parse(qtextbox.Text));

                        connection.Open();
                        int result = command.ExecuteNonQuery();
                        if (result < 0) Console.WriteLine("Error inserting data into database!");
                        connection.Close();
                    }
                }

                    newInvItem = new InventoryItem { Name = itemnamebox.Text, QuantityCurrent = int.Parse(qtextbox.Text) };
                qtextbox.Clear();

                _inventoryItems.Add(newInvItem);
                dataGrid.Items.Add(newInvItem);
            }


        }

        void InitDataGrid(DataGrid dg)
        {
            DataGridTextColumn textColumnName = new DataGridTextColumn
            {
                Header = "Name",
                Binding = new Binding("Name")
            };

            DataGridTextColumn textColumnQuantity = new DataGridTextColumn
            {
                Header = "# Sold Today",
                Binding = new Binding("QuantityCurrent")
            };

            dg.Columns.Add(textColumnName);
            dg.Columns.Add(textColumnQuantity);
        }

        private void qtextboxTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            qtextbox = (sender as TextBox);

            if (String.IsNullOrEmpty(qtextbox.Text))
            {
                salesadditem.IsEnabled = false;
            }
            else
            {
                salesadditem.IsEnabled = true;

            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


        }
        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SaveChanges_Clicked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                for (int j = 0; j < dataGrid.Items.Count; j++)
                {
                    string text = dataGrid.Columns[i].GetCellContent(dataGrid.Items[j]).ToString();

                    using (SqlConnection connection = new SqlConnection(@"Data Source = 'php-sreps.database.windows.net'; User ID = 'swinAdmin'; Password = '__admin12'; Initial Catalog = 'php-sreps';"))
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("SELECT * FROM dbo.Products;");
                        string sql = sb.ToString();
                        int currentProductID = 0;
                        connection.Open();

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if ((reader["productName"].ToString().ToLower() == dataGrid.Columns[1].GetCellContent(dataGrid.Items[j]).ToString().ToLower()))
                                    {
                                        currentProductID = int.Parse((reader["productID"].ToString()));
                                    }
                                }
                            }
                        }
                        connection.Close();

                        string query = "INSERT INTO dbo.Sales (productID, saleQuantity, employeeID) VALUES (@pid, @quantity, @loginid)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@pid", currentProductID);
                            command.Parameters.AddWithValue("@quantity", dataGrid.Columns[1].GetCellContent(dataGrid.Items[j]).ToString());
                            command.Parameters.AddWithValue("@loginid", loginscreen.GetLoginID());

                            connection.Open();
                            //int result = command.ExecuteNonQuery();

                            //if (result < 0) Console.WriteLine("Error inserting data into Database!");
                        }
                        connection.Close();
                    }
                }
            }
        }
    }
}
