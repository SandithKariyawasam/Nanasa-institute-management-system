using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace College_Management_System
{
    public partial class Department : Form
    {
        DataTable classTable = new DataTable();
        int index = -1;
        int selectedClassId = -1;
        string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Nanasa_Management_System;Integrated Security=True";

        public Department()
        {
            InitializeComponent();
        }

        private void Department_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            classTable.Clear();
            string query = "SELECT * FROM vw_ClassDetails";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
            {
                adapter.Fill(classTable);
                dataGridView1.DataSource = classTable;
            }
        }

        private void button1_Click(object sender, EventArgs e) // Add
        {
            if (string.IsNullOrWhiteSpace(txtGrade.Text) ||
                string.IsNullOrWhiteSpace(txtSubjects.Text) ||
                string.IsNullOrWhiteSpace(txtTeacherId.Text))
            {
                MessageBox.Show("Please fill all required fields.");
                return;
            }

            try
            {
                if (!int.TryParse(txtTeacherId.Text.Trim(), out int teacherId))
                {
                    MessageBox.Show("Invalid Teacher ID. Please enter a numeric value.");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("AddClassWithTeacher", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Grade", txtGrade.Text);
                    cmd.Parameters.AddWithValue("@Subjects", txtSubjects.Text);
                    cmd.Parameters.AddWithValue("@Date", dtpDate.Value.Date);
                    cmd.Parameters.AddWithValue("@Time", dtpTime.Value.TimeOfDay);
                    cmd.Parameters.AddWithValue("@TeacherId", teacherId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Class and teacher added successfully.");
                LoadData();
                ClearFields();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("SQL Error: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected Error: " + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e) // Edit
        {
            if (selectedClassId == -1)
            {
                MessageBox.Show("Please select a class to edit.");
                return;
            }

            try
            {
                if (!int.TryParse(txtTeacherId.Text.Trim(), out int teacherId))
                {
                    MessageBox.Show("Invalid Teacher ID. Please enter a numeric value.");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("UpdateClass", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ClassId", selectedClassId);
                    cmd.Parameters.AddWithValue("@Grade", txtGrade.Text);
                    cmd.Parameters.AddWithValue("@Subjects", txtSubjects.Text);
                    cmd.Parameters.AddWithValue("@Date", dtpDate.Value.Date);
                    cmd.Parameters.AddWithValue("@Time", dtpTime.Value.TimeOfDay);
                    cmd.Parameters.AddWithValue("@TeacherId", teacherId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Class updated successfully.");
                LoadData();
                ClearFields();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("SQL Error: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update failed: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e) // Delete
        {
            if (selectedClassId == -1)
            {
                MessageBox.Show("Please select a class to delete.");
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this class?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("DeleteClass", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ClassId", selectedClassId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Class deleted successfully.");
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Delete failed: " + ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e) // Clear
        {
            ClearFields();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                index = e.RowIndex;
                DataGridViewRow row = dataGridView1.Rows[index];

                selectedClassId = Convert.ToInt32(row.Cells["ClassId"].Value);
                txtGrade.Text = row.Cells["Grade"].Value?.ToString();
                txtSubjects.Text = row.Cells["Subjects"].Value?.ToString();

                if (DateTime.TryParse(row.Cells["Date"].Value?.ToString(), out DateTime date))
                    dtpDate.Value = date;
                else
                    dtpDate.Value = DateTime.Now;

                if (DateTime.TryParse(row.Cells["Time"].Value?.ToString(), out DateTime time))
                    dtpTime.Value = time;
                else
                    dtpTime.Value = DateTime.Now;

                // This must be the TeacherId (int), NOT TeacherName
                txtTeacherId.Text = row.Cells["TeacherId"].Value?.ToString();
            }
        }

        private int GetTeacherIdByName(string teacherName)
        {
            string query = "SELECT Id FROM Teacher WHERE Name = @Name";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Name", teacherName);
                conn.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
        }

        private void ClearFields()
        {
            txtGrade.Text = "";
            txtSubjects.Text = "";
            txtTeacherId.Text = "";
            dtpDate.Value = DateTime.Now;
            dtpTime.Value = DateTime.Now;
            selectedClassId = -1;
            index = -1;
        }

        private void label7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            MainForm Mform = new MainForm();
            Mform.Show();
            this.Hide();
        }
    }
}
