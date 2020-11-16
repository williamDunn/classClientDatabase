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
    public partial class AddClient : Form
    {
        private Form Main;
       

        public AddClient(Main mainForm)
        {
            this.Main = mainForm;
            InitializeComponent();
        }

        private void btnAddClient_Click(object sender, EventArgs e)
        {
            // In the main form, the user selects the option to add a new client

            String strConn = "Data Source=CISSQL;Initial Catalog=CPSC285Group4;Integrated Security=True";
            SqlConnection cnn = new SqlConnection(strConn);
            SqlCommand cmdUpdates;
            int intRecordsAffected;

            SqlCommand newID = new SqlCommand("Select MAX(Client_ID) As Result From Client", cnn);
            newID.CommandType = CommandType.Text;
            newID.Connection.Open();

            //This SQL statement takes the information that was input from the form, and adds it to the database. 
            // A unique id is added to the client once the information is set. 
            String strSQL = " ";
            strSQL =

           "Insert into [Client] " + "([Client_ID],[Client_Name], [Client_DOB], [Client_PhoneNumber], [Client_MaritalStatus])" +
                " values ( " + makeNewID() + " ,'" + txtClientName.Text.ToString() + "' , '" + dpClientDOB.Value.ToString("MM/dd/yyyy") + "' , " + txtClientPhoneNumber.Text.ToString() + " , " + isMarried() + ")";
            MessageBox.Show(strSQL);

            try
            {

                cnn = new SqlConnection(strConn);
                cnn.Open();

                cmdUpdates = new SqlCommand();


                cmdUpdates.Connection = cnn;
                cmdUpdates.CommandType = CommandType.Text;
                cmdUpdates.CommandText = strSQL;
                SqlDataReader dr = newID.ExecuteReader();

                intRecordsAffected = cmdUpdates.ExecuteNonQuery();
                MessageBox.Show(intRecordsAffected + " records were updated");
                //Closes Add Client Form if client was suicessfully added
                if (intRecordsAffected == 1)
                {
                    this.Close();
                }
                cnn.Close();
                this.tableAdapterManager.UpdateAll(cPSC285Group4DataSet);
                this.clientTableAdapter.Fill(this.cPSC285Group4DataSet.Client);

            }

            catch (Exception ex)
            {
                MessageBox.Show("Error: Client not added, " + ex.Message);
            }


        }
        //makeNewID gets the highest ID number and adds one to it so the id numbers are sequential and there are no duplicates
        public static int makeNewID()
        {
            int newID = 0;
            String strConnection = "Data Source=CISSQL;Initial Catalog=CPSC285Group4;Integrated Security=True";

            SqlConnection cnnDS = new SqlConnection(strConnection);

            try
            {
                cnnDS.Open();

                String strSQL = "SELECT Client_ID" +
                                    " FROM Client" +
                                    " WHERE Client_ID >= ALL (SELECT Client_ID" +
                                                                " FROM Client)";
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
        public int isMarried()
        {
            int marriedVal = 0;
            if (ckboxClientMaritalStatus.Checked)
            {
                marriedVal = 1;
            }
            else
            {
                marriedVal = 0;
            }
            return marriedVal;
        }

        private void clientBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.clientBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.cPSC285Group4DataSet);

        }

        private void AddClient_Load(object sender, EventArgs e)
        {
            this.clientTableAdapter.Fill(this.cPSC285Group4DataSet.Client);
        }
    }
}



