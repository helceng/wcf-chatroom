using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using Model;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        const string SysSender = "sys";
        const string Hint = @"
        /************************************************************
        Welcome to chatroom! Just two steps required for chatting.

        Step1: Register your userid using command 'r/yourId'
        Step2: Login into chatroom using command 'l/yourId'

        Send public message with command 'p/yourMessage'
        Send private message with command 'u/otherUserId/yourMessage'
        Show all online users with command 's'

        For more commands,press 'h' to see guideline...
        ************************************************************/";

        static void Main(string[] args)
        {
           
            string cmd = string.Empty;
            Console.WriteLine(Hint);

            ClientManager CurrentClient = new ClientManager(new ClientCallBack());
            SRcallback.CommunicationServiceClient proxy = new SRcallback.CommunicationServiceClient(new System.ServiceModel.InstanceContext(CurrentClient.CallBackHandle));
            CurrentClient.Proxy = proxy;

            while ((cmd = Console.ReadLine()) != "exit")
            {
                try
                {
                    OpManager opm = new OpManager(cmd);
                    switch (opm.OP)
                    {
                        case OpType.E:
                            ShowHint(opm.ErrorMsg);
                            break;
                        case OpType.R:
                            CurrentClient.Register(opm.CurrUserId);
                            break;
                        case OpType.L:
                            CurrentClient.Login(opm.CurrUserId);
                            break;
                        case OpType.U:
                            CurrentClient.SendUserMessage(opm.ToUserId,opm.Msg);
                            break;
                        case OpType.P:
                            CurrentClient.SendPublicMessage(opm.Msg);
                            break;
                        case OpType.Q:
                            CurrentClient.LoginOut();
                            break;
                        case OpType.H:
                            ClientHelper.ShowMessage(SysSender, OpManager.HelpText);
                            break;
                        case OpType.S:
                            CurrentClient.ShowOnlineUsers();
                            break;
                        default:
                            ShowHint(opm.ErrorMsg);
                            break;
                    }
                }

                catch (Exception ex)
                {
                    ClientHelper.ShowMessage(SysSender, "exception occurs:" + ex.Message);
                }
            }
        }

        public static void ShowHint(string errorMsg)
        {
            ClientHelper.ShowMessage(SysSender, errorMsg + " ,please use correct command or press 'h' to see helptext");
        }
    }
}