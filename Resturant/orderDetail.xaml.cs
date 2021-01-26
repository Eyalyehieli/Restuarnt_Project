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
    /// Interaction logic for orderDetail.xaml
    /// </summary>
    public partial class orderDetail : Window
    {
        int id;
        static string connectionString = "data source=DESKTOP-CG2IK6Q;initial catalog=Restaurant;user id=sa;password=1234";
        SqlConnection myConnection = new SqlConnection(connectionString);
        public orderDetail(int id)
        {
            InitializeComponent();
            this.id = id;
            order_id_lbl.Content = id.ToString();
            myConnection.Open();
            string query= "SELECT f.food_name,f.food_description,f.food_prep_time,od.notes "+
                         "FROM foods f INNER JOIN orders_description od "+
                         "ON f.food_id = od.food_id "+
                         "WHERE od.order_id = @order_id";
            SqlCommand command = new SqlCommand(query, myConnection);
            command.Parameters.Add("@order_id", SqlDbType.Int).Value = id;
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            orderDetail_data_grid.ItemsSource = dt.DefaultView;
            myConnection.Close();
        }
    }
}
