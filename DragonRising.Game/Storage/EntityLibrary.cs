using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;

namespace DragonRising.Storage
{
   public interface IEntityLibrary
   {
      Dictionary<string, Entity> Templates { get; }

      Entity Get(string templateName);
   }
}