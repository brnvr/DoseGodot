using System;
using System.Runtime.InteropServices;

namespace Dose.Core.Library.ESpeak
{
    internal static partial class Dll
    {
        public unsafe delegate int t_espeak_callback(short* wav, int numsamples, IntPtr events);

        [LibraryImport("libespeak-ng.dll", StringMarshalling = StringMarshalling.Utf8)]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        public static partial int espeak_Initialize(int output, int buflength, string path, int options);

        [LibraryImport("libespeak-ng.dll", StringMarshalling = StringMarshalling.Utf8)]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        public static partial int espeak_Synth(string text, UIntPtr size, uint position, int position_type, uint end_position,
                                                uint flags, IntPtr unique_identifier, IntPtr user_data);

        [LibraryImport("libespeak-ng.dll")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        public static partial void espeak_SetSynthCallback(t_espeak_callback callback);
    }
}