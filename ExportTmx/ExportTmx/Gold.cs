using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ExportTmx
{
    public class Gold
    {
        public Gold(string tmxPath, string destinationPath)
        {
            var doc = XDocument.Load(tmxPath);
            var map = doc.Element("map");
            var tmxDirectory = Path.GetDirectoryName(tmxPath);
            var tilesets = map.Elements("tileset");
            foreach (var tileset in tilesets.Where(x => x.Attribute("source") != null))
            {
                var tsxRelativePath = tileset.Attribute("source").Value;
                var tsxDirectory = Path.GetDirectoryName(tsxRelativePath);
                var tsxPath = Path.Combine(tmxDirectory, tsxRelativePath);
                var tsxDoc = XDocument.Load(tsxPath);
                var tsxTileset = tsxDoc.Element("tileset");
                tsxTileset.SetAttributeValue("firstgid", tileset.Attribute("firstgid").Value);
                var tsxImage = tsxTileset.Element("image");
                var tsxImageSource = tsxImage.Attribute("source").Value;
                var imagePath = Path.Combine(tsxDirectory, tsxImageSource);
                tsxImage.SetAttributeValue("source", imagePath);
                tileset.ReplaceWith(tsxTileset);
            }
            foreach (var tileset in tilesets)
            {
                var tilesetImage = tileset.Element("image");
                var imageSource = tilesetImage.Attribute("source");
                var imagePath = Path.Combine(tmxDirectory, imageSource.Value);
                var imageName = Path.GetFileName(imageSource.Value);
                var destPath = Path.Combine(destinationPath, imageName);
                File.Copy(imagePath, destPath, true);
                imageSource.SetValue(imageName);
            }
            var fullDest = Path.Combine(destinationPath, Path.GetFileName(tmxPath));
            doc.Save(fullDest);
        }
    }
}
