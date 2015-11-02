using DragonRising.GameWorld.Powers;
using DragonRising.GameWorld.Powers.Spells;
using DragonRising.Plans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.Storage
{
   public interface ISpellLibrary
   {
      Spell Get(string name);
      bool Contains(string name);
      void Add(Spell power);
   }
}
