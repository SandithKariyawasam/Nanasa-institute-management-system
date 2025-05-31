using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace College_Management_System
{
    public partial class Teacher : Form
    {
        string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Nanasa_Management_System;Integrated Security=True;";
        int selectedRowId = -1;

        public Teacher()
        {
            InitializeComponent();
        }

        private void Teacher_Load(object sender, EventArgs e)
        {
            dateTimePicker1.MinDate = new DateTime(1950, 1, 1);
            dateTimePicker1.MaxDate = DateTime.Today;
            LoadTeachersFromDatabase();
        }

        private void LoadTeachersFromDatabase()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM vw_TeacherDetails";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void ClearFields()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            comboBox1.Text = "";
            dateTimePicker1.Value = DateTime.Today;
            comboBox2.Text = "";
            textBox5.Text = "";
            textBox3.Text = "";
            selectedRowId = -1;
            textBox1.ReadOnly = false;
        }

        private bool IsValidPhoneNumber(string phone)
        {
            if (!phone.All(char.IsDigit))
            {
                MessageBox.Show("Phone number must contain only digits.");
                return false;
            }
            if (phone.Length != 10)
            {
                MessageBox.Show("Phone number must be exactly 10 digits.");
                return false;
            }
            return true;
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || !int.TryParse(textBox1.Text, out _))
            {
                MessageBox.Show("Valid numeric ID is required.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Name is required.");
                return false;
            }
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select Gender.");
                return false;
            }
            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Please select Subject.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(textBox5.Text) || !IsValidPhoneNumber(textBox5.Text))
            {
                return false;
            }
            return true;
        }

        private void button1_Click(object sender, EventArgs e) // Add
        {
            if (!ValidateInputs()) return;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("sp_InsertTeacher", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", int.Parse(textBox1.Text));
                    cmd.Parameters.AddWithValue("@Name", textBox2.Text);
                    cmd.Parameters.AddWithValue("@Gender", comboBox1.Text);
                    cmd.Parameters.AddWithValue("@Dob", dateTimePicker1.Value.Date);
                    cmd.Parameters.AddWithValue("@Subject", comboBox2.Text);
                    cmd.Parameters.AddWithValue("@Phone", textBox5.Text);
                    cmd.Parameters.AddWithValue("@Location", textBox3.Text);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Teacher added successfully!");
                }

                LoadTeachersFromDatabase();
                ClearFields();
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                MessageBox.Show("A teacher with this ID already exists.");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("SQL error: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding teacher: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e) // Delete
        {
            if (selectedRowId == -1)
            {
                MessageBox.Show("Please select a teacher to delete.");
                return;
            }

            if (MessageBox.Show("Are you sure to delete this teacher?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    using (SqlCommand cmd = new SqlCommand("sp_DeleteTeacher", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", selectedRowId);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Teacher deleted successfully!");
                    }

                    LoadTeachersFromDatabase();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting teacher: " + ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e) // Update
        {
            if (selectedRowId == -1)
            {
                MessageBox.Show("Please select a teacher to update.");
                return;
            }

            if (!ValidateInputs()) return;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("sp_UpdateTeacher", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", selectedRowId);
                    cmd.Parameters.AddWithValue("@Name", textBox2.Text);
                    cmd.Parameters.AddWithValue("@Gender", comboBox1.Text);
                    cmd.Parameters.AddWithValue("@Dob", dateTimePicker1.Value.Date);
                    cmd.Parameters.AddWithValue("@Subject", comboBox2.Text);
                    cmd.Parameters.AddWithValue("@Phone", textBox5.Text);
                    cmd.Parameters.AddWithValue("@Location", textBox3.Text);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Teacher updated successfully!");
                }

                LoadTeachersFromDatabase();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating teacher: " + ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e) // Clear
        {
            ClearFields();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                selectedRowId = Convert.ToInt32(row.Cells["Id"].Value);
                textBox1.Text = row.Cells["Id"].Value.ToString();
                textBox2.Text = row.Cells["Name"].Value.ToString();
                comboBox1.Text = row.Cells["Gender"].Value.ToString();
                dateTimePicker1.Value = Convert.ToDateTime(row.Cells["Dob"].Value);
                comboBox2.Text = row.Cells["Subject"].Value.ToString();
                textBox5.Text = row.Cells["Phone"].Value.ToString();
                textBox3.Text = row.Cells["Location"].Value.ToString();

                textBox1.ReadOnly = true;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            MainForm Mform = new MainForm();
            Mform.Show();
            this.Hide();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label1_Click(object sender, EventArgs e) { }
    }
}
