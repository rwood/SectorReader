using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.IO;
using System.Xml.Serialization;

namespace SectorReader
{
	/// <summary>
	/// win32 api wrapper class to assist in reading raw sectors and getting drive geometry.
	/// Written by Roger Wood.
	/// </summary>
	/// 

	#region Win23 Class
	public class Win32
	{
		/// <summary>
		/// a simple class to make reading raw sectors and finding drive geometry a little nicer in a .net enviroment.
		/// </summary>
		/// 
		#region Public Methods
		public static IntPtr GetHandle(string drive) //drive in the form "C:"
		{
			drive = @"\\.\" + drive;			
			IntPtr h;
			h = CreateFile(drive, Win32.GENERIC_READ, Win32.FILE_SHARE_WRITE, IntPtr.Zero, Win32.OPEN_EXISTING, Win32.FILE_FLAG_NO_BUFFERING, IntPtr.Zero);
			return h;
		}

		public static int ReadFile(IntPtr Handle, uint StartByte, uint BufferSize, byte[] Buffer)  
		{
			uint n = 0;
			SetFilePointer(Handle, StartByte, IntPtr.Zero, (uint)0);
			ReadFile(Handle,Buffer,BufferSize,out n,IntPtr.Zero);
			CloseHandle(Handle);
			return (int)n; //returns the amount of bytes read into buffer.
		}

		public static DISK_GEOMETRY GetDiskGeometry(IntPtr handle)
		{
			uint bytesReturned = 0;
			DISK_GEOMETRY diskParams;
			diskParams.BytesPerSector = (uint)0;
			diskParams.Cylinders = (ulong)0;
			diskParams.MediaType = (byte)0;
			diskParams.SectorsPerTrack = (uint)0;
			diskParams.TracksPerCylinder = (uint)0;
			IntPtr ParamsPointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DISK_GEOMETRY)));
			if(DeviceIoControl(handle, IOCTL_DISK_GET_DRIVE_GEOMETRY, (IntPtr)null, 0, ParamsPointer, (uint)Marshal.SizeOf(typeof(DISK_GEOMETRY)), ref bytesReturned, (IntPtr)null))
				diskParams = (DISK_GEOMETRY)Marshal.PtrToStructure(ParamsPointer, typeof(DISK_GEOMETRY));
			CloseHandle(handle);
			return diskParams;
		}
		#endregion

		#region Win32 API Constants
		//Constants for dwDesiredAccess:
				private const UInt32 GENERIC_READ = 0x80000000;
				private const UInt32 GENERIC_WRITE = 0x40000000;
		//Constants for errors:
				private const UInt32 ERROR_FILE_NOT_FOUND = 2;
				private const UInt32 ERROR_INVALID_NAME = 123;
				private const UInt32 ERROR_ACCESS_DENIED = 5;
				private const UInt32 ERROR_IO_PENDING = 997;
				private const UInt32 ERROR_IO_INCOMPLETE = 996;
		
		//Constants for return value:
				private const Int32 INVALID_HANDLE_VALUE = -1;
		
		//Constants for dwFlagsAndAttributes:
				private const UInt32 FILE_FLAG_OVERLAPPED = 0x40000000;
				private const UInt32 FILE_SHARE_WRITE = 0x00000002;
				private const UInt32 FILE_FLAG_NO_BUFFERING = 0x20000000;
		
		//Constants for dwCreationDisposition:
				private const UInt32 OPEN_EXISTING = 3;

				private const uint IOCTL_DISK_GET_DRIVE_GEOMETRY = 0x70000;
		#endregion

		#region Kernel32.dll Extern Definitions
		[DllImport("kernel32.dll", SetLastError=true)]
		private static extern IntPtr CreateFile(String lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode,
			IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition, UInt32 dwFlagsAndAttributes,
			IntPtr hTemplateFile);

		[DllImport("kernel32.dll", SetLastError=true)]
		private static extern Boolean ReadFile(IntPtr hFile, [Out] Byte[] lpBuffer, UInt32 nNumberOfBytesToRead,
			out UInt32 nNumberOfBytesRead, IntPtr lpOverlapped);

		[DllImport("kernel32.dll", SetLastError=true)]
		private static extern UInt32 SetFilePointer(IntPtr hFile, UInt32 lDistanceToMove, IntPtr lpDistanceToMoveHigh, UInt32 dwMoveMethod);
		
		[DllImport("kernel32.dll")]
		private static extern Boolean CloseHandle(IntPtr hObject);
		
		[DllImport("kernel32.dll")]
		private static extern bool DeviceIoControl(IntPtr hDevice, uint
			dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize, IntPtr lpOutBuffer,
			uint nOutBufferSize, ref uint lpBytesReturned, IntPtr lpOverlapped);
		#endregion

	}  
	#endregion

	#region Disk Geometry Struct
	public struct DISK_GEOMETRY 
	{
		public ulong Cylinders;
		public byte MediaType; //was a MEDIA_TYPE, but I don't know what that is.
		public uint TracksPerCylinder;
		public uint SectorsPerTrack;
		public uint BytesPerSector;
	};
	#endregion
}


