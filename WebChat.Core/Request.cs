using Newtonsoft.Json;

namespace WebChat.Core
{
    public abstract class Request
    {
        [JsonProperty]
        public OpCode Op { get; private set; }

        [JsonConstructor]
        protected Request(OpCode op)
            => Op = op;
    }
}
