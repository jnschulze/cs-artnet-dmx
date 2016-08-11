//
// C# ArtNet DMX Implementation
//
// Author: Niklas Schulze <me@jns.io>

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using artnet;

public class DMXTest
{
    private static ArtnetDmx packet;
    private static Socket socket;
    private static IPEndPoint toAddr;

    // direction map for our up-down-reversing faders
    // true => up, false => down
    private static bool[] directions;

    public static void Main ()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        toAddr = new IPEndPoint(IPAddress.Parse("10.1.255.220"), 6454);

        // create a new ArtNet Packet for Universe 1
        packet = new ArtnetDmx(1);

        //PrintByteArray(packet.toBytes());

        /*
        packet.setChannel(0, 20);
        packet.setChannel(1, 20);
        packet.setChannel(2, 20);
        packet.setChannel(3, 20);
        packet.setChannel(4, 20);
        packet.setChannel(5, 20);
        packet.setChannel(6, 20);
        packet.setChannel(7, 20);
        packet.setChannel(8, 20);
        packet.setChannel(9, 20);
        packet.setChannel(10, 20);
        packet.setChannel(11, 20);
        packet.setChannel(12, 20);
        socket.SendTo(packet.toBytes(), toAddr);
        */


        directions = new bool[512];

        // LED PAR 1
        directions[10] = true;
        directions[11] = false;
        directions[12] = true;

        Timer timer = new Timer(3);
        timer.Elapsed += TimerEvent;
        timer.Start();

        Console.WriteLine("Press any key to exit... ");
        Console.ReadKey();

    }

    private static void _fadeChannel(ushort channel) {

      byte val = packet.getChannel(channel);

      if(directions[channel] && ++val == 255)
          directions[channel] = false;
      else if(!directions[channel] && --val == 0)
          directions[channel] = true;

      //Console.WriteLine("Val is {0}", val);

      packet.setChannel(channel, val);
    }

    private static void _toggleChannel(ushort channel) {
        byte val = packet.getChannel(channel);

        val = (val == 255 ? (byte)0 : (byte)255);

        packet.setChannel(channel, val);
    }

    private static void TimerEvent(Object source, ElapsedEventArgs e)
    {
        // LED PAR 1
        _fadeChannel(10);
        _fadeChannel(11);
        _fadeChannel(12);

        // LED PAR 2
        _toggleChannel(13);
        _toggleChannel(14);
        _toggleChannel(15);

        socket.SendTo(packet.toBytes(), toAddr);
    }

    /*
    public static void PrintByteArray(byte[] bytes)
    {
        var sb = new StringBuilder("new byte[] { ");
        foreach (var b in bytes)
        {
            sb.Append(b + ", ");
        }
        sb.Append("}");
        Console.WriteLine(sb.ToString());
    }
    */
}
