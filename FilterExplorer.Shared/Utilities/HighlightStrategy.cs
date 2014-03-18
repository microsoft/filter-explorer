/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using System.Collections.Generic;

namespace FilterExplorer.Utilities
{
    public class HighlightStrategy
    {
        public HighlightStrategy(int batchSize, List<int> highlightedIndexes)
        {
            BatchSize = batchSize;
            HighlightedIndexes = highlightedIndexes;
        }

        public int BatchSize { get; set; }
        public List<int> HighlightedIndexes { get; set; }
    }
}
