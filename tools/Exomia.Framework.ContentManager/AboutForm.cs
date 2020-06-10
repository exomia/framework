using System;
using System.Reflection;
using System.Windows.Forms;

namespace Exomia.Framework.ContentManager
{
    partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            Load += OnLoad;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            var assembly = Assembly.GetExecutingAssembly();
            Text                    = $"About {assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title}";
            productNameLbl.Text     = assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
            versionLbl.Text         = $"Version {assembly.GetName().Version}";
            copyrightLbl.Text       = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
            companyLbl.Text         = assembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            descriptionTb.Text = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}