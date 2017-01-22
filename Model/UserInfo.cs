using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// 用户信息类
    /// </summary>
    public class UserInfo
    {
        public string UserId { get; set; }
        public string UserRealName {get;set;}
        public string UserNickName { get; set; }
        public string Sex { get; set; }
        public string TelPhone { get; set; }
        public int Age { get; set; }
        public string Region { get; set; }
        public string Hobby { get; set; }
        public string Note { get; set; }
        public UserInfo() { }
        public UserInfo(string id) { UserId = id; }
    }
}
