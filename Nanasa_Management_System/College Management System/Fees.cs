using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace College_Management_System
{
    public partial class Fees : Form
    {
        string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Nanasa_Management_System;Integrated Security=True;";
        int index;

        public Fees()
        {
            InitializeComponent();
        }

        private void Fees_Load(object sender, EventArgs e)
        {
            LoadPaymentData();
        }

        private void LoadPaymentData()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM vw_PaymentDetails";
                SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
        }

        private void button1_Click(object sender, EventArgs e) // Add Payment
        {
            if (!ValidateInputs(out int number, out int amount))
                return;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_InsertPayment", con, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Number", number);
                            cmd.Parameters.AddWithValue("@stid", textBox2.Text.Trim());
                            cmd.Parameters.AddWithValue("@period", dateTimePicker1.Value);
                            cmd.Parameters.AddWithValue("@Subject", textBox5.Text.Trim());
                            cmd.Parameters.AddWithValue("@amount", amount);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show("Payment record added successfully.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error adding record: " + ex.Message);
                    }
                }
            }
            LoadPaymentData();
            ClearInputs();
        }

        private void button2_Click(object sender, EventArgs e) // Delete Payment
        {
            if (!int.TryParse(textBox1.Text, out int number))
            {
                MessageBox.Show("Please enter a valid Number to delete.");
                return;
            }

            var confirm = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (confirm != DialogResult.Yes) return;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_DeletePayment", con, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Number", number);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show("Payment record deleted successfully.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error deleting record: " + ex.Message);
                    }
                }
            }
            LoadPaymentData();
            ClearInputs();
        }

        private void button3_Click(object sender, EventArgs e) // Update Payment
        {
            if (!ValidateInputs(out int number, out int amount))
                return;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_UpdatePayment", con, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Number", number);
                            cmd.Parameters.AddWithValue("@stid", textBox2.Text.Trim());
                            cmd.Parameters.AddWithValue("@period", dateTimePicker1.Value);
                            cmd.Parameters.AddWithValue("@Subject", textBox5.Text.Trim());
                            cmd.Parameters.AddWithValue("@amount", amount);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show("Payment record updated successfully.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error updating record: " + ex.Message);
                    }
                }
            }
            LoadPaymentData();
            ClearInputs();
        }

        private void button4_Click(object sender, EventArgs e) // Navigate to MainForm
        {
            MainForm mainform = new MainForm();
            mainform.Show();
            this.Hide();
        }

        private void button4_Click_1(object sender, EventArgs e) // Clear Inputs
        {
            ClearInputs();
        }

        private void ClearInputs()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox5.Text = "";
            dateTimePicker1.Value = DateTime.Now;
            textBox4.Text = "";
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                index = e.RowIndex;
                DataGridViewRow row = dataGridView1.Rows[index];

                textBox1.Text = row.Cells["Number"].Value?.ToString();
                textBox2.Text = row.Cells["stid"].Value?.ToString();
                dateTimePicker1.Value = Convert.ToDateTime(row.Cells["period"].Value);
                textBox4.Text = row.Cells["amount"].Value?.ToString();
                textBox5.Text = row.Cells["Subject"].Value?.ToString();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void label7_Click(object sender, EventArgs e) => Application.Exit();

        private void label7_Click_1(object sender, EventArgs e) => Application.Exit();

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            MainForm Mform = new MainForm();
            Mform.Show();
            this.Hide();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }

        private void printPreviewDialog1_Load(object sender, EventArgs e) { }

        private void label4_Click(object sender, EventArgs e) { }

        private bool ValidateInputs(out int number, out int amount)
        {
            number = 0;
            amount = 0;

            if (!int.TryParse(textBox1.Text.Trim(), out number))
            {
                MessageBox.Show("Please enter a valid Number.");
                return false;
            }
            if (string.IsNullOrEmpty(textBox2.Text.Trim()))
            {
                MessageBox.Show("Please enter Student ID.");
                return false;
            }
            if (!int.TryParse(textBox4.Text.Trim(), out amount))
            {
                MessageBox.Show("Please enter a valid Amount.");
                return false;
            }
            if (string.IsNullOrEmpty(textBox5.Text.Trim()))
            {
                MessageBox.Show("Please enter Subject.");
                return false;
            }

            return true;
        }
    }
}
