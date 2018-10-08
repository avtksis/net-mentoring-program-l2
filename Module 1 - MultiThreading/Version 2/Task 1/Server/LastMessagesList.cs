using System.Collections.Generic;
using System.Linq;

namespace ChatServer
{
    public class LastMessagesList
    {
        private readonly List<byte[]> _lastMesages = new List<byte[]>();
        private readonly int _maxMessagesCount;
        private readonly object _locker = new object();

        public LastMessagesList(int maxMessageCount)
        {
            _maxMessagesCount = maxMessageCount;
        }

        public List<byte[]> GetMessages()
        {
            lock (_locker)
            {
                return _lastMesages;
            }
        }

        public void AddMessage(byte[] message)
        {
            lock (_locker)
            {
                if (_lastMesages.Count >= _maxMessagesCount) _lastMesages.Remove(_lastMesages.First());

                _lastMesages.Add(message);
            }
        }
    }
}