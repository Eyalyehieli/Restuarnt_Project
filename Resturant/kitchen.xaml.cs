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
using System.Data;

namespace Resturant
{
    /// <summary>
    /// Interaction logic for kitchen.xaml
    /// </summary>
    public partial class kitchen : Window
    {
        static string connectionString = "data source=DESKTOP-CG2IK6Q;initial catalog=Restaurant;user id=sa;password=1234";
        SqlConnection myConnection = new SqlConnection(connectionString);
        public kitchen()
        {
            InitializeComponent();
            myConnection.Open();
            string query = "SELECT o.*,k.is_ready "+
                           "FROM orders o INNER JOIN kitchen k "+
                           "ON o.order_id = k.order_id "+
                           "WHERE k.is_ready <> 1";
            SqlCommand command = new SqlCommand(query, myConnection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            kitchen_data_grid.ItemsSource = dt.DefaultView;
            myConnection.Close();
        }

        private void kitchen_data_grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            DataGrid kitchen_data_grid = sender as DataGrid;
            if(kitchen_data_grid.SelectedIndex!=-1)
            {
                DataGridColumn d = kitchen_data_grid.CurrentColumn;
                int columnIndex = d.DisplayIndex;
                
                if (columnIndex == 2)
                {
                    DataRowView rowView = kitchen_data_grid.SelectedItem as DataRowView;
                    rowView[2] = 1;
                    int order_id_is_ready = (int)rowView[0];
                    string query = "UPDATE kitchen SET is_ready=1 WHERE order_id=@order_id1";
                    myConnection.Open();
                    SqlCommand command = new SqlCommand(query, myConnection);
                    command.Parameters.Add("@order_id1", SqlDbType.Int).Value = order_id_is_ready;
                    command.ExecuteNonQuery();
                    myConnection.Close();
                }
                else
                {
                    DataRowView rowView = kitchen_data_grid.SelectedItem as DataRowView;
                    int id = (int)rowView.Row.ItemArray[0];
                    orderDetail order_detail = new orderDetail(id);
                    this.Hide();
                    order_detail.ShowDialog();
                    this.Show();
                }
            }
        }

    }
}
