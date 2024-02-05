using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MultiIdeogram_CS
{
    public partial class Affecteds : Form
    {
        private bool[] affecteds = null;
        private string[] fileNames = null;

        public Affecteds(bool[] Affecteds, string[] FileNames)
        {
            InitializeComponent();

            affecteds = Affecteds;
            fileNames = FileNames;

            for (int index = 0; index < FileNames.Length; index++)
            {
                clbPatients.Items.Add(FileNames[index].Substring(fileNames[index].LastIndexOf("\\") + 1));
                clbPatients.SetItemChecked(index, affecteds[index]);
            }
        }

        public bool[] TheAffecteds { get { return affecteds; } }

        private void Affecteds_FormClosing(object sender, FormClosingEventArgs e)
        {
            affecteds = new bool[affecteds.Length];

            foreach (int checkedIndex in clbPatients.CheckedIndices)
            { affecteds[checkedIndex] = true; }
        }

        private void Affecteds_Load(object sender, EventArgs e)
        { }

        private void btnBack_Click(object sender, EventArgs e)
        { Close(); }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
                
    }
}
