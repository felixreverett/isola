using Isola.engine.utils;
using Isola.Entities;
using Isola.game.GUI;
using Isola.Inventories;
using Isola.ui;
using Isola.Utilities;
using OpenTK.Windowing.Common;

namespace Isola.engine.ui {
    public class ChatUI : UI {
        private int _visibleLines = 5;
        private string _currentInput = "";
        public bool IsTyping { get; private set; } = false;
        private TextUI _inputDisplay;
        private List<TextUI> _historyLines = new List<TextUI>();
        private PlayerEntity OwnerPlayer;

        public ChatUI(float width, float height, eAnchor anchor, float scale, AssetLibrary assets, PlayerEntity ownerPlayer)
            : base(width, height, anchor, scale, assets, isDrawable: false, toggleDraw: true, isClickable: false) {

            BatchRenderer = _assets.BatchRendererList["Font Atlas"];

            float lineHeight = 12f;
            float inputY = 24f;

            _inputDisplay = new TextUI(width, lineHeight, eAnchor.None, 1.0f, _assets, true, false, false, "> ", 12, "Font Atlas");
            _inputDisplay.LocalRect.X = 4f;
            _inputDisplay.LocalRect.Y = inputY;
            Children.Add("Input", _inputDisplay);

            float historyStartY = inputY + lineHeight + 4f;

            for (int i = 0; i < _visibleLines; i++) {
                float y = historyStartY + (i * lineHeight);

                TextUI line = new TextUI(width, lineHeight, eAnchor.None, 1.0f, _assets, true, true, false, "", 12, "Font Atlas");

                line.LocalRect.X = 4f;
                line.LocalRect.Y = y;

                _historyLines.Add(line);
                Children.Add($"Line_{i}", line);
            }

            OwnerPlayer = ownerPlayer;
        }

        public override void Update() {

            List<ChatMessage> recent = ChatManager.GetRecent(_visibleLines);

            for (int i = 0; i < _visibleLines; i++) {
                int msgIndex = recent.Count - 1 - i;

                if (msgIndex >= 0 && msgIndex < recent.Count) {
                    _historyLines[i].Text = recent[msgIndex].Text;
                } else {
                    _historyLines[i].Text = "";
                }
            }

            _inputDisplay.ToggleDraw = IsTyping;

            string cursor = (DateTime.Now.Millisecond % 1000 < 500) ? "_" : " ";

            _inputDisplay.Text = "> " + _currentInput + cursor;

            base.Update();
        }

        public void ToggleChat() {
            IsTyping = !IsTyping;
            if (!IsTyping) {
                _currentInput = "";
            }
        }

        public void OnTextInput(TextInputEventArgs e) {
            if (!IsTyping) return;

            if (_currentInput.Length < 60) {
                _currentInput += (char)e.Unicode;
            }
        }

        public override void OnKeyDown(KeyboardKeyEventArgs e) {
            if (!IsTyping) return;

            if (e.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape) {
                ToggleChat();
            } else if (e.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.Backspace) {
                if (_currentInput.Length > 0) {
                    _currentInput = _currentInput.Substring(0, _currentInput.Length - 1);
                }
            } else if (e.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.Enter) {
                SubmitMessage();
                ToggleChat();
            }
        }

        private void SubmitMessage() {
            if (string.IsNullOrWhiteSpace(_currentInput)) {
                return;
            }

            if (_currentInput.StartsWith("/")) {
                ParseCommand(_currentInput.Substring(1).Split(' '));
            } else {
                ChatManager.AddMessage("Magnet: " + _currentInput);
            }

            _currentInput = "";
            IsTyping = false;
        }

        private void ParseCommand(string[] command) {
            switch (command[0].ToLower()) {
                case "help":
                    ChatManager.AddMessage("Available commands: /help /clear /give");
                    return;

                case "clear":
                    ChatManager.History.Clear();
                    return;

                case "give":
                    CMD_Give(command);
                    break;
                default:
                    ChatManager.AddMessage("Unrecognised command: \"" + command[0] + "\"");
                    break;
            }
        }

        private void CMD_Give(string[] command) {
            if (command.Length < 2) {
                ChatManager.AddMessage("Usage: /give [item] [amount]");
                return;
            } else {
                string rawInputName = command[1].Replace("\"", "");

                int amount = 1;

                if (command.Length > 2) {
                    if (!int.TryParse(command[2], out amount)) {
                        ChatManager.AddMessage("Error: Invalid amount entered.");
                        return;
                    }
                }

                var itemDef = _assets.ItemList?.FirstOrDefault(i => i.ItemName.Equals(rawInputName, StringComparison.OrdinalIgnoreCase));

                if (itemDef != null) {
                    string trueItemName = itemDef.ItemName;
                    bool added = OwnerPlayer.Inventory.TryAddItem(trueItemName, amount);

                    if (added) {
                        ChatManager.AddMessage($"Added {amount} {trueItemName}(s) to player.");
                    } else {
                        OwnerPlayer.CurrentWorld.AddEntityToWorld(new ItemEntity(OwnerPlayer.Position, trueItemName, 1, _assets));
                        ChatManager.AddMessage($"Inventory full. Dropped {amount} {trueItemName}(s) on ground.");
                    }
                }
            }
        }
    }
}
