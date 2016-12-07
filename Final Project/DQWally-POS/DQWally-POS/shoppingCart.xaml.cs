/*
* ProjectName:  shoppingCart.xaml.cs
* Programer:    Dong Qian (6573448)
* Date:         Dec 4 16, 2016
* Description:  This is a simple POS shopping cart window
*/


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
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace DQWally_POS
{
    /// <summary>
    /// Interaction logic for shoppingCart.xaml
    /// </summary>
    public partial class shoppingCart : Window
    {
        public shoppingCart(List<string> cart)
        {
            InitializeComponent();
            DataTable dt = new DataTable("ShoppingCart");
            DataRow row;
            // Adding a data table to DataGrid
            dt.Columns.Add("Product Name", typeof(string));
            dt.Columns.Add("Quantity", typeof(string));
            dt.Columns.Add("Branch", typeof(string));
            int i = 0;
            while (i < cart.Count)
            {
                row = dt.NewRow();
                row["Product Name"] = cart[i];
                row["Quantity"] = cart[++i];
                row["Branch"] = cart[++i];
                dt.Rows.Add(row);
                i++;
            }
            DataView view = new DataView(dt);
            // Set a DataGrid control's DataSource to the DataView.
            dataGrid.ItemsSource = view;
        }



        /// <summary>
        /// Exist the window
        /// </summary>
        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
