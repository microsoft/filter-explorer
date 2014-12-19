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
