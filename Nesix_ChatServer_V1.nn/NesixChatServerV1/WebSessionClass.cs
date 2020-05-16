using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesixChatServerV1
{
    public class WebSessionClass
    {
        public string sSession = "";
        public string IP = "";
        public string Agent = "";
        public string SUser = "";
        public string Room = "";
        public string sLastWords = "";
        public DateTime LastMSGDate = DateTime.Now;
        public WSINOUTHandler webSocketHandler;
        public NSXUserInfoObject nsxUserInfo;
        public string TypingNowString = "";
        
    }
    public static class SessionDataHandler
    {
        static int nLastID = 10;
        public static Dictionary<string, WebSessionClass> SessionDataList = new Dictionary<string, WebSessionClass>();
        public static int getNextSID()
        {
            return nLastID++;
        }
        public static void  AddNewSession(string SID, string suser, string ip, string agent, string sroom)
        {
            try
            {
                SessionDataList.Add(SID, new WebSessionClass() { Room = sroom,  sSession = SID,
                    Agent = agent, IP = ip, SUser = suser });
            }
            catch(Exception ee)
            {
                string s = ee.ToString();
            }
            return;// SID;
        }
            
    }
}
