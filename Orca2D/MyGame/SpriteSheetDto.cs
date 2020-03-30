using Newtonsoft.Json;
using System.Collections.Generic;

namespace Orca2D.MyGame
{
    public class SpriteSheetDto
    {
        [JsonProperty("TextureName")]
        public string TextureName { get; set; }

        [JsonProperty("Sprites")]
        public IDictionary<string, SpriteDto> Sprites { get; set; }
    }
}
