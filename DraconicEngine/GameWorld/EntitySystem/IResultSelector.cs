using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.Items;
using DraconicEngine.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem
{
    public interface IResultSelector
    {
        ItemUseResult Select(Entity user, List<Fulfilment> fulfilments);
    }

    public class FixedResultSelector : IResultSelector
    {
        ItemUseResult result;
        public FixedResultSelector(bool isDestroyed)
        {
            result = isDestroyed ? ItemUseResult.Destroyed : ItemUseResult.Used;
        }

        public ItemUseResult Select(Entity user, List<Fulfilment> fulfilments)
        {
            return result;
        }
    }
}
