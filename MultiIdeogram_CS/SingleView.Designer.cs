namespace MultiIdeogram_CS
{
    partial class SingleView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SingleView));
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportIntervalsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportIntervalsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCommonIntervalsOnlyshortFormatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeGapBetweenAutozygouseRegionsNarrowerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeGapBetweenAutozygouseRegionsBiggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeAutozygousRegionsThinnerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeAutozygousRegionsDeeperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.affectedAutozygousRegionColourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commonAffectedAutozygousRegionColourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unaffectedAutozygousRegionColourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.addVariantPositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnBack = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.p1 = new System.Windows.Forms.PictureBox();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.cboChromosome = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.hideDottedLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.scaleToChromosome1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.p1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // colorDialog1
            // 
            this.colorDialog1.AllowFullOpen = false;
            this.colorDialog1.FullOpen = true;
            this.colorDialog1.SolidColorOnly = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(799, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveImageToolStripMenuItem,
            this.exportIntervalsToolStripMenuItem,
            this.exportIntervalsToolStripMenuItem1,
            this.exportCommonIntervalsOnlyshortFormatToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveImageToolStripMenuItem
            // 
            this.saveImageToolStripMenuItem.Name = "saveImageToolStripMenuItem";
            this.saveImageToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.saveImageToolStripMenuItem.Text = "Save 300 dpi image as";
            this.saveImageToolStripMenuItem.Click += new System.EventHandler(this.saveImageToolStripMenuItem_Click);
            // 
            // exportIntervalsToolStripMenuItem
            // 
            this.exportIntervalsToolStripMenuItem.Name = "exportIntervalsToolStripMenuItem";
            this.exportIntervalsToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.exportIntervalsToolStripMenuItem.Text = "Export all intervals (long format)";
            this.exportIntervalsToolStripMenuItem.Click += new System.EventHandler(this.exportIntervalsToolStripMenuItem_Click);
            // 
            // exportIntervalsToolStripMenuItem1
            // 
            this.exportIntervalsToolStripMenuItem1.Name = "exportIntervalsToolStripMenuItem1";
            this.exportIntervalsToolStripMenuItem1.Size = new System.Drawing.Size(310, 22);
            this.exportIntervalsToolStripMenuItem1.Text = "Export all intervals (short format)";
            this.exportIntervalsToolStripMenuItem1.Click += new System.EventHandler(this.exportIntervalsToolStripMenuItem1_Click);
            // 
            // exportCommonIntervalsOnlyshortFormatToolStripMenuItem
            // 
            this.exportCommonIntervalsOnlyshortFormatToolStripMenuItem.Name = "exportCommonIntervalsOnlyshortFormatToolStripMenuItem";
            this.exportCommonIntervalsOnlyshortFormatToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.exportCommonIntervalsOnlyshortFormatToolStripMenuItem.Text = "Export common intervals only (short format)";
            this.exportCommonIntervalsOnlyshortFormatToolStripMenuItem.Click += new System.EventHandler(this.exportCommonIntervalsOnlyshortFormatToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.makeGapBetweenAutozygouseRegionsNarrowerToolStripMenuItem,
            this.makeGapBetweenAutozygouseRegionsBiggerToolStripMenuItem,
            this.makeAutozygousRegionsThinnerToolStripMenuItem,
            this.makeAutozygousRegionsDeeperToolStripMenuItem,
            this.toolStripMenuItem2,
            this.scaleToChromosome1ToolStripMenuItem,
            this.hideDottedLineToolStripMenuItem,
            this.toolStripMenuItem4,
            this.affectedAutozygousRegionColourToolStripMenuItem,
            this.commonAffectedAutozygousRegionColourToolStripMenuItem,
            this.unaffectedAutozygousRegionColourToolStripMenuItem,
            this.toolStripMenuItem3,
            this.resetToolStripMenuItem,
            this.toolStripMenuItem1,
            this.addVariantPositionToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // makeGapBetweenAutozygouseRegionsNarrowerToolStripMenuItem
            // 
            this.makeGapBetweenAutozygouseRegionsNarrowerToolStripMenuItem.Name = "makeGapBetweenAutozygouseRegionsNarrowerToolStripMenuItem";
            this.makeGapBetweenAutozygouseRegionsNarrowerToolStripMenuItem.Size = new System.Drawing.Size(336, 22);
            this.makeGapBetweenAutozygouseRegionsNarrowerToolStripMenuItem.Text = "Make gap between autozygouse regions narrower";
            this.makeGapBetweenAutozygouseRegionsNarrowerToolStripMenuItem.Click += new System.EventHandler(this.makeGapBetweenAutozygouseRegionsNarrowerToolStripMenuItem_Click);
            // 
            // makeGapBetweenAutozygouseRegionsBiggerToolStripMenuItem
            // 
            this.makeGapBetweenAutozygouseRegionsBiggerToolStripMenuItem.Name = "makeGapBetweenAutozygouseRegionsBiggerToolStripMenuItem";
            this.makeGapBetweenAutozygouseRegionsBiggerToolStripMenuItem.Size = new System.Drawing.Size(336, 22);
            this.makeGapBetweenAutozygouseRegionsBiggerToolStripMenuItem.Text = "Make gap between autozygouse regions bigger";
            this.makeGapBetweenAutozygouseRegionsBiggerToolStripMenuItem.Click += new System.EventHandler(this.makeGapBetweenAutozygouseRegionsBiggerToolStripMenuItem_Click);
            // 
            // makeAutozygousRegionsThinnerToolStripMenuItem
            // 
            this.makeAutozygousRegionsThinnerToolStripMenuItem.Name = "makeAutozygousRegionsThinnerToolStripMenuItem";
            this.makeAutozygousRegionsThinnerToolStripMenuItem.Size = new System.Drawing.Size(336, 22);
            this.makeAutozygousRegionsThinnerToolStripMenuItem.Text = "Make autozygous regions thinner";
            this.makeAutozygousRegionsThinnerToolStripMenuItem.Click += new System.EventHandler(this.makeAutozygousRegionsThinnerToolStripMenuItem_Click);
            // 
            // makeAutozygousRegionsDeeperToolStripMenuItem
            // 
            this.makeAutozygousRegionsDeeperToolStripMenuItem.Name = "makeAutozygousRegionsDeeperToolStripMenuItem";
            this.makeAutozygousRegionsDeeperToolStripMenuItem.Size = new System.Drawing.Size(336, 22);
            this.makeAutozygousRegionsDeeperToolStripMenuItem.Text = "Make autozygous regions deeper";
            this.makeAutozygousRegionsDeeperToolStripMenuItem.Click += new System.EventHandler(this.makeAutozygousRegionsDeeperToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(333, 6);
            // 
            // affectedAutozygousRegionColourToolStripMenuItem
            // 
            this.affectedAutozygousRegionColourToolStripMenuItem.Name = "affectedAutozygousRegionColourToolStripMenuItem";
            this.affectedAutozygousRegionColourToolStripMenuItem.Size = new System.Drawing.Size(336, 22);
            this.affectedAutozygousRegionColourToolStripMenuItem.Text = "Affected autozygous region colour";
            this.affectedAutozygousRegionColourToolStripMenuItem.Click += new System.EventHandler(this.affectedAutozygousRegionColourToolStripMenuItem_Click);
            // 
            // commonAffectedAutozygousRegionColourToolStripMenuItem
            // 
            this.commonAffectedAutozygousRegionColourToolStripMenuItem.Name = "commonAffectedAutozygousRegionColourToolStripMenuItem";
            this.commonAffectedAutozygousRegionColourToolStripMenuItem.Size = new System.Drawing.Size(336, 22);
            this.commonAffectedAutozygousRegionColourToolStripMenuItem.Text = "Common affected autozygous region colour";
            this.commonAffectedAutozygousRegionColourToolStripMenuItem.Click += new System.EventHandler(this.commonAffectedAutozygousRegionColourToolStripMenuItem_Click);
            // 
            // unaffectedAutozygousRegionColourToolStripMenuItem
            // 
            this.unaffectedAutozygousRegionColourToolStripMenuItem.Name = "unaffectedAutozygousRegionColourToolStripMenuItem";
            this.unaffectedAutozygousRegionColourToolStripMenuItem.Size = new System.Drawing.Size(336, 22);
            this.unaffectedAutozygousRegionColourToolStripMenuItem.Text = "Unaffected autozygous region colour";
            this.unaffectedAutozygousRegionColourToolStripMenuItem.Click += new System.EventHandler(this.unaffectedAutozygousRegionColourToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(333, 6);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(336, 22);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(333, 6);
            // 
            // addVariantPositionToolStripMenuItem
            // 
            this.addVariantPositionToolStripMenuItem.Name = "addVariantPositionToolStripMenuItem";
            this.addVariantPositionToolStripMenuItem.Size = new System.Drawing.Size(336, 22);
            this.addVariantPositionToolStripMenuItem.Text = "Add variant position ";
            this.addVariantPositionToolStripMenuItem.Click += new System.EventHandler(this.addVariantPositionToolStripMenuItem_Click);
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBack.Location = new System.Drawing.Point(12, 503);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 23);
            this.btnBack.TabIndex = 9;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.p1);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(758, 419);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            // 
            // p1
            // 
            this.p1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.p1.Location = new System.Drawing.Point(3, 16);
            this.p1.Name = "p1";
            this.p1.Size = new System.Drawing.Size(752, 400);
            this.p1.TabIndex = 0;
            this.p1.TabStop = false;
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vScrollBar1.Location = new System.Drawing.Point(773, 40);
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(17, 403);
            this.vScrollBar1.TabIndex = 11;
            this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
            // 
            // cboChromosome
            // 
            this.cboChromosome.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboChromosome.FormattingEnabled = true;
            this.cboChromosome.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22"});
            this.cboChromosome.Location = new System.Drawing.Point(650, 19);
            this.cboChromosome.Name = "cboChromosome";
            this.cboChromosome.Size = new System.Drawing.Size(90, 21);
            this.cboChromosome.TabIndex = 1;
            this.cboChromosome.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.cboChromosome);
            this.groupBox2.Location = new System.Drawing.Point(15, 456);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(746, 45);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(573, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Chromosome:";
            // 
            // hideDottedLineToolStripMenuItem
            // 
            this.hideDottedLineToolStripMenuItem.Name = "hideDottedLineToolStripMenuItem";
            this.hideDottedLineToolStripMenuItem.Size = new System.Drawing.Size(336, 22);
            this.hideDottedLineToolStripMenuItem.Text = "Hide dotted line";
            this.hideDottedLineToolStripMenuItem.Click += new System.EventHandler(this.hideDottedLineToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(333, 6);
            // 
            // scaleToChromosome1ToolStripMenuItem
            // 
            this.scaleToChromosome1ToolStripMenuItem.CheckOnClick = true;
            this.scaleToChromosome1ToolStripMenuItem.Name = "scaleToChromosome1ToolStripMenuItem";
            this.scaleToChromosome1ToolStripMenuItem.Size = new System.Drawing.Size(336, 22);
            this.scaleToChromosome1ToolStripMenuItem.Text = "Scale to chromosome 1";
            this.scaleToChromosome1ToolStripMenuItem.Click += new System.EventHandler(this.scaleToChromosome1ToolStripMenuItem_Click);
            // 
            // SingleView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(799, 538);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.vScrollBar1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(819, 581);
            this.MinimumSize = new System.Drawing.Size(819, 581);
            this.Name = "SingleView";
            this.Text = "Chromosome:";
            this.Load += new System.EventHandler(this.SingleView_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.p1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportIntervalsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportIntervalsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exportCommonIntervalsOnlyshortFormatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem affectedAutozygousRegionColourToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem commonAffectedAutozygousRegionColourToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unaffectedAutozygousRegionColourToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addVariantPositionToolStripMenuItem;
        internal System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox p1;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.ComboBox cboChromosome;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem makeGapBetweenAutozygouseRegionsNarrowerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem makeGapBetweenAutozygouseRegionsBiggerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem makeAutozygousRegionsThinnerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem makeAutozygousRegionsDeeperToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem hideDottedLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem scaleToChromosome1ToolStripMenuItem;
    }
}