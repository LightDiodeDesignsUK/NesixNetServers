using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesixChatServerV1
{
    class PageParser
    {
        INOUTHandler parent;
        public PageParser(INOUTHandler parentHandler)
        {
            parent = parentHandler;
            
        }
        
        public string interpretFile(string sFileIn, ref string sID)
        {
            int nMax = 100;
            while (nMax-- > 0)
            {
                if (!sFileIn.Contains("<nsx ") || !sFileIn.Contains("</nsx")) return sFileIn;
                string sRet = sFileIn.Substring(0, sFileIn.IndexOf("<nsx "));
                string sNSX = sFileIn.Substring(sFileIn.IndexOf("<nsx "));
                sNSX = sNSX.Substring(0, sNSX.IndexOf("</nsx>") + 6);
                
                string sCommand = sNSX.Substring(sNSX.IndexOf("command=") + 9);
                sCommand = sCommand.Substring(0, sCommand.IndexOf("("));
                string sParams = sNSX.Substring(sNSX.IndexOf("(") + 1);
                sParams = sParams.Substring(0,sParams.IndexOf(")"));
                sRet += DoCommand(sCommand,sParams, ref sID) + sFileIn.Substring(sFileIn.IndexOf("</nsx>")+6);
                sFileIn = sRet;
            }
            return sFileIn;
        }
        
        public string SUserName = "";
        public string SRoomNo = "255";
        public string SSID = "noSID";
        string DoCommand(string sCommand, string sParams, ref string sID)
        {

            switch(sCommand)
            {


                case ("getnewsession"):
                    {
                            return DateTime.Now.ToLongTimeString().Replace("-", "").Replace(":", "");
                        break;
                    }

                case ("checklogin"):
                    {
                        string sName = "", sPass = "";
                        try
                        {
                            sName = parent.PostData.Substring(parent.PostData.IndexOf("NameTXT"));
                            sName = sName.Substring(0, sName.IndexOf("&"));
                            SUserName = sName.Substring(sName.IndexOf("=") + 1);
                            sPass = parent.PostData.Substring(parent.PostData.IndexOf("PassTXT"));
                            sPass = sPass.Substring(0, sPass.IndexOf("&"));
                            sPass = sPass.Substring(sPass.IndexOf("=") + 1);

                            SSID = parent.PostData.Substring(parent.PostData.IndexOf("SID"));
                            SSID = SSID.Substring(0, SSID.IndexOf("&"));
                            SSID = SSID.Substring(SSID.IndexOf("=") + 1);

                            SRoomNo = parent.PostData.Substring(parent.PostData.IndexOf("Room"));
                            SRoomNo = Uri.UnescapeDataString(SRoomNo.Substring(SRoomNo.IndexOf("=")+1));
                            
                            //sPass = sPass.Substring(sPass.IndexOf("&"));
                        }
                        catch (Exception e) { return "alert('ERROR " + e.Message + "');"; }

                        if (StaticDataHandlers.CheckUserLogin(sName, sPass))
                        {
                            foreach(KeyValuePair<string, WebSessionClass> wsc in SessionDataHandler.SessionDataList)
                            {
                                
                                if (SUserName== wsc.Value.SUser || sID == wsc.Key)
                                {
                                    SSID = wsc.Value.sSession;
                                    SessionDataHandler.SessionDataList[SSID].Room = SRoomNo;
                                    sID = wsc.Key;
                                    return "alert('Login check OK;'); InitChat('" + SRoomNo + "','" + wsc.Key.ToString() + "'); ";
                                }
                                
                            }

                            SessionDataHandler.AddNewSession(SSID, SUserName, parent.hlcIO.Request.RemoteEndPoint.ToString(), parent.hlcIO.Request.UserAgent, SRoomNo);
                            SessionDataHandler.SessionDataList[SSID].SUser = SUserName;

                            SessionDataHandler.SessionDataList[SSID].nsxUserInfo = new NSXUserInfoObject()
                            { sUserName = SUserName, sUserAgent = parent.hlcIO.Request.UserAgent, DateOfLastMsg = DateTime.Now, DateStarted = DateTime.Now, sIP = parent.hlcIO.Request.RemoteEndPoint.Address.ToString(), sRoom = SRoomNo };

                            return "alert('Login check OK;'); InitChat('" + SRoomNo + "','" + SSID + "'); ";

                        }
                        else
                        {
                            return "gotoStart();";
                            return "gotoStart();";
                        }
                    }
            }
            return "";
        }
    }
}
