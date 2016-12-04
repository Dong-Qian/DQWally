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

        public string FindProductPrice(int pID)
        {           
            string ret = string.Empty;
            try
            {
                this.Conntect();
                string sql = "SELECT Price FROM product WHERE ProductID =" + pID + ";";
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

        public string AddOrderLine(int OrdersID, int ProductID, int qty)
        {

            this.Conntect();
            string ret = string.Empty;

            conn.Close();
            return ret;
        }

        public string ChangeOrderStatus(int OrdersID, string status)
        {
           
            string ret = string.Empty;
            try
            {
                this.Conntect();
                string sql = "update orders set OrderStatus ='" + status + "' where OrdersID =" + OrdersID + ";";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                ret = "Order Status Changed";
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


    }
}
