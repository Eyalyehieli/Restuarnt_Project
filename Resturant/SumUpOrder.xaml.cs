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
using System.Data;
using System.Data.SqlClient;

namespace Resturant
{
    /// <summary>
    /// Interaction logic for SumUpOrder.xaml
    /// </summary>
    public partial class SumUpOrder : Window
    {
        int prep_time;
        int payment;
        static string connectionString = "data source=DESKTOP-CG2IK6Q;initial catalog=Restaurant;user id=sa;password=1234";
        SqlConnection myConnection = new SqlConnection(connectionString);
        public SumUpOrder(int prep_time,int payment)
        {
            InitializeComponent();
            this.prep_time = prep_time;
            this.payment = payment;
            payment_lbl.Content = payment.ToString();
            time_for_waiting_lbl.Content = prep_time.ToString();

            try
            {
                myConnection.Open();
                string query = "SELECT f.food_name,f.food_description,f.food_price,f.food_prep_time,od.notes " +
                               "FROM foods f INNER JOIN orders_description od " +
                               "ON f.food_id = od.food_id " +
                               "WHERE od.order_id = (SELECT MAX(order_id) " +
                               "FROM orders_description)";
                SqlCommand command = new SqlCommand(query, myConnection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                dataAdapter.Fill(dt);
                OrderDataGrid.ItemsSource = dt.DefaultView;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            myConnection.Close();


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(1);
        }
    }
}
