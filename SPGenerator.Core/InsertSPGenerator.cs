using SPGenerator.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPGenerator.Core
{
    class InsertSPGenerator : BaseSPGenerator
    {

        protected override string GetSpName(string tableName)
        {
            return tableName + "_Insert";
        }
            
        protected override string GenerateStatement(DBTableInfo tableInfo, List<DBTableColumnInfo> selectedFields, List<DBTableColumnInfo> whereConditionFields)
        {
            List<string> values = new List<string>();
            List<string> fields = new List<string>();
            foreach (DBTableColumnInfo colInf in selectedFields.Where(x => !x.Exclude))
            {
                values.Add(PrefixInputParameter + colInf.ColumnName);
                fields.Add(Wrap(colInf.ColumnName));
            }
            var sb = new StringBuilder();
            sb.Append("\tINSERT INTO " + tableInfo.FullTableName);
            if (values.Any())
            {
                sb.AppendLine(" (" + string.Join(", ", fields) + ")");
                sb.Append("\tVALUES (" + string.Join(", ", values) + ")");
            }
            else
            {
                sb.AppendLine();
                sb.Append("\tDEFAULT VALUES");
            }
            if (tableInfo.Columns.Any(x => x.IsIdentity))
            {
                sb.AppendLine();
                sb.Append("\tSELECT @@IDENTITY");
            }
            return sb.ToString();
        }

        protected override string GenerateInputParameters(List<DBTableColumnInfo> fields)
        {
            var inputParams = new List<string>();
            foreach (DBTableColumnInfo colInf in fields.Where(x => !x.Exclude))
            {
                string par = PrefixInputParameter + colInf.ColumnName + " " + colInf.DataType;
                if (colInf.DataType.Contains("char"))
                {
                    string charMaxLength = colInf.CharacterMaximumLength > 0
                        ? colInf.CharacterMaximumLength.ToString()
                        : "MAX";
                    par += "(" + charMaxLength + ")";
                }
                inputParams.Add(par);
            }
            return string.Join("," + Environment.NewLine, inputParams);
        }
    }
}
