// --------------------------------------------------
// CM3D2.Toolkit - UInt64Ex.cs
// --------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace CM3D2.Toolkit
{
    /// <summary>
    ///     Packed UInt64, with Individually Accessible Words
    ///     <para />
    ///     _Byte =  8 Bits;
    ///     _Word = 16 Bits;
    ///     DWord = 32 Bits;
    ///     QWord = 64 Bits;
    /// </summary>
    /// 
    [StructLayout(LayoutKind.Explicit, Pack = 0)]
    internal struct UInt64Ex
    {
	    [FieldOffset(0)] public UInt64 QWORD;

        [FieldOffset(0)] public UInt32 DWORD_0;

        [FieldOffset(4)] public UInt32 DWORD_1;
    }
}
