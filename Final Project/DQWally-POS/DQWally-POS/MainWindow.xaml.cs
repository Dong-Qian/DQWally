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
using System.Drawing;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace DQWally_POS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MySqlConnection conn;
        MySqlDataAdapter da;
        DataSet ds;
        List<shopCart> orderLine = new List<shopCart>();
        struct shopCart
        {
            public int customerID;
            public int branchID;
            public string productName;
            public string branchName;
            public int productID;
            public int qty;
        }


        public MainWindow()
        {
            InitializeComponent();
            Product_cb.Items.Add("Disco Queen Wallpaper (roll)");
            Product_cb.Items.Add("Countryside Wallpaper (roll)");
            Product_cb.Items.Add("Victorian Lace Wallpaper (roll)");
            Product_cb.Items.Add("Drywall Tape (roll)");
            Product_cb.Items.Add("Drywall Tape (pkg 10)");
            Product_cb.Items.Add("Drywall Repair Compound (tube)");

            Branch_cb.Items.Add("Sporst World");
            Branch_cb.Items.Add("Cambridge Mall");
            Branch_cb.Items.Add("St.jacobs");

            Status_cb.Items.Add("PAID");
            Status_cb.Items.Add("PEND");
            Status_cb.Items.Add("CNCL");

        }

        private void Price_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Conntect();
            MessageBoxResult error;
            if (!string.IsNullOrEmpty(Product_cb.Text))
            {
                int Qty;
                if(Int32.TryParse(Qty_tb.Text, out Qty))
                {
                    Pos pos = new Pos();
                    if (Qty>=0)
                    {                      
                        //int inventory = Int32.Parse(pos.FindProductQuantity(Product_cb.SelectedIndex));
                        string price = pos.FindProductPrice(Product_cb.SelectedIndex + 1);
                        decimal showPrice = decimal.Parse(price) * Qty;
                        Money_bk.Text = "$ " + showPrice.ToString();

                        da = new MySqlDataAdapter("select * from product where productID =" + (Product_cb.SelectedIndex + 1) + ";", conn);
                        ds = new DataSet();
                        da.Fill(ds);
                        dataGrid.ItemsSource = ds.Tables[0].DefaultView;
                                            
                    }
                    else
                    {
                       error = MessageBox.Show("Qty can not be less than 1");
                    }
                   
                }
                else
                {
                   error = MessageBox.Show("Qty can not be Empty");
                }
            }
            else
            {
               error = MessageBox.Show("You have to choose a Product");
            }
            conn.Close();
        }

        private void Find_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Conntect();
            string lName = LName_tb.Text;
            string pArea = pArea_tb.Text;
            string pNum1 = pNum1_tb.Text;
            string pNum2 = pNum2_tb.Text;
            MessageBoxResult error;
            List<string> customer = new List<string>();
            long phone = 0;
            Pos pos = new Pos();
            string ret = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(pArea) && !string.IsNullOrEmpty(pNum1) && !string.IsNullOrEmpty(pNum2))
                {
                    if (pArea.Length == 3 && pNum1.Length == 3 && pNum2.Length == 4)
                    {
                        string pNumber = pArea + pNum1 + pNum2;
                        if (long.TryParse(pNumber.ToString(), out phone))
                        {
                            pNumber = pArea + "-" + pNum1 + "-" + pNum2;
                            customer = pos.FindCustomerByPhone(pNumber);
                            CusID_tb.Text = customer[0];
                            FName_tb.Text = customer[1];
                            LName_tb.Text = customer[2];
                            string[] result = customer[3].Split('-');
                            pArea_tb.Text = result[0];
                            pNum1_tb.Text = result[1];
                            pNum2_tb.Text = result[2];
                        }
                        else
                        {
                           error = MessageBox.Show("Phone Number is not Numeric");
                        }
                    }
                    else
                    {
                        error = MessageBox.Show("Phone Number Usage Example:\n   519 111 1111");
                    }

                }
                else
                {
                    error = MessageBox.Show("Need Phone Number to find the Customer\nPhone Number Usage Example:\n   519 111 1111");
                }
            }
            catch(Exception ex)
            {
                error = MessageBox.Show("Error:\n\t" + ex.Message);
            }

            conn.Close();
        }

        private void Add_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult error;
            try
            {
                List<string> customer = new List<string>();
                Pos pos = new Pos();
                string ret = string.Empty;
                string pArea = pArea_tb.Text;
                string pNum1 = pNum1_tb.Text;
                string pNum2 = pNum2_tb.Text;
                long phone = 0;
                if (!string.IsNullOrEmpty(FName_tb.Text) && !string.IsNullOrEmpty(LName_tb.Text) &&
                    !string.IsNullOrEmpty(pArea_tb.Text) && !string.IsNullOrEmpty(pNum1_tb.Text) &&
                     !string.IsNullOrEmpty(pNum2_tb.Text))

                {
                    if (pArea.Length == 3 && pNum1.Length == 3 && pNum2.Length == 4)
                    {
                        string pNumber = pArea + pNum1 + pNum2;
                        if (long.TryParse(pNumber.ToString(), out phone))
                        {
                            pNumber = pArea + "-" + pNum1 + "-" + pNum2;
                            ret = pos.AddCustomer(FName_tb.Text, LName_tb.Text, pNumber);
                            error = MessageBox.Show(ret);
                            customer = pos.FindCustomerByPhone(pNumber);
                            CusID_tb.Text = customer[0];
                            FName_tb.Text = customer[1];
                            LName_tb.Text = customer[2];
                            string[] result = customer[3].Split('-');
                            pArea_tb.Text = result[0];
                            pNum1_tb.Text = result[1];
                            pNum2_tb.Text = result[2];
                        }
                        else
                        {
                            error = MessageBox.Show("Phone Number is not Numeric");
                        }
                    }
                    else
                    {
                        error = MessageBox.Show("Phone Number Usage Example:\n\t519 111 1111");
                    }
                }
                else
                {
                    error = MessageBox.Show("Please fill in Last Name, First Name, Phone Number");
                }
            }
            catch(Exception ex)
            {
                error = MessageBox.Show("Error:\n\t" + ex.Message);
            }
         }

        public string Conntect()
        {
            string ret = string.Empty;
            string connStr = "server=127.0.0.1;user=superuser;database=dqwally;port=3306;password=Conestoga1;";
            conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                ret = "Connection Success";
            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }

            return ret;
        }

        private void Cart_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult error;
            shopCart shopCart = new shopCart();
            if (!string.IsNullOrEmpty(Product_cb.Text) && !string.IsNullOrEmpty(CusID_tb.Text) &&
               !string.IsNullOrEmpty(Qty_tb.Text) && !string.IsNullOrEmpty(Branch_cb.Text))
            {
                shopCart.productID = Product_cb.SelectedIndex + 1;
                int Qty;
                if (Int32.TryParse(Qty_tb.Text, out Qty))
                {
                    for (int i = 0; i < orderLine.Count; i++)
                    {
                        // Find the same item, modify the quantity
                        if (orderLine[i].productID == Product_cb.SelectedIndex + 1)
                        {
                            orderLine.Remove(orderLine[i]);
                        }
                    }
                    // Add new one
                    shopCart.customerID = Int32.Parse(CusID_tb.Text);
                    shopCart.branchID = Branch_cb.SelectedIndex + 1;
                    shopCart.branchName = Branch_cb.Text;
                    shopCart.productName = Product_cb.Text;
                    shopCart.qty = Qty;
                    orderLine.Add(shopCart);
                    error = MessageBox.Show("Item Added");
                }
                else
                {
                    error = MessageBox.Show("Quantity for the item is not number");
                }             
            }
          
            else
            {
                error = MessageBox.Show("Choose Customer, Product, Quantity and Branch");
            }
        }

        private void ShowCart_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult error;
            string display = "Shopping Cart Detail\n\n";
            foreach (shopCart item in orderLine)
            {
                display += "\tProduct Name: " + item.productName + "  Quantity: " + item.qty + "From " + item.branchName + "\n\n";
            }

            error = MessageBox.Show(display);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult error;
            bool flag = false;
            if (!string.IsNullOrEmpty(Product_cb.Text))
            {
                int product = Product_cb.SelectedIndex + 1;
                for (int i = 0; i< orderLine.Count; i++)
                {
                    if(orderLine[i].productID == product)
                    {
                        orderLine.Remove(orderLine[i]);
                        flag = true;
                        error = MessageBox.Show("Item Deleted");
                    }                   
                }
                if(flag == false)
                {
                    error = MessageBox.Show("You do not have this product in your shopping cart");
                }             
            }
            else
            {
                error = MessageBox.Show("Choose Product to Delete");
            }
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
