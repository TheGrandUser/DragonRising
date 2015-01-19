using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.GameWorld.Alligences
{
   [Serializable]
   public class Alligence
   {
      public static readonly Alligence Neutral = new Alligence() { Name = "Neutral" };
      public string Name { get; set; }
   }
}