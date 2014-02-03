using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterExplorer.Filters
{
    public abstract class Filter
    {
        public string Id
        {
            get
            {
                return GetType().ToString();
            }
        }

        public string Name { get; protected set; }
    }

    public class DummyFilter : Filter
    {
    }
}
