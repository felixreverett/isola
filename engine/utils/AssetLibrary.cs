using Isola.Core.Rendering;
using Isola.Drawing;
using Isola.engine.graphics;
using Isola.engine.graphics.text;
using Isola.Items;
using Microsoft.Extensions.Logging;

namespace Isola.Utilities{
    public class AssetLibrary {
        public List<Item>? ItemList;
        public List<TileData>? TileList;
        public Dictionary<int, TileData> TileLookup { get; private set; } = new();
        public Dictionary<string, IAtlasManager> TextureAtlasManagerList = new();
        public Dictionary<string, BatchRenderer> BatchRendererList = new();
        public Dictionary<string, Shader> ShaderList = new();
        private readonly ILogger<AssetLibrary> _logger;
        private readonly ILoggerFactory _loggerFactory;


        public AssetLibrary(ILoggerFactory loggerFactory) {
            _logger = loggerFactory.CreateLogger<AssetLibrary>();
            _loggerFactory = loggerFactory;
        }

        // Single initialise to ensure assets are loaded in the correct order
        public void Initialise() {
            _logger.LogInformation("Initialising Asset Library...");
            InitialiseShaders();
            InitialiseTextureAtlasManagerList();
            InitialiseItemList();
            InitialiseTileList();
        }

        public bool GetItemFromItemName(string itemName, out Item? item) {
            item = ItemList!.FirstOrDefault(i => i.ItemName == itemName);

            if (item != null) {
                return true;
            } else {
                return false;
            }
        }

        public bool GetTileIDFromTileName(string tileName, out int tileID) {
            tileID = TileList.FirstOrDefault(i => i.Name == tileName).TileID;
            return tileID != -1;
        }

        public void InitialiseItemList() {
            if (ItemList is not null) {
                return;
            }

            ItemList = new List<Item> {
                new Item("Persimmon", 1),
                // Seeds
                new Item_Seeds_WheatSeeds("Wheat Seeds", 43),
                // Tools
                new Item_Hoe("Stone Hoe", 84),
                // Chests
                new Item_WoodChest("Wood Chest", 126)
            };
        }

        public void InitialiseShaders() {
            Shader WorldShader = new(Shader.ParseShader(@"../../../resources/shaders/TextureWithColorAndTextureSlotAndUniforms.glsl"));
            if (WorldShader.CompileShader()) {
                WorldShader.SetTextureArray();
                ShaderList.Add("World Shader", WorldShader);
            } else {
                _logger.LogError("Failed to compile world shader.");
            }

            Shader UIShader = new(Shader.ParseShader(@"../../../resources/shaders/UIShader.glsl")); // todo: move to UI Load?
            if (UIShader.CompileShader()) {
                UIShader.SetTextureArray();
                ShaderList.Add("UI Shader", UIShader);
            } else {
                _logger.LogError("Failed to compile UI shader.");
            }

            Shader ScreenQuadShader = new(Shader.ParseShader(@"../../../resources/shaders/ScreenQuadShader.glsl"));
            if (ScreenQuadShader.CompileShader()) {
                ShaderList.Add("Screen Quad Shader", ScreenQuadShader);
            } else {
                _logger.LogError("Failed to compile Screen Quad shader.");
            }
        }

        public void InitialiseTextureAtlasManagerList() {
            TextureAtlasManagerList.Add("Tile Atlas", new IndexedTextureAtlasManager("atlases/1024 Tile Atlas x16.png", 1024, 16, 8, true, 0.0f));
            TextureAtlasManagerList.Add("Inventory Atlas", new PrecisionTextureAtlasManager("atlases/1024 UI Atlas x16.png", 1024, 1024, 1024, false));
            TextureAtlasManagerList.Add("Item Atlas", new IndexedTextureAtlasManager("atlases/1024 Item Atlas 16x.png", 1024, 16, 8, false, 0.001f));
            TextureAtlasManagerList.Add("Font Atlas", new FontAtlasManager("nosutaru", 4));
            TextureAtlasManagerList.Add("Entity Atlas", new EntityTextureAtlasManager("atlases/1024 Entity Atlas x16.png", 1024, 1024));

            ILogger<BatchRenderer> batchLogger = _loggerFactory.CreateLogger<BatchRenderer>();

            // [!] texture unit 0 is dedicated to the frame buffer object and mustn't be used
            BatchRendererList.Add("Tile Atlas", new BatchRenderer(ShaderList["World Shader"], 1, "atlases/1024 Tile Atlas x16.png", batchLogger, 0.0f));
            BatchRendererList.Add("Inventory Atlas", new BatchRenderer(ShaderList["UI Shader"], 2, "atlases/1024 UI Atlas x16.png", batchLogger));
            BatchRendererList.Add("Item Atlas", new BatchRenderer(ShaderList["UI Shader"], 3, "atlases/1024 Item Atlas 16x.png", batchLogger));
            BatchRendererList.Add("Font Atlas", new BatchRenderer(ShaderList["UI Shader"], 4, "fonts/nosutaru.png", batchLogger));
            BatchRendererList.Add("Entity Atlas", new BatchRenderer(ShaderList["World Shader"], 5, "atlases/1024 Entity Atlas 16x.png", batchLogger, 0.001f));

            _logger.LogInformation("All BatchRenderers initialised.");
        }

        public void InitialiseTileList() {
            if (TileList is not null) {
                return;
            }

            TileList = new List<TileData> {
                new TileData("Grass", 0, 1, false),
                new TileData("Sand", 1, 2, false),
                new TileData("Water", 2, 3, true),
                new TileData("Water_2", 3, 4, true),
                new TileData("Water_3", 4, 5, true),
                new TileData("Water_4", 5, 6, true),
                new TileData("Tilled Soil", 7, 7, false)
            };

            TileLookup = TileList.ToDictionary(t => t.TileID);

            var atlas = (IndexedTextureAtlasManager)TextureAtlasManagerList["Tile Atlas"];

            foreach (var tile in TileList) {
                tile.CalculateTexCoords(atlas);
            }

            _logger.LogInformation("Tile coordinates calculated for {Count} tiles.", TileList.Count);
        }
    }
}
