using Newtonsoft.Json;

namespace Orca2D.MyGame
{
    public class SpriteDto
    {
        [JsonProperty("X")]
        public int X { get; set; }

        [JsonProperty("Y")]
        public int Y { get; set; }

        [JsonProperty("Width")]
        public int Width { get; set; }

        [JsonProperty("Height")]
        public int Height { get; set; }
    }
}
