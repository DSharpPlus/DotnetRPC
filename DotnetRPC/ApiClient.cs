using System.Threading;
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

        /// <summary>
        /// Writes a command with arguments to the RPC pipe.
        /// </summary>
        /// <param name="command">The command to execute, from <see cref="Commands"/></param>
        /// <param name="arguments">The command arguments object, to be serialized as JSON</param>
        /// <returns>Task that resolves when the command has been sent</returns>
        public Task SendCommandAsync(string command, object arguments)
        {
            var frame = new RpcFrame();

            var cmd = new RpcCommand
            {
                Arguments = JObject.FromObject(arguments),
                Command = command
            };

            frame.OpCode = OpCode.Frame;
            frame.SetContent(JsonConvert.SerializeObject(cmd));

            return _pipe.WriteAsync(frame);
        }
    }
}