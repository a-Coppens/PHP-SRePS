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
    public partial class Sales : UserControl
    {
        DataGrid _dg;
        private readonly List<InventoryItem> _inventoryItems = new List<InventoryItem>();
        private int _id = 1;

        public Sales()
        {
            InitializeComponent();
            OutputText.Text = "";

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
            //qtextbox = (sender as TextBox);

            //if (String.IsNullOrEmpty(qtextbox.Text))
            //{
            //    salesadditem.IsEnabled= false;
            //}
            //else
            //{
            //    salesadditem.IsEnabled = true;

            //}
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


        }
        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
