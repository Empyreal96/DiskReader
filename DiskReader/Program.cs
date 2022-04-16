using ReadFromDevice;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace DiskReader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("DiskReader by Empyreal96 (c) 2022");
            Console.WriteLine("");
            if (args.Length == 0)
            {
               // Console.WriteLine($"Parameters incorrect: {args[0]}, {args[1]}\n\n");
                Console.WriteLine("To read disk to file:");
                Console.WriteLine(@"DiskReader.exe  \\.\PhysicalDrive1 C:\Output\Path\image.img");
                Console.WriteLine("");
                Console.WriteLine("To view connected disks:");
                Console.WriteLine(@"DiskReader.exe  ListDisks "); 
                Console.ReadLine();
                Environment.Exit(1);
            }
                if (args[0] == "ListDisks")
            {
                var driveQuery = new ManagementObjectSearcher("select * from Win32_DiskDrive");
                foreach (ManagementObject d in driveQuery.Get())
                {
                    var deviceId = d.Properties["DeviceId"].Value;
                    var partitionQueryText = string.Format("associators of {{{0}}} where AssocClass = Win32_DiskDriveToDiskPartition", d.Path.RelativePath);
                    var partitionQuery = new ManagementObjectSearcher(partitionQueryText);
                    foreach (ManagementObject p in partitionQuery.Get())
                    {
                        var logicalDriveQueryText = string.Format("associators of {{{0}}} where AssocClass = Win32_LogicalDiskToPartition", p.Path.RelativePath);
                        var logicalDriveQuery = new ManagementObjectSearcher(logicalDriveQueryText);
                        foreach (ManagementObject ld in logicalDriveQuery.Get())
                        {
                            var physicalName = Convert.ToString(d.Properties["Name"].Value); // \\.\PHYSICALDRIVE2
                            var diskName = Convert.ToString(d.Properties["Caption"].Value); // WDC WD5001AALS-xxxxxx
                            var diskModel = Convert.ToString(d.Properties["Model"].Value); // WDC WD5001AALS-xxxxxx
                            var diskInterface = Convert.ToString(d.Properties["InterfaceType"].Value); // IDE
                            var capabilities = (UInt16[])d.Properties["Capabilities"].Value; // 3,4 - random access, supports writing
                            var mediaLoaded = Convert.ToBoolean(d.Properties["MediaLoaded"].Value); // bool
                            var mediaType = Convert.ToString(d.Properties["MediaType"].Value); // Fixed hard disk media
                            var mediaSignature = Convert.ToUInt32(d.Properties["Signature"].Value); // int32
                            var mediaStatus = Convert.ToString(d.Properties["Status"].Value); // OK

                            var driveName = Convert.ToString(ld.Properties["Name"].Value); // C:
                            var driveId = Convert.ToString(ld.Properties["DeviceId"].Value); // C:
                            var driveCompressed = Convert.ToBoolean(ld.Properties["Compressed"].Value);
                            var driveType = Convert.ToUInt32(ld.Properties["DriveType"].Value); // C: - 3
                            var fileSystem = Convert.ToString(ld.Properties["FileSystem"].Value); // NTFS
                            var freeSpace = Convert.ToUInt64(ld.Properties["FreeSpace"].Value); // in bytes
                            long freespace = (long)freeSpace;
                            var totalSpace = Convert.ToUInt64(ld.Properties["Size"].Value); // in bytes
                            long totalspace = (long)totalSpace;
                            var driveMediaType = Convert.ToUInt32(ld.Properties["MediaType"].Value); // c: 12
                            var volumeName = Convert.ToString(ld.Properties["VolumeName"].Value); // System
                            var volumeSerial = Convert.ToString(ld.Properties["VolumeSerialNumber"].Value); // 12345678

                            Console.WriteLine("PhysicalName: {0}", physicalName);
                            Console.WriteLine("Disk Name: {0}", diskName);
                            Console.WriteLine("Disk Model: {0}", diskModel);
                            Console.WriteLine("Interface: {0}", diskInterface);
                            // Console.WriteLine("Capabilities: {0}", capabilities);
                            Console.WriteLine("Media Loaded: {0}", mediaLoaded);
                            Console.WriteLine("Media Type: {0}", mediaType);
                            Console.WriteLine("Media Signature: {0}", mediaSignature);
                            Console.WriteLine("Media Status: {0}", mediaStatus);

                            Console.WriteLine("Drive Name: {0}", driveName);
                            Console.WriteLine("Drive ID: {0}", driveId);
                            Console.WriteLine("Compressed: {0}", driveCompressed);
                            Console.WriteLine("Drive Type: {0}", driveType);
                            Console.WriteLine("FileSystem: {0}", fileSystem);
                            Console.WriteLine("Free Space: {0}", freespace.ToFileSize());
                            Console.WriteLine("Total Space: {0}", totalspace.ToFileSize());
                            Console.WriteLine("Drive Media Type: {0}", driveMediaType);
                            Console.WriteLine("Volume Name: {0}", volumeName);
                            Console.WriteLine("Volume Serial: {0}", volumeSerial);

                            Console.WriteLine(new string('-', 79));

                        }
                    }
                }
                Console.WriteLine("Done..");
                Console.ReadLine();

            }
            else
            {
                try
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine($"Parameters incorrect: {args[0]}, {args[1]}\n\n");
                        Console.WriteLine("To read disk to file:");
                        Console.WriteLine(@"DiskReader.exe  \\.\PhysicalDrive1 C:\Output\Path\image.img \n");
                        Console.WriteLine("To view connected disks:");
                        Console.WriteLine(@"DiskReader.exe  ListDisks ");
                        Console.ReadLine();
                        Environment.Exit(1);
                    }
                    else
                    {
                        if (args[0].Contains(@"\\.\PhysicalDrive"))
                        {

                            string path;

                            if (string.IsNullOrEmpty(args[1]))
                            {
                                path = @".\image.img";

                            }
                            else
                            {
                                path = $"{args[1]}";

                            }
                            if (File.Exists(path))
                            {
                                File.Delete(path);
                            }

                            var diskid = Regex.Match(args[0], @"\d+").Value;
                            // string diskID = args[0].Replace(@"\\.\PhysicalDrive", "");
                            Debug.WriteLine($"DiskID: {Int32.Parse(diskid)}");
                            int partcount = 0;
                            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskPartition");
                            //Console.WriteLine("Scanning selected Disk");
                            Console.WriteLine();
                            foreach (ManagementObject queryObj in searcher.Get())
                            {

                                if ((uint)queryObj["DiskIndex"] == Int32.Parse(diskid))
                                {
                                    partcount++;
                                    Debug.WriteLine("-----------------------------------");

                                    Debug.WriteLine("Win32_DiskPartition instance");

                                    Debug.WriteLine("Name:{0}", (string)queryObj["Name"]);

                                    Debug.WriteLine("Index:{0}", (uint)queryObj["Index"]);

                                    Debug.WriteLine("DiskIndex:{0}", (uint)queryObj["DiskIndex"]);

                                    Debug.WriteLine("BootPartition:{0}", (bool)queryObj["BootPartition"]);
                                }




                            }





                            var diskSize = DiskInfo.GetPhysDiskSize(args[0]);
                            Console.WriteLine($"Selected Disk: {args[0]}");
                            Console.WriteLine($"Disk Size: {diskSize.ToFileSize().ToString()}");
                            Console.WriteLine($"No. of Partitions = {partcount}");
                            Console.WriteLine();
                            Console.WriteLine($"Selected Output: {path}");
                            Console.WriteLine("ENTER to continue, or Close this window.");
                            Console.ReadLine();
                            var reader = new BinaryReader(new DeviceStream(args[0]));

                            var writer = new BinaryWriter(new FileStream(path, FileMode.Create));

                            var buffer = new byte[4096];
                            int count;
                            int loopcount = 0;
                            //DeviceStream diskStream = new DeviceStream(args[0]);
                            //Stream fileStream = File.Create(path, 4096, FileOptions.RandomAccess);


                            //int size = 0;
                            try
                            {

                                //diskStream.CopyTo(fileStream);
                                System.Console.WriteLine($"Writing Data to file, this will take several minutes");
                                while ((count = reader.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    writer.Write(buffer, 0, buffer.Length);
                                    // System.Console.Write('.');
                                    if (loopcount % 100 == 0)
                                    {



                                        // writer.Flush();
                                    }
                                    loopcount++;

                                }
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.Message);
                                Debug.WriteLine("");
                                Debug.WriteLine(e.StackTrace);
                                Debug.WriteLine("");
                                Debug.WriteLine(e.Source);
                                //Console.ReadLine();
                            }
                            reader.Close();
                            writer.Flush();
                            writer.Close();
                            Console.WriteLine("Finished");
                            Console.ReadLine();
                        }
                        else
                        {
                            Console.WriteLine($"Parameters incorrect: {args[0]}, {args[1]}\n\n");
                            Console.WriteLine("To read disk to file:");
                            Console.WriteLine(@"DiskReader.exe  \\.\PhysicalDrive1 C:\Output\Path\image.img \n");
                            Console.WriteLine("To view connected disks:");
                            Console.WriteLine(@"DiskReader.exe  ListDisks ");
                            Console.ReadLine();
                            Environment.Exit(1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine();
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine();
                    Console.WriteLine(ex.Source);
                    Console.ReadLine();
                }
            }
        }
        public class DiskInfo
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr CreateFile(
    [MarshalAs(UnmanagedType.LPTStr)] string filename,
    [MarshalAs(UnmanagedType.U4)] FileAccess access,
    [MarshalAs(UnmanagedType.U4)] FileShare share,
    IntPtr securityAttributes, // optional SECURITY_ATTRIBUTES struct or IntPtr.Zero
    [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
    [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
    IntPtr templateFile);

            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            static extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode,
                IntPtr lpInBuffer, uint nInBufferSize,
                IntPtr lpOutBuffer, uint nOutBufferSize,
                out uint lpBytesReturned, IntPtr lpOverlapped);

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern bool CloseHandle(IntPtr hObject);

            struct GET_LENGTH_INFORMATION
            {
                public long Length;
            };

            public static long GetPhysDiskSize(string physDeviceID)
            {
                uint IOCTL_DISK_GET_LENGTH_INFO = 0x0007405C;
                uint dwBytesReturned;

                //Example, physDeviceID == @"\\.\PHYSICALDRIVE1"
                IntPtr hVolume = CreateFile(physDeviceID, FileAccess.ReadWrite,
                    FileShare.None, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);

                GET_LENGTH_INFORMATION outputInfo = new GET_LENGTH_INFORMATION();
                outputInfo.Length = 0;

                IntPtr outBuff = Marshal.AllocHGlobal(Marshal.SizeOf(outputInfo));

                bool devIOPass = DeviceIoControl(hVolume,
                                    IOCTL_DISK_GET_LENGTH_INFO,
                                    IntPtr.Zero, 0,
                                    outBuff, (uint)Marshal.SizeOf(outputInfo),
                                    out dwBytesReturned,
                                    IntPtr.Zero);

                CloseHandle(hVolume);

                outputInfo = (GET_LENGTH_INFORMATION)Marshal.PtrToStructure(outBuff, typeof(GET_LENGTH_INFORMATION));

                Marshal.FreeHGlobal(hVolume);
                Marshal.FreeHGlobal(outBuff);

                return outputInfo.Length;
            }
        }
    }
}
