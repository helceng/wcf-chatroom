using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Model;

namespace Client
{
    public class ClientManager
    {
        private string SysSender = "sys";
        private UserInfo _loginUserInfo;
        public UserInfo LoginUserInfo { set { _loginUserInfo = value; } get { return _loginUserInfo; } }

        private bool _isLogin = false;
        public bool IsLogin { set { _isLogin = value; } get { return _isLogin; } }

        private bool _isOnline = false;
        public bool IsOnline { set { _isOnline = value; } get { return _isOnline; } }

        //private bool _runHeartBeat = false;
        //public bool RunHeartBeat { set { _runHeartBeat = value; } get { return _runHeartBeat; } }

        private int _maxTryTimes = 10;

        private SRcallback.CommunicationServiceClient _proxy;
        public SRcallback.CommunicationServiceClient Proxy { set { _proxy = value; } get { return _proxy; } }

        private ClientCallBack _callBackHandle;
        public ClientCallBack CallBackHandle { set { _callBackHandle = value; } get { return _callBackHandle; } }

        public ClientManager(ClientCallBack cbh)
        {
            _callBackHandle = cbh;

            if (_callBackHandle != null)
            {
                _callBackHandle.OnUserJoined += OnUserJoined;
                _callBackHandle.OnUserLeave += OnUserLeave;
                _callBackHandle.OnUserOffline += OnUserOffline;
                _callBackHandle.OnForceQuit += OnForceQuit;
            }
        }

        public ClientManager(SRcallback.CommunicationServiceClient proxy)
        {
            _proxy = proxy;
        }

        public void Register(string userId)
        {
            try { 
                  _proxy.Register(new UserInfo(userId));
            }
            catch(Exception ex){
                ClientHelper.ShowMessage(SysSender, "failed to register");
            }
        }

        public void Login(string userId) {

            try
            {
                if (_isLogin)
                {
                    ClientHelper.ShowMessage(SysSender, "please logout first if you want to login with other account");
                    return;
                }

                _proxy.Login(userId);
                _loginUserInfo = _proxy.GetUserInfo(userId);
                if (_loginUserInfo != null)
                {
                    _isLogin = true;
                    _isOnline = true;
                    ShowOnlineUsers();
                    SendHeartBeat();
                }
                else {
                    ClientHelper.ShowMessage(SysSender, "user info is not found");
                    _isLogin = false;
                    _isOnline = false;
                }
            }
            catch(Exception ex)
            {
                _isLogin = false;
                _isOnline = false;
                ClientHelper.ShowMessage(SysSender, "failed to login");
            }
        }

        public void ShowOnlineUsers()
        {
            try 
            {
                List<UserInfo> userList = new List<UserInfo>();
                userList = _proxy.GetOnlineUsers().ToList();
                string userNames = string.Empty;
                userList.ForEach((x => { userNames += x.UserId + ","; }));
                ClientHelper.ShowMessage(SysSender, "currently online users:" + userNames.TrimEnd(new char[] { ',' }));
            }
            catch (Exception ex)
            {
                ClientHelper.ShowMessage(SysSender, "failed to get online users:"+ex.Message);
            }
          
        }

        public void SendUserMessage(string toUserId,string msg) {

            try {
                if (!_isOnline||!_isLogin)
                {
                    ClientHelper.ShowMessage(SysSender, "you are offline,please login first");
                    return;
                }
                _proxy.SendUserMsg(_loginUserInfo.UserId, toUserId, msg);
            }
            catch(Exception ex){
              ClientHelper.ShowMessage(SysSender, "failed to send msg");
            }
        }

        public void SendPublicMessage(string msg)
        {
            try
            {
                if (!_isOnline || !_isLogin)
                {
                    ClientHelper.ShowMessage(SysSender, "you are offline,please login first");
                    return;
                }
                _proxy.SendPublicMsg(_loginUserInfo.UserId, msg);
            }
            catch (Exception ex)
            {
                ClientHelper.ShowMessage(SysSender, "failed to send public msg");
            }
        }

        public void LoginOut() {
            try
            {
                if (!_isLogin||_loginUserInfo==null)
                {
                    ClientHelper.ShowMessage(SysSender, "you are not logined in or offline already");
                    return;
                }
                _proxy.Leave(_loginUserInfo.UserId);
                _isLogin = false;
                _isOnline = false;
                _loginUserInfo = null;
            }
            catch (Exception ex)
            {
                ClientHelper.ShowMessage(SysSender, "failed to login out");
            }
        }

        public void SendHeartBeat()
        {
            if (_proxy == null || _loginUserInfo == null || !_isLogin) return;
            int tryTimes = 0;
            var task = new Task(() =>
             {
                 while (_isLogin && tryTimes <= _maxTryTimes)
                 {
                     Thread.Sleep(5000);
                     try
                     {
                         //double chekc
                         if (_isLogin) _proxy.SendHeartBeat(_loginUserInfo.UserId);
                         
                     }
                     catch (Exception ex)
                     {
                         try {
                             if (_isLogin) {
                                 ClientHelper.ShowMessage(SysSender, "trying connecting again");
                                 _proxy.Login(_loginUserInfo.UserId);
                                 _isLogin = true;
                                 _isOnline = true;
                                 tryTimes = 0;
                                 ClientHelper.ShowMessage(SysSender, "connected successfully");
                             }
                         }

                         catch (Exception e)
                         {
                             tryTimes++;
                             ClientHelper.ShowMessage(SysSender, "failed to connect...");
                         }
                     }
                 }
                 _isLogin = false;
                 _isOnline = false;
                 _loginUserInfo = null;
                 ClientHelper.ShowMessage(SysSender, "you are logined out");
             });
            task.Start();//异步执行

        }


        public void OnUserJoined(object sender,UserCallBackEventArgs e)
        {
            ClientHelper.ShowMessage(SysSender, e.UserInfo.UserId + " is online");

        }

        public void OnUserLeave(object sender, UserCallBackEventArgs e)
        {
            _loginUserInfo = null;
            _isLogin = false;
            _isOnline = false;
            ClientHelper.ShowMessage(SysSender, e.UserInfo.UserId + "  is left");

        }

        public void OnUserOffline(object sender, UserCallBackEventArgs e)
        {
            _isOnline = false;
            ClientHelper.ShowMessage(SysSender, e.UserInfo.UserId + " is offline");
        }

        public void OnForceQuit(object sender, UserCallBackEventArgs e)
        {
            _loginUserInfo = null;
            _isLogin = false;
            _isOnline = false;
            ClientHelper.ShowMessage(SysSender,"you are forced to quit because you are logined somewhere else");
        }

    }

  
}
