using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesixChatServerV1
{
    public enum MessageTypes { Text, Control}
    public class RoomDataItem
    {
        public Int64 MsgNo = -1;
        public string sMessage="";
        public string sUserFrom="";
        public string sUserTo="";
        public string SRoomNo = "-2";
        public MessageTypes MsgType;
        public string TextifyMessages()
        {
            string sreturn = "<table><tr><td>" + sMessage + "</td>";
            sreturn += "<table><tr><td>" + sUserFrom + "</td>";
            sreturn += "<table><tr><td>" + sUserTo + "</td>";

            return sreturn;
        }
    }
    public static class NSXRoomDataHandler
    {
        public static Dictionary<string, List<RoomDataItem>> RoomsList = new Dictionary<string, List<RoomDataItem>>();
        public static void setupInitialRooms()
        {
            if (RoomsList.Count != 0) return;
            for (int iRoom = 0; iRoom++ < 8;)
            {
                List<RoomDataItem> rdt = new List<RoomDataItem>();
                string sroom = "Room:" + iRoom.ToString();
                rdt.Add(new RoomDataItem() { SRoomNo = sroom, sMessage = "Welcome", MsgType = MessageTypes.Text, sUserFrom = "Board", sUserTo = "*" });
                RoomsList.Add(sroom, rdt);
                rdt = null;
            }
        }
    }
}
