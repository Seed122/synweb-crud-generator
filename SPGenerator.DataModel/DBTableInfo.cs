using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPGenerator.DataModel
{
    public class DBTableInfo
    {
        public DBTableInfo()
        {
            Columns = new List<DBTableColumnInfo>();
        }
        public string TableName { get; set; }
        public string Schema { get; set; }
        public List<DBTableColumnInfo> Columns { get; set; }

        public string FullTableName
        {
            get { return string.Format("[{0}].[{1}]", Schema, TableName); }
        }

        public override string ToString()
        {
            return FullTableName;
        }
    }
}
