using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace College_Management_System
{
    public partial class Student : Form
    {
        SqlConnection con;
        SqlDataAdapter adapter;
        DataTable studentTable;
        int index;

        public Student()
        {
            InitializeComponent();
            con = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=Nanasa_Management_System;Integrated Security=True");
        }

        private void Student_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                adapter = new SqlDataAdapter("SELECT * FROM vw_Students", con); // Uses SQL View
                studentTable = new DataTable();
                adapter.Fill(studentTable);
                dataGridView1.DataSource = studentTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)  // Add Student
        {
            try
            {
                if (!ValidateInput()) return;

                using (SqlCommand cmd = new SqlCommand("AddStudent", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    AddParameters(cmd);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    MessageBox.Show("Student added successfully!");
                    LoadData();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding student: " + ex.Message);
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)  // Delete Student
        {
            if (index >= 0 && index < dataGridView1.Rows.Count)
            {
                try
                {
                    string id = dataGridView1.Rows[index].Cells["Id"].Value.ToString();
                    using (SqlCommand cmd = new SqlCommand("DeleteStudent", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", id);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();

                        MessageBox.Show("Student deleted successfully!");
                        LoadData();
                        ClearFields();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting student: " + ex.Message);
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
            else
            {
                MessageBox.Show("Please select a valid student to delete.");
            }
        }

        private void button3_Click(object sender, EventArgs e)  // Update Student
        {
            try
            {
                if (!ValidateInput()) return;

                using (SqlCommand cmd = new SqlCommand("UpdateStudent", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    AddParameters(cmd);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    MessageBox.Show("Student updated successfully!");
                    LoadData();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating student: " + ex.Message);
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void AddParameters(SqlCommand cmd)
        {
            string firstName = textBoxFirstName.Text.Trim();
            string lastName = textBoxLastName.Text.Trim();
            DateTime dob = dateTimePicker1.Value.Date;

            cmd.Parameters.AddWithValue("@Id", textBox1.Text.Trim());
            cmd.Parameters.AddWithValue("@Firstname", firstName);
            cmd.Parameters.AddWithValue("@Lastname", lastName);
            cmd.Parameters.AddWithValue("@Gender", comboBox1.Text);
            cmd.Parameters.AddWithValue("@DOB", dob);
            cmd.Parameters.AddWithValue("@Grade", comboBox2.Text); // using Grade
            cmd.Parameters.AddWithValue("@Phone", textBox5.Text.Trim());
            cmd.Parameters.AddWithValue("@Fee", decimal.Parse(textBox3.Text.Trim()));
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Student ID is required.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(textBoxFirstName.Text) || string.IsNullOrWhiteSpace(textBoxLastName.Text))
            {
                MessageBox.Show("First Name and Last Name are required.");
                return false;
            }
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select Gender.");
                return false;
            }
            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Please select Grade.");
                return false;
            }
            if (!decimal.TryParse(textBox3.Text.Trim(), out decimal fee))
            {
                MessageBox.Show("Please enter a valid Fee.");
                return false;
            }
            string phone = textBox5.Text.Trim();
            if (!phone.All(char.IsDigit) || phone.Length != 10)
            {
                MessageBox.Show("Phone number must be exactly 10 digits.");
                return false;
            }
            return true;
        }

        private void button4_Click(object sender, EventArgs e)  // Clear
        {
            ClearFields();
        }

        private void ClearFields()
        {
            textBox1.Text = "";
            textBoxFirstName.Text = "";
            textBoxLastName.Text = "";
            comboBox1.SelectedIndex = -1;
            dateTimePicker1.Value = DateTime.Now;
            comboBox2.SelectedIndex = -1;
            textBox5.Text = "";
            textBox3.Text = "";
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            index = e.RowIndex;
            if (index >= 0 && index < dataGridView1.Rows.Count)
            {
                DataGridViewRow row = dataGridView1.Rows[index];
                textBox1.Text = row.Cells["Id"].Value?.ToString();
                var fullName = row.Cells["FullName"].Value?.ToString()?.Split(' ');
                textBoxFirstName.Text = fullName?[0];
                textBoxLastName.Text = fullName?.Length > 1 ? fullName[1] : "";
                comboBox1.Text = row.Cells["Gender"].Value?.ToString();
                comboBox2.Text = row.Cells["Grade"].Value?.ToString();
                textBox5.Text = row.Cells["Phone"].Value?.ToString();
                textBox3.Text = row.Cells["Fee"].Value?.ToString();
            }
        }

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

        // Unused event handlers
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void label11_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void label12_Click(object sender, EventArgs e) { }
    }
}
