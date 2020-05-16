using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
namespace NesixChatServerV1
{
    
    public delegate void SingleParamDelegate(object Param);
    public delegate void DoubleParaDelegate(object param1, object param2);
    public struct WSServerTaskAndOject { public Task task; public CancellationToken ct; public bool CanBeDisposed; public WSINOUTHandler wsioh; public CancellationTokenSource wsCts; };
    public static class ChatServerTypes
    {
        public static ConnectionWaiter hTTPCW;
        public static MonitorWindow MWuiForm;
        public static ServerFrm UiForm;
        public static void UpdateTextNotSafe(object control, object text)
        {
            if (!control.GetType().ToString().StartsWith("System.Windows.Forms.")) throw new ArrayTypeMismatchException("Not right type setTextNotSafe  Needs CONTROL. ");
            if (text.GetType() != typeof(String)) throw new ArrayTypeMismatchException("Not right type\r\nneeds STRING setTextNotSafe  ");
            ((Control)control).Text = (string) text;
        }
        public static void UpdateTextSafe(Control c, string txt)
        {
            DoubleParaDelegate dpd = new DoubleParaDelegate(UpdateTextNotSafe);
            c.Parent.Invoke(dpd, new object[] { c, txt});
        }

    }
}
