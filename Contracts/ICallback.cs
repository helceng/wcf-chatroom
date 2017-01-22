using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using System.ServiceModel;
using Contracts;

namespace Contracts
{
   public  interface ICallback
    {
       /// <summary>
       /// 欢迎新接入者
       /// </summary>
        [OperationContract(IsOneWay = true)]
       void UserJoined(UserInfo userInfo);

        [OperationContract(IsOneWay = true)]
        void UserLeave(UserInfo userInfo);

        [OperationContract(IsOneWay = true)]
        void UserOffline(UserInfo userInfo);

        [OperationContract(IsOneWay = true)]
        void ShowMsg(UserInfo userInfo,string msg);

        [OperationContract(IsOneWay = true)]
        void ForceToQuit(UserInfo userInfo);


    }
}
