using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using System.ServiceModel;
using System.ServiceModel.Description;
using Contracts;
using Services;


namespace Hosting
{
    public class Program
    {
        static void Main(string[] args)
        {
            //using (ServiceHost host = new ServiceHost(typeof(CalculatorService)))
            //{
            //    host.AddServiceEndpoint(typeof(ICalculator), new WSHttpBinding(), "http://127.0.0.1:8888/calculatorservice");
            //    if (host.Description.Behaviors.Find<ServiceMetadataBehavior>() == null)
            //    {
            //        ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
            //        behavior.HttpGetEnabled = true;
            //        behavior.HttpGetUrl = new Uri("http://127.0.0.1:8888/calculatorservice/metadata");
            //        host.Description.Behaviors.Add(behavior);
            //    }
            //    host.Opened += delegate
            //    {
            //        Console.WriteLine("CalculaorService已经启动，按任意键终止服务！");
            //    };

            //    host.Open();
            //    Console.Read();
            //}


            using (ServiceHost host = new ServiceHost(typeof(Services.CommunicationService)))
            {
                host.Opened += delegate
                {
                    Console.WriteLine("CommunicationService已经启动");
                    Console.WriteLine("WCF心跳包开始监听");
                    while (true)
                    {
                        CommunicationService.HeartBeatListener();
                        System.Threading.Thread.Sleep(2000);
                    }
                };
                host.Open();
                Console.Read();
            }

        }
    }
}