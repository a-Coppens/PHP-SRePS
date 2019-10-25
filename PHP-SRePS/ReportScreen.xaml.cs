using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Interaction logic for ReportScreen.xaml
    /// </summary>
    public partial class ReportScreen : UserControl
    {
        public ReportScreen()
        {
            InitializeComponent();
            LoadSalesToDataGrid();
        }

        public void LoadSalesToDataGrid()
        {
            int dayRange = 0;
            if (pastSalesRange.SelectedItem.ToString() == "Past week sales")
            {
                dayRange = -7;
            }
            else if (pastSalesRange.SelectedItem.ToString() == "Past month sales")
            {
                dayRange = -28;
            }

            List<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@todayDate", Value = DateTime.Today.ToString()},
                new SqlParameter() {ParameterName = "@targetPreviousDate", Value = DateTime.Today.AddDays(dayRange).ToString()}
            };

            SqlAccessor.Open();
            using (SqlDataReader reader = SqlAccessor.RunQuery("SELECT productID, SUM(salesQuantity) FROM Sales WHERE saleDate between '@todayDate' and '@targetPreviousDate' GROUP BY productID;", sqlParameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        // TODO: Needs to set datagrid
                    }
                }
            }
            SqlAccessor.Close();
        }
    }
}
