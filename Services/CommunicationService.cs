using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contracts;
using System.Data;
using System.IO;
using System.Timers;
using System.ServiceModel;
using System.Threading;
using Model;
 
namespace Services
{
    public class CommunicationService:ICommunication
    {
        //内部同步锁
        private static object _lockObj = new object();
        //连接上的客户端
        private static Dictionary<string, ClientInfo> _clientList = new Dictionary<string, ClientInfo>();
        //用户信息
        private static List<UserInfo> _userList = new List<UserInfo>();
        //当前链接
        private ICallback currentCallback = OperationContext.Current.GetCallbackChannel<ICallback>();
       
        private const string SysSender = "sys";
        private UserInfo SysUser = new UserInfo(SysSender);

        #region 'Interface implemented'

        public void Register(UserInfo userInfo) 
        {
            try
            {
                ICallback callback = OperationContext.Current.GetCallbackChannel<ICallback>();
                lock (_lockObj)
                {
                    if (_userList.Find(x=>x.UserId == userInfo.UserId) == null)
                    {
                        _userList.Add(userInfo);
                        callback.ShowMsg(SysUser, userInfo.UserId + " registered successfully");
                        ServiceHelper.ShowMessage(SysSender, userInfo.UserId + " registered successfully");
                    }
                    else {
                        callback.ShowMsg(SysUser,userInfo.UserId + " has already been registered");
                    }
                }
            }

            catch (Exception ex)
            {
                ServiceHelper.ShowMessage(SysSender, "Exception occured:" + ex.Message);
            }
        
        }

        public void Login(string userId)
        {
            try 
            { 
                 ICallback callback = OperationContext.Current.GetCallbackChannel<ICallback>();
                 if (GetUserInfo(userId) == null) { callback.ShowMsg(SysUser, userId + " is not existed"); return; }
                 lock (_lockObj)
                 {
                     if (_clientList.ContainsKey(userId) == true)
                     {
                         //若当前用户有登录，则强制其退出，给客户端发送通知，停止心跳包发送
                         _clientList[userId].CallBack.ForceToQuit(GetUserInfo(userId));
                         _clientList.Remove(userId);
                     }

                     _clientList.Add(userId, new ClientInfo(callback, DateTime.Now));

                     //服务端显示
                     ServiceHelper.ShowMessage(SysSender, userId + " is online,welcome" + "(ServiceInstance HashCode:" + this.GetHashCode() + "）");
                     //客户端回调,所有用户收到新用户上线通知
                     UserJoined(userId);
                 }
                   
            }
            catch(Exception ex){

                  ServiceHelper.ShowMessage(SysSender, "Exception occured:" + ex.Message);
            
            }
           
        }

        public void Leave(string userId)
        {
            try
            {
                if (_clientList.ContainsKey(userId))
                {
                    //给客户端发送离线通知，客户端据此注销用户，关闭心跳包
                    _clientList[userId].CallBack.UserLeave(GetUserInfo(userId));
                    _clientList.Remove(userId);
                  
                    //给其他用户发送用户离开的系统通知
                    _clientList.ToList().ForEach((x =>
                    {
                        try
                        {
                            x.Value.CallBack.ShowMsg(SysUser, userId + " is left");
                        }
                        catch (Exception ex)
                        {
                            ServiceHelper.ShowMessage(SysSender, "Exception occured:" + ex.Message);
                        }
                    }));

                    ServiceHelper.ShowMessage(SysSender, userId + " is left");
                }
            }
            catch (Exception ex)
            {
                ServiceHelper.ShowMessage(SysSender, "Exception occured:" + ex.Message);
            }
        }

        public void SendHeartBeat(string userId)
        {
            try
            {
                lock (_lockObj)
                {
                    ICallback callback = OperationContext.Current.GetCallbackChannel<ICallback>();
                    if (_clientList.ContainsKey(userId) == true)
                    {
                        _clientList.Remove(userId);
                        _userList.Remove(_userList.Find(x => x.UserId == userId));
                    }
                    _clientList.Add(userId, new ClientInfo(callback, DateTime.Now));
                    _userList.Add(new UserInfo(userId));
                    ServiceHelper.ShowMessage(SysSender, userId + " is sending heartbeat package");
                }
            }

            catch (Exception ex)
            {
                ServiceHelper.ShowMessage(SysSender, "Exception occured:" + ex.Message);
            }

        }

        public UserInfo GetUserInfo(string userId)
        {
            return _userList.Find(x => x.UserId == userId);
        }

        public bool IsUserOnline(string userId)
        {
            try
            {
                return _clientList.ContainsKey(userId);
            }
            catch (Exception ex) { return false; }
        }

        public List<UserInfo> GetOnlineUsers()
        {
            try
            {
                List<UserInfo> list = new List<UserInfo>();
                _clientList.ToList().ForEach((x => { list.Add(new UserInfo(x.Key)); }));
                return list;
            }
            catch (Exception ex) { return null; }
        }

        public void SendGroupMsg(string userId, string groupId, string msg) {   }

        public void SendPublicMsg(string userId, string msg)
        {
            ServiceHelper.ShowMessage(SysSender, userId + " is saying to all:" + msg);

            _clientList.ToList().ForEach((x => {
                try { 
                    x.Value.CallBack.ShowMsg(_userList.Find(u => u.UserId == userId), msg);
                }
                catch (Exception ex)
                {
                    ServiceHelper.ShowMessage(SysSender, "Exception occured:" + ex.Message);
                }
            }));
        }
        
        public void SendUserMsg(string fromUserId, string toUserId, string msg)
        {
            if (!_clientList.ContainsKey(toUserId))
            {
                ICallback callback = OperationContext.Current.GetCallbackChannel<ICallback>();
                callback.ShowMsg(SysUser, toUserId + " is not online");
                return;
            }
            ServiceHelper.ShowMessage(SysSender, fromUserId + " is chatting with " + toUserId + " : " + msg);
            _clientList.ToList().ForEach((x => {
                try {
                    if (x.Key == toUserId) x.Value.CallBack.ShowMsg(_userList.Find(u => u.UserId == fromUserId), msg);
                }
                catch (Exception ex)
                {
                    ServiceHelper.ShowMessage(SysSender, "Exception occured:" + ex.Message);
                }
            }));
        }

        #endregion



        #region 'inner method'


        public void UserJoined(string userId)
        {
            _clientList.ToList().ForEach(c => {
                try
                {
                    c.Value.CallBack.UserJoined(_userList.Find(u => u.UserId == userId));
                }
                catch (Exception ex)
                {
                    ServiceHelper.ShowMessage(SysSender, "Exception occured:" + ex.Message);
                }
            });
          
        }

        public static void HeartBeatListener()
        {

            _clientList.ToList().ForEach((x =>
            {
                try
                {
                    if ((DateTime.Now - x.Value.LastOnlineTime) > TimeSpan.FromSeconds(5))
                    {
                        //通知客户端离线
                        try
                        {
                            _clientList[x.Key].CallBack.UserOffline(_userList.Find(u => u.UserId == x.Key));
                        }

                        catch(Exception ex)
                        {
                            ServiceHelper.ShowMessage(SysSender, "Exception occured:" + ex.Message);
                        }
                        finally {
                            _clientList.Remove(x.Key);
                            ServiceHelper.ShowMessage(SysSender, x.Key + " is offline");
                        }
                    }
                }

                catch (Exception ex)
                {
                    ServiceHelper.ShowMessage(SysSender, "Exception occured:" + ex.Message);
                }
            })
            );
        }
        #endregion
    }

}
