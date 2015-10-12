using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonRising;
using DragonRising.GameWorld;
using DraconicEngine.EntitySystem;
using DragonRising.GameWorld.Components;
using DragonRising.GameWorld.Alligences;
using DraconicEngine;

namespace Tinkering
{
   static class Procedure
   {
      class NoProcedure : IProcedure
      {
         public IEnumerable<Effect> GetEffects(Entity focusEntity, WorldContext context, Entity user) =>
            EnumerableEx.Return(Effect.None);
      }

      public static IProcedure None { get; } = new NoProcedure();
   }

   interface IProcedure
   {
      IEnumerable<Effect> GetEffects(Entity focusEntity, WorldContext context, Entity user);
   }

   class SequenceProcedure : IProcedure
   {
      List<IProcedure> procedures = new List<IProcedure>();

      public List<IProcedure> Procedures => procedures;

      public IEnumerable<Effect> GetEffects(Entity focusEntity, WorldContext context, Entity user) =>
         procedures.SelectMany(p => p.GetEffects(focusEntity, context, user));
   }

   class AreaSelector : IProcedure
   {
      public AreaSelector()
      {
      }

      public AreaSelector(Area area, IProcedure procedure)
      {
         Area = area;
         Procedure = procedure;
      }

      public Area Area { get; set; }
      public IProcedure Procedure { get; set; }

      public IEnumerable<Effect> GetEffects(Entity focusEntity, WorldContext context, Entity user) =>
         context.GetEntityInArea(Area).SelectMany(e => Procedure.GetEffects(e, context, user));
   }

   abstract class SaveProcedure : IProcedure
   {
      public string Check { get; set; }
      public string DCExpression { get; set; }

      public IEnumerable<Effect> GetEffects(Entity focusEntity, WorldContext context, Entity user)
      {
         var attribute = focusEntity.GetStat<int>(Check)?.Value;
         var dc = EvalDC(DCExpression, user);

         var mod = (attribute / 2 - 5);

         var checkTotal = context.Random.Next(1, 21) + mod;

         if (checkTotal >= dc)
         {
            return OnSuccess(focusEntity, context, user);
         }
         else
         {
            return OnFailure(focusEntity, context, user);
         }
      }

      protected abstract IEnumerable<Effect> OnSuccess(Entity focusEntity, WorldContext context, Entity user);
      protected abstract IEnumerable<Effect> OnFailure(Entity focusEntity, WorldContext context, Entity user);

      int EvalDC(string expression, Entity user)
      {
         return 15;
      }
   }

   class SaveForHalf : SaveProcedure
   {
      public MakeDamageEffect Damage { get; set; }

      protected override IEnumerable<Effect> OnFailure(Entity focusEntity, WorldContext context, Entity user) =>
         EnumerableEx.Return(Damage.GetEffect(focusEntity, context, user));

      protected override IEnumerable<Effect> OnSuccess(Entity focusEntity, WorldContext context, Entity user) =>
         EnumerableEx.Return(new HalfDamageEffect()
         {
            Effect = Damage.GetEffect(focusEntity, context, user),
            Check = Check
         });
   }

   class SaveSwitch : SaveProcedure
   {
      public IProcedure SuccessProcedure { get; set; }
      public IProcedure FailureProcedure { get; set; }

      protected override IEnumerable<Effect> OnSuccess(Entity focusEntity, WorldContext context, Entity user) =>
         SuccessProcedure.GetEffects(focusEntity, context, user);

      protected override IEnumerable<Effect> OnFailure(Entity focusEntity, WorldContext context, Entity user) =>
         FailureProcedure.GetEffects(focusEntity, context, user);
   }

   class MakeDamageEffect : IProcedure
   {
      public string DamageExpression { get; set; }
      public string Tag { get; set; }

      public DamageEffect GetEffect(Entity focusEntity, WorldContext context, Entity user) =>
         new DamageEffect()
         {
            Target = focusEntity,
            Attacker = user,
            Damage = eval(DamageExpression),
            Tag = Tag
         };


      private int eval(string damageExpression)
      {
         return 10;
      }

      IEnumerable<Effect> IProcedure.GetEffects(Entity focusEntity, WorldContext context, Entity user) =>
         EnumerableEx.Return(GetEffect(focusEntity, context, user));
   }

   abstract class ProcedureValue<T>
   {
   }

   abstract class ConstantValue<T> : ProcedureValue<T>
   {
      public T Value { get; set; }
   }

   abstract class ParameterValue<T>: ProcedureValue<T>
   {
      public string ParemeterName { get; set; }
   }

   abstract class PointAlongDirection : ProcedureValue<Vector>
   {
      public ProcedureValue<int> Distance { get; set; }
      public ProcedureValue<Direction> Direction { get; set; }
   }
}
