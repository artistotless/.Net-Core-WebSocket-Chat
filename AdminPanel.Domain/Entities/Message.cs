using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace AdminPanel.Domain.Entities
{
    public class Message: IEqualityComparer<Message>
    {
        public int MessageID { get; set; }
        public string Text { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool Read { get; set; }
        public int PlayerID { get; set; }
        public Player Player { get; set; }
        public int? OperatorID { get; set; }
        public Operator Operator { get; set; }

        public bool Equals(Message x, Message y)
        {
           return x.PlayerID == y.PlayerID;
        }

        public int GetHashCode([DisallowNull] Message obj)
        {
            return obj.PlayerID.GetHashCode();
        }
    }
}
