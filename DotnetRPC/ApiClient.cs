using System.Threading.Tasks;
using DotnetRPC.Entities;
using DotnetRPC.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotnetRPC
{
    internal class ApiClient
    {
        private readonly PipeClient _pipe;
        private readonly Logger _logger;

        internal ApiClient(PipeClient pipe, Logger logger)
        {
            _pipe = pipe;
            _logger = logger;
        }

        public Task WriteFrameAsync(object arguments)
        {
            var frame = new RpcFrame();

            var cmd = new RpcCommand
            {
                Arguments = JObject.FromObject(arguments),
                Command = Commands.SetActivity
            };

            frame.OpCode = OpCode.Frame;
            frame.SetContent(JsonConvert.SerializeObject(cmd));

            return _pipe.WriteAsync(frame);
        }
    }
}