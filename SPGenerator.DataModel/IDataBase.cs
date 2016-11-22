using System.Collections.Generic;

namespace SPGenerator.DataModel
{
    public interface IDatabase
    {
        string ConnectionString { get; set; }
        List<DBTableInfo> GetDataBaseTables();
    }

}
