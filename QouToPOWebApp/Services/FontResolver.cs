using PdfSharp.Fonts;
using System.Reflection;

namespace QouToPOWebApp.Services
{
    public class FontResolver : IFontResolver
    {
        public static readonly FontResolver Instance = new FontResolver();

        // Default fallback font
        public string DefaultFontName => "Meiryo";

        private static readonly Dictionary<string, string> FontMap = new()
        {
            // Meiryo
            ["Meiryo#Regular"] = "QouToPOWebApp.AppData.Fonts.Meiryo.ttf",
            ["Meiryo#Bold"] = "QouToPOWebApp.AppData.Fonts.Meiryo-bold.ttf",

            // Calibri
            ["Calibri#Regular"] = "QouToPOWebApp.AppData.Fonts.Calibri-regular.ttf",
            ["Calibri#Bold"] = "QouToPOWebApp.AppData.Fonts.Calibri-bold.ttf",
            ["Calibri#Italic"] = "QouToPOWebApp.AppData.Fonts.Calibri-italic.ttf",
            ["Calibri#BoldItalic"] = "QouToPOWebApp.AppData.Fonts.Calibri-bold-italic.ttf",

            // Arial
            ["Arial#Regular"] = "QouToPOWebApp.AppData.Fonts.Arial.ttf",
            ["Arial#Bold"] = "QouToPOWebApp.AppData.Fonts.Arial-bold.ttf",
            ["Arial#Italic"] = "QouToPOWebApp.AppData.Fonts.Arial-italic.ttf",

            // Times New Roman
            ["Times New Roman#Bold"] = "QouToPOWebApp.AppData.Fonts.Times new roman-bold.ttf"
        };

        public byte[] GetFont(string faceName)
        {
            if (!FontMap.TryGetValue(faceName, out string? resource))
            {
                // Try Regular fallback
                var fallbackKey = faceName.Split('#')[0] + "#Regular";
                if (!FontMap.TryGetValue(fallbackKey, out resource))
                {
                    // Final fallback to DefaultFont#Regular
                    fallbackKey = $"{DefaultFontName}#Regular";
                    resource = FontMap[fallbackKey];
                }
            }

            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
            if (stream == null)
                throw new FileNotFoundException("Font resource not found: " + resource);

            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            string key = GetStyleKey(familyName, isBold, isItalic);

            if (!FontMap.ContainsKey(key))
            {
                // Try Regular fallback
                string fallbackRegular = $"{familyName}#Regular";
                if (FontMap.ContainsKey(fallbackRegular))
                    return new FontResolverInfo(fallbackRegular);

                // Final fallback
                return new FontResolverInfo($"{DefaultFontName}#Regular");
            }

            return new FontResolverInfo(key);
        }

        private static string GetStyleKey(string family, bool bold, bool italic)
        {
            if (bold && italic) return $"{family}#BoldItalic";
            if (bold) return $"{family}#Bold";
            if (italic) return $"{family}#Italic";
            return $"{family}#Regular";
        }
    }
}
