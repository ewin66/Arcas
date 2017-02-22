using System;
using Cav.DataAcces;

namespace Arcas.DBA
{
    public class ArcasDBAdapter : DataAccesBase
    {
        public void ExecScript(String SqlText)
        {
            var cmd = this.CreateCommandObject();
            cmd.CommandText = SqlText;
            ExecuteNonQuery(cmd);
        }
    }
}
