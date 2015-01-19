using System;
using DragonRising.Storage;
using System.Collections.Generic;
using System.Linq;
using DragonRising.GameWorld.Items;

namespace DragonRising.Libraries
{
   internal class SimpleItemUsagesLibrary : IItemUsageLibrary
   {
      Dictionary<string, IItemUsage> usages = new Dictionary<string, IItemUsage>();

      public void Add(string name, IItemUsage usage)
      {
         usages.Add(name, usage);
      }

      public bool Contains(string name)
      {
         return usages.ContainsKey(name);
      }

      public IItemUsage Get(string name)
      {
         return usages[name];
      }

      public string NameForUsage(IItemUsage usage)
      {
         if(usages.ContainsValue(usage))
         {
            return usages.First(kvp => kvp.Value == usage).Key;
         }
         throw new ArgumentException("No such usage in library");
      }
   }
}