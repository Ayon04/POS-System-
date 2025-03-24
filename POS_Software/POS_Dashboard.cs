using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Printing;


namespace POS_Software
{
    public partial class POS_Dashboard : Form
    {
        private OracleDataAccess db;
        private DataTable prd = new DataTable();
        private PrintDocument printDocument = new PrintDocument();
        private PrintDialog printDialog = new PrintDialog();
        private PrintDocument printDocument1 = new PrintDocument();


        private int invoiceNumber;

        public POS_Dashboard(string usernames,int id )
        {
            InitializeComponent();
            db = new OracleDataAccess();
            LoadProductsData();
            SetReadOnlyFields();
            printDocument2.PrintPage += new PrintPageEventHandler(PrintReceipt); // Ensure this is assigned


            // string Username = username.Text;
            //Username = usernames;
            username.Text = usernames;
            cashierID.Text = id.ToString();
           
        }

        private void LoadProductsData()
        {
            try
            {
                string query = "SELECT ProductID, ProductName, Category, Price, StockQuantity FROM Products ORDER BY ProductID";
                DataTable productsTable = db.ExecuteQuery(query);
                sales_view.DataSource = productsTable;

                string querys = "SELECT * FROM Sales ORDER BY InvoiceNumber";
                DataTable salesTable = db.ExecuteQuery(querys);
                sales_view_2.DataSource = salesTable;

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
            username.ReadOnly = true;
            cashierID.ReadOnly = true;

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
            paymentMethoad.Text = " ";
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


                 DataTable dtt  = db.ExecuteQuery("SELECT * FROM Sales ORDER BY InvoiceNumber");
               
                sales_view_2.DataSource = dtt;





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

                int CashierID = Convert.ToInt32(cashierID.Text);
                string Username = username.Text;

        
                
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
                    INSERT INTO Sales 
                    (InvoiceNumber, PhoneNumber, ProductID, ProductName, Price, Quantity, TotalAmount, Discount, NetTotal, PaymentMethod, CashReceived, CashChange, CashierID, Username, CreatedAt)
                    VALUES 
                    (
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
                        {CashierID}, 
                        '{Username}', 
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

        private void exit_Click(object sender, EventArgs e)
        {
            // Show a confirmation message before exiting
            DialogResult result = MessageBox.Show("Are you sure you want to exit?", "Exit Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // If the user clicks "Yes", close the application
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

       
       
        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void update_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure a row is selected
                if (sales_view_2.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a sale record to update.", "Update Error");
                    return;
                }

                // Retrieve selected row data
                int invoiceNumber = Convert.ToInt32(sales_view_2.SelectedRows[0].Cells["InvoiceNumber"].Value);
                string newPhoneNumber = phoneNo.Text.Trim();
                string newPaymentMethod = paymentMethoad.Text.Trim();

                // Validate phone number
                if (!long.TryParse(newPhoneNumber, out _) || newPhoneNumber.Length != 11)
                {
                    MessageBox.Show("Phone number must be exactly 11 digits.", "Validation Error");
                    return;
                }

                // Validate payment method
                if (string.IsNullOrWhiteSpace(newPaymentMethod))
                {
                    MessageBox.Show("Please select a valid payment method.", "Validation Error");
                    return;
                }

                // Construct SQL update query
                string updateQuery = $@"
            UPDATE Sales
            SET PhoneNumber = '{newPhoneNumber}', 
                PaymentMethod = '{newPaymentMethod}'
            WHERE InvoiceNumber = {invoiceNumber}";

                // Execute update query
                int rowsAffected = db.ExecuteNonQuery(updateQuery);

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Sale record updated successfully!", "Success");

                    // Refresh sales_view_2
                    DataTable updatedSalesTable = db.ExecuteQuery("SELECT * FROM Sales ORDER BY InvoiceNumber");
                    sales_view_2.DataSource = updatedSalesTable;
                }
                else
                {
                    MessageBox.Show("Update failed. Please try again.", "Update Error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating record: " + ex.Message, "Update Error");
            }
        }

        private void sales_view_2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && sales_view_2.SelectedRows.Count > 0)
            {
                DataGridViewRow row = sales_view_2.SelectedRows[0];

                phoneNo.Text = row.Cells["PhoneNumber"].Value?.ToString() ?? "";
                productID.Text = row.Cells["ProductID"].Value?.ToString() ?? "";
                productName.Text = row.Cells["ProductName"].Value?.ToString() ?? "";
                price.Text = row.Cells["Price"].Value?.ToString() ?? "";
                quantity.Text = row.Cells["Quantity"].Value?.ToString() ?? "";
                totalAmount.Text = row.Cells["TotalAmount"].Value?.ToString() ?? "";
                discount.Text = row.Cells["Discount"].Value?.ToString() ?? "";
                netTotal.Text = row.Cells["NetTotal"].Value?.ToString() ?? "";
                paymentMethoad.Text = row.Cells["PaymentMethod"].Value?.ToString() ?? "";
                cashRecv.Text = row.Cells["CashReceived"].Value?.ToString() ?? "";
                cashChng.Text = row.Cells["CashChange"].Value?.ToString() ?? "";
                cashierID.Text = row.Cells["CashierID"].Value?.ToString() ?? "";
                username.Text = row.Cells["Username"].Value?.ToString() ?? "";
                inv.Text = row.Cells["InvoiceNumber"].Value?.ToString() ?? "";

                cashRecv.ReadOnly = true;
                quantity.ReadOnly = true;


            }
        }

        private void search_inv_Click(object sender, EventArgs e)
        {

            try
            {
                int search_invoices = Convert.ToInt32(search_invoice.Text);
                DataTable sqlls= db.ExecuteQuery(" SELECT * FROM Sales WHERE InvoiceNumber =" + search_invoices + "");

                sales_view_2.DataSource = sqlls;

                if (sqlls.Rows.Count > 0)
                {
                    sales_view_2.DataSource = sqlls;
                }
                else
                {
                    MessageBox.Show("Invoice Number : " + search_invoices + " does not exists in your records", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    searchPro.Clear();
                }

                db.Close();


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void reset_Click(object sender, EventArgs e)
        {
            // Clear Text Fields
            phoneNo.Text = "";
            productID.Text = "";
            productName.Text = "";
            price.Text = "";
            quantity.Text = "";
            totalAmount.Text = "";
            discount.Text = "";
            netTotal.Text = "";
            paymentMethoad.Text = "";
            cashRecv.Text = "";
            cashChng.Text = "";
            cashierID.Text = "";
            username.Text = "";

            // If using DataGridView selection, deselect rows
            if (sales_view_2.SelectedRows.Count > 0)
            {
                sales_view_2.ClearSelection();
            }

            // Show a confirmation message (Optional)
            MessageBox.Show("Form Reset Successfully!", "Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void delete_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure a row is selected in sales_view_2
                if (sales_view_2.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a sale record to delete.", "Delete Error");
                    return;
                }

                // Retrieve selected row data
                int invoiceNumber = Convert.ToInt32(sales_view_2.SelectedRows[0].Cells["InvoiceNumber"].Value);
                int productID = Convert.ToInt32(sales_view_2.SelectedRows[0].Cells["ProductID"].Value);
                int quantity = Convert.ToInt32(sales_view_2.SelectedRows[0].Cells["Quantity"].Value);

                // Confirm deletion
                var result = MessageBox.Show("Are you sure you want to delete this sale?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    // Step 1: Restore the stock quantity in Products table
                    string updateStockQuery = $@"
                    UPDATE Products
                    SET StockQuantity = StockQuantity + {quantity}
                    WHERE ProductID = {productID}";

                    db.ExecuteQuery(updateStockQuery);  // Execute update query to restore stock

                    // Step 2: Delete the sale record from the Sales table
                    string deleteSaleQuery = $@"
                    DELETE FROM Sales
                    WHERE InvoiceNumber = {invoiceNumber}";

                    int rowsAffected = db.ExecuteNonQuery(deleteSaleQuery);

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Sale record deleted successfully!", "Success");

                        // Step 3: Refresh the sales view
                        DataTable updatedSalesTable = db.ExecuteQuery("SELECT * FROM Sales ORDER BY InvoiceNumber");
                        sales_view_2.DataSource = updatedSalesTable;
                    }
                    else
                    {
                        MessageBox.Show("Error deleting sale record. Please try again.", "Delete Error");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting record: " + ex.Message, "Delete Error");
            }
        }


        private void PrintReceipt(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font font = new Font("Arial", 10);
            int startX = 10;
            int startY = 20;
            int offset = 40;

            g.DrawString("", new Font("Arial", 14, FontStyle.Bold), Brushes.Black, startX, startY);
            g.DrawString("=======================================", font, Brushes.Black, startX, startY + 20);

            g.DrawString($"Invoice No: {inv.Text}", font, Brushes.Black, startX, startY + offset);
            offset += 20;
            g.DrawString($"Cashier: {username.Text}", font, Brushes.Black, startX, startY + offset);
            offset += 20;
            g.DrawString($"Date: {DateTime.Now}", font, Brushes.Black, startX, startY + offset);
            offset += 40;

            g.DrawString("Product      Qty   Price   Total", font, Brushes.Black, startX, startY + offset);
            offset += 20;
            g.DrawString("---------------------------------------", font, Brushes.Black, startX, startY + offset);
            offset += 20;

            // Ensure input fields are not null or empty
            if (!string.IsNullOrEmpty(productName.Text) && !string.IsNullOrEmpty(quantity.Text) && !string.IsNullOrEmpty(price.Text))
            {
                string prodName = productName.Text;
                int qty = Convert.ToInt32(quantity.Text);
                decimal priceValue = Convert.ToDecimal(price.Text);
                decimal total = qty * priceValue;

                g.DrawString($"{prodName}   {qty}   {priceValue}   {total}", font, Brushes.Black, startX, startY + offset);
                offset += 30;
            }

            g.DrawString("---------------------------------------", font, Brushes.Black, startX, startY + offset);
            offset += 20;
            g.DrawString($"Total Amount: {totalAmount.Text}", font, Brushes.Black, startX, startY + offset);
            offset += 20;
            g.DrawString($"Discount: {discount.Text}%", font, Brushes.Black, startX, startY + offset);
            offset += 20;
            g.DrawString($"Net Total: {netTotal.Text}", font, Brushes.Black, startX, startY + offset);
            offset += 20;
            g.DrawString($"Cash Received: {cashRecv.Text}", font, Brushes.Black, startX, startY + offset);
            offset += 20;
            g.DrawString($"Change: {cashChng.Text}", font, Brushes.Black, startX, startY + offset);
            offset += 40;

            g.DrawString("Thank you for shopping with us!", font, Brushes.Black, startX, startY + offset);
        }

        private void bill_Click(object sender, EventArgs e)
        {
            // printPreviewDialog1.Document = printDocument2;
            // printPreviewDialog1.ShowDialog();

            //printPreviewDialog1.Document = printDocument2;
            //printPreviewDialog1.ShowDialog();

            // Print after preview
            // printDialog.Document = printDocument2;
            ///if (printDialog.ShowDialog() == DialogResult.OK)
            //  {
            //printDocument2.Print();
            //}
            printPreviewDialog1.Document = printDocument2;
            printPreviewDialog1.ShowDialog();

        }

        private void printDocument2_PrintPage(object sender, PrintPageEventArgs e)//000000
        {
            e.Graphics.DrawString("POS System", new Font("Microsoft Sans Serif", 18, FontStyle.Bold), Brushes.Black, new Point(10, 10));

        }



        private void printPreviewDialog1_Load(object sender, EventArgs e)
        {

        }

        

    }
}
