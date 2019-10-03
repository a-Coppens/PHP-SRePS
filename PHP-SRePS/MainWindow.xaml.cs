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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private srepsDatabase data = new srepsDatabase();

        DataGrid _dg;
      
        private readonly List<InventoryItem> _inventoryItems = new List<InventoryItem>();
        private int _id = 1;

        public MainWindow()
        {
            InitializeComponent();

            OutputText.Text = "";

            _dg = new DataGrid();
            InitDataGrid(_dg);

            

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

            

            /*
            int[] scores = new int[] { 50, 75, 125, 25, 10, 7 };

            IEnumerable<int> scoreQuery =
                from score in scores
                where score > 30
                select score;

            foreach (int i in scoreQuery)
                OutputText.Text += i + " ";
            */
        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           InventoryItem newInvItem;
           if ((sender as Button) == salesadditem) {


                using (SqlConnection connection = new SqlConnection(@"Data Source = 'php-sreps.database.windows.net'; User ID = 'swinAdmin'; Password = '__admin12'; Initial Catalog = 'php-sreps';"))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT * FROM dbo.Products;");
                    String sql = sb.ToString();
                    int currentitemquantity = 0;
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if ((reader["productName"].ToString().ToLower() == itemnamebox.Text.ToLower()))
                                {
                                    currentitemquantity = int.Parse((reader["currentQuantity"].ToString()));
                                }

                            }
                        }
                    }
                    connection.Close();


                    String query = "UPDATE dbo.Products SET currentQuantity = @curQuan  WHERE productName = @name; ";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                                            
                        command.Parameters.AddWithValue("@name", itemnamebox.Text);
                        command.Parameters.AddWithValue("@curQuan",currentitemquantity-int.Parse(qtextbox.Text) );

                        connection.Open();
                        int result = command.ExecuteNonQuery();
                        
                        // Check Error
                        if (result < 0)
                            Console.WriteLine("Error inserting data into Database!");
                        connection.Close();
                        
                    }
                }

                newInvItem = new InventoryItem { ID = "" + _id, Name = itemnamebox.Text, QuantityCurrent = int.Parse(qtextbox.Text) };
                qtextbox.Clear();
                _id++;

                _inventoryItems.Add(newInvItem);
                _dg.Items.Add(newInvItem);


            }


        }

        void InitDataGrid(DataGrid dg)
        {
            MainGrid.Children.Add(dg);
            Grid.SetRow(dg, 4);
            Grid.SetColumn(dg, 1);
            Grid.SetColumnSpan(dg, 3);
            dg.Height = 250;

            DataGridTextColumn textColumnID = new DataGridTextColumn
            {
                Header = "ID",
                Binding = new Binding("ID")
            };

            DataGridTextColumn textColumnName = new DataGridTextColumn
            {
                Header = "Name",
                Binding = new Binding("Name")
            };

            DataGridTextColumn textColumnQuantity = new DataGridTextColumn
            {
                Header = "Quantity",
                Binding = new Binding("QuantityCurrent")
            };

            dg.Columns.Add(textColumnID);
            dg.Columns.Add(textColumnName);
            dg.Columns.Add(textColumnQuantity);
        }

        private void qtextboxTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void saveChanges_Clicked(object sender, RoutedEventArgs e)
        {
            
            for (int i = 0; i < _dg.Columns.Count; i++)
            {
                for (int j = 0; j < _dg.Items.Count; j++)
                {

                    string text = _dg.Columns[i].GetCellContent(_dg.Items[j]).ToString();

                    using (SqlConnection connection = new SqlConnection(@"Data Source = 'php-sreps.database.windows.net'; User ID = 'swinAdmin'; Password = '__admin12'; Initial Catalog = 'php-sreps';"))
                    {

                        StringBuilder sb = new StringBuilder();
                        sb.Append("SELECT * FROM dbo.Products;");
                        String sql = sb.ToString();
                        int currentproductID = 0;
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if ((reader["productName"].ToString().ToLower() == _dg.Columns[1].GetCellContent(_dg.Items[j]).ToString().ToLower()))
                                    {
                                        currentproductID = int.Parse((reader["productID"].ToString()));
                                    }

                                }
                            }
                        }
                        connection.Close();



                        String query = "INSERT INTO dbo.Sales (productID, saleQuantity, employeeID) VALUES (@pid, @quantity, @loginid)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            loginscreen obj = new loginscreen();

                            command.Parameters.AddWithValue("@pid", currentproductID);
                            command.Parameters.AddWithValue("@quantity", _dg.Columns[2].GetCellContent(_dg.Items[j]).ToString());
                            string logid = obj.getLoginID();
                            command.Parameters.AddWithValue("@loginid", logid  );

                            connection.Open();
                            int result = command.ExecuteNonQuery();
                            

                           

                            // Check Error
                            if (result < 0)
                                Console.WriteLine("Error inserting data into Database!");
                        }
                        connection.Close();
                    }
                }
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            
        }
        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void ProductWindowButtonClick(object sender, RoutedEventArgs e)
        {
            ProductsWindow productWindow = new ProductsWindow();
            App.Current.MainWindow = productWindow;
            this.Close();
            productWindow.Show();
        }
    }
}
