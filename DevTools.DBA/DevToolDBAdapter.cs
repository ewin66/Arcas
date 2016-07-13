using System;
using System.Data.SqlClient;
using Cav.BaseClases;

namespace DevTools.DBA
{
    public class DevToolDBAdapter : DBAdapterBase
    {
        public void ExecScript(String SqlText)
        {
            ExecuteNonQuery(new SqlCommand(SqlText));
        }
    }
}
