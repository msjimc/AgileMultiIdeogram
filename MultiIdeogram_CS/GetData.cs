using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MultiIdeogram_CS
{
    public partial class GetData : Form
    {
        private string lastFolder = null;
        private string[] VCF = null;
        private string[] gVCF = null;
        private string[] XLS = null;
        private string[] Birdseed = null;
        private string[] TXT = null;
        private string multiText = null;
        private string[] theseFiles = null;
        private string[] theseFileNames = null;
        private bool[] affectedPatients = null;

        private enum viewAs { Circule, Linear, Single }

        public GetData()
        {
            InitializeComponent();
        }

        private void enableButtons(bool state)
        {
            btnSingle.Enabled = state;
            btnLinear.Enabled = state;
            btnGo.Enabled = state;
        }

        private void btnVCF_Click(object sender, EventArgs e)
        {
            string folder = FileAccessClass.FileString(FileAccessClass.FileJob.Directory, "Select the folder of *.VCF/*.VCF.gz files", lastFolder);
            if (System.IO.Directory.Exists(folder) == false) { return; }

            try
            {
                string[] tempgz = System.IO.Directory.GetFiles(folder, "*.vcf.gz");
                string[] temp = System.IO.Directory.GetFiles(folder, "*.vcf");
                if (temp.Length > 0 || tempgz.Length > 0)
                {
                    int duplicates = 0;
                    foreach (string g in tempgz)
                    {
                        foreach (string v in temp)
                        {
                            if (g.StartsWith(v) == true)
                            { duplicates++; }
                        }
                    }

                    VCF = new string[temp.Length + tempgz.Length - duplicates];
                    int count = tempgz.Length;

                    tempgz.CopyTo(VCF, 0);
                    bool add = true;
                    foreach (string v in temp)
                    {
                        foreach (string g in tempgz)
                        {
                            if (g.StartsWith(v) == true)
                            { add = false; }
                        }
                        if (add == true)
                        {
                            VCF[count] = v;
                            count++;
                        }
                        add = true;
                    }

                    enableButtons(true);
                    lblVCF.Text = folder.Substring(folder.LastIndexOf('\\') + 1);
                    lastFolder = folder;
                }
                else
                {
                    MessageBox.Show("There are no VCF files in this folder.", "No files", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch
            { }
        }

        private void btngVCF_Click(object sender, EventArgs e)
        {
            string folder = FileAccessClass.FileString(FileAccessClass.FileJob.Directory, "Select the folder of *.gVCF/*.g.vVCF.gz files", lastFolder);
            if (System.IO.Directory.Exists(folder) == false) { return; }

            try
            {
                string[] tempgz = System.IO.Directory.GetFiles(folder, "*.g.vcf.gz");
                string[] temp = System.IO.Directory.GetFiles(folder, "*.g.vcf");
                if (temp.Length > 0 || tempgz.Length > 0)
                {
                    int duplicates = 0;
                    foreach (string g in tempgz)
                    {
                        foreach (string v in temp)
                        {
                            if (g.StartsWith(v) == true)
                            { duplicates++; }
                        }
                    }

                    gVCF = new string[temp.Length + tempgz.Length - duplicates];
                    int count =tempgz.Length;

                    tempgz.CopyTo(gVCF, 0);
                    bool add = true;
                    foreach (string v in temp)
                    {
                        foreach (string g in tempgz)
                        {
                            if (g.StartsWith(v) == true)
                            { add = false; }
                        }
                        if (add == true)
                        { 
                            gVCF[count] = v;
                            count++;
                        }
                        add = true;                       
                    }

                    enableButtons(true);
                    lblgVCF.Text = folder.Substring(folder.LastIndexOf('\\') + 1);
                    lastFolder = folder;
                }
                else
                {
                    MessageBox.Show("There are no VCF files in this folder.", "No files", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch
            { }
        }

        private void btnBirdseed_Click(object sender, EventArgs e)
        {
            string folder = FileAccessClass.FileString(FileAccessClass.FileJob.Directory, "Select the folder of *.txt birdseed files", lastFolder);
            if (System.IO.Directory.Exists(folder) == false) { return; }

            try
            {
                string[] temp = System.IO.Directory.GetFiles(folder, "*.txt");
                if (temp.Length > 0)
                {
                    Birdseed = temp;
                    enableButtons(true);
                    lblBirdseed.Text = folder.Substring(folder.LastIndexOf('\\') + 1);
                    lastFolder = folder;
                }
                else
                {
                    MessageBox.Show("There are no Text files in this folder.", "No files", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch
            { }
        }

        private void btnXLS_Click(object sender, EventArgs e)
        {
            string folder = FileAccessClass.FileString(FileAccessClass.FileJob.Directory, "Select the folder of *.xls tab-delimited files", lastFolder);
            if (System.IO.Directory.Exists(folder) == false) { return; }

            try
            {
                string[] temp = System.IO.Directory.GetFiles(folder, "*.xls");
                if (temp.Length > 0)
                {
                    XLS = temp;
                    enableButtons(true);
                    lblXLS.Text = folder.Substring(folder.LastIndexOf('\\') + 1);
                    lastFolder = folder;
                }
                else
                {
                    MessageBox.Show("There are no XLS files in this folder.", "No files", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch
            { }
        }

        private void btnText_Click(object sender, EventArgs e)
        {
            if (rdoFolder.Checked == true)
            {
                string folder = FileAccessClass.FileString(FileAccessClass.FileJob.Directory, "Select the folder of *.txt files", lastFolder);
                if (System.IO.Directory.Exists(folder) == false) { return; }

                try
                {
                    TXT = System.IO.Directory.GetFiles(folder, "*.txt");
                    if (TXT.Length > 0)
                    {
                        enableButtons(true);
                        lblText.Text = folder.Substring(folder.LastIndexOf('\\') + 1);
                        lastFolder = folder;
                    }
                    else
                    {
                        MessageBox.Show("There are no text files in this folder.", "No files", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                catch
                { }
            }
            else
            {
                string file = FileAccessClass.FileString(FileAccessClass.FileJob.Open, "Select file containing data for multiple individuals", "Multi-regions file (*.txt)|*.txt");
                if (System.IO.File.Exists(file) == false) { return; }

                try
                {
                   
                    if (file.Length > 0)
                    {
                        enableButtons(true);
                        lblText.Text = file.Substring(file.LastIndexOf('\\') + 1);
                        lastFolder = file.Substring(0,file.LastIndexOf('\\'));
                        multiText = file;
                    }
                    else
                    {
                        MessageBox.Show("There are no text files in this folder.", "No files", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                catch
                { }
            }

        }

        private void btnSingle_Click(object sender, EventArgs e)
        {
            Visualise(viewAs.Single, chkIncludeRS.Checked, chkVCFGenotype.Checked);
        }

        private void btnLinear_Click(object sender, EventArgs e)
        {
            Visualise(viewAs.Linear, chkIncludeRS.Checked, chkVCFGenotype.Checked);
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            Visualise(viewAs.Circule, chkIncludeRS.Checked, chkVCFGenotype.Checked);
        }

        private void Visualise(viewAs job, bool IgnoreRSField, bool VCFGenotypes)
        { 
            int count = 0;
            if (XLS != null) { count = XLS.Length; }
            if (VCF != null) { count += VCF.Length; }
            if (gVCF != null) { count += gVCF.Length; }
            if (Birdseed != null) { count += Birdseed.Length; }
            if (TXT != null) { count += TXT.Length; }
            string[] names = null;
            if (string.IsNullOrEmpty(multiText) == false)
            {
                names = GetIndividualNames(multiText);
                count += names.Length;
            }

            theseFiles = new string[count];
            theseFileNames = new string[count];
            affectedPatients = new bool[count];
            count = 0;

            if (XLS != null && XLS.Length > 0)
            {
                for (int index = 0; index < XLS.Length; index++)
                {
                    theseFiles[count] = XLS[index];
                    theseFileNames[count++] = XLS[index].Substring(XLS[index].LastIndexOf('\\') + 1);
                }
            }

            if (Birdseed != null && Birdseed.Length > 0)
            {
                for (int index = 0; index < Birdseed.Length; index++)
                {
                    theseFiles[count] = Birdseed[index];
                    theseFileNames[count++] = Birdseed[index].Substring(Birdseed[index].LastIndexOf('\\') + 1);
                }
            }

            if (VCF != null && VCF.Length > 0)
            {
                for (int index = 0; index < VCF.Length; index++)
                {
                    theseFiles[count] = VCF[index];
                    theseFileNames[count++] = VCF[index].Substring(VCF[index].LastIndexOf('\\') + 1);
                }
            }

            if (gVCF != null && gVCF.Length > 0)
            {
                for (int index = 0; index < gVCF.Length; index++)
                {
                    theseFiles[count] = gVCF[index];
                    theseFileNames[count++] = gVCF[index].Substring(gVCF[index].LastIndexOf('\\') + 1);
                }
            }

            if (TXT != null && TXT.Length > 0)
            {
                for (int index = 0; index < TXT.Length; index++)
                {
                    theseFiles[count] = TXT[index];
                    theseFileNames[count++] = TXT[index].Substring(TXT[index].LastIndexOf('\\') + 1);
                }
            }

            if (string.IsNullOrEmpty(multiText) == false)
            {
                for (int index = 0; index < names.Length; index++)
                {
                    theseFiles[count] = names[index];
                    theseFileNames[count++] = names[index];
                }
            }

            Affecteds getAffected = new Affecteds(affectedPatients, theseFiles);
            if (getAffected.ShowDialog() == DialogResult.OK)
            {
                affectedPatients = getAffected.TheAffecteds;
                resetFileOrder();

                if (job == viewAs.Linear)
                {
                    LinearView lv = new LinearView(theseFiles, theseFileNames, affectedPatients, multiText);
                    lv.Show();
                    lv.GetData(IgnoreRSField, VCFGenotypes);
                }
                else if (job==viewAs.Circule)
                {
                    ViewData vd = new ViewData(theseFiles, theseFileNames, affectedPatients, multiText);
                    vd.Show();
                    vd.GetData(IgnoreRSField, VCFGenotypes);
                }
                else if(job==viewAs.Single)
                {
                    SingleView vS = new SingleView(theseFiles, theseFileNames, affectedPatients, multiText);
                    vS.Show();
                    vS.GetData(IgnoreRSField, VCFGenotypes);
                }

            }
            else
            { return; }

        }

        private string[] GetIndividualNames(string multi)
        {
            string[] answer = null;
            System.IO.StreamReader fr = null;
            try
            {
                fr = new System.IO.StreamReader(multi);
                string line = null;
                int counter = 0;
                while (fr.Peek()>0)
                {
                    line = fr.ReadLine();
                    if (line.Contains(":") != true && line.Contains("\t")!=true)
                    { counter++;                    }
                }
                fr.Close();
                fr = new System.IO.StreamReader(multi);
                answer = new string[counter];
                counter = 0;
                while (fr.Peek() > 0)
                {
                    line = fr.ReadLine();
                    if (line.Contains(":") != true && line.Contains("\t") != true)
                    { answer[counter++] = line.Trim(); }
                }
            }
            finally
            { if (fr != null) { fr.Close(); } }

            return answer;
        }

        private void resetFileOrder()
        {
            string[] files = new string[theseFileNames.Length];
            string[] fileNames = new string[theseFileNames.Length];
            bool[] affectedPeople = new bool[theseFileNames.Length];
            int count = 0;

            for (int index = 0; index < fileNames.Length; index++)
            {
                if (affectedPatients[index] == true)
                {
                    files[count] = theseFiles[index];
                    fileNames[count] = theseFileNames[index];
                    affectedPeople[count] = affectedPatients[index];
                    count++;
                }
            }

            for (int index = 0; index < fileNames.Length; index++)
            {
                if (affectedPatients[index] == false)
                {
                    files[count] = theseFiles[index];
                    fileNames[count] = theseFileNames[index];
                    affectedPeople[count] = affectedPatients[index];
                    count++;
                }
            }

            theseFileNames = fileNames;
            theseFiles = files;
            affectedPatients = affectedPeople;

        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            VCF = null;
            gVCF = null;
            XLS = null;
            Birdseed = null;
            TXT = null;
            multiText = "";
            theseFiles = null;
            theseFileNames = null;
            affectedPatients = null;
            btnGo.Enabled = false;
            btnLinear.Enabled = false;
            btnSingle.Enabled = false;
            lblBirdseed.Text = "Not set";
            lblVCF.Text = "Not set";
            lblgVCF.Text = "Not set";
            lblXLS.Text = "Not set";
            lblText.Text = "Not set";
        }

        private void GetData_Load(object sender, EventArgs e)
        {

        }
    }
}
