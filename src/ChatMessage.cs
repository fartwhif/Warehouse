namespace Warehouse
{
    internal class ChatMessage
    {
        public string Channel { get; set; }
        public int ChatterId { get; set; }
        public string ChatterName { get; set; }
        public string Verb { get; set; }
        public string Message { get; set; }
        public bool IsTell { get { return Verb == "tells you"; } }
        public bool IsOpen { get { return Verb == "says"; } }
    }
}
