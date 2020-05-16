using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using System.Net;
using System.Net.WebSockets;



namespace NesixChatServerV1
{
    //
    public class WSINOUTHandler : IDisposable
    {
        public string LastWords = "";
        public string SRoomNo = "255";
        public string sUserID = "unKnown";
        WSServerTaskAndOject wsso;
        CancellationTokenSource ctsServerThread = new CancellationTokenSource();
        public string UserAgent = "";
        public byte[] bInBuff = new byte[1024];
        public HttpListenerContext hlcIO;
        public ConnectionWaiter ParentCW;
        public bool IsNeeded = true;
        public List<byte[]> wsOutWriteList = new System.Collections.Generic.List<byte[]>();
        public string SSIDws = "unset";
        public delegate void wsOnIOFinished(WSINOUTHandler sender);
        public event wsOnIOFinished OnFinished;
        public ServerFrm Frm;
        public WSINOUTHandler(ConnectionWaiter parentRW, HttpListenerContext hlc, ServerFrm frm)
        {
            UserAgent = hlc.Request.UserAgent;
            Frm = frm;
            tIOWriteTimer = new System.Threading.Timer(wsWriteTimerTick, this, 1000, 1250);

            hlcIO = hlc;
            ParentCW = parentRW;
        }
        public System.Threading.Timer tIOWriteTimer;
        public bool isPostDataRead = false;
        public async void SendMessage(ArraySegment<byte> sendingBytes, bool isEnding)
        {
            try
            {
                await wsc.WebSocket.SendAsync(sendingBytes, WebSocketMessageType.Text, true, wsso.ct);
            }
            catch
            {
                //isNeeded = false;
                try
                {
                    wsOutWriteList.Clear();
                    IsNeeded = false;
                }
                catch (Exception e)
                { IsNeeded = false; IsNeeded = false;  }
            }
        }

        
        public async void DoWSServeClientPostDataIn()
        {
            
            //tfServerThread.StartNew(DoWSServeSocket, ctsServerThread.Token);
            /////
            wsso = new WSServerTaskAndOject()
            { ct = ctsServerThread.Token, CanBeDisposed = false, wsioh = this, wsCts = ctsServerThread};
            ////
            wsso.task = Task.Run(() => DoWSServeSocket(wsso));
            
            if (hlcIO.Request.HttpMethod == "post" || hlcIO.Request.HttpMethod == "put")
            {
                while (hlcIO.Request.InputStream.CanRead & !isPostDataRead)
                {
                    int n = await hlcIO.Request.InputStream.ReadAsync(bInBuff, 0, 1024);
                    if (n == 0)
                    {

                        isPostDataRead = true;

                    }
                    if (n != 0)
                    {
                        bInBuff = new byte[1024];
                        string sInString = Encoding.ASCII.GetString(bInBuff, 0, n);
                        sPostDataString += sInString;
                        ChatServerTypes.UpdateTextSafe(ChatServerTypes.UiForm.textBox1, "Post:\r\n" + sInString + "\r\n");
                    }
                }
            }
            
        }
        WebSocketContext wsc;
        public string sPostDataString = "";
        public bool isOKReading = true;
        public WebSessionClass SessionInfo = new WebSessionClass();
        public NSXUserInfoObject NSXUserObj = new NSXUserInfoObject();
        async public void DoWSServeSocket(WSServerTaskAndOject wssto)
        {
            CancellationToken ct = wssto.ct;
            CancellationTokenSource cts = wssto.wsCts;

            NSXUserObj.sIP = hlcIO.Request.RemoteEndPoint.Address.ToString();
            NSXUserObj.sUserAgent = hlcIO.Request.UserAgent;
            NSXUserObj.DateStarted = DateTime.Now;
            NSXUserObj.DateOfLastMsg= DateTime.Now;
            try
            {
                //hlcIO.Response.SendChunked = true;
                wsc = await hlcIO.AcceptWebSocketAsync("Neals");
            }
            catch (WebSocketException we)
            {
                isOKReading = false; IsNeeded = false;
                return;
            }
            WebSocketReceiveResult wr=null;
            while (isOKReading && !ct.IsCancellationRequested)
            {
                byte[] bWsIn = new byte[1024];
                
                try
                {
                    wr = await wsc.WebSocket.ReceiveAsync(new ArraySegment<byte>(bWsIn), wsso.ct);
                }
                catch
                {
                    isOKReading = false; IsNeeded = false;
                    cts.Cancel();
                    wssto.wsioh.Dispose();
                    IsNeeded = false;
                    ctsServerThread.Cancel();
                    wssto.wsioh.Dispose();
                    return;
                }
                if (wr.MessageType == WebSocketMessageType.Close)
                { isOKReading = false; IsNeeded = false;  }
                if (wr.MessageType == WebSocketMessageType.Text)
                {
////                    ChatServerTypes.UpdateTextSafe(ParentCW.Frm.textBox1, "Starting Server \r\n");

                    string sIN = Encoding.ASCII.GetString(bWsIn, 0, wr.Count);
                    //ChatServerTypes.UpdateTextSafe(ParentCW.Frm.textBox1, sIN + "\r\n");
                    string sHeads = "<textarea style=\"height:200px; width:200px;\">";

                    foreach (string hrh in hlcIO.Request.Headers)
                    { sHeads += hrh + "\\" + hlcIO.Request.Headers[hrh] + "\r\n"; }
                    sHeads += "</textarea>\r\n";
                    bInBuff = null;

                    byte[] b = Encoding.ASCII.GetBytes(sHeads);

                    //int iLen; int iExtras;
                    //Int32[] b32 = StringByteInt.BytesToInt32s(b, out iLen, out iExtras);
                    //StringByteInt.minusFromAll(ref b32, 127);

                    //StringByteInt.plusToAll(ref b32, 127);

                    //b = StringByteInt.Int32sToBytes(b32, iLen, iExtras);
                    string sss = Encoding.ASCII.GetString(b, 0, b.Count());
                    bool isBroadcast; bool isToSend;

                    string sData = sIN.Substring(0, sIN.IndexOf("</data>"));
                    sData = sData.Substring(sData.IndexOf("<data>")+6);

                    string sid = sIN.Substring(sIN.IndexOf("<sid>") + 5).ToLower();
                    sid = sid.Substring(0, sid.IndexOf("</sid>") ).ToLower();

                    sID = sid;
                    try
                    {
                        SessionInfo = SessionDataHandler.SessionDataList[sid];
                    }
                    catch { SessionInfo = new WebSessionClass(); }

                    //ChatServerTypes.UpdateTextSafe(ChatServerTypes.UiForm.textBox1, "Session not matching on WS request!");

                    InboundMessageParser.InterpreterResponse[] sRet = InboundMessageParser.InterpretCommands(sID, sData, ref SessionInfo);
                    foreach (InboundMessageParser.InterpreterResponse irMSG in sRet)
                    {
                        if (irMSG.IsToSEnd && irMSG.SResponse != "")
                        {
                            if (irMSG.IsRoomCast)
                            {
                                ParentCW.SendToRoom(irMSG.sRoomTo, irMSG.SResponse);
                            }
                            else if (irMSG.IsBroadcast)
                            { ParentCW.SendToAll(irMSG.SResponse); }
                            else
                            {
                                AddToWriteList(Encoding.ASCII.GetBytes(irMSG.SResponse));
                            }
                        }
                    }
                    //t = Task.Run(() => DoSomeWork(1, token), token);
                    
                }
            }
            ctsServerThread.Cancel();
            wssto.wsioh.Dispose();


            string s = ctsServerThread.Token.CanBeCanceled.ToString() ;
        }
        //////"<NESIXv1.00><SID>" + ThisStation + "</SID><data>" + data + "<data></NESIXv1.00>"
        /// 

        /// </summary>
        /// <param name="bAdd"></param>
        public void AddToWriteList(byte[] bAdd)
        {
            byte[] bPacketHead = Encoding.ASCII.GetBytes("<<nesixv1.00><sid>" + sID + "</sid>");
            byte[] bPacketFoot = Encoding.ASCII.GetBytes("</nesixv1.00>");
            string s = Encoding.ASCII.GetString((bPacketHead.Concat(bAdd).Concat(bPacketFoot).ToArray()));
            if(wsOutWriteList!=null) wsOutWriteList.Add(bPacketHead.Concat(bAdd).Concat(bPacketFoot).ToArray());

            return;
        }
            ///---
         //   foreach (byte b in bAdd) this.wsOutWriteList.Add(new byte[] { b });
         //   if (wsOutWriteList.Count() > 127)
          //      WriteTimerTick(this);
           // ---///
        
        long wscontentSentCount = 0;
        bool isTicking = false;
        string sID = "1";
        bool busyWriteQueue = false;
        public void AddToDiv(string sDiv, string sData)
        {
            string s = InboundMessageParser.GetAddToDivString(SSIDws, sDiv, sData);
            AddToWriteList(Encoding.ASCII.GetBytes(s));
        }
        private void wsWriteTimerTick(object state)
        {
            if (isTicking) return;
            try
            {
                isTicking = true;
                if (IsNeeded == false)
                {
                    hlcIO.Response.Close();
                }
                if(wsOutWriteList==null)
                {
                    tIOWriteTimer.Dispose();
                    this.Dispose();
                }
                    
                if (wsOutWriteList.Count() == 0)
                {                       
                    tIOWriteTimer.Change(200, 200);
                    isTicking = false;
                    return;
                }
                if (wsOutWriteList.Count() > 0 & wsOutWriteList.Count() < 10)
                {
                    tIOWriteTimer.Change(25, 25);
                    isTicking = false;
                }
                if (wsOutWriteList.Count() >= 10)
                {
                    tIOWriteTimer.Change(10, 10);
                }
                if (wsOutWriteList.Count > 25)
                {
                    wsOutWriteList.RemoveRange(0, 10);
                    ChatServerTypes.UpdateTextSafe(ChatServerTypes.UiForm.textBox1, "Dropped-packets-out session:" + SSIDws);
                }
                try
                {
                    
                    SendMessage(new ArraySegment<byte>(wsOutWriteList[0], 0, wsOutWriteList[0].Count()), wsOutWriteList.Count <= 1);
                    
                    wscontentSentCount += wsOutWriteList[0].Count();
                    wsOutWriteList.RemoveAt(0);
                    string sSend = "setdivhtm;";
                    sSend += "<target>*</target>";
                    
                    sSend += "<elementid>";
                    sSend += "DateTimeDiv";
                    sSend += "</elementid>";
                    string sShms = DateTime.Now.ToLongTimeString() + ":";
                    string sMs = "000" + DateTime.Now.Millisecond.ToString();
                    int nnn = sMs.Length;
                    sMs = sMs.Substring(nnn - 3);
                    string sd = sMs;
                    
                    //sd = sd.Substring(0, nnn - 2);
                    sShms += sd;
                    
                    sSend += sShms;
                    byte[] bSend = Encoding.ASCII.GetBytes(sSend);
                    isTicking = false;
                    //AddToWriteList(bSend);
                    
                    //sDiv = sS.substring(sS.toLowerCase().indexOf("<element>") + 9);
                    //sDiv = sDiv.substring(0, sDiv.toLowerCase().indexOf("</element>"));
                }
                catch (WebSocketException e)
                {
                    string s = e.InnerException.ToString();
                    s = s;
                    isTicking = false;
                }
                catch (Exception e)
                {
                    string s = e.InnerException.ToString();
                    s = s;
                    IsNeeded = false;
                    isTicking = false;
                }

            }
            catch
            {
                isTicking = false;
            }
        }

        public void Dispose()
        {
            wsso.wsioh.bInBuff = null;
            wsso.wsioh.wsOutWriteList = null;
            IsNeeded = false;
            tIOWriteTimer.Dispose();
            wsc.WebSocket.Dispose();
            isOKReading = false;
            wsso.task.Dispose();
            
        }
    }
}




