using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPGenerator.DataModel
{
    public class Settings
    {
        public string PrefixWhereParameter = "@";
        public string PrefixInputParameter = "@";
        public string PostfixInsertSp = "_Insert";
        public string PostfixUpdateSp = "_Update";
        public string PostfixDeleteSp = "_Delete";
        public string  errorHandling = "Yes";
    }
}
