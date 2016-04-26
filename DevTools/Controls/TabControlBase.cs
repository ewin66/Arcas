using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DevTools.Controls
{    
    public class TabControlBase : UserControl
    {
        public TabControlBase()
        {
            this.Name = "TabControlBase";
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
