/*
* ProjectName:  Pos.cs
* Programer:    Dong Qian (6573448)
* Date:         Dec 4 16, 2016
* Description:  This is a simple POS Application which connect to MySql Database
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace DQWally_POS
{
    class Pos
    {
        // create variables
        MySqlConnection conn;
        MySqlDataAdapter da;
        DataSet ds;

        // struct to store for some orderline inforamtion
        struct orderLineTemp
        {
           public string productID;
           public string qty;
        }
        
        // list use for display to data table
        List<orderLineTemp> temp = new List<orderLineTemp>();
        List<string> product = new List<string>();
        List<string> branch = new List<string>();


        /// <summary>
		/// Connect to the database dqWally
		/// </summary>
        public string Conntect()
        {
            string ret = string.Empty;
            string connStr = "server=127.0.0.1;user=root;database=dqwally;port=3306;password=Conestoga1;";
            conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ret;
        }


        /// <summary>
		/// Add new cuntomer to the database
		/// </summary>
		/// <param name="fName">Customer's first name.</param>
        /// <param name="LName">Customer's last name</param>
        /// <param name="pNumber">Customer's phone number</param>
		/// <returns>string - contains a message or exception to indicate success or failed</returns>
        public string AddCustomer(string fName, string lName, string pNumber)
        {
            List<string> customer = new List<string>();
            string ret = string.Empty; 
            // find customer in the database first, see if the customer is already exist
            try
            {
                customer = FindCustomerByPhone(pNumber);
                // if not exist, add customer into database
                if(customer[0] == "Customer is not Exist")
                {
                    this.Conntect();
                    string sql = "INSERT INTO customer (FirstName, LastName, PhoneNumber) VALUES ('" + fName + "','" + lName + "','" + pNumber + "');";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                    ret = "Customer Added";
                }
                else
                {
                    ret = "Customer is already Exist"; 
                }
            }
            catch (Exception ex)
            {                
                ret = ex.Message;
            }

            conn.Close();
            return ret;
        }


        /// <summary>
		/// Add new cuntomer to the database
		/// </summary>
		/// <param name="pNumber">a phone number</param>
		/// <returns>list of string - contains a string of cutomers information or exception to indicate success or failed</returns>
        public List<string> FindCustomerByPhone(string pNumber)
        {
            List<string> customer = new List<string>();           
            try
            {
                this.Conntect();
                string sql = "SELECT * FROM customer WHERE PhoneNumber ='" + pNumber + "';";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                // Add custome's ID, first name, last name, phhone number into a list
                if (rdr.Read())
                {
                    customer.Add(rdr[0].ToString());
                    customer.Add(rdr[1].ToString());
                    customer.Add(rdr[2].ToString());
                    customer.Add(rdr[3].ToString());
                    rdr.Close();
                }
                else
                {
                    customer.Add("Customer is not Exist");
                }         
            }
            catch (Exception ex)
            {           
                throw ex;
            }
                    
            conn.Close();
            return customer;
        }


        /// <summary>
		/// Find the customer from the database
		/// </summary>
		/// <param name="pNumber">a phone number</param>
		/// <returns>list of string - contains a string of cutomers information or exception to indicate success or failed</returns>
        public List<string> FindCustomerByID(int cusID)
        {
            List<string> customer = new List<string>();
            try
            {
                this.Conntect();
                string sql = "SELECT * FROM customer WHERE CustomerID ='" + cusID + "';";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                // Add customerID, first name, last name, phone number into a sting list and return
                if (rdr.Read())
                {
                    customer.Add(rdr[0].ToString());
                    customer.Add(rdr[1].ToString());
                    customer.Add(rdr[2].ToString());
                    customer.Add(rdr[3].ToString());
                }
                else
                {
                    rdr.Close();
                    customer.Add("Customer is not Exist");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            conn.Close();
            return customer;
        }


        /// <summary>
        /// Find the product price from the database
        /// </summary>
        /// <param name="pID">prduct ID</param>
        /// <returns>A string contains the product unit price or exceptions</returns>
        public string FindProductPrice(int pID)
        {           
            string ret = string.Empty;
            try
            {
                this.Conntect();
                string sql = "SELECT UnitPrice FROM product WHERE ProductID =" + pID + ";";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                // return the unit price of the psecific product
                if (rdr.Read())
                {
                    ret = rdr[0].ToString();
                }
                else
                {
                    ret = "Product Not Exist";
                }
                
                rdr.Close();
            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }

            return ret;
        }


        /// <summary>
        /// Find the product inventory quantity from the database
        /// </summary>
        /// <param name="pID">prduct ID</param>
        /// <returns>A string contains the product quantity or exceptions</returns>
        public string FindProductQuantity(int pID)
        {
          
            string ret = string.Empty;
            try
            {
                this.Conntect();
                string sql = "SELECT InventoryQuantity FROM product WHERE ProductID =" + pID + ";";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                // return a string contians the inventory quantity
                if (rdr.Read())
                {
                    ret = rdr[0].ToString();
                }
                else
                {
                    ret = "Product Not Exist";
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }

            return ret;
        }


        /// <summary>
        /// Place the order into database
        /// </summary>
        /// <param name="CusID">Customer ID</param>
        /// /// <param name="CusID">Customer ID</param>
        /// /// <param name="status">order status</param>
        /// <returns>A string indicator success or exceptions</returns>
        public string PlaceOrders(int CusID, int branchID, string status)
        {
           
            string ret = string.Empty;
            DateTime now = DateTime.Now;
            now.ToString("yyyy-MM-dd");
            try
            {
                this.Conntect();
                // Add a order to database
                string sql = "INSERT INTO orders (OrderDate, CustomerID, BranchID, OrderStatus) VALUES ('"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + CusID + "," + branchID + ",'" + status + "');";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                ret = "Order Placed";
            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }

            conn.Close();
            return ret;
        }


        /// <summary>
        /// place a orderline into database
        /// </summary>
        /// <param name="OrdersID">order ID</param>
        /// <param name="ProducrID">prduct ID</param>
        /// <param name="qty">prduct Quantity</param>
        /// <returns>A string indicator success or exceptions</returns>
        public string AddToOrderLine(int OrdersID, int ProductID, int qty)
        {

            this.Conntect();
            string ret = string.Empty;
            try
            {
                this.Conntect();
                // add a orderline into database
                string sql = "INSERT INTO orderline (OrdersID, ProductID, Quantity) VALUES (" + OrdersID + "," + ProductID + "," + qty + ");";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                ret = "OrderLine Added";
            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }
            conn.Close();
            return ret;
        }


        /// <summary>
        /// Ajust inventory quantity when the PAID order placed in the database
        /// </summary>
        /// <param name="OrdersID">order ID</param>
        /// <param name="ProducrID">prduct ID</param>
        /// <param name="qty">prduct Quantity</param>
        /// <returns>A string indicator success or exceptions</returns>
        public string AdjustInventory(int OrdersID, int qty, int ProductID)
        {
            string ret = string.Empty;
            try
            {
                this.Conntect();
                // change the inventory level
                string sql = "update product set InventoryQuantity =  InventoryQuantity - " + qty + " where productID = " + ProductID + ";";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }
            conn.Close();
            return ret;
        }


        /// <summary>
        /// change the order status and adjust the inventory level base on status
        /// </summary>
        /// <param name="OrdersID">order ID</param>
        /// <param name="status">order status</param>
        /// <returns>A string indicator success or exceptions</returns>
        public string ChangeOrderStatus(int OrdersID, string status)
        {          
            string ret = string.Empty;
            try
            {
                this.Conntect();
                string orignalStatus = string.Empty;

                // get the orignal status, comppare to the status want to change
                string sql = "select OrderStatus from orders where OrdersID = " + OrdersID + ";";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    orignalStatus = rdr[0].ToString();
                }
                rdr.Close();
                // status can not be same
                if (orignalStatus != status)
                {
                    // only can change status from paid to rfnd, pend to cncl or pend to paid
                    if ((orignalStatus == "PAID" && status == "RFND") || (orignalStatus == "PEND" && status == "CNCL") ||
                        (orignalStatus == "PEND" && status == "PAID"))
                    {
                        #region change from pend to paid                  
                        // from pend to paid, we need to decrease inventory level in the database
                        if (orignalStatus == "PEND" && status == "PAID")
                        {
                            sql = "select ol.ProductID, ol.Quantity, p.ProductName from orderline ol inner join product p on ol.productID = p.ProductID where OrdersID = " + OrdersID + ";";
                            cmd = new MySqlCommand(sql, conn);
                            rdr = cmd.ExecuteReader();
                            while (rdr.Read())
                            {
                                int productID = Int32.Parse(rdr[0].ToString());
                                int qty = Int32.Parse(rdr[1].ToString());
                                string inventory = FindProductQuantity(productID);
                                int pInevntory = Int32.Parse(inventory);
                                if (pInevntory < qty)
                                {
                                    ret = "stock is low for " + rdr[2] + "\nCurrent Stock is : " + inventory;
                                    rdr.Close();
                                    return ret;
                                }
                            }
                            rdr.Close();

                            // change the old status to new status
                            sql = "update orders set OrderStatus ='" + status + "' where OrdersID =" + OrdersID + ";";
                            cmd = new MySqlCommand(sql, conn);
                            cmd.ExecuteNonQuery();
                            ret = "Order Status Changed";

                            // chnage database inventory
                            sql = "select ProductID, Quantity from orderline where OrdersID = " + OrdersID + ";";
                            cmd = new MySqlCommand(sql, conn);
                            rdr = cmd.ExecuteReader();
                            while (rdr.Read())
                            {
                                this.AdjustInventory(OrdersID, Int32.Parse(rdr[1].ToString()), Int32.Parse(rdr[0].ToString()));
                            }
                            rdr.Close();
                        }
                        #endregion

                        // from paid to rfnd, we have to increase the inventory level
                        // and zero out  the orderline quantity

                        #region change from paid to rfnd
                        else if (orignalStatus == "PAID" && status == "RFND")
                        {
                            // change the old status to new status
                            sql = "update orders set OrderStatus ='" + status + "' where OrdersID =" + OrdersID + ";";
                            cmd = new MySqlCommand(sql, conn);
                            cmd.ExecuteNonQuery();
                            ret = "Order Status Changed";

                            // change database inventory
                            sql = "select ProductID, Quantity from orderline where OrdersID = " + OrdersID + ";";
                            cmd = new MySqlCommand(sql, conn);
                            rdr = cmd.ExecuteReader();
                            while (rdr.Read())
                            {
                                this.AdjustInventory(OrdersID, -Int32.Parse(rdr[1].ToString()), Int32.Parse(rdr[0].ToString()));
                                this.AdjustOrderLine(OrdersID, Int32.Parse(rdr[0].ToString()), Int32.Parse(rdr[1].ToString()));
                            }
                            rdr.Close();
                        }
                        #endregion

                        #region change from pend to cncl
                        // from pend to cncl, we have to zero out  the orderline quantity
                        else if (orignalStatus == "PEND" && status == "CNCL")
                        {
                            // change the old status to new status
                            sql = "update orders set OrderStatus ='" + status + "' where OrdersID =" + OrdersID + ";";
                            cmd = new MySqlCommand(sql, conn);
                            cmd.ExecuteNonQuery();
                            ret = "Order Status Changed";

                            // change database inventory
                            sql = "select ProductID, Quantity from orderline where OrdersID = " + OrdersID + ";";
                            cmd = new MySqlCommand(sql, conn);
                            rdr = cmd.ExecuteReader();
                            while (rdr.Read())
                            {
                                this.AdjustOrderLine(OrdersID, Int32.Parse(rdr[0].ToString()), Int32.Parse(rdr[1].ToString()));
                            }
                            rdr.Close();
                        }
                        #endregion
                    }
                    else
                    {
                        ret = "You can't change the Status this way";
                    }
                }
                else
                {
                    ret = "Order Status is Same";
                }
            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }
            conn.Close();
            return ret;
        }


        /// <summary>
        /// Adjust the orderline order quantity to 0
        /// </summary>
        /// <param name="OrdersID">order ID</param>
        /// <param name="productID">product ID</param>
        /// <param name="qty">product quantity</param>
        /// <returns>A string indicator success or exceptions</returns>
        public string AdjustOrderLine(int OrdersID, int ProductID, int qty)
        {
            string ret = string.Empty;
            try
            {
                this.Conntect();
                // change the orderline quantity to zero
                string sql = "update orderline set Quantity = 0 where OrdersID = " + OrdersID + " and ProductID = " + ProductID + " and Quantity =" + qty + ";";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }
            conn.Close();
            return ret;
        }


        /// <summary>
        /// Get the order ID by customer ID
        /// </summary>
        /// <param name="cusID">customer ID</param>
        /// <returns>A string contains the customer ID or exceptions</returns>
        public string GetOrderID(int cusID)
        {
            string ret = string.Empty;
            try
            {
                this.Conntect();
                string sql = "select max(OrdersID) from orders where CustomerID = " + cusID + ";";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    ret = rdr[0].ToString();
                }
                else
                {
                    ret = "Order Does Not Exist";
                }
                rdr.Close();         
            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }
            conn.Close();
            return ret;
        }


        /// <summary>
        /// Get all orders base on the customer from the database
        /// </summary>
        /// <param name="cusID">customer ID</param>
        /// <returns>DataSet include all orders the customer placed</returns>
        public DataSet OrderHistory(int cusID)
        {
            try
            {
                this.Conntect();
                da = new MySqlDataAdapter("select * from orders where CustomerID ='" + cusID + "';", conn);
                ds = new DataSet();
                da.Fill(ds);
                conn.Close();
                return ds;
            }
            catch(Exception ex)
            {
                throw ex;
            }        
        }


        /// <summary>
        /// Get the all product infromation from the database 
        /// </summary>
        /// <returns>DataSer includes all product information</returns>
        public DataSet Inventory()
        {
            try
            {
                this.Conntect();
                da = new MySqlDataAdapter("select * from product;", conn);
                ds = new DataSet();
                da.Fill(ds);
                conn.Close();
                return ds;
            }
            catch(Exception ex)
            {
                throw ex;
            }        
        }


        /// <summary>
        /// Get the specific customer record from database
        /// </summary>
        /// <param name="cusID">customer ID</param>
        /// <returns>DataSet include the cusotmer information</returns>
        public DataSet CustomerRecord(int cusID)
        {
            try
            {
                this.Conntect();
                da = new MySqlDataAdapter("select * from customer where CustomerID ='" + cusID + "';", conn);
                ds = new DataSet();
                da.Fill(ds);
                conn.Close();
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Get all customer inforamtion from the database
        /// </summary>
        /// <returns>DataSet include all cusotmer information</returns>
        public DataSet CustomerDB()
        {
            try
            {
                this.Conntect();
                da = new MySqlDataAdapter("select * from customer;", conn);
                ds = new DataSet();
                da.Fill(ds);
                conn.Close();
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Get all orderline assoicate with the specific order from the database
        /// </summary>
        /// <param name="orderID">order ID</param>
        /// <returns>DataSet include all orderline for that order</returns>
        public DataSet OrderDetail(int orderID)
        {
            try
            {
                this.Conntect();
                da = new MySqlDataAdapter("select ol.OrdersID, p.ProductName, p.UnitPrice, ol.Quantity, (p.UnitPrice * ol.Quantity) as TotalPrice from orderline ol inner join product p on ol.ProductID = p.ProductID where OrdersID =" + orderID + ";", conn);
                ds = new DataSet();
                da.Fill(ds);
                conn.Close();
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Create the sales Record for customer base on the order ID. it can create the sale record for pass order as well
        /// </summary>
        /// <param name="orderID">order ID</param>
        /// <returns>string include the information for the sales record</returns>
        public string SalesRecord(int ordersID)
        {
            #region Get all Needed Infromation for Sales Record
            // all variables use to store the sales record's inforamtion
            string ret = string.Empty;
            string customerID = string.Empty;
            string branchID = string.Empty;
            string productID = string.Empty;
            string productName = string.Empty;
            string price = string.Empty;
            string customerName = string.Empty;
            string branchName = string.Empty;
            string orderDate = string.Empty;
            string orderStatus = string.Empty;
            string quantity = string.Empty;
            double total = 0;
            double sum = 0;


            //string ret = string.Empty;
            try
            {
                this.Conntect();
                string sql = "select CustomerID, BranchID from orders where OrdersID = " + ordersID + ";";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    customerID = rdr[0].ToString();
                    branchID = rdr[1].ToString();
                }
                else
                {
                    ret = "Order Does Not Exist";
                }
                rdr.Close();

                // Get Customer Name
                sql = "select FirstName, LastName from customer where CustomerID = " + customerID + ";";
                cmd = new MySqlCommand(sql, conn);
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    customerName = rdr[0].ToString() + " " + rdr[1].ToString();
                }
                rdr.Close();

                // Get Branch Name
                sql = "select BranchName from branch where BranchID = " + branchID + ";";
                cmd = new MySqlCommand(sql, conn);
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    branchName = rdr[0].ToString();
                }
                rdr.Close();

                // Get OrderDate and Status
                sql = "select OrderDate, OrderStatus from orders where OrdersID = " + ordersID + ";";
                cmd = new MySqlCommand(sql, conn);
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    orderDate = rdr[0].ToString();
                    orderStatus = rdr[1].ToString();
                }
                rdr.Close();

                // Get ProductID and Quantity
                sql = "select ProductID, Quantity from orderline where OrdersID = " + ordersID + ";";
                cmd = new MySqlCommand(sql, conn);
                rdr = cmd.ExecuteReader();
                string orderline = string.Empty;

                while (rdr.Read())
                {
                    orderLineTemp order = new orderLineTemp();
                    order.productID = rdr[0].ToString();
                    order.qty = rdr[1].ToString();
                    temp.Add(order);
                }
                rdr.Close();

                // Get orderline 
                foreach (orderLineTemp item in temp)
                {
                    sql = "select ProductName, UnitPrice from product where ProductID = " + item.productID + ";";
                    cmd = new MySqlCommand(sql, conn);
                    rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        productName = rdr[0].ToString();
                        price = rdr[1].ToString();

                    }
                    total = double.Parse(item.qty) * double.Parse(price);
                    sum += total;
                    orderline += productName + " " + item.qty + " x $" + price + " = $" + Math.Round(total, 2).ToString("#.00") + "\n";
                    rdr.Close();
                }

                #endregion

                // Build up Sales Record
                // depends on status, add last line for different sales record
                ret = "Thank you for shopping at\nDQWally World " + branchName + "\nOn " + orderDate + ",  " + customerName + "!\n";
                ret += orderline;
                ret += "Subtotal = $ " + Math.Round(sum, 2).ToString("#.00") + "\n";
                ret += "HST (13%) = $ " + Math.Round(sum * 0.13, 2).ToString("#.00") + "\n";
                ret += "Sale Total = $ " + Math.Round(sum * 1.13, 2).ToString("#.00") + "\n";
                if (orderStatus == "PAID")
                {
                    ret += "Paid – Thank you!";
                }
                else if(orderStatus == "PEND")
                {
                    ret += "Pending – We’ll contact you soon!";
                }
                else if (orderStatus == "RFND")
                {
                    ret += "Refunded – Please come again!";
                }
                else
                {
                    ret = "Your Order is Canceled";
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }

            temp.Clear();
            conn.Close();
            return ret;
        }


        /// <summary>
        /// Get all product Name from database
        /// </summary>
        /// <returns>List of product Name</returns>
        public List<string> GetProduct()
        {
            string ret = string.Empty;
            try
            {
                this.Conntect();
                string sql = "select ProductName from product;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while(rdr.Read())
                {
                    product.Add(rdr[0].ToString());
                }               
                rdr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            conn.Close();
            return product;
        }


        /// <summary>
        /// Get all Branch Name from database
        /// </summary>
        /// <returns>List of Branch Name</returns>
        public List<string> GetBranch()
        {
            string ret = string.Empty;
            try
            {
                this.Conntect();
                string sql = "select BranchName from branch;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while(rdr.Read())
                {
                    branch.Add(rdr[0].ToString());
                }               
                rdr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            conn.Close();
            return branch;
        }
    }
}
