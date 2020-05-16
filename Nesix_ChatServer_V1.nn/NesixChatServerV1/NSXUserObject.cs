using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesixChatServerV1
{
    public class NSXUserInfoObject
    {
        public string sUserName="";
        public string sUserAgent="";
        public WSINOUTHandler ParentWSIO;
        public string sIP = "0.0.0.0";
        public DateTime DateStarted, DateOfLastMsg;
        public string sRoom = "";
    }
}
