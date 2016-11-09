using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPGenerator.DataModel;

namespace SPGenerator.Core
{
    class SelectOneSPGenerator:BaseSPGenerator
    {
        protected override string GetSpName(string tableName)
        {
            return tableName + "_SelectOne";
        }

        protected override string GenerateStatement(DBTableInfo tableInfo, List<DBTableColumnInfo> selectedFields, List<DBTableColumnInfo> whereConditionFields)
        {
            //List<string> fields = new List<string>();
            //foreach (DBTableColumnInfo colInf in selectedFields.Where(x => !x.Exclude))
            //{
            //    fields.Add(Wrap(colInf.ColumnName));
            //}
            //var sb = new StringBuilder();
            //sb.AppendLine("\tSELECT " + string.Join(", ", fields) + " FROM " + tableInfo.FullTableName);
            //sb.Append(GenerateWhereStatement(whereConditionFields));
            //return sb.ToString();
            return "\tSELECT * FROM " + tableInfo.FullTableName;
        }

        protected override string GenerateInputParameters(List<DBTableColumnInfo> fields)
        {
            return "";
        }
    }
}
