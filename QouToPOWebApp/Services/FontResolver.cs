using PdfSharp.Fonts;

namespace QouToPOWebApp.Services
{
    public class FontResolver : IFontResolver
    {
        private readonly string _fontPath;
        private readonly Dictionary<string, byte[]> _fontData = new Dictionary<string, byte[]>();

        public FontResolver(string fontPath)
        {
            _fontPath = fontPath;
        }

        public FontResolver(Dictionary<string, string> fontPaths)
        {
            // Load font data from paths
            foreach (var font in fontPaths)
            {
                _fontData[font.Key] = File.ReadAllBytes(font.Value);
            }
        }

        public byte[] GetFont(string faceName)
        {
            if (_fontData.TryGetValue(faceName, out var fontData))
            {
                return fontData;
            }

            throw new KeyNotFoundException($"Font '{faceName}' not found in the resolver.");
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            // Resolve the font name (adjust this logic for your fonts)
            if (familyName == "Meiryo")
            {
                return new FontResolverInfo("Meiryo");
            }
            if (familyName == "Meiryo-bold")
            {
                return new FontResolverInfo("Meiryo-bold");
            }

            return null; // Fallback
        }
    }
}
