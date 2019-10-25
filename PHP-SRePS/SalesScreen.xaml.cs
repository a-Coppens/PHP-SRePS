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

        public SalesScreen()
        {
            InitializeComponent();
            InitDataGrid(dataGrid);

            SqlAccessor.Open();

            // Load from database -> combo box
            using (SqlDataReader reader = SqlAccessor.RunQuery("SELECT * FROM dbo.Products;"))
            {
                if (reader != null)
                {
                    int i = 0;
                    while (reader.Read())
                    {
                        itemnamebox.Items.Add(reader["productName"].ToString());
                        i++;
                    }
                }
            }

            SqlAccessor.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InventoryItem newInvItem;
            if ((sender as Button) == salesadditem)
            {
                int currentItemQuantity = 0;
                SqlAccessor.Open();

                // Get the current quantity of a product
                using (SqlDataReader reader = SqlAccessor.RunQuery("SELECT * FROM dbo.Products;"))
                {
                    if (reader != null)
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

                // Reduce the current quantity of the product based on value
                List<SqlParameter> sqlParameters = new List<SqlParameter>()
                {
                    new SqlParameter() {ParameterName = "@curQuan", Value = currentItemQuantity - int.Parse(qtextbox.Text)},
                    new SqlParameter() {ParameterName = "@name", Value = itemnamebox.Text}
                };

                SqlAccessor.RunQuery("UPDATE dbo.Products SET currentQuantity = @curQuan WHERE productName = @name;", sqlParameters);

                SqlAccessor.Close();

                // UI / Storing
                newInvItem = new InventoryItem { Name = itemnamebox.Text, QuantityCurrent = int.Parse(qtextbox.Text) };
                qtextbox.Clear();
                _inventoryItems.Add(newInvItem);
                dataGrid.Items.Add(newInvItem);
            }
        }

        // TODO: Initialize the datagrid columns (header + bindings) in XAML instead of here
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

        private void QtextboxTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            qtextbox = (sender as TextBox);

            if (string.IsNullOrEmpty(qtextbox.Text))
            {
                salesadditem.IsEnabled = false;
            }
            else
            {
                salesadditem.IsEnabled = true;

            }
        }

        private void SaveChanges_Clicked(object sender, RoutedEventArgs e)
        {
            int currentProductID = int.MaxValue;
            SqlAccessor.Open();

            // Match each datagrid item with product table item, and insert product into sales table
            for (int j = 0; j < dataGrid.Items.Count; j++)
            {
                // Match product
                using (SqlDataReader reader = SqlAccessor.RunQuery("SELECT * FROM dbo.Products;"))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            if ((reader["productName"].ToString().ToLower() == _inventoryItems[j].Name.ToLower()))
                            {
                                currentProductID = int.Parse((reader["productID"].ToString()));
                            }
                        }
                    }
                }

                // Add product to sale
                if (currentProductID != int.MaxValue)
                {
                    DateTime t = DateTime.Today;
                    if (saleDatePicker.SelectedDate.HasValue) t = saleDatePicker.SelectedDate.Value;

                    List<SqlParameter> sqlParameters = new List<SqlParameter>()
                    {
                        new SqlParameter() {ParameterName = "@pid", Value = currentProductID},
                        new SqlParameter() {ParameterName = "@date", Value = t},
                        new SqlParameter() {ParameterName = "@quantity", Value = _inventoryItems[j].QuantityCurrent},
                        new SqlParameter() {ParameterName = "@loginid", Value = loginscreen.GetLoginName()}
                    };

                    using (SqlDataReader reader = SqlAccessor.RunQuery("INSERT INTO dbo.Sales (productID, saleDate, salesQuantity, employee) VALUES (@pid, @date, @quantity, @loginid)", sqlParameters))
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                if ((reader["productName"].ToString().ToLower() == _inventoryItems[j].Name.ToLower()))
                                {
                                    currentProductID = int.Parse((reader["productID"].ToString()));
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Error, currentProductID not found in database");
                }
            }

            SqlAccessor.Close();

            // UI / Storing
            _inventoryItems.Clear();
            dataGrid.Items.Clear();
        }
    }
}
