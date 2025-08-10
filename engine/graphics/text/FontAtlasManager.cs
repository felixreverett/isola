using System.Xml.Linq;
using Isola.Drawing;

namespace Isola.engine.graphics.text
{
    /// <summary>
    /// Structure for mapping pixel locations to characters in a font file
    /// </summary>
    public class FontAtlasManager : IAtlasManager
    {
        // map
        private string _fontName;
        private string _fntFile;
        public int LineHeight { get; private set; }
        public int Base {  get; private set; }
        public int ScaleW { get; private set;}
        public int ScaleH { get; private set; }
        public Dictionary<int, CharData> Characters { get; private set; } = new Dictionary<int, CharData>();

        // Atlas information
        public string AtlasFileName { get; private set; }

        public FontAtlasManager(string fontName, int textureUnit)
        {
            Console.WriteLine($"Debug: FontAtlasManager constructor started for {fontName}."); //debug
            _fontName = fontName;
            _fntFile = $@"../../../resources/fonts/{_fontName}.fnt";
            AtlasFileName = $@"../../../resources/fonts/{_fontName}.png";
            ParseFntFile(_fntFile);
        }

        /// <summary>
        /// Parses .fnt files to prepare the corresponding mapping for the atlas
        /// </summary>
        /// <param name="fileName"></param>
        private void ParseFntFile(string fileName)
        {
            string fileContent = "";

            try
            {
                fileContent = File.ReadAllText(fileName);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"[!] Error: file '{fileName}' was not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Error: {ex.Message}");
            }

            if (string.IsNullOrEmpty(fileContent))
            {
                Console.WriteLine("[!] Error: XML content for parsing is empty.");
            }

            try
            {
                var doc = XDocument.Parse(fileContent);
                var fontNode = doc.Element("font");
                var commonNode = fontNode?.Element("common");

                if (commonNode != null)
                {
                    LineHeight = int.Parse(commonNode.Attribute("lineHeight")?.Value ?? "0");
                    Base = int.Parse(commonNode.Attribute("base")?.Value ?? "0");
                    ScaleW = int.Parse(commonNode.Attribute("scaleW")?.Value ?? "0");
                    ScaleH = int.Parse(commonNode.Attribute("scaleH")?.Value ?? "0");
                }

                else
                {
                    Console.WriteLine("[!] Error: 'common' node could not found in font file.");
                }

                var charsNode = fontNode?.Element("chars");

                if (charsNode != null)
                {
                    foreach (var charNode in charsNode.Elements("char"))
                    {
                        var charInfo = new CharData
                        {
                            Id = int.Parse(charNode.Attribute("id")?.Value ?? "0"),
                            X = int.Parse(charNode.Attribute("x")?.Value ?? "0"),
                            Y = int.Parse(charNode.Attribute("y")?.Value ?? "0"),
                            Width = int.Parse(charNode.Attribute("width")?.Value ?? "0"),
                            Height = int.Parse(charNode.Attribute("height")?.Value ?? "0"),
                            XOffset = int.Parse(charNode.Attribute("xoffset")?.Value ?? "0"),
                            YOffset = int.Parse(charNode.Attribute("yoffset")?.Value ?? "0"),
                            XAdvance = int.Parse(charNode.Attribute("xadvance")?.Value ?? "0"),
                        };
                        Characters[charInfo.Id] = charInfo;
                    }
                }

                else
                {
                    Console.WriteLine("[!] Error: 'chars' node not found in font file.");
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"[!] An error occurred while parsing the font file: {ex.Message}");
            }
        }

        public TexCoords GetIndexedAtlasCoords(char character)
        {
            //Console.WriteLine("Get Indexed Atlas Coords was called"); //debug
            TexCoords texCoords = new TexCoords();

            if (Characters.TryGetValue(character, out CharData charData))
            {
                //Console.WriteLine($"Debug: got the character {(char)charData.Id}"); //debug
                texCoords.MinX = (float)charData.X / ScaleW;
                texCoords.MaxX = (float)(charData.X + charData.Width) / ScaleW;
                texCoords.MinY = 1.0f - (float)(charData.Y + charData.Height) / ScaleH;
                texCoords.MaxY = 1.0f - (float)charData.Y / ScaleH;
            }

            else
            {
                Console.WriteLine($"Warning: Character '{character}' not found in font atlas.");
                return GetIndexedAtlasCoords('?');
            }

            return texCoords;
        }
    }

    public struct CharData
    {
        public int Id;
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public int XOffset;
        public int YOffset;
        public int XAdvance;
    }
}
