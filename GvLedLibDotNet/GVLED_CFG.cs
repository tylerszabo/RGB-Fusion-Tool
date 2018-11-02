using System.Runtime.InteropServices;

namespace GvLedLibDotNet
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GVLED_CFG
    {
        public GVLED_CFG(
            uint nType,
            uint nSpeed,
            uint dwTime1,
            uint dwTime2,
            uint dwTime3,
            uint nMinBrightness,
            uint nMaxBrightness,
            uint dwColor,
            uint nAngle,
            uint nOn,
            uint nSync)
        {
            this.nType = nType;
            this.nSpeed = nSpeed;
            this.dwTime1 = dwTime1;
            this.dwTime2 = dwTime2;
            this.dwTime3 = dwTime3;
            this.nMinBrightness = nMinBrightness;
            this.nMaxBrightness = nMaxBrightness;
            this.dwColor = dwColor;
            this.nAngle = nAngle;
            this.nOn = nOn;
            this.nSync = nSync;
        }

        public uint nType;
        public uint nSpeed;
        public uint dwTime1;
        public uint dwTime2;
        public uint dwTime3;
        public uint nMinBrightness;
        public uint nMaxBrightness;
        public uint dwColor;
        public uint nAngle;
        public uint nOn;
        public uint nSync;
    }
}
