using Isola.Drawing;
using Isola.engine.graphics.text;
using Isola.ui;
using Isola.Utilities;
using Isola.World;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using System.Collections.Generic;

namespace Isola.game.GUI
{
    public class TextUI : UI
    {
        private string _text;
        private Vector3 _color;
        private readonly IAtlasManager _fontAtlasManager;

        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                }
            }
        }

        public Vector3 Color
        {
            get => _color;
            set => _color = value;
        }

        public TextUI
        (
            float koWidth, float koHeight, eAnchor anchor, float scale, bool isDrawable, bool toggleDraw, bool isClickable,
            FontAtlasManager fontAtlasManager, string text, Vector3 color, string atlasName
        ) : base
        (
            koWidth, koHeight, anchor, scale, isDrawable, toggleDraw, isClickable
        )
        {
            _fontAtlasManager = fontAtlasManager;
            BatchRenderer = AssetLibrary.BatchRendererList[atlasName];
            _text = text;
            _color = color;
        }

        private void Draw()
        {
            if (string.IsNullOrEmpty(Text))
            {
                return;
            }

            BatchRenderer.StartBatch();

        }
    }
}
