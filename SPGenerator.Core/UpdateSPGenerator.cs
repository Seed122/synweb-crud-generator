using SPGenerator.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPGenerator.Core
{
    class UpdateSPGenerator : BaseSPGenerator
    {
        protected override string GetSpName(string tableName)
        {
            return tableName + "_Update";
        }
        protected override string GenerateStatement(DBTableInfo tableInfo, List<DBTableColumnInfo> selectedFields, List<DBTableColumnInfo> whereConditionFields)
        {
            if(!selectedFields.Any()){
                throw new ArgumentException("selectedFields");
            }
            var sb = new StringBuilder();

            sb.AppendLine("\tUPDATE " + tableInfo.FullTableName + " SET");

            var statements = new List<string>();
            foreach (DBTableColumnInfo colInf in selectedFields.Where(x => !x.Exclude))
            {
                statements.Add("\t\t"+Wrap(colInf.ColumnName) + "=" + PrefixInputParameter + colInf.ColumnName);
            }
            sb.AppendLine(string.Join("," + Environment.NewLine, statements));
            sb.Append(GenerateWhereStatement(whereConditionFields));
            return sb.ToString();
        }
    }
}
