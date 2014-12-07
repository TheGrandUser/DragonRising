using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine
{
    public enum TileVisibility
    {
        NotSeen,
        Explored,
        Seen,
    }

    public class Tile
    {
        public bool BlocksMovement { get; set; }
        public bool BlocksSight { get; set; }
        public TileVisibility Visibility { get; set; }

        public Tile(bool blocksMovement)
        {
            this.BlocksMovement = blocksMovement;
            this.BlocksSight = this.BlocksMovement;
            this.Visibility = TileVisibility.NotSeen;
        }

        public Tile(bool blocksMovement, bool blocksSight)
        {
            this.BlocksMovement = blocksMovement;
            this.BlocksSight = blocksSight;
            this.Visibility = TileVisibility.NotSeen;
        }
    }
}
