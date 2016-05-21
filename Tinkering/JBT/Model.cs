using LanguageExt;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace Tinkering.JBT
{
   class Position
   {
      public Position(IEnumerable<int> moves)
      {
         this.Moves = moves.ToImmutableList();
      }

      public Position(params int[] moves)
      {
         this.Moves = moves.ToImmutableList();
      }

      public ImmutableList<int> Moves { get; }
      public Position AddMove(int move) => new Position(Moves.Add(move));
      public Position AddMove(IEnumerable<int> moves) => new Position(this.Moves.AddRange(moves));
      public Position AddMove(Position other) => new Position(this.Moves.AddRange(other.Moves));

      public override string ToString() => $"[{string.Join(" ", Moves)}]";
      public override bool Equals(object obj)
      {
         var other = obj as Position;

         if (other != null)
         {
            return Moves.SequenceEqual(other.Moves);
         }

         return false;
      }
      public override int GetHashCode()
      {
         var first = Moves.FirstOrDefault();
         return Moves.Skip(1).Aggregate(first, (acc, cur) => acc ^ cur);
      }
   }

   enum Status
   {
      Uninitialized,
      Running,
      Failed,
      Succeeded,
      Terminated,
   }

   abstract class ModelTask
   {
      List<ModelTask> children;
      Position position = new Position();
      Option<ModelTask> guard = None;

      public ModelTask(Option<ModelTask> guard, params ModelTask[] children)
      {
         this.guard = guard;
         this.children = children.ToList();
      }

      public IReadOnlyList<ModelTask> Children => children.AsReadOnly();
      public Option<ModelTask> Guard => guard;
      public Position Position => position;

      public abstract ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent);
      public void ComputePositions()
      {
         this.position = new Position();
         for (int i = 0; i < this.children.Count; i++)
         {
            var currentChild = children[i];
            var currentChildPos = position.AddMove(i);
            currentChild.position = currentChildPos;
            ComputePositions(currentChild);
         }
      }

      public ModelTask FindNode(Position moves)
      {
         ModelTask currentTask = this;
         foreach (var currentMove in moves.Moves)
         {
            if (currentTask.children.Count <= currentMove)
            {
               return null;
            }
            currentTask = currentTask.children[currentMove];
         }

         return currentTask;
      }

      void ComputePositions(ModelTask t)
      {
         for (int i = 0; i < t.children.Count; i++)
         {
            var currentChild = t.children[i];
            var currentChildPos = t.position.AddMove(i);
            currentChild.position = currentChildPos;
            ComputePositions(currentChild);
         }
      }
   }

   abstract class ModelComposite : ModelTask
   {
      public ModelComposite(Option<ModelTask> guard, params ModelTask[] children)
         : base(guard, children)
      {
         if (children.Length == 0) throw new IllegalReturnStatusException("The list of children can not be empty");
      }
   }

   class ModelDynamicPriorityList : ModelComposite
   {
      public ModelDynamicPriorityList(Option<ModelTask> guard, params ModelTask[] children)
         : base(guard, children)
      {
      }

      public override ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent) => new ExecutionDynamicPriorityList(this, executor, parent);
   }

   enum ParallelPolicy
   {
      SequencePolicy,
      SelectorPolicy
   }

   class ModelParallel : ModelComposite
   {
      public ModelParallel(Option<ModelTask> guard, ParallelPolicy policy, params ModelTask[] children)
         : base(guard, children)
      {
         Policy = policy;
      }

      public ParallelPolicy Policy { get; }
      public override ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent) => new ExecutionParallel(this, executor, parent);
   }

   class ModelRandomSelector : ModelComposite
   {
      public ModelRandomSelector(Option<ModelTask> guard, params ModelTask[] children)
         : base(guard, children)
      {
      }

      public override ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent) => new ExecutionModelRandomSelector(this, executor, parent);
   }

   class ModelRandomSequence : ModelComposite
   {
      public ModelRandomSequence(Option<ModelTask> guard, params ModelTask[] children)
         : base(guard, children)
      {
      }

      public override ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent) => new ExecutionModelRandomSequence(this, executor, parent);
   }
   
   class ModelSelector : ModelComposite
   {
      public ModelSelector(Option<ModelTask> guard, params ModelTask[] children)
         : base(guard, children)
      {
      }

      public override ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent) => new ExecutionModelSelector(this, executor, parent);
   }

   class ModelSequence : ModelComposite
   {
      public ModelSequence(Option<ModelTask> guard, params ModelTask[] children)
         : base(guard, children)
      {
      }

      public override ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent) => new ExecutionModelSequence(this, executor, parent);
   }
   
   class ModelStaticPriorityList : ModelComposite
   {
      public ModelStaticPriorityList(Option<ModelTask> guard, params ModelTask[] children)
         : base(guard, children)
      {
      }

      public override ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent) => new ExecutionModelStaticPriorityList(this, executor, parent);
   }

   abstract class ModelDecorator : ModelTask
   {
      public ModelDecorator(Option<ModelTask> guard, ModelTask child)
         : base(guard, child)
      {
      }

      public ModelTask Child => Children[0];
   }

   class ModelHierarchicalContextManager : ModelDecorator
   {
      public ModelHierarchicalContextManager(Option<ModelTask> guard, ModelTask child)
         : base(guard, child)
      {

      }

      public override ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent) =>
         new ExecutionModelHierarchicalContextManager(this, executor, parent);
   }

   class ModelInterrupter : ModelDecorator
   {
      public ModelInterrupter(Option<ModelTask> guard, ModelTask child)
         : base(guard, child)
      {

      }

      public override ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent) =>
         new ExecutionInterrupter(this, executor, parent);
   }
   
   class ModelInverter : ModelDecorator
   {
      public ModelInverter(Option<ModelTask> guard, ModelTask child)
         : base(guard, child)
      {

      }

      public override ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent) =>
         new ExecutionInverter(this, executor, parent);
   }
   
   class ModelLimit : ModelDecorator
   {
      public ModelLimit(Option<ModelTask> guard, int maxTimes, ModelTask child)
         : base(guard, child)
      {
         MaxTimes = maxTimes;
      }

      public int MaxTimes { get; }

      public override ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent) =>
         new ExecutionLimit(this, executor, parent);
   }
   
   class ModelRepeat : ModelDecorator
   {
      public ModelRepeat(Option<ModelTask> guard, ModelTask child)
         : base(guard, child)
      {

      }

      public override ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent) =>
         new ExecutionRepeat(this, executor, parent);
   }

   class ModelSafeContextManager : ModelDecorator
   {
      public ModelSafeContextManager(Option<ModelTask> guard, ModelTask child)
         : base(guard, child)
      {

      }

      public override ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent) =>
         new ExecutionSafeContextManager(this, executor, parent);
   }

   class ModelSafeOutputContextManager : ModelDecorator
   {
      public ModelSafeOutputContextManager(Option<ModelTask> guard, IEnumerable<string> outputVariables, ModelTask child)
         : base(guard, child)
      {
         this.OutputVariables = outputVariables.ToImmutableList();
      }

      public ImmutableList<string> OutputVariables { get; }

      public override ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent) =>
         new ExecutionSafeOutputContextManager(this, executor, parent);
   }

   class ModelSucceeder : ModelDecorator
   {
      public ModelSucceeder(Option<ModelTask> guard, ModelTask child)
         : base(guard, child)
      {

      }

      public override ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent) =>
         new ExecutionSucceeder(this, executor, parent);
   }

   class ModelUntilFail : ModelDecorator
   {
      public ModelUntilFail(Option<ModelTask> guard, ModelTask child)
         : base(guard, child)
      {

      }

      public override ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent) =>
         new ExecutionUntilFail(this, executor, parent);
   }
   
   abstract class ModelLeaf : ModelTask
   {
      public ModelLeaf(Option<ModelTask> guard)
         : base(guard)
      {
      }
   }

   class ModelFailure : ModelLeaf
   {
      public ModelFailure(Option<ModelTask> guard)
         : base(guard)
      {

      }

      public override ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent) =>
         new ExecutionFailure(this, executor, parent);
   }
   
   class ModelPerformInterruption : ModelLeaf
   {
      public ModelPerformInterruption(Option<ModelTask> guard, ModelInterrupter interrupter, Status desiredResult)
         : base(guard)
      {
         Interrupter = interrupter;
         DesiredResult = desiredResult;
      }

      public ModelInterrupter Interrupter { get; set; }
      public Status DesiredResult { get; }

      public override ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent) =>
         new ExecutionPerformInterruption(this, executor, parent);
   }

   class ModelSubtreeLookup : ModelLeaf
   {
      public ModelSubtreeLookup(Option<ModelTask> guard, string treeName)
         : base(guard)
      {
         TreeName = treeName;
      }

      public string TreeName { get; }

      public override ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent) =>
         new ExecutionSubtreeLookup(this, executor, parent);
   }

   class ModelSuccess : ModelLeaf
   {
      public ModelSuccess(Option<ModelTask> guard)
         : base(guard)
      {

      }

      public override ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent) =>
         new ExecutionSuccess(this, executor, parent);
   }
   
   class ModelVariableRenamer : ModelLeaf
   {
      public ModelVariableRenamer(Option<ModelTask> guard, string variableName, string newVariableName)
         : base(guard)
      {
         VariableName = variableName;
         NewVariableName = newVariableName;
      }

      public string VariableName { get; }
      public string NewVariableName { get; }

      public override ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent) =>
         new ExecutionVariableRenamer(this, executor, parent);
   }

   class ModelWait : ModelLeaf
   {
      public ModelWait(Option<ModelTask> guard, int duration)
         : base(guard)
      {
         Duration = duration;
      }

      public int Duration { get; }

      public override ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent) =>
         new ExecutionWait(this, executor, parent);
   }
   
   class ModelToFill : ModelLeaf
   {
      public ModelToFill(Option<ModelTask> guard)
         : base(guard)
      {

      }

      public override ExecutionTask CreateExecuter(BTExecuter executor, ExecutionTask parent) =>
         new ExecutionToFill(this, executor, parent);
   }
   
   abstract class ModelAction : ModelLeaf
   {
      public ModelAction(Option<ModelTask> guard)
         : base(guard)
      {
      }
   }

   abstract class ModelCondition : ModelLeaf
   {
      public ModelCondition(Option<ModelTask> guard)
         : base(guard)
      {
      }
   }
}
