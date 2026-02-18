using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isola.engine.utils {

    public struct ChatMessage {
        public string Text;
        public ChatMessage(string text) {
            Text = text;
        }
    }
    public static class ChatManager {
        public static List<ChatMessage> History = new List<ChatMessage>();
        private static int MaxHistory = 100;
        public static void AddMessage(string text) {
            History.Add(new ChatMessage(text));

            if (History.Count > MaxHistory) {
                History.RemoveAt(0);
            }
        }

        public static List<ChatMessage> GetRecent(int count) {
            int start = Math.Max(0, History.Count - count);
            int amount = Math.Min(count, History.Count - start);
            return History.GetRange(start, amount);
        }
    }
}
