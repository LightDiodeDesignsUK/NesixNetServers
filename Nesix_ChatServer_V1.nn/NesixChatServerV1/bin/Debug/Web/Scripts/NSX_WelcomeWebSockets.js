// JUST FOR WELCOME.HTML

window.addEventListener("load", init);
var outp;
var pageDiv;
var websocket;
var ThisStation;

var sButtons = "";
//function SetStatusDiv(sMess)
//{
 //   document.getElementById("statusDiv").innerHTML = sMess;
  //  document.getElementById("statusDiv").style.backgroundColor = "green";
//}
// JUST FOR WELCOME.HTML
function doLogonCheck()
{
    alert("1");
    document.getElementById("UserDetailsForm").submit();
}

function init2(event) 
{    
    try {
        ThisStation = ""; //? 
        //outp = document.getElementById("outDIV");
        pageDiv = document.getElementById("statusDiv");
        var sWH = window.innerWidth + "," + window.innerHeight;
        
        websocket = new WebSocket(document.location.origin.replace("http:", "ws:") , ["Neals"]);
        websocket.onopen = function (evt) { onOpen(evt); };
        websocket.onclose = function (evt) { onClose(evt); };
        websocket.onmessage = function (evt) { onMessage(evt); };
        websocket.onerror = function (evt) { onError(evt); };
        
    }
    catch(e)
    {
        alert("!");
        alert( e.toString());
    }
}

function onOpen(evt) {
    //writeToScreen("CONNECTED");
    RaiseWSMessage("getDiv;<div>mainPageDiv</div>");
}

function onClose(evt) {
    try {
        document.getElementById("statusDiv").innerHTML = ("Ateempting re-connection");
        document.getElementById("statusDiv").fontSize = "36px";
        document.getElementById("statusDiv").color = "cyan";
        document.getElementById("statusDiv").borderColor = "magenta";
        document.getElementById("statusDiv").backgroundColor = "red";
        //setTimeout(init2, 2000);
    }
    catch (e) { var e2 = e; }

    try
    {
        document.getElementById("statusDiv").innerHTML = "Live-link lost..";
        document.getElementById("statusDiv").style.backgroundColor = "red";
        document.getElementById("statusDiv").style.color = "yellow";
        document.getElementById("statusDiv").style.fontSize = "40px";
    } catch (e) { var e22 = e; }
    try
    {
        document.getElementById("statusDiv").style.visibility = "hidden";
        document.getElementById("statusDiv").style.visibility = "hidden";
    } catch (e) { var e222 = e; }
    

}

function onMessage(evt)
{
 //   writeToScreen('<span style="color: blue;">RESPONSE: ' + evt.data + '</span>');
    document.getElementById("statusDiv").style.backgroundColor = "lightgray";
    document.getElementById("statusDiv").style.font.color = "black";
    ProcessWsBtnChangeReq(evt.data.toString());
}

function onError(evt)
{
    document.getElementById("mainPageDiv").innerHTML = "DISCONNECTED<hr \>" + evt.data;
}



function setNameProp()
{
    var d = document.getElementById("nameTextBox");
    var sYN = d.value;
    //SetStatusDiv(sYN);
    SetPropery("sNAME", sYN);
}

function SetPropery(sProp, sVal)
{    
    document.getElementById("StatusDiv").style.backgroundColor = "magenta";
    btnMsg = "SETPROP; <prop>sNAME</prop><value>" + sVal + "</value>";
    RaiseWSMessage(btnMsg);
}

function RaiseWSMessage(data)
{
    var sMess = "<nesixv1.00><sid>" + ThisStation + "</sid><data>" + data + "<data></nesixv1.00>";
    websocket.send(sMess);
}
function ProcessWsBtnChangeReq(sInStr) {
    try {
        if(sInStr.indexOf(";")===-1) return;
        var sss = sInStr;
        var sTarget = ""; var sStation = "";
        var sNum = ""; var sOP = "";
        var sDiv = "";
        var sS = "";
        var sSetDivString = "";
        var nPlayers = 0; var sPlayers = ""; var sPlaying = "";
        while (sss.toLowerCase().indexOf("<nesixv1.00>") > 0) {
            sS = sss.substring(sS.indexOf("</nesixv1.00>"));
            try { sss = sss.substring(sss.toLowerCase().indexOf("<nesixv1.00>") + 13); }
            catch (e) { return; }
            sStation = sS.substring(sS.toLowerCase().indexOf("<sid>") + 5).toLowerCase();
            sStation = sStation.substring(0, sStation.indexOf("</sid>"));
            sS = sS.substring(sS.toLowerCase().indexOf("</sid>") + 6);

            sOP = sS.substring(0, sS.indexOf(";")).toLowerCase();
            if (sOP === "ping") {
                sPlayers = sS.substring(sS.toLowerCase().indexOf("<numplayers>") + 12);
                sPlayers = sPlayers.substring(0, sPlayers.toLowerCase().indexOf("</numplayers>"));
                nPlayers = parseInt(sPlayers);

                sPlaying = sS.substring(sS.toLowerCase().indexOf("<playing>") + 9);
                sPlaying = sPlaying.substring(0, sPlaying.toLowerCase().indexOf("</playing>"));

                sCounts = sS.substring(sS.toLowerCase().indexOf("<counts>") + 8);
                sCounts = sCounts.substring(0, sCounts.toLowerCase().indexOf("</counts>"));


                //sPlaying = sPlaying.replace("p", "Player ");
                SetPlayersDiv("Player " + sStation + " of " + sPlayers + " PLAYERs ");
                interpretPingInfo(sPlaying, sCounts);
                return;
            }
            else if (sOP === "setdivhtm") {
                sTarget = sS.substring(sS.toLowerCase().indexOf("<target>") + 8);
                sTarget = sTarget.substring(0, sTarget.toLowerCase().indexOf("</target>"));
                sDiv = sS.substring(sS.toLowerCase().indexOf("<elementid>") + 11);
                sDiv = sDiv.substring(0, sDiv.toLowerCase().indexOf("</elementid>"));

                var i = sS.toLowerCase().indexOf("</elementid>") + 12;
                sSetDivString = sS.substring(i);
            }

        }
    }
    catch (e) {
        alert(e.toString());
    }

    switch (sOP.toLowerCase())
    {
        case "close":
        {
            websocket.abort();
            websocket.close();
            websocket = null;
            break;
        }
         case "reset":
        {
            break;
        }
        case "setdivhtm":
        {
            if (sTarget.trim() === ThisStation | sTarget === "*")
            {
                //document.getElementById(sDiv).innerHTML = sS;
                var d = document.getElementById(sDiv);

                d.innerHTML = sSetDivString;
                //if (sDiv === "StatusDiv") doResize();
                //d.style.width = screen.width * 0.75;
            }

            break;
        }
    }
}
    ////////////////
var sLastPingString = "";
var sLastCounts = "";
var bLastP1on = true;
function interpretPingInfo(s, sCounts)
{}

function doResize()
{
    var a = false;
    if (a!==a)
    {
        try
        {
            var nH = window.innerHeight;
            var nW = window.innerWidth;
            var nHF = (nH / 76) + 12;
            var nWF = (nW / 76) + 12;
            if (nHF < nWF) nHF = nHF;
            else nHF = nWF;

            winDiv = document.getElementById("winDIv");
            winDiv.style.fontSize = (nWF * 10).toString() + "px";

            document.getElementById('bingotable').style.height = (nH - 120).toString() + "px";
            document.getElementById('bingotable').style.width = (nW - 24).toString() + "px";
            document.getElementById('bingotable').style.fontSize = (nHF).toString() + "px";
            //RaiseWSMessage("resize;<width>" + window.innerWidth.toString() + "</width>" +
            //    "<height>" + window.innerHeight.toString() + "</height>");
            return;
        } catch (e) { var ss = e; };
    }
}
function ReloadWindow()
{
    websocket.close();
    window.location.assign(window.location.href);
}
function lockStation()
{
    ss = document.styleSheets[0].rules;
    var ssss = ss[0].selectorText;
    for (var i = 0; i < ss.length; i++)
    {
        ssss = ss[i].selectorText;
        if (ssss === ".inactiveTDInactiveNumDiv")
        {
            ss[i].style.backgroundImage = null;
            ss[i].style.backgroundColor = "gray";
            ss[i].style.color = "gray";
            //SetStatusDiv("Locked Ready To Play.")
        }
    }
    
    RaiseWSMessage("lockstation;");
}