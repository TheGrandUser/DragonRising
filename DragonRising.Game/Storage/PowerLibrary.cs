using DragonRising.GameWorld.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.Storage
{
   public interface IPowerLibrary
   {
      Power Get(string name);
      bool Contains(string name);
      void Add(Power power);
   }
}
