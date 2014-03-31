/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using FilterExplorer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FilterExplorer.Filters
{
    public static class FilterFactory
    {
        private static List<Type> _ignoredInStream = new List<Type>()
        {
            typeof(EmbossFilter),
            typeof(RotationLeftFilter),
            typeof(RotationRightFilter),
            typeof(FlipHorizontalFilter),
            typeof(FlipVerticalFilter),
            typeof(BlurFilter)
        };

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

        public static ObservableList<Filter> CreateStreamFilters()
        {
            var filters = new ObservableList<Filter>();
            var types = typeof(Filter).GetTypeInfo().Assembly.ExportedTypes
                                      .Where(type => type.GetTypeInfo().IsClass &&
                                             type.GetTypeInfo().IsAbstract == false &&
                                             type.GetTypeInfo().IsSubclassOf(typeof(Filter)));

            foreach (var type in types)
            {
                if (!_ignoredInStream.Contains(type))
                {
                    filters.Add((Filter)Activator.CreateInstance(type));
                }
            }

            return filters;
        }
    }
}
