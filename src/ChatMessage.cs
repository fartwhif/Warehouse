namespace Warehouse
{
    internal class ChatMessage
    {
        public string Channel { get; set; }
        public int ChatterId { get; set; }
        public string ChatterName { get; set; }
        public string Verb { get; set; }
        public string Message { get; set; }
    }
}
