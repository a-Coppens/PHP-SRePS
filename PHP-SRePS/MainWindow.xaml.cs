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
        DataGrid dg;
        private List<InventoryItem> _inventoryItems = new List<InventoryItem>();

        public MainWindow()
        {
            InitializeComponent();

            OutputText.Text = "";

            dg = new DataGrid();
            MainGrid.Children.Add(dg);
            Grid.SetRow(dg, 4);
            Grid.SetColumn(dg, 3);
            dg.Height = 250;

            for (int i = 1; i <= 4; ++i)
            {
                var column = new DataGridTextColumn
                {
                    Header = "Column" + i,
                    Binding = new Binding("Column" + i)
                };
                dg.Columns.Add(column);
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

        readonly List<string> inputs = new List<string>();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string input = inputTextBox.Text;
            inputs.Add(input);
            InventoryItem newInvItem = new InventoryItem { ID = "M0023", Name = "ProductExample1 20x capsules", QuantityCurrent = 12 };
            inputTextBox.Clear();

            _inventoryItems.Add(newInvItem);

            var t = from invItem in _inventoryItems
                    select invItem.Name;

            //dg.CurrentCell
            dg.Items.Add("www");
        }
    }
}
