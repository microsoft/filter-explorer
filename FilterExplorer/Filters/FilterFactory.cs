using FilterExplorer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace FilterExplorer.Filters
{
    public static class FilterFactory
    {
        public static Filter CreateFilter(string id)
        {
            return (Filter)Activator.CreateInstance(Type.GetType(id));
        }

        public static ObservableList<Filter> CreateFilters(List<string> ids)
        {
            var list = new ObservableList<Filter>();

            foreach (var id in ids)
            {
                list.Add(CreateFilter(id));
            }

            return list;
        }

        public static ObservableList<Filter> CreateAllFilters()
        {
            var filters = new ObservableList<Filter>();
            var types = typeof(Filter).GetTypeInfo().Assembly.ExportedTypes
                                      .Where(type => type.GetTypeInfo().IsClass &&
                                             type.GetTypeInfo().IsAbstract == false &&
                                             type.GetTypeInfo().IsSubclassOf(typeof(Filter)));

            foreach (var type in types)
            {
                filters.Add((Filter)Activator.CreateInstance(type));
            }

            return filters;
        }
    }
}
