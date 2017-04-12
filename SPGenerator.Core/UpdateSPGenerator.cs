using SPGenerator.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPGenerator.Core
{
    class UpdateSPGenerator : BaseSPGenerator
    {
        protected override string GetSpName(string tableName, List<DBTableColumnInfo> whereConditionCols)
        {
            return tableName + "_Update";
        }
        protected override string GenerateStatement(DBTableInfo tableInfo, List<DBTableColumnInfo> selectedCols, List<DBTableColumnInfo> whereConditionCols)
        {
            if(!selectedCols.Any()){
                throw new ArgumentException("selectedCols");
            }
            var sb = new StringBuilder();

            sb.AppendLine("\tUPDATE " + tableInfo.FullTableName + " SET");

            var statements = new List<string>();
            foreach (DBTableColumnInfo colInf in selectedCols.Where(x => !x.Exclude))
            {
                statements.Add("\t\t"+Wrap(colInf.ColumnName) + "=" + PrefixInputParameter + colInf.ColumnName);
            }
            sb.AppendLine(string.Join("," + Environment.NewLine, statements));
            sb.Append(GenerateWhereStatement(whereConditionCols));
            return sb.ToString();
        }
    }
}
