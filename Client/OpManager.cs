using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

         try {

             string[] opr = cmd.Split(new char[] { '/' });

             if (CommandRegExpression.Register.IsMatch(cmd)) { OP = OpType.R; CurrUserId = opr[1].Trim(); return; }

             if (CommandRegExpression.Login.IsMatch(cmd)) { OP = OpType.L; CurrUserId = opr[1].Trim(); return; }

             if (CommandRegExpression.PrivateSend.IsMatch(cmd)) { OP = OpType.U; ToUserId = opr[1].Trim(); Msg = opr[2].Trim(); return; }

             if (CommandRegExpression.PublicSend.IsMatch(cmd)) { OP = OpType.P; Msg = opr[1].Trim(); return; }

             if (CommandRegExpression.LoginOut.IsMatch(cmd)) { OP = OpType.Q; return; }

             if (CommandRegExpression.Help.IsMatch(cmd)) { OP = OpType.H; return; }

             if (CommandRegExpression.ShowOnlineUser.IsMatch(cmd)) { OP = OpType.S; return; }

             OP = OpType.E; ErrorMsg = "Invalid cmd";
         }

         catch {

             OP = OpType.E; ErrorMsg = "Invalid cmd";
         }
         


         //string[] opr = cmd.Split(new char[] { '/' });
         //if (opr == null || opr.Length == 0) { OP = OpType.E; return; }
         //switch (opr[0].Trim().ToUpper()) {
         //    case "R":
         //        if (opr.Length != 2) { return; }
         //        else { OP = OpType.R; CurrUserId = opr[1].Trim(); }
         //        break;
         //    case "L":
         //        if (opr.Length != 2) { return; }
         //        else { OP = OpType.L; CurrUserId = opr[1].Trim(); }
         //        break;
         //    case "U":
         //        if (opr.Length != 3) { return; }
         //        else { OP = OpType.U; ToUserId = opr[1].Trim(); Msg = opr[2].Trim(); }
         //        break;
         //    case "P":
         //        if (opr.Length != 2) { return; }
         //        else { OP = OpType.P; Msg = opr[1].Trim(); }
         //        break;
         //    case "Q":
         //        OP = OpType.Q;
         //        break;
         //    case "H":
         //        OP = OpType.H;
         //        break;
         //    case "S":
         //        OP = OpType.S;
         //        break;
         //    default: OP = OpType.E; ErrorMsg = "Invalid cmd"; break;

     }

     /// <summary>
     /// 利用正则表达式分析输入命令
     /// </summary>
     /// <param name="cmd"></param>
     public void RegTest(string cmd)
     {

         //Console.WriteLine("String: " + cmd + " is " + (CommandRegExpression.Register.IsMatch(cmd) ? "matched" : "unmatched ") + "with pattern : " + CommandRegExpression.Register.ToString());
         //Console.WriteLine("String: " + cmd + " is " + (CommandRegExpression.Login.IsMatch(cmd) ? "matched" : "unmatched ") + "with pattern : " + CommandRegExpression.Login.ToString());
         //Console.WriteLine("String: " + cmd + " is " + (CommandRegExpression.PrivateSend.IsMatch(cmd) ? "matched" : "unmatched ") + "with pattern : " + CommandRegExpression.PrivateSend.ToString());
         //Console.WriteLine("String: " + cmd + " is " + (CommandRegExpression.PublicSend.IsMatch(cmd) ? "matched" : "unmatched ") + "with pattern : " + CommandRegExpression.PublicSend.ToString());
         //Console.WriteLine("String: " + cmd + " is " + (CommandRegExpression.ShowOnlineUser.IsMatch(cmd) ? "matched" : "unmatched ") + "with pattern : " + CommandRegExpression.ShowOnlineUser.ToString());
         //Console.WriteLine("String: " + cmd + " is " + (CommandRegExpression.LoginOut.IsMatch(cmd) ? "matched" : "unmatched ") + "with pattern : " + CommandRegExpression.LoginOut.ToString());
         //Console.WriteLine("String: " + cmd + " is " + (CommandRegExpression.Help.IsMatch(cmd) ? "matched" : "unmatched ") + "with pattern : " + CommandRegExpression.Help.ToString());
     
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

    public class CommandRegExpression 
    {
        //命令行 正则表达式集

        //注册 Register  格式：r/userid 
        public static  Regex Register { get { return new Regex("^[rR]/");}}

        //登录 Login  格式：l/userid 
        public static Regex Login { get { return new Regex("^[lL]/"); } }

        //给特定用户发送消息 Send message to another user  格式：u/touserid/msg
        public static Regex PrivateSend { get { return new Regex("^[uU]/.*/.*"); } }

        //给所有用户发送消息 Send message to all  格式：p/msg
        public static Regex PublicSend { get { return new Regex("^[pP]/"); } }

        //显示在线用户 Show online users  格式：s 
        public static Regex ShowOnlineUser { get { return new Regex("^(s|S)$"); } }

        //退出登录 login out  格式：q
        public static Regex LoginOut { get { return new Regex("^(q|Q)$"); } }

        //查看帮助信息 See help info  格式：h 
        public static Regex Help { get { return new Regex("^(h|H)$"); } }

    
    }
}
