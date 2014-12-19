/*
 * Copyright (c) 2014 Microsoft Mobile
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
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
