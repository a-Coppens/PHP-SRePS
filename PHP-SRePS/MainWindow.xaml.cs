using System;
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
        private readonly List<InventoryItem> _inventoryItems = new List<InventoryItem>();
        private int _id = 1;

        public MainWindow()
        {
            InitializeComponent();

            OutputText.Text = "";

            _dg = new DataGrid();
            InitDataGrid(_dg);

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
            InventoryItem newInvItem = new InventoryItem { ID = "M" + _id, Name = inputTextBoxName.Text, QuantityCurrent = int.Parse(inputTextBoxQuantity.Text) };
            inputTextBoxName.Clear();
            inputTextBoxQuantity.Clear();
            _id++;

            _inventoryItems.Add(newInvItem);
            _dg.Items.Add(newInvItem);
        }

        void InitDataGrid(DataGrid dg)
        {
            MainGrid.Children.Add(dg);
            Grid.SetRow(dg, 4);
            Grid.SetColumn(dg, 3);
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
    }
}
