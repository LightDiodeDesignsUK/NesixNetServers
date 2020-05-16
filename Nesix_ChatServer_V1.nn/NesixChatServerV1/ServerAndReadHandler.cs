using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web;
using NesixChatServerV1;
namespace NesixChatServerV1
{
    public class ServerListenerHandler
    {
        HttpListener hl;
        public ServerFrm frmUIForm;
        public ServerListenerHandler( ServerFrm owner)
        {
            frmUIForm = owner;
        }
        public void Startup(int Port)
        { 
            hl = new HttpListener();
            hl.Prefixes.Add("http://*:6366/");
            hl.Start();
            frmUIForm.StartButton.Enabled = false;
            ChatServerTypes.hTTPCW = new ConnectionWaiter(hl, frmUIForm);
            new TaskFactory().StartNew(ChatServerTypes.hTTPCW.WaitForConnections);
        }
    }
    public class ConnectionWaiter
    {

        HttpListenerContext hlc;
        HttpListener hl;
        List<INOUTHandler> IOHandlerList = new List<INOUTHandler>();
        List<WSINOUTHandler> wsIOHandlerList = new List<WSINOUTHandler>();


        public void ReportClients()
        {
            if (ChatServerTypes.MWuiForm == null) return;

            ChatServerTypes.MWuiForm.AddToClientListSafe(new string[] { DateTime.Now.ToShortTimeString() }, true);
            string[] sItems;
            for (int i=0; i< wsIOHandlerList.Count; i++)
            {
                if (!wsIOHandlerList[i].IsNeeded)
                {
                    wsIOHandlerList[i].Dispose();
                    wsIOHandlerList.RemoveAt(i);
                }
                else
                {
                    try
                    {
                        sItems = new string[]
                        {
                        ((WSINOUTHandler)wsIOHandlerList[i]).hlcIO.Request.RemoteEndPoint.Address.ToString(),
                        wsIOHandlerList[i].SessionInfo.SUser,
                        wsIOHandlerList[i].SessionInfo.sLastWords,
                        wsIOHandlerList[i].SessionInfo.Room,
                        //wsIOHandlerList[i].wsOutWriteList.Count().ToString(),
                        wsIOHandlerList[i].SessionInfo.LastMSGDate.ToLongTimeString(),
                        wsIOHandlerList[i].SSIDws
                        };
                        ChatServerTypes.MWuiForm.AddToClientListSafe(sItems, false);
                    }
                    catch { }
                }
            }

            
        }

        public void SendToAll(string sSend)
        {
            for (int i = 0; i < wsIOHandlerList.Count; i++)
            {
                wsIOHandlerList[i].AddToWriteList(Encoding.ASCII.GetBytes(sSend));
            }
        }

        public void SendToRoom(string sroom, string sSend)
        {
            for (int i = 0; i < wsIOHandlerList.Count; i++)
            {
                if (wsIOHandlerList[i].SessionInfo.Room == sroom)
                {
                    wsIOHandlerList[i].AddToWriteList(Encoding.ASCII.GetBytes(sSend));
                }
            }
        }


        public ServerFrm Frm;
        public ConnectionWaiter(HttpListener Hl, ServerFrm frm)
        {
            hl = Hl; Frm = frm;
        }
        public bool isNeeded = true;
        public bool isPostDataRead = false;
        public void WaitForConnections()
        {
            while (isNeeded)
            {
                try
                {
                    hlc = hl.GetContext();
                }
                catch
                {
                    isNeeded = false;
                    return;
                }
                if (hlc.Request.IsWebSocketRequest)
                {
                    WSINOUTHandler wsioh = new WSINOUTHandler(this, hlc, Frm);
                    wsioh.OnFinished += ws_OnFinished;
                    wsIOHandlerList.Add(wsioh);
                    new TaskFactory().StartNew(wsioh.DoWSServeClientPostDataIn);
                }
                else
                {
                    INOUTHandler ioh = new INOUTHandler(this, hlc);
                    ioh.OnFinished += Ioh_OnFinished;
                    IOHandlerList.Add(ioh);
                    new TaskFactory().StartNew(ioh.DoServeClientDataIn);
                }
            }
        }

        private void ws_OnFinished(WSINOUTHandler sender)
        {
            sender.tIOWriteTimer.Change(-1, -1);
            sender.bInBuff = null;
            sender.hlcIO = null;
            sender.wsOutWriteList.Clear();
            sender.wsOutWriteList = null;
            wsIOHandlerList.Remove(sender);
        }


        private void Ioh_OnFinished(INOUTHandler sender)
        {
            sender.tIOWriteTimer.Change(-1, -1);
            sender.bInBuff = null;
            sender.hlcIO = null;
            sender.outWriteList.Clear();
            sender.outWriteList = null;
            IOHandlerList.Remove(sender);
        }
    }

    //WSINOUTHandler
    public class INOUTHandler
    {
        long nContentLength = -1;
        public byte[] bInBuff = new byte[1024];
        public HttpListenerContext hlcIO;
        ConnectionWaiter parent;
        public bool IsNeeded = true;
        public List<byte[]> outWriteList = new System.Collections.Generic.List<byte[]>();

        public delegate void OnIOFinished(INOUTHandler sender);
        public event OnIOFinished OnFinished;
        PageParser WebPageParser;
        public INOUTHandler(ConnectionWaiter parentRW, HttpListenerContext hlc)
        {
            tIOWriteTimer = new System.Threading.Timer(WriteTimerTick, this, 1250, 1250);
            WebPageParser = new PageParser(this);
            hlcIO = hlc;
            parent = parentRW;
        }
        public System.Threading.Timer tIOWriteTimer;
        public bool isPostDataRead = false;
        public string PostData = "";
        public void DoServeClientDataIn()
        {
            if (hlcIO.Request.HttpMethod == "POST")
                while (!isPostDataRead)
                {
                    int n = hlcIO.Request.InputStream.Read(bInBuff, 0, 1024);
                    if (n == 0)
                    {
                        isPostDataRead = true;
                    }
                    else
                    {
                        string sInString = Encoding.ASCII.GetString(bInBuff, 0, n);
                        //                (parent. OnMessage).
                        PostData = sInString;
                        ChatServerTypes.UpdateTextSafe(ChatServerTypes.UiForm.textBox1, "Logon: \r\n" + sInString);
                    }
                }
            //string sHeads = "<textarea style=\"height:600px; width:600px;\">";

            //foreach (string hrh in hlcIO.Request.Headers)
            //{ sHeads += hrh + "\\" + hlcIO.Request.Headers[hrh] + "\r\n"; }
            //sHeads += "</textarea>\r\n";
            bInBuff = null;
            hlcIO.Response.StatusCode = 200;
            hlcIO.Response.SendChunked = false;
            string sRet = "";
            string sFile = "Web\\";
            string sID = "";
            try
            {
                hlcIO.Response.StatusCode = 200;
                if (hlcIO.Request.Url.AbsolutePath == "/")
                {
                    sFile += "Welcome.html";
                }
                else
                {
                    string sF = hlcIO.Request.Url.AbsolutePath;
                    if (sF.Contains("css")) sF = sF;
                    sF = sF.StartsWith("/") ? sF.Substring(1) : sFile;
                    sF = sF.Replace("/", "\\");
                    sFile += sF;
                }


                sRet = System.IO.File.ReadAllText(sFile);
                sRet = WebPageParser.interpretFile(sRet, ref sID);
            }
            catch
            {
                hlcIO.Response.StatusCode = 404;
                sRet = "<html><body>404</body></html>";
            }
            bool isBinary = false;
            hlcIO.Response.ContentType = UrlHelper.getMimeType(sFile, out isBinary);
            hlcIO.Response.ContentEncoding = System.Text.Encoding.ASCII;
            hlcIO.Response.ContentLength64 = sRet.Length;
            nContentLength = sRet.Length;
            byte[] b = Encoding.ASCII.GetBytes(sRet);

            int iLen; int iExtras;
            Int32[] b32 = StringByteInt.BytesToInt32s(b, out iLen, out iExtras);
            StringByteInt.minusFromAll(ref b32, 127);

            StringByteInt.plusToAll(ref b32, 127);

            b = StringByteInt.Int32sToBytes(b32, iLen, iExtras);
            string sss = Encoding.ASCII.GetString(b, 0, b.Count());
            AddToWriteList(b);
        }
        //////////////////
        ///
        public void AddToWriteList(byte[] bAdd)
        {
            this.outWriteList.Add(bAdd);
            if (outWriteList.Count() > 127)
                WriteTimerTick(this);
        }
        long contentSentCount = 0;
        bool isTicking = false;
        private void WriteTimerTick(object state)
        {
            if (isTicking) return;
            try
            {
                if (IsNeeded == false)
                {
                    hlcIO.Response.Close();
                }
                if (outWriteList.Count() > 0) tIOWriteTimer.Change(50, 500);
                if (outWriteList.Count() <= 0) tIOWriteTimer.Change(250, 500);
                if (outWriteList.Count() <= 0)
                {

                    isTicking = false;
                    return;
                }
                try
                {
                    hlcIO.Response.OutputStream.Write(outWriteList[0], 0, outWriteList[0].Count());
                    contentSentCount += outWriteList[0].Count();
                    outWriteList.RemoveAt(0);
                    if (nContentLength == -1) return;
                    if (nContentLength == contentSentCount)
                        IsNeeded = false;
                    isTicking = false;
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                    s = s;
                    isTicking = false;
                }

            }
            catch
            {
                isTicking = false;
            }
        }


    }   
}
