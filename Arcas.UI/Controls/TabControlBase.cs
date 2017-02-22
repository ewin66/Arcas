using System;
using System.Windows.Forms;
using Arcas.BL;

namespace Arcas.Controls
{
    public class TabControlBase : UserControl
    {
        public TabControlBase()
        {
            this.Name = this.GetType().FullName;
        }

        public virtual void RefreshTab() { }

        public event ProgressStateDelegat StateProgress;

        protected void SetSateProgress(String Message)
        {
            if (StateProgress != null)
                StateProgress(Message);
        }
    }
}
