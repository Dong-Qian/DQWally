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

        public string AddCustomer(string fName, string lName, int pNumber)
        {
            string ret = string.Empty;
            this.Conntect();
            try
            {
                string sql = "INSERT INTO customer (FirstName, LastName, PhoneNumber) VALUES ('" + fName + "','" + lName + "'," + pNumber + ");";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                ret = "Added Success";
            }
            catch(Exception ex)
            {
                ret = ex.Message;
            }
            conn.Close();
            return ret;
        }

        public string FindCustomer(string LName)
        {
            string ret = string.Empty;
            this.Conntect();
            try
            {
                string sql = "SELECT CustomerID FROM customer WHERE LastName ='" + LName + "';";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if(!rdr.Read())
                {
                    ret = "Customer Not Exist";
                }
                else
                {
                    ret = rdr[0].ToString();
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

        public string FindCustomer(long pNumber)
        {
            this.Conntect();
            string ret = string.Empty;
            try
            {
                string sql = "SELECT CustomerID FROM customer WHERE PhoneNUmber =" + pNumber + ";";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (!rdr.Read())
                {
                    ret = "Customer Not Exist";
                }
                else
                {
                    ret = rdr[0].ToString();
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

        public string FindProduct(int pID)
        {
            string ret = string.Empty;
            try
            {
                string sql = "SELECT ProductID FROM product WHERE ProductID =" + pID + ";";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                   
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
