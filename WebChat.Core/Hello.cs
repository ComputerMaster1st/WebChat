using Newtonsoft.Json;

namespace WebChat.Core
{
    public class Hello : Request
    {
        [JsonConstructor]
        public Hello(OpCode op) : base(op)  { }
    }
}
