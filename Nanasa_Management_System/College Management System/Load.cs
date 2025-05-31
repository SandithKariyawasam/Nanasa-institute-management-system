using System.Windows.Forms;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace College_Management_System
{
    public partial class Load : Form
    {
        public Load()
        {
            InitializeComponent();
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void Load_Load(object sender, EventArgs e)
        {
            // ✅ Test database connection when form loads
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    MessageBox.Show("✅ Connected to database: " + conn.Database, "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Database connection failed:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Load_Load_1(object sender, EventArgs e)
        {
            timer1.Start();
        }

        int startpos = 0;
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            startpos += 1;
            MyProgressBar.Value = startpos;
            if (MyProgressBar.Value == 60)
            {
                MyProgressBar.Value = 0;
                timer1.Stop();
                Login log = new Login();
                log.Show();
                this.Hide();
            }
        }

        private void MyProgressBar_Click(object sender, EventArgs e)
        {

        }
    }
}
