using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiIdeogram_CS
{
    public partial class GenePosition : Form
    {
        DNARegion currentgene;

        public GenePosition()
        {
            InitializeComponent();
        }

        public GenePosition(DNARegion gene)
        {
            InitializeComponent();

            if (object.ReferenceEquals(gene, null) == true)
            {
                cboChromosome.SelectedIndex = 0;
                txtStart.Text = "0";
               btnDelete.Enabled = false;
            }
            else
            {
                cboChromosome.SelectedIndex = gene.Chromosome;
                txtStart.Text = gene.StartPoint.ToString();
                btnDelete.Enabled = true;
            }
            currentgene = gene;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            currentgene = null;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void GenePosition_Load(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int i = -1;
            try
            {
                i = Convert.ToInt32(txtStart.Text.Trim().Replace(",", ""));
                
            }
            catch (Exception ex)
            { }
            currentgene = new DNARegion(cboChromosome.SelectedIndex,i, i);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void txtStart_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            try
            {
                int i = Convert.ToInt32(tb.Text.Trim().Replace(",",""));
                e.Cancel = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Not a number (do not include formating characters)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        public DNARegion getGene()
        { return currentgene; }
    }
}
