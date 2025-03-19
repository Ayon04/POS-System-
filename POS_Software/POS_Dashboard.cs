using System;
using System.Data;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace POS_Software
{
    public partial class POS_Dashboard : Form
    {
        private OracleDataAccess db;

        public POS_Dashboard()
        {
            InitializeComponent();
            db = new OracleDataAccess();
            LoadProductsData();
        }

        private void LoadProductsData()
        {
            try
            {
                string query = "SELECT ProductID, ProductName, Category, Price, StockQuantity FROM Products order by ProductID";
                DataTable productsTable = db.ExecuteQuery(query);
                sales_view.DataSource = productsTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message);
            }
        }

        private void sales_view_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Handle cell click events if needed
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void sales_view_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
