using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace DQWally_POS
{
    class Pos
    {
        MySqlConnection conn;
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

        public string AddCustomer(string fName, string lName, string pNumber)
        {
            List<string> customer = new List<string>();
            string ret = string.Empty;
            this.Conntect();
            try
            {
                customer = FindCustomerByPhone(pNumber);
                ret = "Customer already Exist";
            }
            catch (Exception ex)
            {
                string sql = "INSERT INTO customer (FirstName, LastName, PhoneNumber) VALUES ('" + fName + "','" + lName + "','" + pNumber + "');";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                ret = "Added Success";
            }

            conn.Close();
            return ret;
        }

        public List<string> FindCustomerByPhone(string pNumber)
        {
            List<string> customer = new List<string>();
            this.Conntect();
            try
            {
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
                    throw new Exception("Customer is not Exist");
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
            this.Conntect();
            string ret = string.Empty;
            try
            {
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
            this.Conntect();
            string ret = string.Empty;
            try
            {
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
    }
}
