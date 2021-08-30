using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace System
{
    internal static partial class Libraries
    {
        internal const string Advapi32 = "advapi32.dll";
        internal const string Kernel32 = "kernel32.dll";
        internal const string NtDll = "ntdll.dll";

        internal const string CryptoNative = "libSystem.Security.Cryptography.Native.OpenSsl";
        internal const string SystemNative = "libSystem.Native";
    }

    internal static class Interop
    {        internal enum BOOL : int
        {
            FALSE = 0,
            TRUE = 1,
        }

        internal static class Advapi32
        {
            [DllImport(Libraries.Advapi32, CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern bool OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, out SafeAccessTokenHandle TokenHandle);
            
            internal struct TOKEN_ELEVATION
            {
                public BOOL TokenIsElevated;
            }

            [DllImport(Libraries.Advapi32, SetLastError = true)]
            internal static extern bool GetTokenInformation(
               SafeAccessTokenHandle TokenHandle,
               uint TokenInformationClass,
               SafeLocalAllocHandle TokenInformation,
               uint TokenInformationLength,
               out uint ReturnLength);

            internal enum TOKEN_INFORMATION_CLASS : uint
            {
                TokenUser = 1,
                TokenGroups,
                TokenPrivileges,
                TokenOwner,
                TokenPrimaryGroup,
                TokenDefaultDacl,
                TokenSource,
                TokenType,
                TokenImpersonationLevel,
                TokenStatistics,
                TokenRestrictedSids,
                TokenSessionId,
                TokenGroupsAndPrivileges,
                TokenSessionReference,
                TokenSandBoxInert,
                TokenAuditPolicy,
                TokenOrigin,
                TokenElevationType,
                TokenLinkedToken,
                TokenElevation,
                TokenHasRestrictions,
                TokenAccessInformation,
                TokenVirtualizationAllowed,
                TokenVirtualizationEnabled,
                TokenIntegrityLevel,
                TokenUIAccess,
                TokenMandatoryPolicy,
                TokenLogonSid,
                TokenIsAppContainer,
                TokenCapabilities,
                TokenAppContainerSid,
                TokenAppContainerNumber,
                TokenUserClaimAttributes,
                TokenDeviceClaimAttributes,
                TokenRestrictedUserClaimAttributes,
                TokenRestrictedDeviceClaimAttributes,
                TokenDeviceGroups,
                TokenRestrictedDeviceGroups,
                TokenSecurityAttributes,
                TokenIsRestricted,
                MaxTokenInfoClass
            }
        }

        internal static class Kernel32
        {
            [DllImport(Libraries.Kernel32)]
            internal static extern IntPtr GetCurrentProcess();
        }

        internal static class NtDll
        {
            [DllImport(Libraries.NtDll, ExactSpelling = true)]
            private static extern int RtlGetVersion(ref RTL_OSVERSIONINFOEX lpVersionInformation);

            internal static unsafe int RtlGetVersionEx(out RTL_OSVERSIONINFOEX osvi)
            {
                osvi = default;
                osvi.dwOSVersionInfoSize = (uint)sizeof(RTL_OSVERSIONINFOEX);
                return RtlGetVersion(ref osvi);
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            internal unsafe struct RTL_OSVERSIONINFOEX
            {
                internal uint dwOSVersionInfoSize;
                internal uint dwMajorVersion;
                internal uint dwMinorVersion;
                internal uint dwBuildNumber;
                internal uint dwPlatformId;
                internal fixed char szCSDVersion[128];
            }
        }

        internal static class OpenSsl
        {
            [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_OpenSslGetProtocolSupport")]
            internal static extern int OpenSslGetProtocolSupport(int protocol);

            [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_OpenSslVersionNumber")]
            internal static extern long OpenSslVersionNumber();
        }

        internal static class OpenSslNoInit
        {
            [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_OpenSslAvailable")]
            private static extern int OpenSslAvailable();

            private static readonly Lazy<bool> s_openSslAvailable =
                new Lazy<bool>(() => OpenSslAvailable() != 0);

            internal static bool OpenSslIsAvailable => s_openSslAvailable.Value;
        }

        internal static class Sys
        {
            [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetEUid")]
            internal static extern uint GetEUid();
        }
    }
}

namespace Microsoft.Win32.SafeHandles
{
    internal sealed class SafeLocalAllocHandle : SafeBuffer
    {
        public SafeLocalAllocHandle() : base(true) { }

        internal static readonly SafeLocalAllocHandle Zero = new SafeLocalAllocHandle();

        internal static SafeLocalAllocHandle LocalAlloc(int cb)
        {
            var h = new SafeLocalAllocHandle();
            h.SetHandle(Marshal.AllocHGlobal(cb));
            h.Initialize((ulong)cb);
            return h;
        }

        // 0 is an Invalid Handle
        internal SafeLocalAllocHandle(IntPtr handle) : base(true)
        {
            SetHandle(handle);
        }

        internal static SafeLocalAllocHandle InvalidHandle
        {
            get
            {
                return new SafeLocalAllocHandle(IntPtr.Zero);
            }
        }

        protected override bool ReleaseHandle()
        {
            Marshal.FreeHGlobal(handle);
            return true;
        }
    }
}
