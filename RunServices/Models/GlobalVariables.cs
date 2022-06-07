using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RunServices.Models
{
    public static class GlobalVariables
    {
        public static string UserIP { get; set; }
        public static string HostName { get; set; }
        public static string path
        {
            get { return @"C:\AppData\Log.txt"; }
        }
    }
}