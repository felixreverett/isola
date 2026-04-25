using Isola.game.GUI;

namespace Isola.engine.utils {

    public struct ChatMessage {
        public string Text;
        public eTextColor Color;
        public ChatMessage(string text, eTextColor color = eTextColor.White) {
            Text = text;
            Color = color;
        }
    }
    public static class ChatManager {
        public static List<ChatMessage> History = new List<ChatMessage>();
        private static int MaxHistory = 100;
        public static void AddMessage(string text, eTextColor color = eTextColor.White) {
            History.Add(new ChatMessage(text, color));

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
