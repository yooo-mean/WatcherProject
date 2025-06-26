using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Management;
using Newtonsoft.Json;
using Grpc.Net.Client;
using SystemInfoGrpc; // 자동 생성된 namespace
using System.Threading.Tasks;
using System.Xml;

namespace WatcherUI_Core
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            //MyLabel.Visibility = Visibility.Visible;  // 또는 Visibility.Hidden

            var info = SystemInfoCollector.GetSystemInfo();
            foreach (var item in info)
            {
                Console.WriteLine($"{item.Key}: {item.Value}");
            }

            // 예: TextBox에 표시
            TxtOutput.Text = string.Join(Environment.NewLine, info.Select(i => $"{i.Key}: {i.Value}"));

            // JSON 문자열로 직렬화
            string json = JsonConvert.SerializeObject(info, Newtonsoft.Json.Formatting.Indented);

            //테스트용 메시지 박스
            MessageBox.Show(json);
        }

        public class SystemInfoCollector
        {
            public static Dictionary<string, string> GetSystemInfo()
            {
                var info = new Dictionary<string, string>();

                // 사용자명 & 컴퓨터 이름
                info["Username"] = Environment.UserName;
                info["MachineName"] = Environment.MachineName;
                info["OSVersion"] = Environment.OSVersion.ToString();

                // 모니터 해상도
                //var screen = Screen.PrimaryScreen.Bounds;
                //info["ScreenWidth"] = screen.Width.ToString();
                //info["ScreenHeight"] = screen.Height.ToString();

                // IP 주소 & MAC 주소
                var ipAddress = NetworkInterface
                    .GetAllNetworkInterfaces()
                    .SelectMany(ni => ni.GetIPProperties().UnicastAddresses)
                    .Where(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork && !ip.Address.ToString().StartsWith("169.254"))
                    .Select(ip => ip.Address.ToString())
                    .FirstOrDefault() ?? "Unknown";
                info["IPAddress"] = ipAddress;

                var macAddress = NetworkInterface
                    .GetAllNetworkInterfaces()
                    .Where(ni => ni.OperationalStatus == OperationalStatus.Up)
                    .Select(ni => ni.GetPhysicalAddress().ToString())
                    .FirstOrDefault() ?? "Unknown";
                info["MACAddress"] = macAddress;

                // CPU 정보
                var cpuSearcher = new ManagementObjectSearcher("select * from Win32_Processor");
                foreach (var item in cpuSearcher.Get())
                {
                    info["CPU"] = item["Name"]?.ToString() ?? "Unknown";
                    break; // 첫 번째만
                }

                // 메모리 정보
                var memSearcher = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                foreach (var item in memSearcher.Get())
                {
                    var totalMemoryKB = item["TotalVisibleMemorySize"]?.ToString() ?? "0";
                    var totalMemoryMB = (int.Parse(totalMemoryKB) / 1024).ToString();
                    info["TotalMemoryMB"] = totalMemoryMB;
                    break;
                }

                return info;
            }
        }

        private async Task SendJsonToGrpcServer(string json)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5201"); // 주소 확인 필수
            var client = new SystemInfoService.SystemInfoServiceClient(channel);

            var request = new SystemInfoRequest { Json = json };
            var reply = await client.SendSystemInfoAsync(request);

            MessageBox.Show($"서버 응답: {reply.Message}");
        }

    }
}
