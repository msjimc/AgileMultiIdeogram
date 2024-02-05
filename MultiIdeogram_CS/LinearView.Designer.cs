namespace MultiIdeogram_CS
{
    partial class LinearView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LinearView));
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportIntervalsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportIntervalsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCommonIntervalsOnlyshortFormatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.affectedAutozygousRegionColourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commonAffectedAutozygousRegionColourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unaffectedAutozygousRegionColourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.addVariantPositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnBack = new System.Windows.Forms.Button();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.p1 = new System.Windows.Forms.PictureBox();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.showDottedLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.p1)).BeginInit();
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
            this.menuStrip1.Size = new System.Drawing.Size(793, 24);
            this.menuStrip1.TabIndex = 6;
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
            this.affectedAutozygousRegionColourToolStripMenuItem,
            this.commonAffectedAutozygousRegionColourToolStripMenuItem,
            this.unaffectedAutozygousRegionColourToolStripMenuItem,
            this.toolStripMenuItem3,
            this.showDottedLineToolStripMenuItem,
            this.toolStripMenuItem2,
            this.resetToolStripMenuItem,
            this.toolStripMenuItem1,
            this.addVariantPositionToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // affectedAutozygousRegionColourToolStripMenuItem
            // 
            this.affectedAutozygousRegionColourToolStripMenuItem.Name = "affectedAutozygousRegionColourToolStripMenuItem";
            this.affectedAutozygousRegionColourToolStripMenuItem.Size = new System.Drawing.Size(309, 22);
            this.affectedAutozygousRegionColourToolStripMenuItem.Text = "Affected autozygous region colour";
            this.affectedAutozygousRegionColourToolStripMenuItem.Click += new System.EventHandler(this.affectedAutozygousRegionColourToolStripMenuItem_Click);
            // 
            // commonAffectedAutozygousRegionColourToolStripMenuItem
            // 
            this.commonAffectedAutozygousRegionColourToolStripMenuItem.Name = "commonAffectedAutozygousRegionColourToolStripMenuItem";
            this.commonAffectedAutozygousRegionColourToolStripMenuItem.Size = new System.Drawing.Size(309, 22);
            this.commonAffectedAutozygousRegionColourToolStripMenuItem.Text = "Common affected autozygous region colour";
            this.commonAffectedAutozygousRegionColourToolStripMenuItem.Click += new System.EventHandler(this.commonAffectedAutozygousRegionColourToolStripMenuItem_Click);
            // 
            // unaffectedAutozygousRegionColourToolStripMenuItem
            // 
            this.unaffectedAutozygousRegionColourToolStripMenuItem.Name = "unaffectedAutozygousRegionColourToolStripMenuItem";
            this.unaffectedAutozygousRegionColourToolStripMenuItem.Size = new System.Drawing.Size(309, 22);
            this.unaffectedAutozygousRegionColourToolStripMenuItem.Text = "Unaffected autozygous region colour";
            this.unaffectedAutozygousRegionColourToolStripMenuItem.Click += new System.EventHandler(this.unaffectedAutozygousRegionColourToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(306, 6);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(309, 22);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(306, 6);
            // 
            // addVariantPositionToolStripMenuItem
            // 
            this.addVariantPositionToolStripMenuItem.Name = "addVariantPositionToolStripMenuItem";
            this.addVariantPositionToolStripMenuItem.Size = new System.Drawing.Size(309, 22);
            this.addVariantPositionToolStripMenuItem.Text = "Add variant position ";
            this.addVariantPositionToolStripMenuItem.Click += new System.EventHandler(this.addVariantPositionToolStripMenuItem_Click);
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(7, 814);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 23);
            this.btnBack.TabIndex = 8;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // GroupBox1
            // 
            this.GroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.GroupBox1.Controls.Add(this.p1);
            this.GroupBox1.Location = new System.Drawing.Point(10, 27);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(756, 769);
            this.GroupBox1.TabIndex = 7;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Autozygous regions";
            // 
            // p1
            // 
            this.p1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.p1.Location = new System.Drawing.Point(3, 16);
            this.p1.Name = "p1";
            this.p1.Size = new System.Drawing.Size(750, 750);
            this.p1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.p1.TabIndex = 0;
            this.p1.TabStop = false;
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Location = new System.Drawing.Point(769, 43);
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(16, 750);
            this.vScrollBar1.TabIndex = 9;
            this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(306, 6);
            // 
            // showDottedLineToolStripMenuItem
            // 
            this.showDottedLineToolStripMenuItem.Name = "showDottedLineToolStripMenuItem";
            this.showDottedLineToolStripMenuItem.Size = new System.Drawing.Size(309, 22);
            this.showDottedLineToolStripMenuItem.Text = "Hide dotted line";
            this.showDottedLineToolStripMenuItem.Click += new System.EventHandler(this.showDottedLineToolStripMenuItem_Click);
            // 
            // LinearView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(793, 848);
            this.Controls.Add(this.vScrollBar1);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "LinearView";
            this.Text = "Autosomal chromosomes";
            this.Load += new System.EventHandler(this.LinearView_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.GroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.p1)).EndInit();
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
        internal System.Windows.Forms.GroupBox GroupBox1;
        internal System.Windows.Forms.PictureBox p1;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem showDottedLineToolStripMenuItem;
    }
}
