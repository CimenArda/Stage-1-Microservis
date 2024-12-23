using Grpc.Net.Client;
using gRPCService2;
using System;
using System.Threading.Tasks;

namespace gRPCService1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var greetClient = new Greeter.GreeterClient(channel);
            var response = greetClient.SayHello(new HelloRequest
            {
                Name = Console.ReadLine()
            });
            await Task.Run(async () =>
            {
                while (await response.ResponseStream.MoveNext(new System.Threading.CancellationToken()))
                {
                    Console.WriteLine($"Gelen Mesaj:{ response.ResponseStream.Current.Message}");
                }
            });
       
        }
    }
}
