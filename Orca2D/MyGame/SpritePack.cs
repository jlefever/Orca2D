using Orca2D.MyGame.Entities;

namespace Orca2D.MyGame
{
    public class SpritePack
    {
        public Sprite King1 { get; set; }
        public Sprite King2 { get; set; }
        public Sprite King3 { get; set; }
        public Sprite King4 { get; set; }
        public Sprite King5 { get; set; }
        public Sprite King6 { get; set; }
        public Sprite King7 { get; set; }
        public Sprite King8 { get; set; }

        public bool IsValid()
        {
            return King1 != null
                && King2 != null
                && King3 != null
                && King4 != null
                && King5 != null
                && King6 != null
                && King7 != null
                && King8 != null;
        }
    }
}
