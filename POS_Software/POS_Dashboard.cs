using System;
using System.Data;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace POS_Software
{
    public partial class POS_Dashboard : Form
    {
        private OracleDataAccess db;
        private DataTable prd = new DataTable();

        public POS_Dashboard()
        {
            InitializeComponent();
            db = new OracleDataAccess();
            LoadProductsData();
            SetReadOnlyFields();
        }

        private void LoadProductsData()
        {
            try
            {
                string query = "SELECT ProductID, ProductName, Category, Price, StockQuantity FROM Products ORDER BY ProductID";
                DataTable productsTable = db.ExecuteQuery(query);
                sales_view.DataSource = productsTable;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message);
            }
        }

        private void SetReadOnlyFields()
        {
            productID.ReadOnly = true;
            productName.ReadOnly = true;
            price.ReadOnly = true;
            totalAmount.ReadOnly = true;
            netTotal.ReadOnly = true;
            cashChng.ReadOnly = true;
            paymentMethoad.DropDownStyle = ComboBoxStyle.DropDownList;
            dateTime.Enabled = false;

        }


            private void sales_view_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (sales_view.SelectedRows.Count > 0)
            {
                productID.Text = sales_view.SelectedRows[0].Cells["ProductID"].Value.ToString();
                productName.Text = sales_view.SelectedRows[0].Cells["ProductName"].Value.ToString();
                price.Text = sales_view.SelectedRows[0].Cells["Price"].Value.ToString();
            }
        }



        private void insert_Click(object sender, EventArgs e)
        {
            
        }

        private void clear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            productID.Clear();
            productName.Clear();
            price.Clear();
            quantity.Clear();
            totalAmount.Clear();
            phoneNo.Clear();
            discount.Clear();
            netTotal.Clear();
            paymentMethoad.Text = string.Empty;
            cashRecv.Clear();
            cashChng.Clear();
            
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

        private void sales_view_CellContentClick_2(object sender, DataGridViewCellEventArgs e)
        {
            if (sales_view.SelectedRows.Count > 0)
            {

                productID.Text = sales_view.SelectedRows[0].Cells["ProductID"].Value.ToString();
                productName.Text = sales_view.SelectedRows[0].Cells["ProductName"].Value.ToString();
                price.Text = sales_view.SelectedRows[0].Cells["Price"].Value.ToString();


            }
        }

        private void paymentMethoad_SelectedIndexChanged(object sender, EventArgs e)
        {
          
        }

        private void load_Click(object sender, EventArgs e)
        {


            try
            {
                // int EmpSearch = Convert.ToInt32(txtSearch.Text);
                DataTable empdt = db.ExecuteQuery("SELECT ProductID, ProductName, Category, Price, StockQuantity FROM Products ORDER BY ProductID");

                sales_view.DataSource = empdt;

                db.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void insert_Click_1(object sender, EventArgs e)
        {
            try
            {


                int ProductID = Convert.ToInt32(productID.Text);
                string ProductName = productName.Text;
                int Price = Convert.ToInt32(price.Text);
                int Quantity = Convert.ToInt32(quantity.Text);
                string PhoneNo = phoneNo.Text;
                int Discount = Convert.ToInt32(discount.Text);
                string PaymentMethod = paymentMethoad.Text;
                int CashRecv = Convert.ToInt32(cashRecv.Text);
                DateTime dateTime = DateTime.Now;
                int TotalAmount = Price * Quantity;
                int disAmt = (TotalAmount * Discount) / 100;
                int NetTotal = TotalAmount - disAmt; // Net Total calculation

                int CashChng = CashRecv - NetTotal; // Cash Change calculation

                // **Validation**
                if (string.IsNullOrWhiteSpace(PhoneNo) ||
                    string.IsNullOrWhiteSpace(PaymentMethod))

                {
                    MessageBox.Show("All fields are required.", "Validation Error");
                    return;
                }

                if (!long.TryParse(PhoneNo, out _) || PhoneNo.Length != 11)
                {
                    MessageBox.Show("Phone number must be exactly 11 digits.", "Validation Error");
                    return;
                }

                if (Quantity < 1)
                {
                    MessageBox.Show("Quantity must be at least 1.", "Validation Error");
                    return;
                }

                if (Discount < 0 || Discount > 100)
                {
                    MessageBox.Show("Discount must be a number between 0 and 100.", "Validation Error");
                    return;
                }



                if (CashRecv < NetTotal)
                {
                    MessageBox.Show("Insufficient cash received.", "Validation Error");
                    return;
                }


                cashChng.Text = CashChng.ToString();
                totalAmount.Text = TotalAmount.ToString();
                netTotal.Text = NetTotal.ToString();
                cashChng.Text = CashChng.ToString();


                string invoiceQuery = "SELECT COALESCE(MAX(InvoiceNumber), 102030) + 1 FROM Sales";
                DataTable invoiceTable = db.ExecuteQuery(invoiceQuery);
                int invoiceNumber = Convert.ToInt32(invoiceTable.Rows[0][0]);
                string dateTimeFormatted = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");  // Format DateTime

                
                string checkStockQuery = $"SELECT StockQuantity FROM Products WHERE ProductID = {ProductID}";
                int availableStock = Convert.ToInt32(db.ExecuteScalar(checkStockQuery) ?? 0);

                if (availableStock < Quantity)
                {
                    MessageBox.Show("Error: Not enough stock available!");
                }
                else
                {

                    string insertQuery = $@"
                INSERT INTO Sales (InvoiceNumber, PhoneNumber, ProductID, ProductName, Price, Quantity, TotalAmount, Discount, NetTotal, PaymentMethod, CashReceived, CashChange, CashierID, Username, CreatedAt)
                VALUES (
                    {invoiceNumber}, 
                    '{PhoneNo}', 
                    {ProductID}, 
                    '{ProductName}', 
                    {Price}, 
                    {Quantity}, 
                    {TotalAmount}, 
                    {Discount}, 
                    {NetTotal}, 
                    '{PaymentMethod}', 
                    {CashRecv}, 
                    {CashChng}, 
                    22023010, 
                    'akash09', 
                    TO_DATE('{dateTimeFormatted}', 'YYYY-MM-DD HH24:MI:SS')
                )";
                    // db.ExecuteQuery(insertQuery); // Execute insert query

                    // Update stock quantity after sale
                    string updateStockQuery = $@"
                UPDATE Products
                SET StockQuantity = StockQuantity - {Quantity}
                WHERE ProductID = {ProductID} AND StockQuantity >= {Quantity}";

                    db.ExecuteQuery(updateStockQuery); // Execute update query


                    int affectedRows = db.ExecuteNonQuery(insertQuery);

                    if (affectedRows > 0)
                    {
                        MessageBox.Show("Transaction successful!", "Success");
                        //ClearFields();
                    }
                    else
                    {
                        MessageBox.Show("Transaction failed.", "Error");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error to add try Again " + ex.Message);
            }


        }

        private void search_Click(object sender, EventArgs e)
        {
            try
            {
                int searchPros = Convert.ToInt32(searchPro.Text);
                DataTable sqll = db.ExecuteQuery(" SELECT * FROM Products WHERE ProductID =" + searchPros + ""); 

                sales_view.DataSource = sqll;

                if (sqll.Rows.Count > 0)
                {
                    sales_view.DataSource = sqll;
                }
                else
                {
                    MessageBox.Show("Product ID : "+searchPros+" does not exists in your records", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    searchPro.Clear();
                }

                db.Close();


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
