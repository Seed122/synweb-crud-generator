using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPGenerator.DataModel;

namespace SPGenerator.Core
{
    class SelectBySPGenerator : BaseSPGenerator
    {
        protected override string GetSpName(string tableName, List<DBTableColumnInfo> whereConditionCols)
        {
            if (whereConditionCols.Count > 1)
                throw new NotImplementedException();
            var whereCol = whereConditionCols.First();
            return $"{tableName}_SelectBy{whereCol.ColumnName}";
        }

        protected override string GenerateStatement(DBTableInfo tableInfo, List<DBTableColumnInfo> selectedCols, List<DBTableColumnInfo> whereConditionCols)
        {
            string selectFieldsStr;
            if (selectedCols.Count == tableInfo.Columns.Count)
            {
                selectFieldsStr = "*";
            }
            else
            {
                List<string> fields = new List<string>();
                foreach (DBTableColumnInfo colInf in selectedCols.Where(x => !x.Exclude))
                {
                    fields.Add(Wrap(colInf.ColumnName));
                }
                selectFieldsStr = string.Join(", ", fields);
            }

            var sb = new StringBuilder();
            sb.AppendLine($"\tSELECT {selectFieldsStr} FROM {tableInfo.FullTableName}{Environment.NewLine}{GenerateWhereStatement(whereConditionCols)}");
            return sb.ToString();
        }

        protected override string GenerateInputParameters(List<DBTableColumnInfo> fields)
        {
            return "";
        }
    }
}
