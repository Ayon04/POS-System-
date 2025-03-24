using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace POS_Software
{
    public partial class Cashier_login : Form
    {
        private OracleDataAccess oracleDataAccess;
        public Cashier_login()
        {
            InitializeComponent();

            oracleDataAccess = new OracleDataAccess();



        }

        private void exit_login_Click(object sender, EventArgs e)
        {
            Application.Exit();
           

        }

        private void login_cashier_Click(object sender, EventArgs e)
        {
            try
            {


                string Username  = user_name.Text;
                string Password = user_password.Text;



                if (string.IsNullOrWhiteSpace(user_name.Text) || string.IsNullOrWhiteSpace(user_password.Text))
                {
                    MessageBox.Show("Please enter values for all fields.", "Login Error");
                    return;
                }

               
               
                string loginsql = "SELECT COUNT(*) FROM Cashier where Username='" + Username + "' and Password='" + Password + "'";


                int UserCount = Convert.ToInt32(oracleDataAccess.ExecuteScalar(loginsql));
             


                if (UserCount > 0)
                {
                    MessageBox.Show("Login successfully!", "Success");

                  
                    

                    string userQuery = "SELECT Username,CashierID FROM Cashier WHERE Username = '" + Username+"' AND Password = '"+Password+"'";
                    DataTable result = oracleDataAccess.ExecuteQuery(userQuery);
                    string loggedInUser = result.Rows[0]["Username"].ToString();
                    int loggedInUserID = Convert.ToInt32(result.Rows[0]["CashierID"]);
                    POS_Dashboard pd = new POS_Dashboard(loggedInUser, loggedInUserID);
                    pd.Show();
                    this.Hide();

                }

                else
                {
                    MessageBox.Show("Login failed. Username or Password incorrect.", "Login Error");
                }

            }

            catch (Exception exp)
            {

                MessageBox.Show("Error: " + exp.Message, "Error");

            }


        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                user_password.UseSystemPasswordChar = false;
            }

            else
            {
                user_password.UseSystemPasswordChar = true;



            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Cashier_login_Load(object sender, EventArgs e)
        {

        }
    }
}
