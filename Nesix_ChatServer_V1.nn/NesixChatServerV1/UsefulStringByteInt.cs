using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesixChatServerV1
{
    public static class UrlHelper
    {
        public static string getMimeType(string s2, out bool isBinary)
        {
            
            if (s2.Contains("."))
            {
                isBinary = false;
                string s = s2.Substring(s2.LastIndexOf(".")+1);
                switch (s.ToLower())
                {
                    case "html":    { return "text/html"; }
                    case "htm":     { return "text/html"; }
                    case "js":      { return "text/javascript"; }
                    case "css":     { return "text/css"; }
                    case "jpg":     { isBinary = true;  return "image/jpeg"; }
                    case "txt": { isBinary = false; return "text/plain"; }
                    case "text": { isBinary = false; return "text/plain"; }
                    case "zip": { isBinary = false; return "application/zip"; }

                }
            }
            isBinary = true;
            return "binary/unknown";
        }
    }
    public static class StringByteInt
    {
        public static void minusFromAll(ref Int32[] ints, int nMinus)
        {
            for(int n=0; n< ints.Length; n++)
            {
                ints[n] -= (nMinus += n);
            }
        }
        public static void plusToAll(ref Int32[] ints, int nPlus)
        {
            for (int n = 0; n < ints.Length; n++)
            {
                ints[n] += nPlus += n;
            }
        }


        public static Int32[] BytesToInt32s(byte[] bIn, out int count32, out int extraDigits)
        {
            extraDigits = 0;
            count32 = (bIn.Length / 4) + 1;
            extraDigits = (count32 * 4) - bIn.Length;
            Int32[] iRet = new Int32[count32];
            int i = 0;
            for( int iB = 0; iB < bIn.Length; iB+=4)
            {
                byte[] bLong = new byte[4];
                if (iB <= bIn.Length - 4)
                { bLong = new byte[] { bIn[iB + 0], bIn[iB + 1], bIn[iB + 2], bIn[iB + 3] }; }

                if (iB == bIn.Length - 3)
                { bLong = new byte[] {  bIn[iB + 0], bIn[iB + 1], bIn[iB + 2] ,0}; }

                if (iB == bIn.Length - 2)
                { bLong = new byte[] { bIn[iB + 0], bIn[iB + 1],0 ,0 }; }

                if (iB == bIn.Length - 1)
                { bLong = new byte[] { bIn[iB + 0], 0, 0, 0 }; }

                Int32 iResult = BitConverter.ToInt32(bLong, 0);

                iRet[i] = iResult;
                i++;
            }

            return iRet;
        }
        public static byte[] Int32sToBytes(Int32[] inInts, int length, int extras)
        {
            byte[] bRet = new byte[(length * 4) - extras];
            int iCount = 0;
            foreach(Int32 i32 in inInts)
            {
                Int32 iTemp = i32;
                try
                {
                    if (iCount < bRet.Length) bRet[iCount++] = (byte)((byte)(iTemp) & 255);
                    if (iCount < bRet.Length) bRet[iCount++] = (byte)((byte)(iTemp >>= 8) & 255);
                    if (iCount < bRet.Length) bRet[iCount++] = (byte)((byte)(iTemp >>= 8) & 255);
                    if (iCount < bRet.Length) bRet[iCount++] = (byte)((byte)(iTemp >>= 8) & 255);
                }
                catch { }
            }
            return bRet;
        }


    }
}
