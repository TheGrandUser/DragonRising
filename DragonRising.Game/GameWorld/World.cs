using DraconicEngine.GameWorld.EntitySystem;
using DragonRising.GameWorld.Alligences;
using DragonRising.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.GameWorld
{
   public sealed class World
   {
      public static World Current { get; set; }
      public int MapWidth { get; set; } = 160;
      public int MapHeight { get; set; } = 80;
      public int DungeonLevel { get; set; } = 1;
      public Entity Player { get; set; }

      //public Scene Scene => scenes.Count > 0 ? scenes.Peek() : null;
      public Scene Scene => scene;

      public IAlligenceManager Alligences { get; set; }

      public World()
      {
         this.Alligences = new SimpleAlligenceManager();
      }

      public World(Entity player)
      {
         this.Player = player;
         this.Alligences = new SimpleAlligenceManager();

         var dragons = this.Alligences.GetOrAddByName("Dragons");
         var greenskins = this.Alligences.GetOrAddByName("Greenskins");

         AlligenceManager.Current.SetRelationship(dragons, greenskins, Relationship.Enemy);
         player.AsCreature(cc => cc.Alligence = dragons);

         GenerateNewScene();
      }

      public void NextLevel()
      {
         this.DungeonLevel++;
         GenerateNewScene();
      }

      public void GenerateNewScene()
      {
         this.PopScene();

         var greenskins = new GreenskinsGenerator();
         var generator = new DungeonGenerator(greenskins, new StandardItemGenerator());

         Scene scene = new Scene(MapWidth, MapHeight);
         scene.FocusEntity = this.Player;
         scene.Level = this.DungeonLevel;

         var startPoint = generator.MakeMap(scene);

         this.PushScene(scene);
         this.Player.SetLocation(startPoint);

         this.Scene.ClearFoV();
         this.Scene.UpdateFoV();
      }


      Scene scene;
      //Stack<Scene> scenes = new Stack<Scene>();

      //public void PushScene(Scene scene) => scenes.Push(scene);
      public void PushScene(Scene scene) => this.scene = scene;

      //public void PopScene() { if (scenes.Count > 0) { scenes.Pop(); } }
      public void PopScene() => this.scene = null;
   }
}
