using System.Windows;


namespace PHP_SRePS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TabItem_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
            
        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (productTab.IsSelected) {
                var grid = new ProductsScreen();
                // Our grid should refresh after this, 
                // although in normal circumstances it would by default regardless.
                grid.InvalidateVisual();
            }
            if (salesRecordTab.IsSelected) {
                var grid = new SalesScreen();
                // Our grid should refresh after this, 
                // although in normal circumstances it would by default regardless.
                grid.InvalidateVisual();
            }

        }
    }
}
