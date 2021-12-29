using System;
using System.Collections.Generic;
using System.Linq;
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

namespace RabbitMQ
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// 2021年12月19日14:40:03
    /// 小希
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var DATA = new List<LogInfo>();
            for (int i = 0; i < 100; i++)
            {
                DATA.Add(new LogInfo() { content = "i：" + i, logsType = i % 4 == 0 ? LogInfo.LogsType.error : LogInfo.LogsType.info });
            }
            Server.SendIfno(DATA.ToArray());
        }
    }
}
