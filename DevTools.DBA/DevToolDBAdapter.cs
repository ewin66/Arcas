using System;
using Cav.DataAcces;

namespace DevTools.DBA
{
    public class DevToolDBAdapter : DataAccesBase
    {
        public void ExecScript(String SqlText)
        {
            var cmd = this.CreateCommandObject();
            cmd.CommandText = SqlText;
            ExecuteNonQuery(cmd);
        }
    }
}
