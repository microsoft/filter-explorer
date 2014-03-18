using System;
using System.Collections.Generic;
using System.Text;

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
