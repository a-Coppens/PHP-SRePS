using System;
using System.Data.Entity.Core.Objects;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace PHP_SRePS
{
    /// <summary>
    /// Interaction logic for ProductsWindow.xaml
    /// </summary>
    public partial class ProductsWindow : Window
    {

        private DataGrid _products;
        srepsDatabase data = new srepsDatabase();


        public ProductsWindow()
        {
            InitializeComponent();
            _products = new DataGrid();
            //InitDataGrid(_products);
        }

        private void Close_Clicked(object sender, RoutedEventArgs e)
        {
            Window_Closed();
        }

        private void Apply_Clicked(object sender, RoutedEventArgs e)
        {
            Window_Closed();
        }

        private void Window_Closed()
        {
        
                MainWindow main = new MainWindow();
                App.Current.MainWindow = main;
                this.Close();
                main.Show();
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            

            
            var query =
            from product in data.Products
            orderby product.productID
            select new { product.productID, product.productName, product.currentQuantity};

            myDataGrid.ItemsSource = query.ToList();
        }
    }

    //public void InitDataGrid(DataGrid productGrid)
    //{
    //    ProductGrid.Children.Add(productGrid);
    //    Grid.SetRow(productGrid, 1);
    //    Grid.SetColumn(productGrid, 1);
    //    ProductGrid.Height = 400;

    //    DataGridTextColumn textColumnProductID = new DataGridTextColumn
    //    {
    //        Header = "Product ID",
    //        Binding = new Binding("ProductID")
    //    };

    //    DataGridTextColumn textColumnProductName = new DataGridTextColumn
    //    {
    //        Header = "Product Name",
    //        Binding = new Binding("ProductName")
    //    };

    //    DataGridTextColumn textColumnCurrentQuantity = new DataGridTextColumn
    //    {
    //        Header = "Current Quantity",
    //        Binding = new Binding("CurrentQuantity")
    //    };

    //    productGrid.Columns.Add(textColumnProductID);
    //    productGrid.Columns.Add(textColumnProductName);
    //    productGrid.Columns.Add(textColumnCurrentQuantity);
    //}
}

