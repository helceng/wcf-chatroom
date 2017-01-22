using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Model;

namespace Client
{
    public class ClientCallBack : SRcallback.CommunicationServiceCallback
    {
        /// <summary>
        /// 回调引发该事件
        /// </summary>
        public delegate void EventHandler(object sender, UserCallBackEventArgs e);
        public event EventHandler OnUserJoined;
        public event EventHandler OnUserLeave;
        public event EventHandler OnUserOffline;
        public event EventHandler OnForceQuit;


        /// <summary>
        /// 服务端向客户端写消息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="msg"></param>
        public void ShowMsg(UserInfo info, string msg)
        {
            Console.WriteLine(DateTime.Now.ToString() + " " + info.UserId + " : " + msg);

        }


        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="joinedUser"></param>
        public void UserJoined(UserInfo joinedUser)
        {
            if (this.OnUserJoined != null)
            {
                this.OnUserJoined(this, new UserCallBackEventArgs(joinedUser));
            }
        }

        /// <summary>
        /// 用户登出
        /// </summary>
        /// <param name="leftUser"></param>
        public void UserLeave(UserInfo leftUser)
        {
            if (this.OnUserLeave != null)
            {
                this.OnUserLeave(this, new UserCallBackEventArgs(leftUser));
            }

        }
        /// <summary>
        /// 心跳包检查用户离线
        /// </summary>
        /// <param name="offlinedUser"></param>
        public void UserOffline(UserInfo offlinedUser)
        {

            if (this.OnUserOffline != null)
            {
                this.OnUserOffline(this, new UserCallBackEventArgs(offlinedUser));
            }
        }

        /// <summary>
        /// 被同用户不同终端强制下线回调
        /// </summary>
        /// <param name="quitUser"></param>

        public void ForceToQuit(UserInfo quitUser) {

            if (this.OnForceQuit != null)
            {
                this.OnForceQuit(this, new UserCallBackEventArgs(quitUser));
            }
        }
    }
}
