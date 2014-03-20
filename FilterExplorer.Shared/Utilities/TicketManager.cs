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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FilterExplorer.Utilities
{
    public class TicketManager
    {
        private static int _maxTickets = (int)Windows.UI.Xaml.Application.Current.Resources["MaximumTickets"];
        private static List<Ticket> _tickets = new List<Ticket>();
        private static long _index = 0;
        private static long _waitTime = 0;

        public static async Task<Ticket> AcquireTicket()
        {
            var count = _tickets.Count;

#if DEBUG
            var totalWaitTime = 0;
#endif

            while (_tickets.Count >= _maxTickets)
            {
                var waitTime = (int)_waitTime + 1;
                
#if DEBUG
                totalWaitTime += waitTime;
#endif

                await Task.Delay(waitTime);
            }

#if DEBUG
            if (totalWaitTime > 0)
            {
                System.Diagnostics.Debug.WriteLine("Had to wait for a ticket for a total of " + totalWaitTime + " ms");
            }
#endif

            Ticket ticket = new Ticket(_index++, DateTime.Now);

            _tickets.Add(ticket);

            return ticket;
        }

        public static void ReleaseTicket(Ticket ticket)
        {
            var timespan = (long)(DateTime.Now - ticket.Timestamp).TotalMilliseconds;
            var waitTime = (_waitTime + timespan) / 2;
            
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Ticket " + ticket.Id + " was reserved for " + waitTime + " ms");
#endif

            _tickets.Remove(ticket);

            _waitTime = _tickets.Count > 0 ? waitTime : 0;
        }
    }
}
