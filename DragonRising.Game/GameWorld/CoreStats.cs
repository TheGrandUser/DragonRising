using DraconicEngine.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.GameWorld
{
   public static class CoreStats
   {
      public static readonly string Level = nameof(Level);
      public static readonly string XP = nameof(XP);

      public static CharacterStat<int> GetLevel(this Entity self) => self.GetStat<int>(Level);
      public static CharacterStat<int> GetXP(this Entity self) => self.GetStat<int>(XP);
   }
}
