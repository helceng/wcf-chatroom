using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
   public class ClientHelper
    {
       public static void ShowMessage(string sender, string msg)
       {
           Console.WriteLine(DateTime.Now.ToString() + " " + sender + " : " + msg);
       }
    }
}
