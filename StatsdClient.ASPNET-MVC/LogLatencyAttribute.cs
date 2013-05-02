using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace StatsdClient.ASPNET_MVC
{
    public class LogLatencyAttribute : ActionFilterAttribute
    {
      private Stopwatch _stopwatch;
      private string _name;
      private IStatsd _statsd;

      public LogLatencyAttribute(string name)
      {
        if (string.IsNullOrEmpty(name))
        {
          throw new ArgumentException("Name cannot be null or empty.", "name");
        }
        _name = name;
      }

      public override void OnActionExecuting(ActionExecutingContext filterContext)
      {
        _stopwatch = Stopwatch.StartNew();
        if (!filterContext.Controller.TempData.ContainsKey("statsdclient"))
        {
          throw new ArgumentNullException("statsdclient", "Could not find the statsdclient reference in the controller's TempData.");
        }
        _statsd = filterContext.Controller.TempData["statsdclient"] as IStatsd;
      }

      public override void OnActionExecuted(ActionExecutedContext filterContext)
      {
        _stopwatch.Stop();
        _statsd.LogTiming(_name, (int)_stopwatch.ElapsedMilliseconds);
      }
    }
}
