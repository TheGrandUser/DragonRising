using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Components.Behaviors.bt4
{
    public enum Status
    {
        Invalid,
        Success,
        Failure,
        Running,
        Suspended
    }

    public delegate void BehaviorObserver(Status status);

    public abstract class Behavior
    {
        public Status status = Status.Invalid;
        public BehaviorObserver observer;

        public Behavior()
        {

        }

        public Status Tick()
        {
            if (this.status == Status.Invalid)
            {
                OnInitialize();
            }

            this.status = Update();

            if (this.status != Status.Running)
            {
                OnTerminate(this.status);
            }

            return this.status;
        }

        public abstract Status Update();
        public virtual void OnInitialize() { }
        public virtual void OnTerminate(Status status) { }


    }

    public class BehaviorTree
    {
        LinkedList<Behavior> behaviors = new LinkedList<Behavior>();

        public void Start(Behavior bh, BehaviorObserver observer = null)
        {
            bh.observer = observer;

            behaviors.AddFirst(bh);
        }

        public void Stop(Behavior bh, Status result)
        {
            System.Diagnostics.Debug.Assert(result != Status.Running);
            bh.status = result;

            if (bh.observer != null)
            {
                bh.observer(result);
            }
        }

        public void Tick()
        {
            behaviors.AddLast((Behavior)null);

            while (Step())
            {
                continue;
            }
        }

        public bool Step()
        {
            var current = behaviors.First.Value;
            behaviors.RemoveFirst();

            if (current == null)
            {
                return false;
            }

            current.Tick();

            if (current.status != Status.Running && current.observer != null)
            {
                current.observer(current.status);
            }
            else
            {
                behaviors.AddLast(current);
            }
            return true;
        }
    }

    public abstract class Composite : Behavior
    {
        public LinkedList<Behavior> children = new LinkedList<Behavior>();
    }

    public class Sequence : Composite
    {
        protected BehaviorTree behaviorTree;
        protected LinkedListNode<Behavior> current;

        public Sequence(BehaviorTree bt)
        {
            this.behaviorTree = bt;
        }

        public override void OnInitialize()
        {
            this.current = this.children.First;
            BehaviorObserver observer = this.OnChildComplete;
            this.behaviorTree.Start(current.Value, observer);
        }

        protected void OnChildComplete(Status status)
        {
            var child = this.current.Value;
            if (child.status == Status.Failure)
            {
                this.behaviorTree.Stop(this, Status.Failure);
                return;
            }

            System.Diagnostics.Debug.Assert(child.status == Status.Success);
            if ((current = current.Next) == this.children.Last)
            {
                this.behaviorTree.Stop(this, Status.Success);
            }
            else
            {
                BehaviorObserver observer = this.OnChildComplete;
                this.behaviorTree.Start(this.current.Value, observer);
            }
        }

        public override Status Update()
        {
            return Status.Running;
        }
    }
}
