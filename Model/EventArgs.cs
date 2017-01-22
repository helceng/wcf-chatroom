using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class UserCallBackEventArgs:EventArgs
    {
        public UserInfo UserInfo { get; set; }

        public UserCallBackEventArgs(UserInfo arg) { UserInfo = arg; }

    }
}
