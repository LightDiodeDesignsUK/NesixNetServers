using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesixChatServerV1
{
    static class InboundMessageParser
    {   public struct InterpreterResponse
        {   public string SResponse;    public bool IsToSEnd;   public bool IsBroadcast; public bool IsRoomCast;    public string sRoomTo;   }

        public static InterpreterResponse[] InterpretCommands(string sID, string sIn, ref WebSessionClass SessInf)
        {
            string sin = sIn.ToLower();
            InterpreterResponse irRet = new InterpreterResponse() { SResponse = "", IsBroadcast = false, IsToSEnd = false };
            irRet.IsToSEnd = true; irRet.IsBroadcast = false;
            string sOP = "getdiv";
            sOP = sIn.Substring(0, sIn.IndexOf(";") ).ToLower();
            switch (sOP)
            {
                case "regsession":
                    {
                        foreach (KeyValuePair<string, WebSessionClass> wsc in SessionDataHandler.SessionDataList)
                        {
                            if (wsc.Key == sID)
                            {
                                SessInf = wsc.Value;

                                irRet.SResponse = GetSetDivString(sID, "statusDiv", "Welcome to " + SessInf.Room + " " + SessInf.SUser);
                                irRet.IsToSEnd = true; irRet.IsBroadcast = false;
                                return new InterpreterResponse[] { irRet };
                            }
                        }
                        break;
                    }
                case "msgpost":
                    {
                        string sKey = sIn.Substring(sIn.IndexOf("<key>") + 5);
                        sKey = sKey.Substring(0, sKey.IndexOf("</key>"));
                        string sDiv = sIn.Substring(sIn.IndexOf("<div>") + 5);
                        sDiv = sDiv.Substring(0, sDiv.IndexOf("</div>"));
                        SessInf.TypingNowString += (sKey.Contains("Enter")) ? "\r\n" : sKey;
                        if (sKey == "Enter")
                        {
                            SessionDataHandler.SessionDataList[SessInf.sSession].sLastWords = SessInf.TypingNowString;
                            SessionDataHandler.SessionDataList[SessInf.sSession].LastMSGDate = DateTime.Now;
                            SessInf.sLastWords = SessInf.TypingNowString;
                            InterpreterResponse ir1 = new InterpreterResponse();
                            InterpreterResponse ir2 = new InterpreterResponse();
                            ir1.SResponse = GetSetDivString("*", sDiv, "");// SessInf.TypingNowString);
                            ir1.IsBroadcast = false; ir1.IsToSEnd = true;

                            ir2.SResponse = GetAddToDivString("*", "mainPageDiv", "<span style=\"background-color:green\">" + SessInf.SUser + "</span>:<div style=\"background-color:blue;\">" + SessInf.TypingNowString.Replace("\r\n", "<br />") + "</div>");
                            ir2.IsRoomCast = true;
                            ir2.sRoomTo = SessInf.Room;
                            ir2.IsBroadcast = true; ir2.IsToSEnd = true;
                            if (sKey == "Enter")

                                SessInf.TypingNowString = "";
                            return new InterpreterResponse[] { ir1, ir2 };
                        }
                        if (sKey == "ShiftEnter")
                        {
                            SessInf.TypingNowString += "\r\n";
                            InterpreterResponse ir = new InterpreterResponse()
                            {
                                IsBroadcast = false,
                                IsToSEnd = true,
                                IsRoomCast = false
                            };
                            string sKeys = GetSetDivString("*", sDiv, SessInf.TypingNowString.Replace("\r\n","<br />"));
                            return new InterpreterResponse[] { irRet };
                        }


                        if (sKey == "Enter")
                        {
                            SessInf.TypingNowString = "";
                            return new InterpreterResponse[] { irRet };
                        }

                        if (!sKey.Contains("Enter"))
                        {
                            irRet.IsBroadcast = false;
                            irRet.IsToSEnd = true;
                            string sKeys = GetSetDivString("*", sDiv, SessInf.TypingNowString);
                            return new InterpreterResponse[] { irRet };
                        }
                        break;
                    }
                case "getdiv":
                    {
                        try
                        {
                            string sDiv = sIn.Substring(0, sIn.IndexOf("</div>"));
                            sDiv = sDiv.Substring(sDiv.ToLower().IndexOf("<div>") + 5);
                            irRet.SResponse = GetSetDivString(sID, sDiv, "User: " + SessInf.SUser + ".");
                            irRet.IsToSEnd = true; irRet.IsBroadcast = false;
                            return new InterpreterResponse[] { irRet };
                        }
                        catch (Exception e)
                        {
                            ChatServerTypes.UpdateTextSafe(ChatServerTypes.UiForm.textBox1, "GetDiv Error;\r\n");
                            return new InterpreterResponse[] { };
                        }
                    }
            }
            return new InterpreterResponse[] { irRet };
        }
        public static string GetSetDivString(string sTargetSID, string sDiv, string sTXT)
        {
            string sSend2 = "setdivhtm;";
            sSend2 += "<target>" + sTargetSID + "</target>";
            sSend2 += "<elementid>";
            sSend2 += sDiv;
            sSend2 += "</elementid>";
            sSend2 += sTXT;
            return sSend2;
        }
        public static string GetAddToDivString(string sTargetSID, string sDiv, string sTXT)
        {
            string sSend2 = "addtodivhtm;";
            sSend2 += "<target>" + sTargetSID + "</target>";
            sSend2 += "<elementid>";
            sSend2 += sDiv;
            sSend2 += "</elementid>";
            sSend2 += sTXT;
            return sSend2;
        }
        public static string InterpretASPX(string sASP)
        {
            return sASP; 
        }
    }
}
