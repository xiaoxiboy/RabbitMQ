using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ
{
    public class Server
    {
        public static void Receive(Action<string> action )
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "myqueue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    
                    /// 7. 发送消息确认信号（手动消息确认）
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    action(message);
                };
                //8. 启动消费者
                //autoAck:true；自动进行消息确认，当消费端接收到消息后，就自动发送ack信号，不管消息是否正确处理完毕
                //autoAck:false；关闭自动消息确认，通过调用BasicAck方法手动进行消息确认
                channel.BasicConsume(queue: "myqueue", autoAck: false, consumer: consumer);
            }
        }
        public static bool SendIfno(params LogInfo[] log) {
                //1.1.实例化连接工厂
                var factory = new ConnectionFactory() { HostName = "localhost" };
                //2. 建立连接
                using (var connection = factory.CreateConnection())
                {
                    //3. 创建信道
                    using (var channel = connection.CreateModel())
                    {
                    //4. 申明交换机  ’direct‘类型 交换机 实现简单一对多发送 一个消息发送到多个消费端 
                    channel.ExchangeDeclare(exchange:"Logs",type:ExchangeType.Direct,durable:true,autoDelete:false,arguments:null);
                    // 两个队列    队列名  持久化 是否自动删除队列（没有消费是否自动删除)   其他参数
                    // 所有日志队列
                    channel.QueueDeclare(queue: "LogsAll", durable: true,  autoDelete: false, arguments: null);
                    // 异常日志专属队列
                    channel.QueueDeclare(queue: "LogsError", durable: true,  autoDelete: false, arguments: null);
                        foreach (LogInfo.LogsType suit in Enum.GetValues(typeof(LogInfo.LogsType)))
                        {
                        // 将日志的枚举str值作为路由键   发送给 Logs 交换机的 信息  路由键 =>  是否是日志的枚举str => 进入到 LogsAll 队列
                        channel.QueueBind(queue: "LogsAll",exchange: "Logs",routingKey:suit.ToString());
                        }
                    // 将异常日志的枚举str值作为路由键   发送给 Logs 交换机的 信息  路由键 =>  是否是异常日志的枚举str => 进入到 LogsError 队列
                    channel.QueueBind(queue: "LogsError", exchange: "Logs", routingKey: "error");
                    // 两者都是的话就会都进入队列
                    }
                }
                return true;

        }
        public static bool SendLogs(params LogInfo [] log)
        {
           
                SendIfno();
                //1.1.实例化连接工厂
                var factory = new ConnectionFactory() { HostName = "localhost" };
                //2. 建立连接
                using (var connection = factory.CreateConnection())
                {
                    //3. 创建信道
                    using (var channel = connection.CreateModel())
                    {
                        foreach (var item in log)
                        {
                            channel.BasicPublish(mandatory: true,  exchange: "Logs", routingKey: item.logsType.ToString(), body: Encoding.UTF8.GetBytes(item.content));
                        }
                    }
                }
                return true;
            }
    }
}
