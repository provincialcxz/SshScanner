using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Renci.SshNet;

/*
ssh <IP-адрес> whoami - позволит узнать username
*/

namespace SshScanner
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("  _____     _      _____                                 \r\n / ____|   | |    / ____|                                \r\n| (___  ___| |__ | (___   ___ __ _ _ __  _ __   ___ _ __ \r\n \\___ \\/ __| '_ \\ \\___ \\ / __/ _` | '_ \\| '_ \\ / _ \\ '__|\r\n ____) \\__ \\ | | |____) | (_| (_| | | | | | | |  __/ |   \r\n|_____/|___/_| |_|_____/ \\___\\__,_|_| |_|_| |_|\\___|_|   \nmade by provincialcxz\n\n");

            string username = "your_username";
            string password = "your_password";
            string baseIp = "192.168.1.";

            List<string> iptrue = new List<string>();

            var tasks = new List<Task>();

            Console.WriteLine("Start scanning...");

            for (int i = 1; i < 255; i++)
            {
                string ip = baseIp + i.ToString();

                tasks.Add(Task.Run(async () =>
                {
                    if (await PingHostAsync(ip))
                    {
                        Console.WriteLine($"IP {ip} is up. Trying SSH connection...");

                        if (await TrySshConnectionAsync(ip, username, password))
                        {
                            lock (iptrue)
                            {
                                iptrue.Add(ip);
                            }
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
                }));
            }

            await Task.WhenAll(tasks);

            Console.WriteLine("Scan completed.\n");

            Console.WriteLine("Successful IPs:");
            if (iptrue.Count == 0)
            {
                Console.WriteLine("N/A");
            }
            else
            {
                foreach (string ip in iptrue)
                {
                    Console.WriteLine(ip);
                }
            }
        }

        static async Task<bool> PingHostAsync(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = new Ping();

            try
            {
                PingReply reply = await Task.Run(() => pinger.Send(nameOrAddress));
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                Console.WriteLine("Error");
            }

            return pingable;
        }

        static Task<bool> TrySshConnectionAsync(string ip, string username, string password)
        {
            return Task.Run(() =>
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
            });
        }
    }
}
