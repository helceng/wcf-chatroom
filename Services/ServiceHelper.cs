using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contracts;

namespace Services
{
   public class ServiceHelper
    {
       public static void ShowMessage(string sender,string msg){
          Console.WriteLine(DateTime.Now.ToString()+" "+sender+" : "+msg);
       }



    }

   public class ClientInfo {

       public ICallback CallBack { set; get; }
       public DateTime LastOnlineTime { get; set; }
       public ClientInfo(ICallback cb, DateTime dt) { this.CallBack = cb; this.LastOnlineTime = dt; }
   }
}
