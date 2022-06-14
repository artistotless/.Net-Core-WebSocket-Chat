using AdminPanel.Domain.Entities;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace AdminPanel.App.Models
{
    public class MessageComparer : IEqualityComparer<Message>
    {
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
