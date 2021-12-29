using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ
{
   public class LogInfo
    {
        public enum LogsType {error,info,debug,warn }
        public LogsType logsType { get; set; }
        public DateTime createtime { get; set; } = DateTime.Now;
        public string content { get; set; } = string.Empty;
    }
}
