using Isola.ui;
using Isola.Utilities;

namespace Isola.engine.ui
{
    /// <summary>
    /// Master UI class used to contain all other UI elements
    /// </summary>
    public class MasterUI : UI
    {
        public MasterUI(float width, float height, eAnchor anchor, float scale, bool isDrawable = false, bool toggleDraw = true, bool isClickable = false)
            : base(width, height, anchor, scale, isDrawable, toggleDraw, isClickable)
        {
            BatchRenderer = AssetLibrary.BatchRendererList["Inventory Atlas"];
        }

        
    }
}
