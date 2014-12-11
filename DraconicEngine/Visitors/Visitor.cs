using DraconicEngine.GameWorld.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Visitors
{
   public interface IVisitor
   {
      void VisitScene(Scene scene);
      void VisitTile(Tile tile, int column, int row);
      void VisitEntity(Entity entity);
      void VisitEntityComponent(Component component);
   }

   public interface IEntityHandler
   {
      Type EntityType { get; }
      void Handle(Entity entity);
   }

   public interface IComponentHandler
   {
      Type ComponentType { get; }
      void Handler(Component component);
   }
}