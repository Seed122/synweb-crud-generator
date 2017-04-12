using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPGenerator.DataModel;

namespace SPGenerator.Core
{
    class SelectOneSPGenerator:BaseSPGenerator
    {
        protected override string GetSpName(string tableName, List<DBTableColumnInfo> whereConditionCols)
        {
            return tableName + "_SelectOne";
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

            return
                $"\tSELECT {selectFieldsStr} FROM {tableInfo.FullTableName}{Environment.NewLine}{GenerateWhereStatement(whereConditionCols)}";
        }

        protected override string GenerateInputParameters(List<DBTableColumnInfo> fields)
        {
            return "";
        }
    }
}
