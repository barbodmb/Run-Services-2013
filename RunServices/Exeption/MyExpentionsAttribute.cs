using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RunServices.Exeption
{
    public class MyExpentionsAttribute:FilterAttribute,IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                //filterContext.HttpContext.Response.Write("<h1>Error!</h1>" + filterContext.Exception.Message);

                //var path = @"C:\AppData\test.txt"; // Directory.GetCurrentDirectory() + (@"\ErrorLog.txt");
                //using (StreamWriter sw = new StreamWriter(path))
                //{
                //    sw.Write("\n"+filterContext.Exception.Message);
                //}
            }
        }
    }
}