using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ChatboxVido.Models;
using System.Windows.Data;
using System.Windows.Navigation;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChatboxVido
{
    /// <summary>
    /// Interaction logic for LoginVido.xaml
    /// </summary>
    public partial class LoginVido : Window
    {
        
        public LoginVido()
        {
            InitializeComponent();
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            var user = DataProvider.Instance.DB.Accounts.First().UserName;
            var pass = DataProvider.Instance.DB.Accounts.First().Password;
            string userText = User.Text.ToString();
            string PassText = Password.Password.ToString();
            if (user == userText || pass == PassText)
            {
                MessageBox.Show("đăng nhập thành công");
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                MessageBox.Show("đăng nhập thất bại");
            }
            this.Close();
        }
    }
}
