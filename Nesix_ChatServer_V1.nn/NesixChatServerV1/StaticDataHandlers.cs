using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesixChatServerV1
{
    public static class StaticDataHandlers
    {
        public static Dictionary<string, string> UPlist = new Dictionary<string, string>();
        static List<NSXUserInfoObject> UserDataList = new List<NSXUserInfoObject>();
        public static bool CheckUserLogin(string sName, string sPass)
        {
            if(UPlist.Count()==0)
            {
                UPlist.Add("NEAL", "PASSWORD");
                UPlist.Add("NEAL2", "PASSWORD");
                UPlist.Add("NEAL3", "PASSWORD");
                UPlist.Add("NEAL4", "PASSWORD");
            }
            sName = sName.Substring(sName.IndexOf("=") + 1);
            sPass = sPass.Substring(sPass.IndexOf("=") + 1);
            try
            {
                if (sName.Length + sPass.Length < 10) return false;
                if (UPlist[sName.ToUpper()].ToLower() == sPass.ToLower()) return true;
                return false;
            }
            catch (Exception e)
            {
                string s = e.ToString();
                return false;
            }
        }
        
    }
}
