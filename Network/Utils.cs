using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Network
{
    public class Utils
    {
        public static IPAddress[] ResolveHost(string hostname, bool onlyIPv4Addresses)
        {
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(hostname);
                return hostEntry.AddressList.Where(a => a.AddressFamily == AddressFamily.InterNetwork || !onlyIPv4Addresses).ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static byte[] RawSerialize(Object srcObj)
        {
            int rawsize = Marshal.SizeOf(srcObj);
            byte[] rawdata = new byte[rawsize];

            GCHandle handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
            Marshal.StructureToPtr(srcObj, handle.AddrOfPinnedObject(), false);
            handle.Free();
            return rawdata;
        }

        // Method that will convert a byte array to a struct:
        // use typeof(struct) to get 'Type' param.
        static public Object GetStruct(byte[] buffer, Type structType)
        {
            var structSize = Marshal.SizeOf(structType);
            if (buffer.Length != structSize)
            {
                throw new ArgumentException("Buffer length must be of equal size as struct");
            }

            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Object obj = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), structType);
            handle.Free();
            return obj;
        }
    }
}
