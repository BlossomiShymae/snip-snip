namespace snip_snip
{
    public static class Endpoints
    {
        public static readonly string Url = "https://raw.communitydragon.org";
        public static readonly string JsonUrl = Url + "/json";

        public static string GetVersion(string url)
        {
            return url
                .Split(".org/", StringSplitOptions.RemoveEmptyEntries)
                .Last()
                .Split('/', StringSplitOptions.RemoveEmptyEntries)
                .First();
        }

        public static string GetFilesExported(string version)
        {
            return $"{Url}/{version}/cdragon/files.exported.txt";
        }

        public static string GetAsset(string url, string version)
        {
            var entries = url
                .Split($".org/{version}/", StringSplitOptions.RemoveEmptyEntries);

            if (entries.Length > 1)
                return entries.Last();
            else
                return string.Empty;
        }

        public static string GetJsonPath(string file, string version)
        {
            if (file.Contains('/'))
                return $"{JsonUrl}/{version}/{string.Join('/', file.Split('/', StringSplitOptions.RemoveEmptyEntries).Where(x => !x.Contains('.')))}/";
            else
                return $"{JsonUrl}/{version}/";
        }
    }
}