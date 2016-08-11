//
// C# ArtNet DMX Implementation
//
// Author: Niklas Schulze <me@jns.io>

using System;
using System.Runtime.InteropServices;
using System.Diagnostics; // for Debug.Assert

namespace artnet {

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct ArtnetDmx {
        fixed byte ID[8];     //"Art-Net"
        ushort OpCode;        // See Doc. Table 1 - OpCodes eg. 0x5000 OpOutput / OpDmx
        ushort version;       // 0x0e00 (aka 14)
        byte seq;             // monotonic counter
        byte physical;        // 0x00
        byte subUni;          // low universe (0-255)
        byte net;             // high universe (not used)
        ushort length;        // data length (2 - 512)
        fixed byte data[512]; // universe data

        public ArtnetDmx(byte uni) {
            fixed (byte* p = this.ID)
            {
                *p = (byte)'A';
                *(p+1) = (byte)'r';
                *(p+2) = (byte)'t';
                *(p+3) = (byte)'-';
                *(p+4) = (byte)'N';
                *(p+5) = (byte)'e';
                *(p+6) = (byte)'t';
                *(p+7) = (byte)0;
            }

            OpCode = 0x5000;
            version = (ushort)_bswap(14);
            seq = 0;
            physical = 0;
            subUni = uni;
            net = 0;
            length = (ushort)_bswap(512);
      }

      private static uint _bswap(uint val) {
          return (val << 8) | (val >> 8);
      }

      public void setChannel(ushort channel, byte val) {
          Debug.Assert(channel < 512);
          fixed(byte* p = this.data) {
              p[channel] = val;
          }
      }

      public byte getChannel(ushort channel) {
          Debug.Assert(channel < 512);
          fixed(byte* p = this.data) {
              return p[channel];
          }
      }

      public void incrChannel(ushort channel) {
          Debug.Assert(channel < 512);
          fixed(byte* p = this.data) {
              if(p[channel] < 255)
                  p[channel]++;
          }
      }

      public void decrChannel(ushort channel) {
          Debug.Assert(channel < 512);
          fixed(byte* p = this.data) {
              if(p[channel] > 0)
                  p[channel]--;
          }
      }

      public void setSequence(byte s) {
          seq = s;
      }

      public byte getSequence() {
          return seq;
      }

      public byte[] toBytes() {
          Byte[] bytes = new Byte[Marshal.SizeOf(typeof(ArtnetDmx))];
          GCHandle s = GCHandle.Alloc(this, GCHandleType.Pinned);
          try {
              Marshal.Copy(s.AddrOfPinnedObject(), bytes, 0, bytes.Length);
              return bytes;
          }
          finally {
              s.Free();
          }
      }
  };
}
