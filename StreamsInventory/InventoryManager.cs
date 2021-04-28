using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamsInventory
{
    public class InventoryManager
    {
        public string _csvFile;
        public VideoItemCollection _videos = new VideoItemCollection();
        public InventoryManager(string csvFile )
        {
            _csvFile = csvFile;
        }

        public InventoryResult AppendJson(string json)
        {
            InventoryResult ret = null;
            VideoItemCollection videos = new VideoItemCollection();
            try
            {
                JObject jsonResult = JObject.Parse(json);

                foreach (var node in jsonResult["value"].Children())
                {
                    VideoItem item = new VideoItem()
                    {
                        id = node["id"].ToString(),
                        name = node["name"].ToString(),
                        description = node["description"].ToString(),
                        playbackUrl = node["playbackUrl"].ToString(),
                        duration = node["media"]["duration"].ToString(),
                        creatorName = node["creator"]["name"].ToString(),
                        creatorEmail = node["creator"]["mail"].ToString()
                    };
                    videos.Add(item);
                }
                FileMode fileMode = FileMode.OpenOrCreate;
                if (_videos.Count > 0)
                {
                    fileMode = FileMode.Append;
                }
                CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    // Don't write the header again.
                    HasHeaderRecord = _videos.Count == 0,
                };
                using (var stream = File.Open(_csvFile, fileMode))
                using (var writer = new StreamWriter(stream))
                using (var csv = new CsvWriter(writer, config))
                {
                    csv.WriteRecords(videos);
                }

                _videos.AddRange(videos);

                ret = new InventoryResult()
                {
                    Result = true,
                    ProcessCount = videos.Count,
                    TotalCount = _videos.Count
                };

            }
            catch (Exception ex)
            {
                ret = new InventoryResult()
                {
                    Result = false,
                    Error = ex
                };
            }
            return ret;
        }
    }
}
