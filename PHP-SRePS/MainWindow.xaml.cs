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
        DataGrid _dg;
        String drpdwntext="";
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
            qtextbox = (sender as TextBox);

            if (String.IsNullOrEmpty(qtextbox.Text))
            {
                salesadditem.IsEnabled= false;
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

        private void ProductWindowButtonClick(object sender, RoutedEventArgs e)
        {
            ProductsWindow productWindow = new ProductsWindow();
            App.Current.MainWindow = productWindow;
            this.Close();
            productWindow.Show();
        }
    }
}
