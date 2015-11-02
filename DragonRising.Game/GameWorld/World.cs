using DraconicEngine.GameViews;
using DraconicEngine.EntitySystem;
using DragonRising.GameWorld.Alligences;
using DragonRising.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonRising.GameWorld.Generators;
using System.Diagnostics;
using DraconicEngine;

namespace DragonRising.GameWorld
{
   public sealed class World : IDisposable
   {
      public static World Current { get; set; }
      public int MapWidth { get; set; } = 160;
      public int MapHeight { get; set; } = 80;
      public int DungeonLevel { get; set; } = 1;
      public Entity Player { get; set; }

      public EntityEngine EntityEngine { get; } = new EntityEngine();

      //public Scene Scene => scenes.Count > 0 ? scenes.Peek() : null;
      public Scene Scene => scene;

      public IAlligenceManager Alligences { get; set; }

      public World()
      {
         this.Alligences = new SimpleAlligenceManager();
      }

      public World(Entity player)
      {
         if (player == null) { throw new ArgumentNullException(nameof(player)); }

         this.Player = player;
         this.Alligences = new SimpleAlligenceManager();

         var dragons = this.Alligences.GetOrAddByName("Dragons");
         var greenskins = this.Alligences.GetOrAddByName("Greenskins");

         AlligenceManager.Current.SetRelationship(dragons, greenskins, Relationship.Enemy);
         player.AsCreature(cc => cc.Alligence = dragons);

         this.EntityEngine.AddEntity(player);
      }
      
      Scene scene;
      //Stack<Scene> scenes = new Stack<Scene>();

      //public void PushScene(Scene scene) => scenes.Push(scene);
      public void PushScene(Scene scene) => this.scene = scene;

      //public void PopScene() { if (scenes.Count > 0) { scenes.Pop(); } }
      public void PopScene()
      {
         if (this.scene != null)
         {
            this.scene.FocusEntity = null;
            this.scene.EntityStore.Dispose();
            
            this.scene = null;
         }
      }

      public void Dispose()
      {
      }
   }
}
