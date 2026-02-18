using Isola.engine.utils;
using Isola.game.GUI;
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

        public ChatUI(float width, float height, eAnchor anchor, float scale)
            : base(width, height, anchor, scale, isDrawable: false, toggleDraw: true, isClickable: false) {

            BatchRenderer = AssetLibrary.BatchRendererList["Font Atlas"];

            float lineHeight = 12f;
            float inputY = 24f;

            _inputDisplay = new TextUI(width, lineHeight, eAnchor.None, 1.0f, true, false, false, "> ", 12, "Font Atlas");
            _inputDisplay.LocalRect.X = 4f;
            _inputDisplay.LocalRect.Y = inputY;
            Children.Add("Input", _inputDisplay);

            float historyStartY = inputY + lineHeight + 4f;

            for (int i = 0; i < _visibleLines; i++) {
                float y = historyStartY + (i * lineHeight);

                TextUI line = new TextUI(width, lineHeight, eAnchor.None, 1.0f, true, true, false, "", 12, "Font Atlas");

                line.LocalRect.X = 4f;
                line.LocalRect.Y = y;

                _historyLines.Add(line);
                Children.Add($"Line_{i}", line);
            }

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
            }
        }

        private void SubmitMessage() {
            if (string.IsNullOrWhiteSpace(_currentInput)) {
                ToggleChat();
                return;
            }

            if (_currentInput.StartsWith("/")) {
                ChatManager.AddMessage("Command: " + _currentInput);
                //todo: parsecommand
            } else {
                ChatManager.AddMessage("Magnet: " + _currentInput);
            }

            _currentInput = "";
            IsTyping = false;
        }
    }
}
