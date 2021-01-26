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
    /// Interaction logic for MenuWin.xaml
    /// </summary>
    public partial class MenuWin : Window
    {
        static string connectionString = "data source=DESKTOP-CG2IK6Q;initial catalog=Restaurant;user id=sa;password=1234";
        int order_id;
        int max_prep_time = 0;
        int payment = 0;
        SqlConnection myConnection = new SqlConnection(connectionString);
        public MenuWin()
        {

            InitializeComponent();
            MessageBox.Show("Select from each category the number of portions you want, to select tap on the portion");
            myConnection.Open();
            try
            {
                string sqlDataPull = "SELECT categories.category_name FROM categories";
                SqlCommand command = new SqlCommand(sqlDataPull, myConnection);
                command.CommandType = System.Data.CommandType.Text;
                SqlDataReader dr1 = command.ExecuteReader();
                while (dr1.Read())
                {
                    sqlDataPull = dr1[0].ToString();
                    categories_combo_box.Items.Add(sqlDataPull);
                }
                dr1.Close();

                sqlDataPull = "INSERT INTO orders VALUES(0)";
                command = new SqlCommand(sqlDataPull, myConnection);
                command.CommandType = System.Data.CommandType.Text;
                command.ExecuteNonQuery();
                sqlDataPull = "SELECT MAX(o.order_id) FROM orders o";
                command = new SqlCommand(sqlDataPull, myConnection);
                command.CommandType = System.Data.CommandType.Text;
                order_id = (int)command.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            myConnection.Close();
        }


        private void categories_combo_box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            string selectedCategory = categories_combo_box.SelectedItem.ToString();
            myConnection.Open();
            try
            {
                string query = "SELECT f.food_id,f.food_name,f.food_description,f.food_price,f.food_prep_time " +
                               "FROM foods f INNER JOIN categories c " +
                               "ON f.category_id = c.category_id " +
                               "WHERE c.category_name = @selected_categorie_name";
                SqlCommand command = new SqlCommand(query, myConnection);
                command.Parameters.Add("@selected_categorie_name", System.Data.SqlDbType.VarChar, 50);
                command.Parameters["@selected_categorie_name"].Value = selectedCategory;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                dataAdapter.Fill(dt);
                categoriesDataGrid.ItemsSource = dt.DefaultView;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            myConnection.Close();
        }

        private void categoriesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int prep_time;
            DataGrid data_grid = sender as DataGrid;
            if (data_grid.SelectedIndex != -1)
            {
                DataRowView rowview = data_grid.SelectedItem as DataRowView;
                DataGridRow row = (DataGridRow)data_grid.ItemContainerGenerator.ContainerFromIndex(data_grid.SelectedIndex);
                DataGridCell RowColumn = data_grid.Columns[0].GetCellContent(row).Parent as DataGridCell;
                string cellValue = ((TextBlock)RowColumn.Content).Text;
                row.Visibility = Visibility.Hidden;
                //data_grid.Items.Remove(data_grid.SelectedItem);
                myConnection.Open();
                string query = "INSERT INTO orders_description VALUES(@food_id,@order_id,@notes)";
                SqlCommand command = new SqlCommand(query, myConnection);
                command.Parameters.Add("@food_id", System.Data.SqlDbType.Int).Value = (int)rowview.Row[0];
                command.Parameters.Add("@order_id", System.Data.SqlDbType.Int).Value = order_id;
                command.Parameters.Add("@notes", System.Data.SqlDbType.VarChar, 100).Value = "Do it quickly";
                command.ExecuteNonQuery();
                myConnection.Close();
                DataGridCell rowColumn = data_grid.Columns[1].GetCellContent(row).Parent as DataGridCell;
                string cellvalueName = ((TextBlock)rowColumn.Content).Text;
                MessageBox.Show(cellvalueName + " has been selected");
                DataGridCell rowColumnPayment = data_grid.Columns[3].GetCellContent(row).Parent as DataGridCell;
                payment += Int32.Parse(((TextBlock)rowColumnPayment.Content).Text);
                DataGridCell rowColumnPrepTime = data_grid.Columns[4].GetCellContent(row).Parent as DataGridCell;
                prep_time = Int32.Parse(((TextBlock)rowColumnPrepTime.Content).Text);
                if (prep_time > max_prep_time) { max_prep_time = prep_time; }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            myConnection.Open();
            string query = "UPDATE orders SET order_prep_time = @prep_time WHERE order_id = (SELECT MAX(order_id) FROM orders)";
            SqlCommand command = new SqlCommand(query,myConnection);
            command.Parameters.Add("@prep_time", System.Data.SqlDbType.Int).Value = max_prep_time;
            command.ExecuteNonQuery();
            query = "INSERT INTO kitchen Values(@orderId1,0)";
            command = new SqlCommand(query, myConnection);
            command.Parameters.Add("@orderId1", SqlDbType.Int).Value = order_id;
            command.ExecuteNonQuery();
            myConnection.Close();
            SumUpOrder sumUpOrder = new SumUpOrder(max_prep_time,payment);
            this.Hide();
            sumUpOrder.ShowDialog();
            this.Show();
        }
    }
}
