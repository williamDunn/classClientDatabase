/*
 * Group 4: Dago, Juliana, Miguel, Will, Brian
 * Assignment 11
 * Add Class
 */

using System;
using System.Collections;
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
    public partial class AddClass : Form
    {
        private Form Main;

        ArrayList instructorIds = new ArrayList(); //----------

        public int clientID;
       
        public AddClass(Main mainForm, int ID)
        {
            this.Main = mainForm;
            this.clientID = ID;
            InitializeComponent();
        }

        private void classBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.tableAdapterManager.UpdateAll(this.cPSC285Group4DataSet);
        }

        private void AddClass_Load(object sender, EventArgs e)
        {
            this.clientTableAdapter.Fill(this.cPSC285Group4DataSet.Client);
            this.classTableAdapter.Fill(this.cPSC285Group4DataSet.Class);
            this.staffTableAdapter.Fill(this.cPSC285Group4DataSet.Staff);

            //Reads in instructors and loads them into comboBox on load

            String strConn = "Data Source=CISSQL;Initial Catalog=CPSC285Group4;Integrated Security=True";
            SqlConnection cnn = new SqlConnection(strConn);
            cnn.Open();

            String strSQL = "";
            strSQL = "SELECT Staff_Name, Staff_ID FROM Staff WHERE Staff_Position = 'Instructor'";

            SqlCommand cmdUpdates = new SqlCommand(strSQL, cnn);
            SqlDataReader read = cmdUpdates.ExecuteReader();

            this.cbInstructor.DataSource = null;
            this.cbInstructor.Items.Clear();

            while (read.Read())
            {
                cbInstructor.Items.Add(read[0]);
                instructorIds.Add(Convert.ToInt32(read[1]));
            }

            read.Close();

            //Reads in times and loads them into combobox on load without duplicates

            String strSQL2 = "";
            strSQL2 = "SELECT distinct Time FROM Class";

            SqlCommand cmdUpdates2 = new SqlCommand(strSQL2, cnn);
            SqlDataReader read2 = cmdUpdates2.ExecuteReader();

            this.cbTimeSlots.DataSource = null;
            this.cbTimeSlots.Items.Clear();

            while (read2.Read())
            {
                cbTimeSlots.Items.Add(read2[0]);
            }
            read2.Close();
        }

        private void staffBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.staffBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.cPSC285Group4DataSet);
        }

        private void btnAddClass_Click(object sender, EventArgs e)
        {
            int arrayPos = cbInstructor.SelectedIndex;
            String getInstructorId = instructorIds[arrayPos].ToString();
            int instructorId = Int32.Parse(getInstructorId);

            String strConn = "Data Source=CISSQL;Initial Catalog=CPSC285Group4;Integrated Security=True";
            SqlConnection cnn = new SqlConnection(strConn);

            SqlCommand newID = new SqlCommand("Select MAX(Class_ID) As Result From Class", cnn);

            SqlCommand cmdUpdates;
            int intRecordsAffected;

            String strSQL = "";
            
            strSQL =
                "Insert into [Class]" + "([Class_ID],[Instructor_ID],[Client_ID],[Date],[Time])" +
                "values ( '" + newClassID() + "', '" + instructorId + "', '" + int.Parse(clientID.ToString()) + "', '" + dpClassDate.Value.ToString("MM/dd/yyyy") + "', '" + cbTimeSlots.Text.ToString() + "')";
           
            MessageBox.Show(strSQL);

            //Checks for selected time input and validates (must be 9 - 17)
            String time;
            time = cbTimeSlots.Text.ToString();

            //Checks for day of week
            String day = dpClassDate.Value.DayOfWeek.ToString();

            String checkCount = "";


            //query to get count, if count is greater than 1, cannot insert row/class
            checkCount = "SELECT count(*) FROM Class WHERE Instructor_ID = " + instructorId + " and Client_ID = " + clientID + " and Date = "
                + dpClassDate.Value + " and Time = " + cbTimeSlots.Text;
            //check count and validate
                //code to check count and validate

            if ((day == "Sunday") || (day == "Saturday"))
            {
                MessageBox.Show("Error: No weekend sessions");
            }
            else
            {
                if (int.Parse(time.ToString()) >= 9 || int.Parse(time.ToString()) <= 17)
                {
                    try
                    {
                        cnn = new SqlConnection(strConn);
                        cnn.Open();

                        cmdUpdates = new SqlCommand();

                        cmdUpdates.Connection = cnn;
                        cmdUpdates.CommandType = CommandType.Text;
                        cmdUpdates.CommandText = strSQL;


                        intRecordsAffected = cmdUpdates.ExecuteNonQuery();
                        MessageBox.Show(intRecordsAffected + " records were updated");
                        cnn.Close();
                        this.tableAdapterManager.UpdateAll(cPSC285Group4DataSet);
                        this.clientTableAdapter.Fill(this.cPSC285Group4DataSet.Client);
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: Client not added, " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Error: Invalid time selection, must be 9 - 17");
                }
            }
        }

        public int newClassID()
        {
            int newID = 0;
            String strConnection = "Data Source=CISSQL;Initial Catalog=CPSC285Group4;Integrated Security=True";

            SqlConnection cnnDS = new SqlConnection(strConnection);

            try
            {
                cnnDS.Open();

                String strSQL = "SELECT Class_ID" +
                                    " FROM Class" +
                                    " WHERE Class_ID >= ALL (SELECT Class_ID" +
                                                                " FROM Class)";
                SqlCommand cmdReader = new SqlCommand(strSQL, cnnDS);
                SqlDataReader odrReader = cmdReader.ExecuteReader();

                if (odrReader.HasRows)
                {
                    while (odrReader.Read())
                    {
                        newID = int.Parse(odrReader[0].ToString()) + 1;
                    }
                }
                odrReader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return newID;
        }


       
        private void timeError_RightToLeftChanged(object sender, EventArgs e)
        {

        }
    }
}
