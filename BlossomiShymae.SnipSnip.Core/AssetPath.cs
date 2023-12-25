using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlossomiShymae.SnipSnip.Core
{
    public record AssetPath(string Value)
    {
        public static readonly string Url = "https://raw.communitydragon.org";

        /// <summary>
        /// CommunityDragon version of asset path e.g. "latest", "pbe".
        /// </summary>
        public string Version => Value
            .Split(".org/", StringSplitOptions.RemoveEmptyEntries)
            .Last()
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .First();

        /// <summary>
        /// The files.exported.txt file the asset is referenced in.
        /// </summary>
        public string FilesExportedPath => $"{Url}/{Version}/cdragon/files.exported.txt";
        
        /// <summary>
        /// The asset path mapped to RAW website.
        /// </summary>
        public string Path 
        {
            get
            {
                var entries = Value.Split($".org/{Version}/", StringSplitOptions.RemoveEmptyEntries);

                if (entries.Length > 1)
                    return entries.Last();
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// The JSON path of asset.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string GetJsonPath(string file)
        {
            if (file.Contains('/'))
                return $"{Url}/json/{Version}/{string.Join('/', file.Split('/', StringSplitOptions.RemoveEmptyEntries).Where(x => !x.Contains('.')))}/";
            else
                return $"{Url}/json/{Version}/";
        }
    }
}