using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;

namespace DraconicEngine.Storage
{
   public class EntityLibrary
   {
      Dictionary<string, EntityTemplate> templates = new Dictionary<string, EntityTemplate>();

      public Dictionary<string, EntityTemplate> Templates { get { return templates; } }

      public EntityLibrary()
      {

      }
   }
}