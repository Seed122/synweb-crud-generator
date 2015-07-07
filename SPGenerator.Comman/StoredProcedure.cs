using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPGenerator.Common
{
    public class StoredProcedure
    {
        public string Name { get; set; }
        public string Script { get; set; }
        public string DropScript { get; set; }
    }
}
