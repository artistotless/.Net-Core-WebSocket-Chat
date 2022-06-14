public class ChatMessageScheme
{
    public string type { get; set; }
    public Data data { get; set; }

    public class Data
    {
        public int messageId { get; set; }
        public string text { get; set; }
        public string timeStamp { get; set; }
        public Operator @operator { get; set; }
    }

    public class Operator
    {
        public string name { get; set; }
    }
}

