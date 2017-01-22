using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
   public  class OpManager
    {
     public OpType OP = OpType.E;
     public string Msg;
     public string CurrUserId;
     public string ToUserId;
     public string ErrorMsg = "Invalid Parameters";
     public static string HelpText = @"
                      /*********************************************
                      Register user:                  r/userid
                      Login user:                     l/userid
                      Send message to another user：  u/touserid/msg
                      Send message to all：           p/msg
                      Show online users:              s
                      Login out:                      q
                      See help info:                  h
                      *********************************************/
                                   ";


     public OpManager(string cmd) {
         string[] opr = cmd.Split(new char[] { '/' });
         if (opr == null || opr.Length == 0) { OP = OpType.E; return; }
         switch (opr[0].Trim().ToUpper()) {
             case "R":
                 if (opr.Length != 2) { return; }
                 else { OP = OpType.R; CurrUserId = opr[1].Trim(); }
                 break;
             case "L":
                 if (opr.Length != 2) { return; }
                 else { OP = OpType.L; CurrUserId = opr[1].Trim(); }
                 break;
             case "U":
                 if (opr.Length != 3) { return; }
                 else { OP = OpType.U; ToUserId = opr[1].Trim(); Msg = opr[2].Trim(); }
                 break;
             case "P":
                 if (opr.Length != 2) { return; }
                 else { OP = OpType.P; Msg = opr[1].Trim(); }
                 break;
             case "Q":
                 OP = OpType.Q;
                 break;
             case "H":
                 OP = OpType.H;
                 break;
             case "S":
                 OP = OpType.S;
                 break;
             default: OP = OpType.E; ErrorMsg = "Invalid cmd"; break;
         }
     }

     /// <summary>
     /// 利用正则表达式分析输入命令
     /// </summary>
     /// <param name="cmd"></param>
     public void ExtractOperation(string cmd)
     { 
     
     
     }

    }

   public enum OpType { 
       E=-1,//unknown
       R=0,//注册
       L=1,//登录
       U=2,//给特定用户发送信息
       P=3,//给所有用户发送信息
       Q=4, //登出
       H=5, //帮助
       S=6 //获取在线用户
   }
}
