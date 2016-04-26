using System;
using System.Data.SqlClient;
using Ppr.BaseClases;

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
