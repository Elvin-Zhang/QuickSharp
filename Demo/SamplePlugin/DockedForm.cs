using QuickSharp.Core;
using WeifenLuo.WinFormsUI.Docking;

namespace SamplePlugin
{
    public partial class DockedForm : BaseDockedForm
    {
        public DockedForm(string formKey) : base(formKey)
        {
            InitializeComponent();
        }

        protected override void SetFormDefaultState()
        {
            /*
             * Set the initial state of the form.
             */

            DockState = DockState.DockRight;
            Show();
        }
    }
}
