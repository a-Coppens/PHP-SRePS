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
using System.Windows.Shapes;
using System.Data.SqlClient;
 
namespace PHP_SRePS
{
    /// <summary>
    /// Interaction logic for loginscreen.xaml
    /// </summary>
    public partial class loginscreen : Window
    {
        static string loginid = "";
        public loginscreen()
        {
            InitializeComponent();
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            loginid = txtID.Text;
            SqlConnection sqlConn = new SqlConnection(@"Data Source = 'php-sreps.database.windows.net'; User ID = 'swinAdmin'; Password = '__admin12'; Initial Catalog = 'php-sreps';");
            try
            {
                if (sqlConn.State == System.Data.ConnectionState.Closed)
                    sqlConn.Open();
                string query = "SELECT COUNT(1) FROM loginInfo WHERE ClientName=@ClientName AND Password=@Password";
                SqlCommand sqlCmd = new SqlCommand(query, sqlConn)
                {
                    CommandType = System.Data.CommandType.Text
                };
                sqlCmd.Parameters.AddWithValue("@ClientName", txtID.Text);
                sqlCmd.Parameters.AddWithValue("@Password", txtpassword.Password);
                int count = Convert.ToInt32(sqlCmd.ExecuteScalar());
                if(count==1)
                {
                    MainWindow dashboard = new MainWindow();
                    dashboard.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("Username or Password is incorrect.");
                }
            }       
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static string GetLoginID()
        {
            return loginid;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
