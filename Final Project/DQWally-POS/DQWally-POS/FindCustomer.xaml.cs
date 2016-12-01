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

namespace DQWally_POS
{
    /// <summary>
    /// Interaction logic for FindCustomer.xaml
    /// </summary>
    public partial class FindCustomer : Window
    {
        public FindCustomer()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string lName = LName_tb.Text;
            string PNumber = PNumber_tb.Text;
            long phone = 0;
            Pos pos = new Pos();
            string ret =string.Empty;
            if(!string.IsNullOrEmpty(lName) && string.IsNullOrEmpty(PNumber))
            {
                ret = pos.FindCustomer(lName);
                Error_lb.Content = ret;
            }
            else if(string.IsNullOrEmpty(lName) && !string.IsNullOrEmpty(PNumber))
            {
                if(Int64.TryParse(PNumber_tb.Text.ToString(), out phone))
                {
                    ret = pos.FindCustomer(phone);
                    Error_lb.Content = ret;
                }
                else
                {
                    Error_lb.Content = "Phone number must be a number";
                }
               
            }
            else
            {
                Error_lb.Content = "At least needs to fill one filed";
            }
        }
    }
}
