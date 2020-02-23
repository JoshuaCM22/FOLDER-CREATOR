using System;
using System.IO;
using System.Windows.Forms;
using MetroFramework.Forms;
using System.Threading;

namespace FOLDER_CREATOR // Created by: Joshua C. Magoliman
{
    public partial class Form_Main : MetroForm
    {
        private bool isbtnCreateAlreadyClicked = false;
        string folderName, txtStartFromStringHolder, txtUntilStringHolder = "";
        int userFrom, userUntil = 0;
        private Thread workerThread1 = null;

        public Form_Main()
        {
            InitializeComponent();
        }
        private void SearchingPath()
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.ShowDialog();
            txtPath.Text = folderBrowserDialog1.SelectedPath;
            this.ActiveControl = lblPath;
        }
        private void Option1Folder()
        {
            bool exists = Directory.Exists(txtPath.Text + "\\" + txtName.Text);
            if (!exists)
            {
                Directory.CreateDirectory(txtPath.Text + "\\" + txtName.Text);
                MessageBox.Show("Successfully Created", "ATTENTION", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.ActiveControl = lblPath;
            }
            else
            {
                MessageBox.Show("The folder is already exists", "ATTENTION", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.ActiveControl = lblPath;
            }
        }
        private void OptionMoreThan1Folder()
        {
            if (txtStartFrom.Text == "")
            {
                MessageBox.Show("Start From is required!", "ATTENTION", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.ActiveControl = txtStartFrom;
            }
            else if (txtUntil.Text == "")
            {
                MessageBox.Show("Until is required!", "ATTENTION", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.ActiveControl = txtUntil;
            }
            else
            {
                isbtnCreateAlreadyClicked = true;
                txtStartFromStringHolder = txtStartFrom.Text;
                txtStartFromStringHolder = RemovingComma(txtStartFromStringHolder);
                userFrom = int.Parse(txtStartFromStringHolder);
                switch (userFrom)
                {
                    case 0:
                        MessageBox.Show("Start From should not be zero!", "ATTENTION", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.txtStartFrom.Clear();
                        this.ActiveControl = txtStartFrom;
                        break;
                    default:
                        txtUntilStringHolder = txtUntil.Text;
                        txtUntilStringHolder = RemovingComma(txtUntilStringHolder);
                        userUntil = int.Parse(txtUntilStringHolder);
                        switch (userUntil)
                        {
                            case 0:
                                MessageBox.Show("Until should not be zero!", "ATTENTION", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.txtUntil.Clear();
                                this.ActiveControl = txtUntil;
                                break;
                            case 1:
                                MessageBox.Show("Until should not be 1", "ATTENTION", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.txtUntil.Clear();
                                this.ActiveControl = txtUntil;
                                break;
                            default:
                                if (userFrom > userUntil)
                                {
                                    MessageBox.Show("Until should be greater than Start From!", "ATTENTION", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    this.txtStartFrom.Clear();
                                    this.txtUntil.Clear();
                                    this.ActiveControl = txtStartFrom;
                                }
                                else if (userFrom == userUntil)
                                {
                                    MessageBox.Show("Start From and Until should not equal!", "ATTENTION", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    this.txtStartFrom.Clear();
                                    this.txtUntil.Clear();
                                    this.ActiveControl = txtStartFrom;
                                }
                                else
                                {
                                    folderName = this.txtName.Text;
                                    bool exists = Directory.Exists(txtPath.Text + "\\" + txtName.Text);
                                    if (!exists)
                                    {
                                        DialogResult result = MessageBox.Show("Are you sure with your Start From and Until?", "ATTENTION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                        if (result == DialogResult.Yes)
                                        {
                                            this.workerThread1 = new Thread(new ThreadStart(this.HeavyOperation));
                                            this.workerThread1.Start();
                                            txtStartFrom.Enabled = false;
                                            txtUntil.Enabled = false;
                                            btnCreate.Enabled = false;
                                            txtPath.Enabled = false;
                                            txtName.Enabled = false;
                                            rbOne.Enabled = false;
                                            rbTwo.Enabled = false;
                                        }
                                        else if (result == DialogResult.No)
                                        {
                                            PuttingComma(txtStartFrom);
                                            PuttingComma(txtUntil);
                                        }
                                    }
                                }
                                break;
                        }
                        break;
                }
            }
        }
        private delegate void CreatingMoreThanOneFoldersDelegate();
        private CreatingMoreThanOneFoldersDelegate creatingMoreThanOneFoldersDelegate = null;
        private int getValueOfx = 0;
        private void HeavyOperation()
        {
            MessageBox.Show("Creating the folders will take time.\n Please click 'OK' to start", "ATTENTION", MessageBoxButtons.OK, MessageBoxIcon.Information);

            for (int x = userFrom; x <= userUntil; x = x + 1)
            {
                getValueOfx = x;
                this.Invoke(this.creatingMoreThanOneFoldersDelegate);
            }
            MessageBox.Show("Successfully created the " + txtUntil.Text + " folders", "ATTENTION", MessageBoxButtons.OK, MessageBoxIcon.Information);
            isbtnCreateAlreadyClicked = false;
            Invoke(new MethodInvoker(Reset));
            this.workerThread1.Abort();
        }

        private void Reset()
        {
            this.ActiveControl = txtPath;
            this.lblStartFrom.Visible = false;
            this.lblUntil.Visible = false;
            this.txtStartFrom.Visible = false;
            this.txtUntil.Visible = false;
            this.txtStartFrom.Enabled = true;
            this.txtUntil.Enabled = true;
            this.btnCreate.Enabled = true;
            this.txtPath.Enabled = true;
            this.txtName.Enabled = true;
            this.rbOne.Enabled = true;
            this.rbTwo.Enabled = true;
            this.txtPath.Clear();
            this.txtName.Clear();
            this.txtStartFrom.Clear();
            this.txtUntil.Clear();
            this.rbOne.Checked = false;
            this.rbTwo.Checked = false;

        }
        private void Form_Main_Load(object sender, EventArgs e)
        {
            Reset();
            this.creatingMoreThanOneFoldersDelegate = new CreatingMoreThanOneFoldersDelegate(this.CreatingMoreThanOneFolders);
        }
        private void CreatingMoreThanOneFolders()
        {
            this.txtName.Text += " " + getValueOfx + " -";
            Directory.CreateDirectory(txtPath.Text + "\\" + txtName.Text);
            this.txtName.Clear();
            this.txtName.Text = folderName;
        }
        private string RemovingComma(string param_StringVariable)
        {
            string removedString = "";
            if (param_StringVariable.Contains(",") && isbtnCreateAlreadyClicked == true)
            {
                removedString = param_StringVariable.Replace(",", "");
                return removedString;
            }
            else
            {
                return removedString = param_StringVariable;
            }
        }
        private void pbMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.ActiveControl = txtStartFrom;
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }
        private void pbClose_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("You want to exit the program?", "ATTENTION", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (txtPath.Text != "" && txtName.Text != "" && rbOne.Checked != false || rbTwo.Checked != false)
            {
                if (this.rbOne.Checked == true)
                {
                    Option1Folder();
                }
                else if (this.rbTwo.Checked == true)
                {
                    OptionMoreThan1Folder();
                }
            }
            else if (txtPath.Text == "")
            {
                MessageBox.Show("Path is required!", "ATTENTION", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SearchingPath();
            }
            else if (txtName.Text == "")
            {
                MessageBox.Show("Name is required!", "ATTENTION", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.ActiveControl = txtName;
            }
            else if (rbOne.Checked == false && rbTwo.Checked == false)
            {
                MessageBox.Show("Options is required!", "ATTENTION", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.ActiveControl = lblOptions;
            }
        }
        private void rbOne_CheckedChanged(object sender, EventArgs e)
        {
            if (rbOne.Checked == true)
            {
                rbTwo.Checked = false;
                this.lblStartFrom.Visible = false;
                this.lblUntil.Visible = false;
                this.txtStartFrom.Visible = false;
                this.txtUntil.Visible = false;
                this.txtStartFrom.Clear();
                this.txtUntil.Clear();
                this.ActiveControl = lblPath;
            }
        }
        private void rbTwo_Click(object sender, EventArgs e)
        {
            this.ActiveControl = txtStartFrom;
        }
        private void rbTwo_CheckedChanged(object sender, EventArgs e)
        {
            if (rbTwo.Checked == true)
            {
                rbOne.Checked = false;

                this.lblStartFrom.Visible = true;
                this.lblUntil.Visible = true;
                this.txtStartFrom.Visible = true;
                this.txtUntil.Visible = true;
                this.ActiveControl = txtStartFrom;
            }
        }
        private void btnSearchPath_Click(object sender, EventArgs e)
        {
            SearchingPath();
        }
        private void txtStartFrom_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
        private void txtUntil_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
        private void PuttingComma(TextBox param_TextBoxName)
        {
            if (param_TextBoxName.Text.Length >= 4 && isbtnCreateAlreadyClicked == false)
            {
                double patternFormat = Convert.ToDouble(param_TextBoxName.Text);
                param_TextBoxName.Text = patternFormat.ToString("#,##0");
                param_TextBoxName.SelectionStart = param_TextBoxName.Text.Length;
            }
        }
        private void txtStartFrom_TextChanged(object sender, EventArgs e)
        {
            PuttingComma(txtStartFrom);
        }
        private void txtUntil_TextChanged(object sender, EventArgs e)
        {
            PuttingComma(txtUntil);
        }
    }
}
