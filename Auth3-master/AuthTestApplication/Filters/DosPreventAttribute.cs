using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Timers;

namespace AuthTestApplication.Filters
{
    public class DosPreventAttribute : ActionFilterAttribute
    {
        public DosPreventAttribute()
        {
            CreateVisitedTimer();
            CreateBanningTimer();
        }

        private static Stack<string> _banList = new Stack<string>();
        private static Dictionary<string, int> _visitedList = new Dictionary<string, int>();
        private const int BanRequestsCount = 20;
        private const int VisitedListClearInterval = 2 * 1000;
        private const int BanListClearInterval = 30 * 60 * 1000; 
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if(HttpContext.Current.Request.IsAuthenticated)
                return;

            var userIp = HttpContext.Current.Request.UserHostAddress;
            if (_banList.Contains(userIp))
            {
                HttpContext.Current.Response.StatusCode = 403;
                HttpContext.Current.Response.End();
            }

            if (!_visitedList.ContainsKey(userIp))
            {
                _visitedList[userIp] = 1;
            }
            else if (_visitedList[userIp] == BanRequestsCount)
            {
                _banList.Push(userIp);
                _visitedList.Remove(userIp);
            }
            else
            {
                _visitedList[userIp]++;
            }
        }

        private static Timer CreateVisitedTimer()
        {
            var timer = new Timer { Interval = VisitedListClearInterval };

            timer.Start();
            timer.Elapsed += VisitedTimerElapsed;

            return timer;
        }
        
        private static Timer CreateBanningTimer()
        {
            var timer = new Timer { Interval = BanListClearInterval };

            timer.Start();
            timer.Elapsed += delegate { _banList.Pop(); };
            return timer;
        }

        private static void VisitedTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var keys = new List<string>(_visitedList.Keys);
            foreach (string key in keys)
            {
                _visitedList[key]--;
                if (_visitedList[key] == 0)
                {
                    _visitedList.Remove(key);
                }
            }
        }
    }
}