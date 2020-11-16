using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace A11RAMIREZD
{
    public partial class Main : Form
    {
        private String addToDate = "";

        public Main()
        {
            InitializeComponent();
        }
        public int iD { get { return int.Parse(client_IDTextBox.Text); } set { client_IDTextBox.Text = value.ToString(); } }


        private void Main_Load(object sender, EventArgs e)
        {
            this.classTableAdapter.Fill(this.cPSC285Group4DataSet.Class);

            try
            {
                this.clientTableAdapter.Fill(this.cPSC285Group4DataSet.Client);

                this.staffTableAdapter.Fill(this.cPSC285Group4DataSet.Staff);

                showClassInfoQuery(addToDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
            }

            // adds client to combobox drop down

            String strConn = "Data Source=CISSQL;Initial Catalog=CPSC285Group4;Integrated Security=True";
            SqlConnection cnn = new SqlConnection(strConn);
            cnn.Open();

            String strSQL = "";
            strSQL = "SELECT Client_Name FROM Client";

            SqlCommand cmdUpdates = new SqlCommand(strSQL, cnn);
            SqlDataReader read = cmdUpdates.ExecuteReader();

            this.client_NameComboBox.DataSource = null;
            this.client_NameComboBox.Items.Clear();

            while (read.Read())
            {
                client_NameComboBox.Items.Add(read[0]);
            }

            read.Close();


        }

        private void staffBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.staffBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.cPSC285Group4DataSet);
        }

        private void staffBindingNavigatorSaveItem_Click_1(object sender, EventArgs e)
        {
            this.Validate();
            this.staffBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.cPSC285Group4DataSet);
        }

        private void btnClass_Click(object sender, EventArgs e)
        {
            
            AddClass form2 = new AddClass(this,iD);
           
            form2.ShowDialog();

            this.staffTableAdapter.Fill(this.cPSC285Group4DataSet.Staff);
        }

        private void btnAddClient_Click(object sender, EventArgs e)
        {
            
            AddClient form3 = new AddClient(this);
            form3.Show();
            
            this.clientTableAdapter.Fill(this.cPSC285Group4DataSet.Client);
        }

        public void showClassInfoQuery(String addToDate)
        {
            
            String strConnection = "Data Source=CISSQL;Initial Catalog=CPSC285Group4;Integrated Security=True";
            String[] info = new String[20];

            SqlConnection cnnDS = new SqlConnection(strConnection);

            try
            {
                cnnDS.Open();

                String strSQL = "SELECT C.[Class_ID], CL.[Client_Name], C.[Time], CONVERT(varchar, C.[Date], 101)" +
                                    " FROM (Class as C INNER JOIN Staff as S on C.[Instructor_ID] = S.[Staff_ID]) INNER JOIN Client as CL on CL.[Client_ID] = C.[Client_ID]" +
                                    " WHERE Instructor_ID = " + staff_IDTextBox.Text + addToDate;
                SqlCommand cmdReader = new SqlCommand(strSQL, cnnDS);
                SqlDataReader odrReader = cmdReader.ExecuteReader();

                int i = 0;
                if (odrReader.HasRows)
                {
                    while (odrReader.Read())
                    {
                        info[i] = String.Format("{0,-4} {1,-20} {2,-4} {3,-12}", odrReader[0].ToString(), odrReader[1].ToString(), odrReader[2].ToString(), odrReader[3].ToString());
                        i++;
                    }
                }
                odrReader.Close();
              
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            lstBoxClasses.DataSource = info;

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rdBtnCurrent_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtnCurrent.Checked)
            {
                showClassInfoQuery(" AND CONVERT(date, C.[Date]) = CONVERT(date, GETDATE())");
            }
        }

        private void rdBtnNextWeek_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtnNextWeek.Checked)
            {
                showClassInfoQuery(" AND C.[Date] BETWEEN CONVERT(date, GETDATE()) AND CONVERT(date, (GETDATE() + 7))");
            }
        }

        private void rdBtnNextMonth_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtnNextMonth.Checked)
            {
                showClassInfoQuery(" AND C.[Date] BETWEEN CONVERT(date, GETDATE()) AND CONVERT(date, (GETDATE() + 31))");
            }
        }

        private void staffBindingSource_CurrentChanged_1(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(staff_PositionTextBox.Text) && staff_PositionTextBox.Text == "Instructor")
            {
                rdBtnCurrent.Visible = true;
                rdBtnNextWeek.Visible = true;
                rdBtnNextMonth.Visible = true;
                lstBoxClasses.Visible = true;
                rdBtnCurrent.Checked = false;
                rdBtnNextWeek.Checked = false;
                rdBtnNextMonth.Checked = false;
            }
            else
            {
                rdBtnCurrent.Visible = false;
                rdBtnNextWeek.Visible = false;
                rdBtnNextMonth.Visible = false;
                lstBoxClasses.Visible = false;
                rdBtnCurrent.Checked = false;
                rdBtnNextWeek.Checked = false;
                rdBtnNextMonth.Checked = false;
            }
            lstBoxClasses.DataSource = null;
        }
    }
}
