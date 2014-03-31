/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using System;

namespace FilterExplorer.Utilities
{
    public class Ticket : IDisposable
    {
        public long Id { get; private set; }
        public DateTime Timestamp { get; private set; }

        public Ticket(long id, DateTime timestamp)
        {
            Id = id;
            Timestamp = timestamp;
        }

        public void Dispose()
        {
            TicketManager.ReleaseTicket(this);
        }
    }
}
