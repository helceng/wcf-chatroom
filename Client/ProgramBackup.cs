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
    class ProgramBackup
    {
        static UserInfo loginUser = null;

        static void Mainbk(string[] args)
        {
            Task task = null;
            string cmd = string.Empty;
            bool isOnline = false;
            bool runHeartBeart = false;
            Console.WriteLine("welcome to chat room,please login in first(l/userid), use command 'h' to see help text");

            ClientCallBack cb = new ClientCallBack();
            cb.OnUserJoined += OnUserJoined;
            SRcallback.CommunicationServiceClient proxy = new SRcallback.CommunicationServiceClient(new System.ServiceModel.InstanceContext(cb));


            //send msg to all with command: (p/msg),send msg to specific user with command: (u/username/msg)
            while ((cmd = Console.ReadLine()) != "exit")
            {
                try {
                    OpManager opm = new OpManager(cmd);
                    switch (opm.OP)
                    {
                        case OpType.E:
                            ClientHelper.ShowMessage("sys", opm.ErrorMsg+" please use correct command or see help text with 'h'");
                            break;
                        case OpType.R:
                            proxy.Register(new UserInfo(opm.CurrUserId));
                            break;
                        case OpType.L:
                            {
                                //登录
                                loginUser = new UserInfo(opm.CurrUserId);
                                proxy.Login(opm.CurrUserId);

                                //获取所有在线用户
                                List<UserInfo> userList = new List<UserInfo>();
                                userList = proxy.GetOnlineUsers().ToList();
                                string userNames = string.Empty;
                                userList.ForEach((x => { userNames += x.UserId + ","; }));
                                ClientHelper.ShowMessage("sys","currently online users:"+userNames.TrimEnd(new char[]{','}));
                                runHeartBeart = true;
                                //开始发送心跳包
                                task = new Task(() =>
                                {
                                    while (true && runHeartBeart)
                                    {
                                        Thread.Sleep(5000);
                                        try
                                        {
                                            proxy.SendHeartBeat(loginUser.UserId);
                                            isOnline = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            isOnline = false;
                                            ClientHelper.ShowMessage("sys", "trying connecting again...");
                                            try
                                            {
                                                proxy.Login(loginUser.UserId);
                                                isOnline = true;
                                            }
                                            catch (Exception e) { Thread.Sleep(10000); }
                                        }
                                    }
                                });
                                task.Start();//异步执行
                            }
                         
                            break;
                        case OpType.U:
                            if (!isOnline) { ClientHelper.ShowMessage("sys", "you are offline or service is not avaliable"); continue; }
                            proxy.SendUserMsg(loginUser.UserId,opm.ToUserId,opm.Msg);
                            break;
                        case OpType.P:
                            if (!isOnline) { ClientHelper.ShowMessage("sys", "you are offline or service is not avaliable"); continue; }
                            proxy.SendPublicMsg(loginUser.UserId,opm.Msg);
                            Thread.Sleep(500);
                            break;
                        case OpType.Q:
                            if (!isOnline) { ClientHelper.ShowMessage("sys", "you are offline or service is not avaliable"); continue; }
                            proxy.Leave(loginUser.UserId);
                            runHeartBeart = false;
                            break;
                        case OpType.H:
                            ClientHelper.ShowMessage("sys", OpManager.HelpText);
                            break;
                        default:
                            ClientHelper.ShowMessage("sys", opm.ErrorMsg + " please use correct command or see help text with 'h'");
                            break;
                    }
                }

                 catch(Exception ex){
                        ClientHelper.ShowMessage("sys", "exception occurs:"+ex.Message);
                        ClientHelper.ShowMessage("sys", "please login in again");
                 }
            }
        }


        public static void OnUserJoined(object sender,UserCallBackEventArgs e)
        {
            ClientHelper.ShowMessage("sys", e.UserInfo.UserId + " is online");
        }
    }
}