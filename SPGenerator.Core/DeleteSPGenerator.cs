using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPGenerator.DataModel;

namespace SPGenerator.Core
{
    class DeleteSPGenerator:BaseSPGenerator
    {
        protected override string GetSpName(string tableName, List<DBTableColumnInfo> whereConditionCols)
        {
            return tableName + "_Delete";
        }

        protected override string GenerateStatement(DBTableInfo tableInfo, List<DBTableColumnInfo> selectedCols, List<DBTableColumnInfo> whereConditionCols)
        {
            return "\tDELETE FROM " + tableInfo.FullTableName + Environment.NewLine 
                + GenerateWhereStatement(whereConditionCols);
        }

    }
}
