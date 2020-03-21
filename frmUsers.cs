using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.Odbc;
namespace Products
{
    public partial class frmUsers : frmInheritance
    {
        string strUserName;
        string strPassword;
        string strFirstName;
        string strLastName;
        bool boolUserExist = false;
        int intUserID = 0;

        string strAccessConnectionString = "Driver={Microsoft Access Driver (*.mdb)}; Dbq=Products.mdb; Uid=Admin; Pwd=;";

        public frmUsers()
        {
            InitializeComponent();
        }

        private void frmUsers_Load(object sender, EventArgs e)
        {
            controlsLoad();
            loadUsers();
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            frmMain frmMain = new frmMain();
            frmMain.Show();
            this.Hide();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (btnCreate.Text == "Save")
            {
                if(txtUserName.Text =="")
                {
                    MessageBox.Show("User Name field cannot be left empty","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
                else if(txtPassword.Text=="")
                {
                    MessageBox.Show("Passowrd field cannot be left empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if(txtFirstName.Text=="")
                {
                    MessageBox.Show("First Name field cannot be left empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if(txtLastName.Text=="")
                {
                    MessageBox.Show("Last Name field cannot be left empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    checkIfUserExists();
                    if (boolUserExist == false)
                    {
                        createUsers();
                        controlsLoad();
                        clearTxtBoxes();
                        loadUsers();
                    }
                    else if (boolUserExist== true)
                    {
                        MessageBox.Show("User already exists","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                }
            }
            else if (btnCreate.Text == "Create")
            {
                controlsCreate();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            controlsEdit();
            editUsers();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            updateUser();
            controlsLoad();
            loadUsers();
            clearTxtBoxes();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            deleteUser();
            controlsLoad();
            clearTxtBoxes();
            loadUsers();
        }
        private void controlsLoad()
        {
            txtUserName.Enabled = false;
            txtPassword.Enabled = false;
            txtFirstName.Enabled = false;
            txtLastName.Enabled = false;

            cboUsers.Enabled = true;

            btnCreate.Enabled = true;
            btnEdit.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            btnReturn.Enabled = true;

            btnCreate.Text = "Create";
        }
        private void controlsCreate()
        {
            txtUserName.Enabled = true;
            txtPassword.Enabled = true;
            txtFirstName.Enabled = true;
            txtLastName.Enabled = true;

            cboUsers.Enabled = false;

            btnCreate.Enabled = true;
            btnEdit.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            btnReturn.Enabled = false;

            btnCreate.Text = "Save";
        }
        private void controlsEdit()
        {
            txtUserName.Enabled = true;
            txtPassword.Enabled = true;
            txtFirstName.Enabled = true;
            txtLastName.Enabled = true;

            cboUsers.Enabled = true;

            btnCreate.Enabled = false;
            btnEdit.Enabled = false;
            btnUpdate.Enabled = true;
            btnDelete.Enabled = true;
            btnReturn.Enabled = false;

        }
        private void clearTxtBoxes()
        {
            txtUserName.Text = "";
            txtPassword.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
        }
        private void loadUsers()
        {

            cboUsers.DataSource = null;
            cboUsers.Items.Clear();

            OdbcConnection OdbcConnection = new OdbcConnection();
            OdbcConnection.ConnectionString = strAccessConnectionString;

            string query = "select username from Users";

            OdbcCommand cmd = new OdbcCommand(query, OdbcConnection);

            OdbcConnection.Open();
            OdbcDataReader dr = cmd.ExecuteReader();
            AutoCompleteStringCollection UserCollection = new AutoCompleteStringCollection();

            while (dr.Read())
            {
                UserCollection.Add(dr.GetString(0));
            }

            cboUsers.DataSource = UserCollection;
            OdbcConnection.Close();
        }
        private void checkIfUserExists()
        {
            string query = "select * from Users where UserName ='" + txtUserName.Text + "'";

            OdbcConnection OdbcConnection = new OdbcConnection();
            OdbcCommand cmd;
            OdbcDataReader dr;

            OdbcConnection.ConnectionString = strAccessConnectionString;

            OdbcConnection.Open();
            cmd = new OdbcCommand(query, OdbcConnection);
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                boolUserExist = true;
            }

            dr.Close();
            OdbcConnection.Close();
            dr.Dispose();
            OdbcConnection.Dispose();
        }
        private void createUsers()
        {
            string query = "select * from users where ID=0";

            OdbcConnection OdbcConnection = new OdbcConnection();
            OdbcDataAdapter da = new OdbcDataAdapter(query,OdbcConnection);

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataRow dr;

            OdbcConnection.ConnectionString = strAccessConnectionString;

            da.Fill(ds, "Users");
            dt = ds.Tables["Users"];

            try
            {
                dr = dt.NewRow();
                dr["UserName"] = txtUserName.Text;
                dr["Password"] = txtPassword.Text;
                dr["FirstName"] = txtFirstName.Text;
                dr["LastName"] = txtLastName.Text;

                dt.Rows.Add(dr);
                OdbcCommandBuilder cmd = new OdbcCommandBuilder(da);

                da.Update(ds,"Users");
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message.ToString());
            }
            finally
            {
                OdbcConnection.Close();
                OdbcConnection.Dispose();
            }
        }
        private void editUsers()
        {
            string query = "select * from users where username ='" + cboUsers.Text + "'";


            OdbcConnection OdbcConnection = new OdbcConnection();
            OdbcCommand cmd;
            OdbcDataReader dr;

            OdbcConnection.ConnectionString = strAccessConnectionString;

            OdbcConnection.Open();
            cmd = new OdbcCommand(query, OdbcConnection);
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                intUserID = dr.GetInt32(0);
                txtUserName.Text = dr.GetString(1);
                txtPassword.Text = dr.GetString(2);
                txtFirstName.Text = dr.GetString(3);
                txtLastName.Text = dr.GetString(4);
            }

            dr.Close();
            OdbcConnection.Close();
            dr.Dispose();
            OdbcConnection.Dispose();
        }
        private void updateUser()
        {
            string query = "select * from users where ID=" + intUserID;

            OdbcConnection OdbcConnection = new OdbcConnection();
            OdbcDataAdapter da = new OdbcDataAdapter(query, OdbcConnection);
            DataSet ds = new DataSet();


            OdbcConnection.ConnectionString = strAccessConnectionString;
            da.FillSchema(ds, SchemaType.Source, "Users");
            da.Fill(ds, "Users");
            DataTable dt;

            dt = ds.Tables["Users"];
            DataRow dr;
            dr = dt.NewRow();


            try
            {
                dr = dt.Rows.Find(intUserID);
                dr.BeginEdit();

                dr["UserName"] = txtUserName.Text;
                dr["Password"] = txtPassword.Text;
                dr["FirstName"] = txtFirstName.Text;
                dr["LastName"] = txtLastName.Text;

                dr.EndEdit();

                OdbcCommandBuilder cmd = new OdbcCommandBuilder(da);
                da.Update(ds, "Users");

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message.ToString());
            }
            finally
            {
                OdbcConnection.Close();
                OdbcConnection.Dispose();
            }
        }
        private void deleteUser()
        {
            string query = "delete from users where ID =" + intUserID;


            OdbcConnection OdbcConnection = new OdbcConnection();
            OdbcCommand cmd;
            OdbcDataReader dr;

            OdbcConnection.ConnectionString = strAccessConnectionString;

            OdbcConnection.Open();

            cmd = new OdbcCommand(query, OdbcConnection);
            dr = cmd.ExecuteReader();

            if (dr.Read())
            {
            }

            dr.Close();
            OdbcConnection.Close();
            dr.Dispose();
            OdbcConnection.Dispose();
        }
    }
    
}
