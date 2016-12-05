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
        MySqlConnection conn;
        MySqlDataAdapter da;
        DataSet ds;
        struct orderLineTemp
        {
           public string productID;
           public string qty;
        }
        List<orderLineTemp> temp = new List<orderLineTemp>();


        public string Conntect()
        {
            string ret = string.Empty;
            string connStr = "server=127.0.0.1;user=super;database=dqwally;port=3306;password=Conestoga1;";
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

        public string AddCustomer(string fName, string lName, string pNumber)
        {
            List<string> customer = new List<string>();
            string ret = string.Empty;        
            try
            {
                customer = FindCustomerByPhone(pNumber);
                if(customer[0] == "Customer is not Exist")
                {
                    this.Conntect();
                    string sql = "INSERT INTO customer (FirstName, LastName, PhoneNumber) VALUES ('" + fName + "','" + lName + "','" + pNumber + "');";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                    ret = "Customer Added";
                }
            }
            catch (Exception ex)
            {                
                ret = ex.Message;
            }

            conn.Close();
            return ret;
        }

        public List<string> FindCustomerByPhone(string pNumber)
        {
            List<string> customer = new List<string>();
           
            try
            {
                this.Conntect();
                string sql = "SELECT * FROM customer WHERE PhoneNumber ='" + pNumber + "';";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
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

        public List<string> FindCustomerByID(int cusID)
        {
            List<string> customer = new List<string>();
            try
            {
                this.Conntect();
                string sql = "SELECT * FROM customer WHERE CustomerID ='" + cusID + "';";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
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


        public string FindProductPrice(int pID)
        {           
            string ret = string.Empty;
            try
            {
                this.Conntect();
                string sql = "SELECT UnitPrice FROM product WHERE ProductID =" + pID + ";";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
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

        public string FindProductQuantity(int pID)
        {
          
            string ret = string.Empty;
            try
            {
                this.Conntect();
                string sql = "SELECT InventoryQuantity FROM product WHERE ProductID =" + pID + ";";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
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

        public string PlaceOrders(int CusID, int branchID, string status)
        {
           
            string ret = string.Empty;
            DateTime now = DateTime.Now;
            now.ToString("yyyy-MM-dd");
            try
            {
                this.Conntect();
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

        public string AddToOrderLine(int OrdersID, int ProductID, int qty)
        {

            this.Conntect();
            string ret = string.Empty;
            try
            {
                this.Conntect();
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

        public string AdjustInventory(int OrdersID, int qty, int ProductID)
        {
            string ret = string.Empty;
            try
            {
                this.Conntect();
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

        public string ChangeOrderStatus(int OrdersID, string status)
        {          
            string ret = string.Empty;
            try
            {
                this.Conntect();
                string orignalStatus = string.Empty;
                // Get the orignal status
                string sql = "select OrderStatus from orders where OrdersID = " + OrdersID + ";";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    orignalStatus = rdr[0].ToString();
                }
                rdr.Close();
                if (orignalStatus != status)
                {
                    if ((orignalStatus == "PAID" && status == "RFND") || (orignalStatus == "PEND" && status == "CNCL") ||
                        (orignalStatus == "PEND" && status == "PAID"))
                    {
                        sql = "update orders set OrderStatus ='" + status + "' where OrdersID =" + OrdersID + ";";
                        cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                        ret = "Order Status Changed";

                        // change from pend to paid
                        sql = "select ProductID, Quantity from orderline where OrdersID = " + OrdersID + ";";
                        cmd = new MySqlCommand(sql, conn);
                        rdr = cmd.ExecuteReader();

                        if (orignalStatus == "PEND" && status == "PAID")
                        {
                            while (rdr.Read())
                            {
                                this.AdjustInventory(OrdersID, Int32.Parse(rdr[1].ToString()), Int32.Parse(rdr[0].ToString()));
                            }
                        }
                        else if (orignalStatus == "PAID" && status == "RFND")
                        {
                            while (rdr.Read())
                            {
                                this.AdjustInventory(OrdersID, -Int32.Parse(rdr[1].ToString()), Int32.Parse(rdr[0].ToString()));
                                this.AdjustOrderLine(OrdersID, Int32.Parse(rdr[0].ToString()), Int32.Parse(rdr[1].ToString()));
                            }
                            rdr.Close();
                        }
                        else if(orignalStatus == "PEND" && status == "CNCL")
                        {
                            while (rdr.Read())
                            {
                                this.AdjustOrderLine(OrdersID, Int32.Parse(rdr[0].ToString()), Int32.Parse(rdr[1].ToString()));
                            }
                        }
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

        public string AdjustOrderLine(int OrdersID, int ProductID, int qty)
        {
            string ret = string.Empty;
            try
            {
                this.Conntect();
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

        public DataSet OrderDetail(int orderID)
        {
            try
            {
                this.Conntect();
                da = new MySqlDataAdapter("select ord.OrdersID, ord.ProductID, pr.ProductName, pr.UnitPrice, ord.Quantity from orderline ord inner join product pr on ord.ProductID = pr.ProductID where OrdersID =" + orderID + ";", conn);
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

        public string SalesRecord(int ordersID)
        {
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
                // Build up Sales Record

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
    }
}
