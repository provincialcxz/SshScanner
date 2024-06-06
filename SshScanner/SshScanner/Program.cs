using System.Net.NetworkInformation;
using Renci.SshNet;

namespace SshScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            string username = "your_username";
            string password = "your_password";
            string baseIp = "192.168.1."; // ваша подсеть

            List<string> iptrue = new List<string>();

            for (int i = 1; i < 255; i++)
            {
                string ip = baseIp + i.ToString();

                if (PingHost(ip))
                {
                    Console.WriteLine($"IP {ip} is up. Trying SSH connection...");

                    if (TrySshConnection(ip, username, password))
                    {
                        iptrue.Add(ip);
                        Console.WriteLine($"IP {ip} ---------------> TRUE");
                    }
                    else
                    {
                        Console.WriteLine($"IP {ip} - false");
                    }
                }
                else
                {
                    Console.WriteLine($"IP {ip} is down.");
                }
            }

            Console.WriteLine("Scan completed.\n");

            Console.WriteLine("Successful IPs:");
            foreach (string ip in iptrue)
            {
                Console.WriteLine(ip);
            }
        }

        static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = new Ping();

            try
            {
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                Console.WriteLine("Error");
            }

            return pingable;
        }

        static bool TrySshConnection(string ip, string username, string password)
        {
            try
            {
                using (var client = new SshClient(ip, username, password))
                {
                    client.Connect();
                    client.Disconnect();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}