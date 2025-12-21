using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AVMatrixController
{
    public static class MatrixProtocol
    {
        private const byte PACKET_HEADER_1 = 0xa5;
        private const byte PACKET_HEADER_2 = 0x6c;
        private const byte PACKET_END = 0xae;
        private const byte DEVICE_TYPE_MATRIX = 0x82;
        private const byte DEVICE_TYPE_LCD = 0x03;
        private const byte DEVICE_TYPE_SEARCH = 0x81;
        private const byte DEVICE_ID_BROADCAST = 0xff;
        private const byte INTERFACE_LAN = 0x01;

        public static class Commands
        {
            public const byte READ_STATUS = 0x53;
            public const byte READ_LCD_STATUS = 0x50;
            public const byte SET_DEVICE_NAME = 0x0f;
            public const byte SET_LCD_BACKLIGHT_TIME = 0x51;
            public const byte SET_LCD_BRIGHTNESS = 0x52;
            public const byte SET_IP = 0x05;
            public const byte SEARCH_DEVICE = 0xff;
        }

        public static byte[] CreateSearchPacket()
        {
            List<byte> packet = new List<byte>();
            packet.Add(PACKET_HEADER_1);
            packet.Add(PACKET_HEADER_2);
            
            byte[] dataLength = BitConverter.GetBytes((ushort)0x14);
            packet.Add(dataLength[0]);
            packet.Add(dataLength[1]);
            
            packet.Add(DEVICE_TYPE_SEARCH);
            packet.Add(DEVICE_ID_BROADCAST);
            packet.Add(INTERFACE_LAN);
            
            for (int i = 0; i < 9; i++) packet.Add(0x00);
            
            packet.Add(Commands.SEARCH_DEVICE);
            
            ushort checksum = CalculateChecksum(packet.ToArray());
            byte[] checksumBytes = BitConverter.GetBytes(checksum);
            packet.Add(checksumBytes[0]);
            packet.Add(checksumBytes[1]);
            
            packet.Add(PACKET_END);
            
            return packet.ToArray();
        }

        public static byte[] CreateReadStatusPacket()
        {
            List<byte> packet = new List<byte>();
            packet.Add(PACKET_HEADER_1);
            packet.Add(PACKET_HEADER_2);
            
            byte[] dataLength = BitConverter.GetBytes((ushort)0x14);
            packet.Add(dataLength[0]);
            packet.Add(dataLength[1]);
            
            packet.Add(DEVICE_TYPE_MATRIX);
            packet.Add(0x01);
            packet.Add(INTERFACE_LAN);
            
            for (int i = 0; i < 9; i++) packet.Add(0x00);
            
            packet.Add(Commands.READ_STATUS);
            
            ushort checksum = CalculateChecksum(packet.ToArray());
            byte[] checksumBytes = BitConverter.GetBytes(checksum);
            packet.Add(checksumBytes[0]);
            packet.Add(checksumBytes[1]);
            
            packet.Add(PACKET_END);
            
            return packet.ToArray();
        }

        public static byte[] CreateReadLcdStatusPacket()
        {
            List<byte> packet = new List<byte>();
            packet.Add(PACKET_HEADER_1);
            packet.Add(PACKET_HEADER_2);
            
            byte[] dataLength = BitConverter.GetBytes((ushort)0x14);
            packet.Add(dataLength[0]);
            packet.Add(dataLength[1]);
            
            packet.Add(DEVICE_TYPE_LCD);
            packet.Add(0x01);
            packet.Add(INTERFACE_LAN);
            
            for (int i = 0; i < 9; i++) packet.Add(0x00);
            
            packet.Add(Commands.READ_LCD_STATUS);
            
            ushort checksum = CalculateChecksum(packet.ToArray());
            byte[] checksumBytes = BitConverter.GetBytes(checksum);
            packet.Add(checksumBytes[0]);
            packet.Add(checksumBytes[1]);
            
            packet.Add(PACKET_END);
            
            return packet.ToArray();
        }

        public static string CreateRoutingCommand(int input, List<int> outputs)
        {
            if (input < 1 || input > 8)
                throw new ArgumentException("Input must be between 1 and 8", nameof(input));
            
            if (outputs.Any(o => o < 1 || o > 8))
                throw new ArgumentException("All outputs must be between 1 and 8", nameof(outputs));

            if (outputs.Count == 8 && outputs.OrderBy(x => x).SequenceEqual(Enumerable.Range(1, 8)))
            {
                return $"{input}All.";
            }
            else if (outputs.Count == 1)
            {
                return $"{input}v{outputs[0]}.";
            }
            else
            {
                string outputList = string.Join(",", outputs.OrderBy(x => x));
                return $"{input}v{outputList}.";
            }
        }

        public static string CreateAllToAllCommand()
        {
            return "All#.";
        }

        public static byte[] CreateAsciiCommandBytes(string command)
        {
            return Encoding.ASCII.GetBytes(command);
        }

        private static ushort CalculateChecksum(byte[] data)
        {
            int sum = 0;
            foreach (byte b in data)
            {
                sum += b;
            }
            return (ushort)(sum & 0xFFFF);
        }

        public static bool ValidateResponsePacket(byte[] response)
        {
            if (response == null || response.Length < 6)
                return false;

            if (response[0] != PACKET_HEADER_1 || response[1] != PACKET_HEADER_2)
                return false;

            if (response[response.Length - 1] != PACKET_END)
                return false;

            return true;
        }

        public static DeviceStatusResponse ParseStatusResponse(byte[] response)
        {
            if (!ValidateResponsePacket(response) || response.Length < 30)
                throw new ArgumentException("Invalid response packet");

            var result = new DeviceStatusResponse();
            
            result.Command = response[16];
            result.Status = response[17];
            
            if (result.Status != 0x00)
            {
                result.Success = false;
                return result;
            }

            result.Success = true;
            
            result.OutputStatus = new int[8];
            for (int i = 0; i < 8; i++)
            {
                if (response.Length > 18 + i)
                    result.OutputStatus[i] = response[18 + i];
            }

            if (response.Length > 27)
            {
                result.IpMode = response[27] == 0x00 ? "Static" : "Dynamic";
            }

            if (response.Length > 29)
            {
                int nameLength = response[29];
                if (response.Length >= 30 + nameLength * 2)
                {
                    byte[] nameBytes = new byte[nameLength * 2];
                    Array.Copy(response, 30, nameBytes, 0, nameLength * 2);
                    result.DeviceName = Encoding.Unicode.GetString(nameBytes);
                }
            }

            return result;
        }

        public static LcdStatusResponse ParseLcdStatusResponse(byte[] response)
        {
            if (!ValidateResponsePacket(response) || response.Length < 20)
                throw new ArgumentException("Invalid response packet");

            var result = new LcdStatusResponse();
            
            result.Command = response[16];
            result.Status = response[17];
            
            if (result.Status != 0x00)
            {
                result.Success = false;
                return result;
            }

            result.Success = true;
            
            if (response.Length > 18)
                result.BacklightTime = response[18];
            
            if (response.Length > 19)
                result.Brightness = response[19];

            return result;
        }

        public static SearchDeviceResponse ParseSearchResponse(byte[] response)
        {
            if (!ValidateResponsePacket(response) || response.Length < 20)
                throw new ArgumentException("Invalid response packet");

            var result = new SearchDeviceResponse();
            
            result.DeviceType = response[4];
            result.Status = response[17];
            result.Success = result.Status == 0x00;

            if (response.Length > 18)
            {
                int modelNameLength = response.Length - 20;
                byte[] modelBytes = new byte[modelNameLength];
                Array.Copy(response, 18, modelBytes, 0, modelNameLength);
                result.ModelName = Encoding.ASCII.GetString(modelBytes);
            }

            return result;
        }
    }

    public class DeviceStatusResponse
    {
        public bool Success { get; set; }
        public byte Command { get; set; }
        public byte Status { get; set; }
        public int[] OutputStatus { get; set; } = new int[8];
        public string IpMode { get; set; } = "";
        public string DeviceName { get; set; } = "";
    }

    public class LcdStatusResponse
    {
        public bool Success { get; set; }
        public byte Command { get; set; }
        public byte Status { get; set; }
        public int BacklightTime { get; set; }
        public int Brightness { get; set; }

        public string GetBacklightTimeDescription()
        {
            return BacklightTime switch
            {
                0 => "15s Dim",
                1 => "60s Dim",
                2 => "15s Off",
                3 => "60s Off",
                4 => "Always On",
                _ => "Unknown"
            };
        }
    }

    public class SearchDeviceResponse
    {
        public bool Success { get; set; }
        public byte DeviceType { get; set; }
        public byte Status { get; set; }
        public string ModelName { get; set; } = "";
    }
}
