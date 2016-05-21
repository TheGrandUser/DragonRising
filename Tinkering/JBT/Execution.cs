using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using System.Collections;
using System.Collections.Immutable;
using System.Reactive.Subjects;

// source translated from https://github.com/gaia-ucm/jbt

namespace Tinkering.JBT
{
   interface IContext
   {
      Option<object> GetVariable(string name);
      bool SetVariable(string name, object value);

      void Clear();
      bool ClearVariable(string name);

      ModelTask GetBT(string name);
   }

   interface IBTLibrary : IEnumerable<KeyValuePair<string, ModelTask>>
   {
      ModelTask GetBT(string name);
   }

   class GenericBTLibrary : IBTLibrary
   {
      IDictionary<string, ModelTask> trees = new Dictionary<string, ModelTask>();

      public ModelTask GetBT(string name) => trees[name];

      public bool AddBTLibrary(IBTLibrary library)
      {
         bool overwritten = false;
         foreach (var tree in library)
         {
            if (this.trees.ContainsKey(tree.Key)) { overwritten = true; }
            trees[tree.Key] = tree.Value;
         }
         return overwritten;
      }

      public bool AddBT(string name, ModelTask tree)
      {
         if (trees.ContainsKey(name))
         {
            trees[name] = tree;
            return true;
         }
         trees[name] = tree;
         return false;
      }

      public IEnumerator<KeyValuePair<string, ModelTask>> GetEnumerator() => trees.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
   }

   static class BTLibraryFactory
   {
      public static IBTLibrary CreateBTLibrary(List<IBTLibrary> libraries)
      {
         var result = new GenericBTLibrary();

         foreach (var library in libraries)
         {
            result.AddBTLibrary(library);
         }

         return result;
      }

      public static IBTLibrary CreateLibrary(List<Tuple<ModelTask, string>> behaviorTrees)
      {
         var result = new GenericBTLibrary();

         foreach (var tuple in behaviorTrees)
         {
            result.AddBT(tuple.Item2, tuple.Item1);
         }

         return result;
      }

   }

   interface IBTExecuter
   {
      void Tick();
      void Terminate();
      ModelTask BehaviorTree { get; }
      Status Status { get; }
      IContext RootContext { get; }
   }

   interface ITaskState
   {
      Option<object> GetStateVariable(string name);
   }

   abstract class ExecutionTask
   {
      BTExecuter executor;
      IContext context;

      //listeners/observable
      Subject<TaskEvent> listeners = new Subject<TaskEvent>();

      bool spawnable = true;
      bool tickable = false;
      bool terminated = false;
      ExecutionTask parent;
      Position position;

      public ExecutionTask(ModelTask modelTask, BTExecuter executor, ExecutionTask parent)
      {
         this.ModelTask = modelTask;
         this.executor = executor;
         this.parent = parent;

         position = parent?.position.AddMove(GetMove()) ?? new Position();
      }
      public ModelTask ModelTask { get; }
      public Status Status { get; private set; } = Status.Uninitialized;
      public Position Position => position;

      public void Spawn(IContext context)
      {
         if (!spawnable) { throw new SpawnException("The task cannot be spawned. It has already been spawned."); }
         this.context = context;
         this.spawnable = false;
         this.tickable = true;
         this.Status = Status.Running;
         this.executor.RequestInsertionIntoList(BTExecutorList.Open, this);
         var prevState = this.executor.GetTaskState(Position);
         RestoreState(prevState);
         Spawn();
      }

      protected abstract void Spawn();

      private void RestoreState(Option<ITaskState> prevState)
      {
         throw new NotImplementedException();
      }

      public void Tick()
      {
         throw new NotImplementedException();
      }

      public void Terminate()
      {
         throw new NotImplementedException();
      }

      private int GetMove()
      {
         for (int i = 0; i < parent.ModelTask.Children.Count; i++)
         {
            if (parent.ModelTask.Children[i] == ModelTask)
            {
               return i;
            }
         }
         return 0;
      }
   }

   class TaskEvent
   {
      public TaskEvent(Status newStatus, Status previousStatus)
      {
         NewStatus = newStatus;
         PreviousStatus = previousStatus;
      }

      public Status NewStatus { get; }
      public Status PreviousStatus { get; }
   }

   class BasicContext : IContext
   {
      IDictionary<string, object> variables = new Dictionary<string, object>();
      GenericBTLibrary library = new GenericBTLibrary();

      public void Clear() => variables.Clear();

      public bool ClearVariable(string name) => variables.Remove(name);

      public ModelTask GetBT(string name) => library.GetBT(name);

      public virtual Option<object> GetVariable(string name) => variables.TryGetValue(name);

      public bool SetVariable(string name, object value)
      {
         if (value == null)
         {
            return variables.Remove(name);
         }
         variables[name] = value; return true;
      }

      public bool AddBTLibrary(IBTLibrary library) => this.library.AddBTLibrary(library);
      public bool AddBT(string name, ModelTask tree) => this.library.AddBT(name, tree);
   }

   class HierarchicalContext : BasicContext
   {
      public HierarchicalContext()
      {
      }

      public IContext Parent { get; set; }
      public override Option<object> GetVariable(string name)
      {
         var result = base.GetVariable(name);

         return result.Match(
            o => Some(o),
            () => Parent?.GetVariable(name) ?? None);
      }
   }

   class SafeContext : IContext
   {
      IContext inputContext;
      bool cleared = false;
      IDictionary<string, object> localVariables = new Dictionary<string, object>();
      HashSet<string> localModifiedVariables = new HashSet<string>();

      public SafeContext(IContext inputContext)
      {
         this.inputContext = inputContext;
      }

      public Option<object> GetVariable(string name)
      {
         if (localModifiedVariables.Contains(name) || cleared)
         {
            return localVariables.TryGetValue(name);
         }
         else
         {
            return localVariables.TryGetValue(name).Match(
               o => Some(o),
               () => inputContext.GetVariable(name));
         }
      }

      public bool SetVariable(string name, object value)
      {
         localModifiedVariables.Add(name);
         if (value == null)
         {
            return localVariables.Remove(name);
         }
         else
         {
            localVariables[name] = value;
            return true;
         }
      }

      public void Clear()
      {
         localVariables.Clear();
         cleared = true;
      }

      public bool ClearVariable(string name)
      {
         localModifiedVariables.Add(name);
         return localVariables.Remove(name);
      }

      public ModelTask GetBT(string name) => inputContext.GetBT(name);
   }

   class SafeOutputContext : IContext
   {
      IContext inputContext;
      bool cleared = false;
      IDictionary<string, object> localVariables = new Dictionary<string, object>();
      HashSet<string> localModifiedVariables = new HashSet<string>();
      ImmutableHashSet<string> outputVariables;

      public SafeOutputContext(IContext inputContext, IEnumerable<string> outputVariables)
      {
         this.inputContext = inputContext;
         this.outputVariables = outputVariables.ToImmutableHashSet();
      }

      public Option<object> GetVariable(string name)
      {
         if (this.outputVariables.Contains(name))
         {
            return inputContext.GetVariable(name);
         }
         else if (localModifiedVariables.Contains(name) || cleared)
         {
            return localVariables.TryGetValue(name);
         }
         else
         {
            return localVariables.TryGetValue(name).Match(
               o => Some(o),
               () => inputContext.GetVariable(name));
         }
      }

      public bool SetVariable(string name, object value)
      {
         if (this.outputVariables.Contains(name))
         {
            return this.inputContext.SetVariable(name, value);
         }
         else
         {
            localModifiedVariables.Add(name);
            if (value == null)
            {
               return localVariables.Remove(name);
            }
            else
            {
               localVariables[name] = value;
               return true;
            }
         }
      }

      public void Clear()
      {
         localVariables.Clear();
         foreach (var outputVar in outputVariables)
         {
            inputContext.ClearVariable(outputVar);
         }
         cleared = true;
      }

      public bool ClearVariable(string name)
      {
         if (outputVariables.Contains(name))
         {
            return inputContext.ClearVariable(name);
         }
         else
         {
            localModifiedVariables.Add(name);
            return localVariables.Remove(name);
         }
      }

      public ModelTask GetBT(string name) => inputContext.GetBT(name);
   }

   static class ContextFactory
   {
      public static IContext CreateContext(IBTLibrary library)
      {
         var result = new BasicContext();
         result.AddBTLibrary(library);
         return result;
      }

      public static IContext CreateContext(IEnumerable<IBTLibrary> libraries)
      {
         var result = new BasicContext();
         foreach (var library in libraries)
         {
            result.AddBTLibrary(library);
         }
         return result;
      }

      public static IContext CreateContext(List<Tuple<ModelTask, string>> behaviourTrees)
      {
         BasicContext result = new BasicContext();

         foreach (var tuple in behaviourTrees)
         {
            result.AddBT(tuple.Item2, tuple.Item1);
         }

         return result;
      }
      public static IContext createContext() => new BasicContext();
   }

   enum BTExecutorList
   {
      Open,
      Tickable
   }

   class BTExecuter : IBTExecuter
   {
      ExecutionTask executionBT;
      List<ExecutionTask> tickableTasks = new List<ExecutionTask>();
      List<ExecutionTask> openTasks = new List<ExecutionTask>();
      bool firstTimeTicked = true;

      List<ExecutionTask> currentTickableInsertions = new List<ExecutionTask>();
      List<ExecutionTask> currentTickableRemovals = new List<ExecutionTask>();
      List<ExecutionTask> currentOpenInsertions = new List<ExecutionTask>();
      List<ExecutionTask> currentOpenRemovals = new List<ExecutionTask>();

      IDictionary<ModelInterrupter, ExecutionInterrupter> interrupters = new Dictionary<ModelInterrupter, ExecutionInterrupter>();
      IDictionary<Position, ITaskState> tasksStates = new Dictionary<Position, ITaskState>();

      public BTExecuter(Some<ModelTask> modelBT, IContext context)
      {
         this.BehaviorTree = modelBT;
         this.BehaviorTree.ComputePositions();
         this.RootContext = context ?? new BasicContext();

      }

      public void Tick()
      {
         var currentStatus = Status;
         if (currentStatus == Status.Running || currentStatus == Status.Uninitialized)
         {
            ProcessInsertionsAndRemovals();
            if (this.firstTimeTicked)
            {
               this.executionBT = this.BehaviorTree.CreateExecuter(this, null);
               this.executionBT.Spawn(this.RootContext);
               this.firstTimeTicked = false;
            }
            else
            {
               foreach (var t in tickableTasks)
               {
                  t.Tick();
               }
            }
            ProcessInsertionsAndRemovals();
         }
      }

      public void Terminate()
      {
         executionBT?.Terminate();
      }

      public Option<ExecutionInterrupter> GetExecutionInterrupter(ModelInterrupter modelInterrupter) =>
         this.interrupters.TryGetValue(modelInterrupter);

      public void RegisterInterrupter(ExecutionInterrupter interrupter) => this.interrupters[interrupter.ModelTask] = interrupter;
      public void UnregisterInterrupter(ExecutionInterrupter interrupter) => this.interrupters.Remove(interrupter.ModelTask);

      public void RequestInsertionIntoList(BTExecutorList listType, ExecutionTask t)
      {
         if (listType == BTExecutorList.Open)
         {
            if (!this.currentOpenInsertions.Contains(t)) { this.currentOpenInsertions.Add(t); }
         }
         else
         {
            if (!this.currentTickableInsertions.Contains(t)) { this.currentTickableInsertions.Add(t); }
         }
      }
      public void RequestRemovalFromList(BTExecutorList listType, ExecutionTask t)
      {
         if (listType == BTExecutorList.Open)
         {
            if (!this.currentOpenRemovals.Contains(t)) { this.currentOpenRemovals.Add(t); }
         }
         else
         {
            if (!this.currentTickableRemovals.Contains(t)) { this.currentTickableRemovals.Add(t); }
         }
      }
      public void CancelInsertionRequest(BTExecutorList listType, ExecutionTask t)
      {
         if (listType == BTExecutorList.Open)
         {
            currentOpenInsertions.Remove(t);
         }
         else
         {
            currentTickableInsertions.Remove(t);
         }
      }
      public void CancelRemovalRequest(BTExecutorList listType, ExecutionTask t)
      {
         if (listType == BTExecutorList.Open)
         {
            currentOpenRemovals.Remove(t);
         }
         else
         {
            currentTickableRemovals.Remove(t);
         }
      }

      private void ProcessInsertionsAndRemovals()
      {
         tickableTasks.AddRange(this.currentTickableInsertions);
         foreach (var t in this.currentTickableRemovals) { tickableTasks.Remove(t); }

         openTasks.AddRange(this.currentOpenInsertions);
         foreach (var t in this.currentOpenRemovals) { openTasks.Remove(t); }

         currentOpenInsertions.Clear();
         currentOpenRemovals.Clear();
         currentTickableInsertions.Clear();
         currentTickableRemovals.Clear();
      }

      public ModelTask BehaviorTree { get; }

      public Status Status => executionBT?.Status ?? Status.Uninitialized;

      public IContext RootContext { get; }

      public bool SetTaskState(Position taskPosition, Option<ITaskState> state)
      {
         return state.Match(
            Some: ts =>
            {
               if (!this.tasksStates.ContainsKey(taskPosition))
               {
                  this.tasksStates.Add(taskPosition, ts);
                  return true;
               }
               return false;
            },
            None: () => this.tasksStates.Remove(taskPosition));
      }

      public Option<ITaskState> GetTaskState(Position taskPosition) => this.tasksStates.TryGetValue(taskPosition);
      public void ShareTaskStatesFrom(BTExecuter executer) => this.tasksStates = executer.tasksStates;
      public bool ClearTaskSate(Position taskPosition) => tasksStates.Remove(taskPosition);

      public override string ToString() => $"[Root: {this.BehaviorTree.GetType().Name}, Status: {Status}]";
   }

   static class BTExecuterFactory
   {
      public static IBTExecuter CreateBTExecuter(Some<ModelTask> treeToRun, IContext context) => new BTExecuter(treeToRun, context);
      public static IBTExecuter CreateBTExecuter(Some<ModelTask> treeToRun) => new BTExecuter(treeToRun);
   }

   class TaskState : ITaskState
   {
      IDictionary<string, object> variables = new Dictionary<string, object>();

      public Option<object> GetStateVariable(string name) => variables.TryGetValue(name);
      public bool SetStateVariable(string name, Option<object> value) =>
         value.Match(
            Some: v =>
            {
               if (!variables.ContainsKey(name)) { variables.Add(name, value); return true; }
               return false;
            },
            None: () => variables.Remove(name));
      public void Clear() => variables.Clear();
      public bool ClearStateVariable(string name) => variables.Remove(name);
   }




   class ExecutionParallel : ExecutionTask
   {
      public ExecutionParallel(ModelParallel modelParallel, BTExecuter executor, ExecutionTask parent)
         : base(modelParallel, executor, parent)
      {
      }
   }

   class ExecutionModelRandomSelector : ExecutionTask
   {
      public ExecutionModelRandomSelector(ModelRandomSelector modelTask, BTExecuter executor, ExecutionTask parent) : base(modelTask, executor, parent)
      {
      }
   }

   class ExecutionModelRandomSequence : ExecutionTask
   {
      public ExecutionModelRandomSequence(ModelRandomSequence modelTask, BTExecuter executor, ExecutionTask parent) : base(modelTask, executor, parent)
      {
      }
   }

   class ExecutionDynamicPriorityList : ExecutionTask
   {
      public ExecutionDynamicPriorityList(ModelDynamicPriorityList modelTask, BTExecuter executor, ExecutionTask parent) : base(modelTask, executor, parent)
      {
      }
   }

   class ExecutionModelSelector : ExecutionTask
   {
      public ExecutionModelSelector(ModelSelector modelTask, BTExecuter executor, ExecutionTask parent) : base(modelTask, executor, parent)
      {
      }
   }

   class ExecutionModelSequence : ExecutionTask
   {
      public ExecutionModelSequence(ModelSequence modelTask, BTExecuter executor, ExecutionTask parent) : base(modelTask, executor, parent)
      {
      }
   }

   class ExecutionModelStaticPriorityList : ExecutionTask
   {
      public ExecutionModelStaticPriorityList(ModelStaticPriorityList modelTask, BTExecuter executor, ExecutionTask parent) : base(modelTask, executor, parent)
      {
      }
   }

   class ExecutionModelHierarchicalContextManager : ExecutionTask
   {
      public ExecutionModelHierarchicalContextManager(ModelHierarchicalContextManager modelTask, BTExecuter executor, ExecutionTask parent) : base(modelTask, executor, parent)
      {
      }
   }

   class ExecutionInterrupter : ExecutionTask
   {
      public ExecutionInterrupter(ModelInterrupter modelTask, BTExecuter executor, ExecutionTask parent) : base(modelTask, executor, parent)
      {
      }

      public new ModelInterrupter ModelTask => (ModelInterrupter)base.ModelTask;
   }

   class ExecutionInverter : ExecutionTask
   {
      public ExecutionInverter(ModelInverter modelTask, BTExecuter executor, ExecutionTask parent) : base(modelTask, executor, parent)
      {
      }
   }

   class ExecutionLimit : ExecutionTask
   {
      public ExecutionLimit(ModelLimit modelTask, BTExecuter executor, ExecutionTask parent) : base(modelTask, executor, parent)
      {
      }
   }

   class ExecutionRepeat : ExecutionTask
   {
      public ExecutionRepeat(ModelRepeat modelTask, BTExecuter executor, ExecutionTask parent) : base(modelTask, executor, parent)
      {
      }
   }

   class ExecutionSafeContextManager : ExecutionTask
   {
      public ExecutionSafeContextManager(ModelSafeContextManager modelTask, BTExecuter executor, ExecutionTask parent) : base(modelTask, executor, parent)
      {
      }
   }

   class ExecutionSafeOutputContextManager : ExecutionTask
   {
      public ExecutionSafeOutputContextManager(ModelSafeOutputContextManager modelTask, BTExecuter executor, ExecutionTask parent) : base(modelTask, executor, parent)
      {
      }
   }


   ///
   class ExecutionSucceeder : ExecutionTask
   {
      public ExecutionSucceeder(ModelSucceeder modelTask, BTExecuter executor, ExecutionTask parent) : base(modelTask, executor, parent)
      {
      }
   }
   class ExecutionUntilFail : ExecutionTask
   {
      public ExecutionUntilFail(ModelUntilFail modelTask, BTExecuter executor, ExecutionTask parent) : base(modelTask, executor, parent)
      {
      }
   }
   class ExecutionFailure : ExecutionTask
   {
      public ExecutionFailure(ModelFailure modelTask, BTExecuter executor, ExecutionTask parent) : base(modelTask, executor, parent)
      {
      }
   }

   class ExecutionPerformInterruption : ExecutionTask
   {
      public ExecutionPerformInterruption(ModelPerformInterruption modelTask, BTExecuter executor, ExecutionTask parent) : base(modelTask, executor, parent)
      {
      }
   }
   class ExecutionSubtreeLookup : ExecutionTask
   {
      public ExecutionSubtreeLookup(ModelSubtreeLookup modelTask, BTExecuter executor, ExecutionTask parent) : base(modelTask, executor, parent)
      {
      }
   }
   class ExecutionSuccess : ExecutionTask
   {
      public ExecutionSuccess(ModelSuccess modelTask, BTExecuter executor, ExecutionTask parent) : base(modelTask, executor, parent)
      {
      }
   }
   class ExecutionVariableRenamer : ExecutionTask
   {
      public ExecutionVariableRenamer(ModelVariableRenamer modelTask, BTExecuter executor, ExecutionTask parent) : base(modelTask, executor, parent)
      {
      }
   }
   class ExecutionWait : ExecutionTask
   {
      public ExecutionWait(ModelWait modelTask, BTExecuter executor, ExecutionTask parent) : base(modelTask, executor, parent)
      {
      }
   }

   class ExecutionToFill : ExecutionTask
   {
      public ExecutionToFill(ModelToFill modelTask, BTExecuter executor, ExecutionTask parent)
         : base(modelTask, executor, parent)
      {
      }
   }
}
