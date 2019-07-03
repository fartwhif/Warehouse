namespace Warehouse
{
    internal class ChatMessage
    {
        public bool IsEither(int id, string name) { return (id != 0 && ChatterId == id) || (!string.IsNullOrEmpty(name) && name == ChatterName); }
        public string Channel { get; set; }
        public int ChatterId { get; set; }
        public string ChatterName { get; set; }
        public string Verb { get; set; }
        public string Message { get; set; }
        public bool IsTell => Verb == "tells you";
        public bool IsOpen => Verb == "says";
        public bool ParseSuccess { get; set; }
        public string ParsedCommand { get; set; }
        public string ParsedParameters { get; set; }
    }
}
