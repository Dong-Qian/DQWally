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
        DataSet ds;
        Pos pos = new Pos();
        List<shopCart> orderLine = new List<shopCart>();
        List<string> product = new List<string>();
        MessageBoxResult error;

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
            product = pos.GetProduct();
            foreach (string item in product)
            {
                Product_cb.Items.Add(item);
            }

            Branch_cb.Items.Add("Sporst World");
            Branch_cb.Items.Add("Cambridge Mall");
            Branch_cb.Items.Add("St.jacobs");

            Status_cb.Items.Add("PAID");
            Status_cb.Items.Add("PEND");
            Status_cb.Items.Add("RFND");
            Status_cb.Items.Add("CNCL");

        }

        private void Price_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Product_cb.Text))
                {
                    int Qty;
                    if (Int32.TryParse(Qty_tb.Text, out Qty))
                    {
                        if (Qty > 0)
                        {
                            //int inventory = Int32.Parse(pos.FindProductQuantity(Product_cb.SelectedIndex));
                            string price = pos.FindProductPrice(Product_cb.SelectedIndex + 1);
                            double showPrice = double.Parse(price) * Qty;
                            Money_bk.Text = "$ " + showPrice.ToString();
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
            }
            catch(Exception ex)
            {
                error = MessageBox.Show("Error:\n\t" + ex.Message);
            }
        }

        private void Find_btn_Click(object sender, RoutedEventArgs e)
        {
            string lName = LName_tb.Text;
            string pArea = pArea_tb.Text;
            string pNum1 = pNum1_tb.Text;
            string pNum2 = pNum2_tb.Text;
            List<string> customer = new List<string>();
            long phone = 0;
            string ret = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(CusID_tb.Text))
                {
                    int cusID = 0;
                    if (Int32.TryParse(CusID_tb.Text, out cusID))
                    {
                        if (cusID > 0)
                        {
                            customer = pos.FindCustomerByID(cusID);
                            CusID_tb.Text = customer[0];
                            FName_tb.Text = customer[1];
                            LName_tb.Text = customer[2];
                            string[] result = customer[3].Split('-');
                            pArea_tb.Text = result[0];
                            pNum1_tb.Text = result[1];
                            pNum2_tb.Text = result[2];
                            ds = pos.CustomerRecord(Int32.Parse(customer[0]));
                            dataGrid.ItemsSource = ds.Tables[0].DefaultView;
                            customer = null;
                        }
                        else
                        {
                            error = MessageBox.Show("Customer ID can not be less than 1");
                        }
                    }
                     else
                    {
                        error = MessageBox.Show("Customer ID need to be Number");
                    }
                 }
                 else if (!string.IsNullOrEmpty(pArea) && !string.IsNullOrEmpty(pNum1) && !string.IsNullOrEmpty(pNum2))
                    {
                        if (pArea.Length == 3 && pNum1.Length == 3 && pNum2.Length == 4)
                        {
                            string pNumber = pArea + pNum1 + pNum2;
                            if (long.TryParse(pNumber.ToString(), out phone))
                            {
                                pNumber = pArea + "-" + pNum1 + "-" + pNum2;
                                customer = pos.FindCustomerByPhone(pNumber);
                                if (customer[0] == "Customer is not Exist")
                                {
                                    error = MessageBox.Show(customer[0]);
                                }
                                else
                                {
                                    CusID_tb.Text = customer[0];
                                    FName_tb.Text = customer[1];
                                    LName_tb.Text = customer[2];
                                    string[] result = customer[3].Split('-');
                                    pArea_tb.Text = result[0];
                                    pNum1_tb.Text = result[1];
                                    pNum2_tb.Text = result[2];
                                    ds = pos.CustomerRecord(Int32.Parse(customer[0]));
                                    dataGrid.ItemsSource = ds.Tables[0].DefaultView;
                                    customer = null;
                                }
                            }
                            else
                            {
                                error = MessageBox.Show("Phone Number is not Numeric");
                            }
                        }
                        else
                        {
                            error = MessageBox.Show("Phone Number: Wrong Format");
                        }                  
                }
                else
                {
                    error = MessageBox.Show("Enter Phone Number or Customer ID");
                }
            }
            catch(Exception ex)
            {
                error = MessageBox.Show("Error:\n\t" + ex.Message);
            }
        }

        private void Add_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> customer = new List<string>();
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
                            ds = pos.CustomerRecord(Int32.Parse(customer[0]));                          
                            dataGrid.ItemsSource = ds.Tables[0].DefaultView;
                            customer = null;
                        }
                        else
                        {
                            error = MessageBox.Show("Phone Number is not Numeric");
                        }
                    }
                    else
                    {
                        error = MessageBox.Show("Phone Number: Wrong Format");
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

        private void Cart_btn_Click(object sender, RoutedEventArgs e)
        {
            shopCart shopCart = new shopCart();
            if (!string.IsNullOrEmpty(Product_cb.Text) && !string.IsNullOrEmpty(CusID_tb.Text) &&
               !string.IsNullOrEmpty(Qty_tb.Text) && !string.IsNullOrEmpty(Branch_cb.Text))
            {
                shopCart.productID = Product_cb.SelectedIndex + 1;
                int Qty;
                if (Int32.TryParse(Qty_tb.Text, out Qty))
                {
                    if (Qty > 0)
                    {
                        if (orderLine.Count != 0)
                        {
                            for (int i = 0; i < orderLine.Count; i++)
                            {
                                // Find the same item, modify the quantity
                                if (orderLine[i].productID == Product_cb.SelectedIndex + 1)
                                {
                                    orderLine.Remove(orderLine[i]);
                                }
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
                        error = MessageBox.Show("Qty can not be less than 1");
                    }
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
            string display = "Shopping Cart Detail\n\n";
            foreach (shopCart item in orderLine)
            {
                display += "Product Name: " + item.productName + "  Quantity: " + item.qty + " From " + item.branchName + "\n\n";
            }

            error = MessageBox.Show(display);
        }

        private void Delete_btn_Click(object sender, RoutedEventArgs e)
        {
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

        private void CheckOut_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (shopCart item in orderLine)
                {
                    string inventory = pos.FindProductQuantity(item.productID);
                    int pInevntory = Int32.Parse(inventory);
                    if (pInevntory < item.qty)
                    {
                        error = MessageBox.Show(item.productName + "'s stock is too low. Stock: " + pInevntory);
                        return;
                    }
                }

                string status = Status_cb.Text;

                string ret = string.Empty;
                // shopping cart not empty
                if (orderLine.Count != 0)
                {
                    // Add orders to data base              
                    if (!string.IsNullOrEmpty(Status_cb.Text))
                    {
                        if (status != "RFND" && status != "CNCL")
                        {
                            // Create Order
                            ret = pos.PlaceOrders(orderLine[0].customerID, orderLine[0].branchID, status);
                            // Get the Order ID
                            string orderID = string.Empty;
                            orderID = pos.GetOrderID(orderLine[0].customerID);

                            // If Order success created
                            if (ret == "Order Placed")
                            {
                                // Add all items in shopping cart into orderline     
                                foreach (shopCart item in orderLine)
                                {
                                    pos.AddToOrderLine(Int32.Parse(orderID), item.productID, item.qty);

                                    // If the status is Paid, adjust the inventory level
                                    if (status == "PAID")
                                    {
                                        pos.AdjustInventory(Int32.Parse(orderID), item.qty, item.productID);
                                    }
                                }
                                orderLine.Clear();
                            }
                        }
                        else
                        {
                            error = MessageBox.Show("You can not create order with RFND or CNCL status");
                        }
                    }
                    else
                    {
                        error = MessageBox.Show("Choose Status for order");
                    }

                }
                else
                {
                    error = MessageBox.Show("Cart is Empty");
                }
                //// Create order when shopping cart is empty
                //else if (!string.IsNullOrEmpty(CusID_tb.Text) && !string.IsNullOrEmpty(Branch_cb.Text) && !string.IsNullOrEmpty(Status_cb.Text)
                //    && !string.IsNullOrEmpty(Product_cb.Text) && !string.IsNullOrEmpty(Qty_tb.Text))
                //{
                //    if (status != "RFND" && status != "CNCL")
                //    {
                //        int Qty;
                //        if (Int32.TryParse(Qty_tb.Text, out Qty))
                //        {
                //            if (Qty > 0)
                //            {
                //                int cusID = Int32.Parse(CusID_tb.Text);
                //                int branchID = Branch_cb.SelectedIndex + 1;
                //                ret = pos.PlaceOrders(cusID, branchID, status);
                //                string orderID = string.Empty;
                //                orderID = pos.GetOrderID(cusID);
                //                if (status == "PAID")
                //                {
                //                    pos.AdjustInventory(Int32.Parse(orderID), Qty, Product_cb.SelectedIndex + 1);
                //                    error = MessageBox.Show(ret);
                //                }
                //                if (ret == "Order Placed")
                //                {
                //                    pos.AddToOrderLine(Int32.Parse(orderID), Product_cb.SelectedIndex + 1, Qty);
                //                }
                //            }
                //            else
                //            {
                //                error = MessageBox.Show("Quantity can not be less than 1");
                //            }
                //        }
                //        else
                //        {
                //            error = MessageBox.Show("Quantity needs be Numberic");
                //        }
                //    }

                //    else
                //    {
                //        error = MessageBox.Show("You can not create order with RFND or CNCL status");
                //    }
                //}
                //else
                //{
                //    error = MessageBox.Show("Choose Customer, Product, Quantity, Branch and Satus");
                //}

                if (ret == "Order Placed")
                {
                    error = MessageBox.Show("*********************\nThank you for shopping at\nWally’s World Sports World\nOn Sept. 20, 2016, Sean Clarke!\nOrder ID: 5001\nVictorian Lace Wallpaper(roll) 4 x $14.95 = $59.80\nDrywall Repair Compound(tube) 1 x $6.95 = $6.95\nDrywall Tape(roll) 2 x $3.95 = $7.90\nSubtotal = $ 74.65\nHST(13 %) = $ 9.70\nSale Total = $ 84.35\n)");
                }
            }
            catch (Exception ex)
            {
                error = MessageBox.Show("Error:\n\t" + ex.Message);
            }

           
        }

        private void Inventory_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Branch_cb.Text))
                {
                    ds = pos.Inventory();
                    dataGrid.ItemsSource = ds.Tables[0].DefaultView;
                }
                else
                {
                    error = MessageBox.Show("Choose the Branch");
                }
            }
            catch(Exception ex)
            {
                error = MessageBox.Show(ex.Message);
            }
        }

        private void OrderHistory_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(CusID_tb.Text))
                {
                    int cusID = Int32.Parse(CusID_tb.Text);
                    ds = pos.OrderHistory(cusID);
                    dataGrid.ItemsSource = ds.Tables[0].DefaultView;
                }
                else
                {
                    error = MessageBox.Show("Choose the Customer");
                }
            }
            catch (Exception ex)
            {
                error = MessageBox.Show(ex.Message);
            }
        }

        private void ChangeStatus_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ret = string.Empty;
                int orderID = 0;
                if (!string.IsNullOrEmpty(OrderID_tb.Text) && !string.IsNullOrEmpty(Status_cb.Text))
                {
                    if (Int32.TryParse(OrderID_tb.Text, out orderID) && orderID > 0)
                    {

                        ret = pos.ChangeOrderStatus(orderID, Status_cb.Text);
                        error = MessageBox.Show(ret);

                    }
                    else
                    {
                        error = MessageBox.Show("Order ID must be Numberic and lager than 0");
                    }
                }
                else
                {
                    error = MessageBox.Show("Choose Order ID and Status");
                }
            }
            catch (Exception ex)
            {
                error = MessageBox.Show(ex.Message);
            }
        }

        private void CustomerDB_tb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ds = pos.CustomerDB();
                dataGrid.ItemsSource = ds.Tables[0].DefaultView;
            }
            catch (Exception ex)
            {
                error = MessageBox.Show(ex.Message);
            }
        }

        private void Orderline_btn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(OrderID_tb.Text))
            {
                string orderIDStr = OrderID_tb.Text;
                int orderID = 0;
                if (Int32.TryParse(orderIDStr, out orderID) && orderID > 0)
                {
                    try
                    {
                        ds = pos.OrderDetail(orderID);
                        dataGrid.ItemsSource = ds.Tables[0].DefaultView;
                    }
                    catch (Exception ex)
                    {
                        error = MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    error = MessageBox.Show("Order ID must be Numberic and lager than 0");
                }
            }
            else
            {
                error = MessageBox.Show("Choose Order ID");
            }
        }

        private void SalesRecord_btn_Click(object sender, RoutedEventArgs e)
        {
            string ret = string.Empty;
            int orderID = 0;
            if (!string.IsNullOrEmpty(OrderID_tb.Text))
            {
                if (Int32.TryParse(OrderID_tb.Text, out orderID) && orderID > 0)
                {
                    ret =pos.SalesRecord(orderID);
                    error = MessageBox.Show(ret);
                }
                else
                {
                    error = MessageBox.Show("Order ID must be Numberic and lager than 0");
                }
            }
            else
            {
                error = MessageBox.Show("Choose Order ID and Status");
            }
        }
    }
}
