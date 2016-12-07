/*
* ProjectName:  MainWindow.xaml.cs
* Programer:    Dong Qian (6573448)
* Date:         Dec 4 16, 2016
* Description:  This is a simple POS Application which connect to MySql Database
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
        DataSet ds; // dataSet variable use to display select table from database
        Pos pos = new Pos();
        List<shopCart> orderLine = new List<shopCart>();    // a list contains the orderline table
        List<string> product = new List<string>();  // a list contains the product table
        List<string> branch = new List<string>();   // a list contains the branch table
        MessageBoxResult error; // messagebox to show all information
        
        // struct to store the item information when customer place order
        struct shopCart
        {
            public int customerID;
            public int branchID;
            public string productName;
            public string branchName;
            public int productID;
            public int qty;
        }

        // Main window load up
        public MainWindow()
        {
            InitializeComponent();
            // load product table in to a combox
            product = pos.GetProduct();
            foreach (string item in product)
            {
                Product_cb.Items.Add(item);
            }
            
            // load branch in to a combox
            branch = pos.GetBranch();
            foreach (string item in branch)
            {
                Branch_cb.Items.Add(item);
            }
            
            // load the status into combox
            Status_cb.Items.Add("PAID");
            Status_cb.Items.Add("PEND");
            Status_cb.Items.Add("RFND");
            Status_cb.Items.Add("CNCL");

        }
        

        /// <summary>
        /// Call FindProductPrice method to check the specific item's unit price
        /// </summary>
        private void Price_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // product textbox can not be empty
                if (!string.IsNullOrEmpty(Product_cb.Text))
                {
                    int Qty;
                    // Qty must be a number and lager than 0
                    if (Int32.TryParse(Qty_tb.Text, out Qty))
                    {
                        // if Qty is larger than 0, find the unit price and mutiple to Qty
                        // display the total price
                        // otherwise display the errors
                        if (Qty > 0)
                        {
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


        /// <summary>
        /// Call FindCustomerByID or FindCustomerByphone method to find the customer information
        /// </summary>
        private void Find_btn_Click(object sender, RoutedEventArgs e)
        {
            string lName = LName_tb.Text;
            string pArea = pArea_tb.Text;
            string pNum1 = pNum1_tb.Text;
            string pNum2 = pNum2_tb.Text;
            List<string> customer = new List<string>();
            long phone = 0;
            string ret = string.Empty;
            
            // 1. find the customer by ID
            // 2. find the customer by phone number
            try
            {
                // customer ID can not be empty and must be a non negative number 
                #region Find Customer by customer ID
                if (!string.IsNullOrEmpty(CusID_tb.Text))
                {
                    int cusID = 0;
                    if (Int32.TryParse(CusID_tb.Text, out cusID))
                    {
                        if (cusID > 0)
                        {
                            // Get the information from database and display to the specific textbox
                            // it returns a list
                            // display all errors
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
                #endregion

                // Get the customer information by phone number, which is unqiue phone number
                // phone number can not be empty
                // phone number must be a number
                // display all errror
                #region Find customer by phone number
                else if (!string.IsNullOrEmpty(pArea) && !string.IsNullOrEmpty(pNum1) && !string.IsNullOrEmpty(pNum2))
                    {
                        if (pArea.Length == 3 && pNum1.Length == 3 && pNum2.Length == 4)
                        {
                            string pNumber = pArea + pNum1 + pNum2;
                            if (long.TryParse(pNumber.ToString(), out phone))
                            {
                                pNumber = pArea + "-" + pNum1 + "-" + pNum2;
                                // find the customer information 
                                // return as a list
                                customer = pos.FindCustomerByPhone(pNumber);
                                if (customer[0] == "Customer is not Exist")
                                {
                                    error = MessageBox.Show(customer[0]);
                                }
                                else
                                {
                                    // fill into textbox area
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
                #endregion
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


        /// <summary>
        /// Call AddCustomer method to add a new customer to the database
        /// </summary>
        private void Add_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // add a new cutomer needs customer's first name, last name and phone number. 
                // display all errors
                List<string> customer = new List<string>();
                string ret = string.Empty;
                string pArea = pArea_tb.Text;
                string pNum1 = pNum1_tb.Text;
                string pNum2 = pNum2_tb.Text;
                long phone = 0;
                 // all these attributes can not be empty
                if (!string.IsNullOrEmpty(FName_tb.Text) && !string.IsNullOrEmpty(LName_tb.Text) &&
                    !string.IsNullOrEmpty(pArea_tb.Text) && !string.IsNullOrEmpty(pNum1_tb.Text) &&
                     !string.IsNullOrEmpty(pNum2_tb.Text))

                {
                    // phone number must be a number
                    if (pArea.Length == 3 && pNum1.Length == 3 && pNum2.Length == 4)
                    {
                        string pNumber = pArea + pNum1 + pNum2;
                        if (long.TryParse(pNumber.ToString(), out phone))
                        {
                            pNumber = pArea + "-" + pNum1 + "-" + pNum2;
                            // call addCustomer to add customer
                            ret = pos.AddCustomer(FName_tb.Text, LName_tb.Text, pNumber);
                            error = MessageBox.Show(ret);
                            // find out the new added customer and display to the textbox
                            customer = pos.FindCustomerByPhone(pNumber);
                            CusID_tb.Text = customer[0];
                            FName_tb.Text = customer[1];
                            LName_tb.Text = customer[2];
                            string[] result = customer[3].Split('-');
                            pArea_tb.Text = result[0];
                            pNum1_tb.Text = result[1];
                            pNum2_tb.Text = result[2];
                            // also show to the dataGrid
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
             
           
        /// <summary>
        /// Add items into shopping cart, needs product, Qty and branch
        /// </summary>
        private void Cart_btn_Click(object sender, RoutedEventArgs e)
        {
            shopCart shopCart = new shopCart();
            int pID = 0;
            int qty = 0;
            // all these fields can not be empty
            if (!string.IsNullOrEmpty(Product_cb.Text) && !string.IsNullOrEmpty(CusID_tb.Text) &&
               !string.IsNullOrEmpty(Qty_tb.Text) && !string.IsNullOrEmpty(Branch_cb.Text))
            {                                       
                // qty needs to be a number and lager than 0
                int Qty;
                if (Int32.TryParse(Qty_tb.Text, out Qty))
                {
                    if (Qty > 0)
                    {
                        pID = Product_cb.SelectedIndex + 1;                 
                        // go through the shopping cart and find the the inventory quantity
                        // if the inventory is less than order quantity, display errors
                        string inventory = pos.FindProductQuantity(pID);
                        int pInevntory = Int32.Parse(inventory);
                        if (pInevntory < Qty)
                        {
                            error = MessageBox.Show(Product_cb.Text + "'s stock is too low. Stock: " + pInevntory);
                            return;
                        }

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
                        // Add all infotamtion about the item into the a shopping cart struct
                        shopCart.productID = pID;
                        shopCart.customerID = Int32.Parse(CusID_tb.Text);
                        shopCart.branchID = Branch_cb.SelectedIndex + 1;
                        shopCart.branchName = Branch_cb.Text;
                        shopCart.productName = Product_cb.Text;
                        shopCart.qty = Qty;
                        // add this shoping cart into orderline list
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


        /// <summary>
        /// Show the shopping cart to the custome, basic is like orderline
        /// </summary>
        private void ShowCart_btn_Click(object sender, RoutedEventArgs e)
        {
            List<string> shopcart = new List<string>();
            if (orderLine.Count != 0)
            {
                foreach (shopCart item in orderLine)
                {
                    shopcart.Add(item.productName);
                    shopcart.Add(item.qty.ToString());
                    shopcart.Add(item.branchName);
                }
                shoppingCart cartWindow = new shoppingCart(shopcart);
                cartWindow.ShowDialog();
            }
            else
            {
                error = MessageBox.Show("Shopping Cart is Empty");
            }
        }


        /// <summary>
        /// Delete the item from the shopping cart
        /// </summary>
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


        /// <summary>
        /// Check out the order, it should contains mutiple items (orderline)
        /// </summary>
        private void CheckOut_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string status = Status_cb.Text;
                string ret = string.Empty;
                string orderID = string.Empty;
                // shopping cart not empty
                if (orderLine.Count != 0)
                {
                    // Add orders to data base, status can not be empty            
                    if (!string.IsNullOrEmpty(Status_cb.Text))
                    {
                        // Customer can not make a new order with RFND and CNCL status.
                        // Only allow PAID and PEND
                        if (status != "RFND" && status != "CNCL")
                        {
                            // Create Order
                            ret = pos.PlaceOrders(orderLine[0].customerID, orderLine[0].branchID, status);
                            // Get the Order ID
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
                
                if (ret == "Order Placed")
                {
                    ret = pos.SalesRecord(Int32.Parse(orderID));
                    error = MessageBox.Show(ret);
                }
            }
            catch (Exception ex)
            {
                error = MessageBox.Show("Error:\n\t" + ex.Message);
            }           
        }


        /// <summary>
        /// Display the product inventory
        /// </summary>
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


        /// <summary>
        /// Display the customer's order history
        /// </summary>
        private void OrderHistory_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // in order to diaply orders, customer ID can not be empty
                // display errors
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


        /// <summary>
        /// Change the status of the order form PAID to RFND, PEND to PAID, or PEND to CNCL
        /// </summary>
        private void ChangeStatus_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ret = string.Empty;
                int orderID = 0;
                // orderID and status can not be empty
                if (!string.IsNullOrEmpty(OrderID_tb.Text) && !string.IsNullOrEmpty(Status_cb.Text))
                {
                // orderID needs be number and larger than 0
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
    

        /// <summary>
        /// Display the customer table to the dataGrid
        /// </summary>
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


        /// <summary>
        /// Display the orderline detail to the specific orders
        /// Actually it combines two tables
        /// </summary>
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
                        // call orderDetail method and return a dataSet use to show on dataGrid
                        ds = pos.OrderDetail(orderID);
                        dataGrid.ItemsSource = ds.Tables[0].DefaultView;
                        OrderID_tb.Text = "";
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


        /// <summary>
        /// Display the Sales Record
        /// </summary>
        private void SalesRecord_btn_Click(object sender, RoutedEventArgs e)
        {
            string ret = string.Empty;
            int orderID = 0;
            if (!string.IsNullOrEmpty(OrderID_tb.Text))
            {
                if (Int32.TryParse(OrderID_tb.Text, out orderID) && orderID > 0)
                {
                    // call salesReord method and return a sting contains all inforamtion about sales record
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
