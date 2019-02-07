using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Ssc.SscUtil {
    public class SystemUtil {
        public static string CreateRandomString(int length) {
            if (length < 0) {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            return Guid.NewGuid().ToString().Substring(0, length);
        }

        public static Guid GetUUID() {
            return Guid.NewGuid();
        }

        public static string GetMD5(string input) {
            using (var md5 = MD5.Create()) {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
                return string.Join("", md5.ComputeHash(result).Select(x => x.ToString("X2")));
            }
        }

        public static IPAddress GetHostAddress(AddressFamily addressFamily) {
            var hostName = Dns.GetHostName();
            var addressList = Dns.GetHostEntry(hostName).AddressList;
            var list = addressList.Where(x => x.AddressFamily == addressFamily);
            foreach (var item in list) {
                var ping = new Ping();
                var replay = ping.Send(item);

                if (replay.Status == IPStatus.Success) return replay.Address;
            }

            return null;
        }

        public static string GetPorjectRootDirectory() {
            var output = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directory = System.IO.Path.GetDirectoryName(output);
            return directory;
        }
    }
}