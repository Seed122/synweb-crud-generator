using SPGenerator;
using SPGenerator.Core;
using SPGenerator.DAL;
using SPGenerator.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using SPGenerator.Common;

namespace SPGenerator.UI.Models
{
    internal class MainWinModel
    {
        public List<DBTableInfo> GetDbInfo(string connectionString)
        {
            IDatabase database =  GetDataBaseObject(connectionString);
            return database.GetDataBaseTables().OrderBy(x => x.FullTableName).ToList();
        }

        public StoredProcedure GenerateSp(DBTableInfo tableInfo, string nodeName, List<DBTableColumnInfo> selectedFields, List<DBTableColumnInfo> whereConditionFields)
        {
            BaseSPGenerator spGenerator = SPFactory.GetSpGeneratorObject(nodeName);
            StoredProcedure procedure = spGenerator.GenerateSp(tableInfo, selectedFields, whereConditionFields);
            SaveProcedureToFile(procedure);
            return procedure;
        }
        private IDatabase GetDataBaseObject(string connectionString)
        {
            return new SqlDatabase(connectionString);
        }

        private void SaveProcedureToFile(StoredProcedure procedure)
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Stored Procedures");
            if(!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var path = Path.Combine(dir, procedure.Name + ".sql");
            File.WriteAllText(path, procedure.Script, Encoding.UTF8);
        }
    }
}
