using System.Runtime.InteropServices;

namespace Ra3NatNegServer.Protocols.NatNeg;

// Reference: http://wiki.tockdom.com/wiki/MKWii_Network_Protocol/Server/mariokartwii.natneg.gs.nintendowifi.net

public enum PacketType : byte
{
    Init = 0x00,
    InitAck = 0x01,
    Connect = 0x05,
    ConnectAck = 0x06,
    Report = 0x0D,
    ReportAck = 0x0E,
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct PacketHeader
{
    public static readonly ReadOnlyMemory<byte> Magic = new byte[6]
    {
        0xFD, 0xFC, 0x1E, 0x66, 0x6A, 0xB2
    };
    public const byte Version = 3;
    public const string GameName = "redalert3pc";
    public const int GameNameLength = 11;

    public fixed byte ActualMagic[6];
    public byte ActualVersion;
    public PacketType Type;
    public uint ClientId;

    public static unsafe void Verify(byte* data)
    {
        var p = (PacketHeader*)data;

        if (!Magic.Span.SequenceEqual(new(p->ActualMagic, Magic.Length)))
        {
            throw new InvalidDataException();
        }
        if (p->ActualVersion is not Version)
        {
            throw new NotSupportedException();
        }
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct InitPacket
{
    public const PacketType Type = PacketType.Init;

    public PacketHeader Header;
    public byte PortType;
    public byte HostFlag;
    public byte UseGamePort;
    public uint LocalIpAddress;
    public ushort LocalPort;
    public fixed byte GameName[PacketHeader.GameNameLength];
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct InitAckPacket
{
    public const PacketType Type = PacketType.InitAck;
    public static readonly byte[] Padding = new byte[7]
    {
        0xFF, 0xFF, 0x6D, 0x16, 0xB5, 0x7D, 0xEA
    };

    public PacketHeader Header;
    public byte PortType;
    public byte HostFlag;
    public fixed byte ActualPadding[7];
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ConnectPacket
{
    public const PacketType Type = PacketType.Connect;

    public PacketHeader Header;
    public uint PeerPublicIpAddress;
    public ushort PeerPublicPort;
    public byte GotData;
    public byte Status;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct ReportPacket
{
    public const PacketType Type = PacketType.Report;

    public PacketHeader Header;
    public byte PortType;
    public byte HostFlag;
    public byte NatNegResult;
    public uint NatType;
    public uint NatMappdingScheme;
    public fixed byte GameName[PacketHeader.GameNameLength];
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ReportAckPacket
{
    public const PacketType Type = PacketType.ReportAck;

    public PacketHeader Header;
    public byte PortType;
    public byte HostFlag;
    public byte NatNegResult;
    public uint NatType;
    public ushort Zero;
}