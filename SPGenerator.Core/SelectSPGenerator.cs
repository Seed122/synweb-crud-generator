using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPGenerator.DataModel;

namespace SPGenerator.Core
{
    class SelectSPGenerator: BaseSPGenerator
    {
        protected override string GetSpName(string tableName)
        {
            return tableName + "_Select";
        }

        protected override string GenerateStatement(DBTableInfo tableInfo, List<DBTableColumnInfo> selectedFields, List<DBTableColumnInfo> whereConditionFields)
        {
            //List<string> fields = new List<string>();
            //foreach (DBTableColumnInfo colInf in selectedFields.Where(x => !x.Exclude))
            //{
            //    fields.Add(Wrap(colInf.ColumnName));
            //}
            //return "\tSELECT " + string.Join(", ", fields) + " FROM " + tableInfo.FullTableName;
            return "\tSELECT * FROM " + tableInfo.FullTableName;
        }

        protected override string GenerateInputParameters(List<DBTableColumnInfo> fields)
        {
            return "";
        }
    }
}
