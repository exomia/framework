using System.Windows.Forms;

namespace Exomia.Framework.ContentManager
{
    partial class AboutForm
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
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.arrLbl = new System.Windows.Forms.Label();
            this.logoPb = new System.Windows.Forms.PictureBox();
            this.productNameLbl = new System.Windows.Forms.Label();
            this.versionLbl = new System.Windows.Forms.Label();
            this.copyrightLbl = new System.Windows.Forms.Label();
            this.companyLbl = new System.Windows.Forms.Label();
            this.descriptionTb = new System.Windows.Forms.TextBox();
            this.okBtn = new System.Windows.Forms.Button();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPb)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67F));
            this.tableLayoutPanel.Controls.Add(this.arrLbl, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.logoPb, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.productNameLbl, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.versionLbl, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.copyrightLbl, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.companyLbl, 1, 4);
            this.tableLayoutPanel.Controls.Add(this.descriptionTb, 1, 5);
            this.tableLayoutPanel.Controls.Add(this.okBtn, 1, 6);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(9, 9);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 7;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(417, 208);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // arrLbl
            // 
            this.arrLbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.arrLbl.Location = new System.Drawing.Point(143, 51);
            this.arrLbl.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.arrLbl.MaximumSize = new System.Drawing.Size(0, 17);
            this.arrLbl.Name = "arrLbl";
            this.arrLbl.Size = new System.Drawing.Size(271, 17);
            this.arrLbl.TabIndex = 25;
            this.arrLbl.Text = "All rights reserved.";
            this.arrLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // logoPb
            // 
            this.logoPb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logoPb.Image = global::Exomia.Framework.ContentManager.Properties.Resources.logo_x192;
            this.logoPb.Location = new System.Drawing.Point(3, 3);
            this.logoPb.Name = "logoPb";
            this.tableLayoutPanel.SetRowSpan(this.logoPb, 5);
            this.logoPb.Size = new System.Drawing.Size(131, 98);
            this.logoPb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logoPb.TabIndex = 12;
            this.logoPb.TabStop = false;
            // 
            // productNameLbl
            // 
            this.productNameLbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productNameLbl.Location = new System.Drawing.Point(143, 0);
            this.productNameLbl.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.productNameLbl.MaximumSize = new System.Drawing.Size(0, 17);
            this.productNameLbl.Name = "productNameLbl";
            this.productNameLbl.Size = new System.Drawing.Size(271, 17);
            this.productNameLbl.TabIndex = 19;
            this.productNameLbl.Text = "product name";
            this.productNameLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // versionLbl
            // 
            this.versionLbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.versionLbl.Location = new System.Drawing.Point(143, 17);
            this.versionLbl.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.versionLbl.MaximumSize = new System.Drawing.Size(0, 17);
            this.versionLbl.Name = "versionLbl";
            this.versionLbl.Size = new System.Drawing.Size(271, 17);
            this.versionLbl.TabIndex = 0;
            this.versionLbl.Text = "version";
            this.versionLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // copyrightLbl
            // 
            this.copyrightLbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.copyrightLbl.Location = new System.Drawing.Point(143, 34);
            this.copyrightLbl.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.copyrightLbl.MaximumSize = new System.Drawing.Size(0, 17);
            this.copyrightLbl.Name = "copyrightLbl";
            this.copyrightLbl.Size = new System.Drawing.Size(271, 17);
            this.copyrightLbl.TabIndex = 21;
            this.copyrightLbl.Text = "copyright";
            this.copyrightLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // companyLbl
            // 
            this.companyLbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.companyLbl.Location = new System.Drawing.Point(143, 68);
            this.companyLbl.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.companyLbl.MaximumSize = new System.Drawing.Size(0, 17);
            this.companyLbl.Name = "companyLbl";
            this.companyLbl.Size = new System.Drawing.Size(271, 17);
            this.companyLbl.TabIndex = 22;
            this.companyLbl.Text = "company";
            this.companyLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // descriptionTb
            // 
            this.descriptionTb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.descriptionTb.Location = new System.Drawing.Point(143, 107);
            this.descriptionTb.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.descriptionTb.Multiline = true;
            this.descriptionTb.Name = "descriptionTb";
            this.descriptionTb.ReadOnly = true;
            this.descriptionTb.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.descriptionTb.Size = new System.Drawing.Size(271, 68);
            this.descriptionTb.TabIndex = 23;
            this.descriptionTb.TabStop = false;
            this.descriptionTb.Text = "Description";
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okBtn.Location = new System.Drawing.Point(334, 182);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(80, 23);
            this.okBtn.TabIndex = 24;
            this.okBtn.Text = "OK";
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // AboutForm
            // 
            this.AcceptButton = this.okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 226);
            this.Controls.Add(this.tableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About Exomia.Framework.ContentManager";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPb)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tableLayoutPanel;
        private PictureBox logoPb;
        private Label productNameLbl;
        private Label versionLbl;
        private Label copyrightLbl;
        private Label companyLbl;
        private Label arrLbl;
        private TextBox descriptionTb;
        private Button okBtn;
    }
}