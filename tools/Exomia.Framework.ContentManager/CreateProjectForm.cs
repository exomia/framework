using System.Windows.Forms;

namespace Exomia.Framework.ContentManager
{
    /// <summary>
    ///     Form for creating projects.
    /// </summary>
    public partial class CreateProjectForm : Form
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CreateProjectForm"/> class.
        /// </summary>
        public CreateProjectForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
