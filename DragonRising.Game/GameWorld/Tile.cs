using DraconicEngine;
using DragonRising.Storage;
using Newtonsoft.Json;
using System;

namespace DragonRising
{
   [Serializable]
   public class TileType
   {
      public int Id { get; set; }
      public string Name { get; set; }
      public string Description { get; set; } = string.Empty;
      public Character InView { get; set; }
      public Character Explored { get; set; }
      public bool BlocksMovement { get; set; }
      public bool BlocksSight { get; set; }
   }
}