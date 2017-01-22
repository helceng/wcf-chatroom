using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Model;
using System.ServiceModel;
namespace Contracts
{
    [ServiceContract(Name = "CommunicationService", Namespace = "http://www.Helceng.com/", CallbackContract = typeof(ICallback), SessionMode = SessionMode.Required)]
    public interface ICommunication
    {
        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="userInfo"></param>
        [OperationContract(IsOneWay = true)]
        void Register(UserInfo userInfo);

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userInfo"></param>
        [OperationContract(IsOneWay = true)]
        void Login(string userId);

        [OperationContract(IsOneWay = true)]
        void Leave(string userId);


        /// <summary>
        /// 发送心跳包
        /// </summary>
        /// <param name="userId"></param>
        [OperationContract(IsOneWay = true)]
        void SendHeartBeat(string userId);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OperationContract]
        UserInfo GetUserInfo(string userId);

        [OperationContract]
        bool IsUserOnline(string userId);

        [OperationContract]
        List<UserInfo> GetOnlineUsers();

        [OperationContract(IsOneWay = true)]
        void SendGroupMsg(string userId, string groupId,string msg);

        [OperationContract(IsOneWay = true)]
        void SendPublicMsg(string userId, string msg);

        [OperationContract(IsOneWay = true)]
        void SendUserMsg(string fromUserId, string toUserId, string msg);

    }


}