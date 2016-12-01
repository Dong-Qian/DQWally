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
using DQWally_POS;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace DQWally_POS
{
    /// <summary>
    /// Interaction logic for AddCustomer.xaml
    /// </summary>
    public partial class AddCustomer : Window
    {
        public AddCustomer()
        {
            InitializeComponent();        
        }

        private void Add_bn_Click(object sender, RoutedEventArgs e)
        {
            Pos pos = new Pos();
            string ret = string.Empty;
            int pNumber = 0;
            if (!string.IsNullOrEmpty(FName_tb.Text) || !string.IsNullOrEmpty(LName_tb.Text) || !string.IsNullOrEmpty(PNumber_tb.Text))
            {
                if (Int32.TryParse(PNumber_tb.Text.ToString(), out pNumber))
                {
                    ret = pos.AddCustomer(FName_tb.Text, LName_tb.Text, pNumber);
                    FName_tb.Text = "";
                    LName_tb.Text = "";
                    PNumber_tb.Text = "";
                }
                else
                {
                    ret = "Phone number needs to be Numbers!";
                }
            }
            else
            {
                ret = "Please fill in all fields";
            }
            Error_lb.Content = ret;
        }
    }
}
