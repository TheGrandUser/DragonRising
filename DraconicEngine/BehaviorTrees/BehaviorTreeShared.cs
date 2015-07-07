using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DraconicEngine.BehaviorTrees.bt2
{
    public enum Status
    {
        Invalid,
        Success,
        Failure,
        Running,
    }

    public abstract class Node
    {
        public abstract Task Create();
        public abstract void Destroy(Task task);
    }

    public abstract class Task
    {
        protected Node node;

        public Task(Node node)
        {
            this.node = node;
        }

        public abstract Status Update();
        public virtual void OnInitialize() { }
        public virtual void OnTerminate(Status status) { }
    }

    public class Behavior : IDisposable
    {
        protected Task task = null;
        protected Node node = null;
        protected Status status = Status.Invalid;

        public Behavior() { }

        public Behavior(Node node)
        {
            Setup(node);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.status = Status.Invalid;
                Teardown();
            }
        }

        public void Setup(Node node)
        {
            Teardown();
            this.node = node;
            this.task = this.node.Create();
        }

        public void Teardown()
        {
            if (this.task == null)
            {
                return;
            }

            System.Diagnostics.Debug.Assert(this.status != Status.Running);
            node.Destroy(this.task);
            this.task = null;
        }

        public Status Tick()
        {
            if (this.status == Status.Invalid)
            {
                this.task.OnInitialize();
            }

            this.status = this.task.Update();

            if (this.status != Status.Running)
            {
                this.task.OnTerminate(this.status);
            }

            return this.status;
        }

        public TTask GetTask<TTask>()
           where TTask : Task
        {
            return this.task as TTask;
        }
    }

    public abstract class Composite : Node
    {
        List<Node> children = new List<Node>();
        public List<Node> Children { get { return children; } }
    }

    public class Sequence : Task
    {
        protected Node currentChild;
        protected Behavior currentBehavior;
        protected int currentIndex;

        public Sequence(Composite node)
           : base(node)
        {
        }

        public Composite Node { get { return (Composite)this.node; } }

        public override void OnInitialize()
        {
            this.currentIndex = this.Node.Children.Count - 1;
            this.currentChild = this.Node.Children.FirstOrDefault();
            this.currentBehavior.Setup(this.currentChild);
        }

        public override Status Update()
        {
            while (true)
            {
                var s = this.currentBehavior.Tick();
                if (s != Status.Success)
                {
                    return s;
                }
                if (++currentIndex == this.Node.Children.Count)
                {
                    return Status.Success;
                }
                this.currentChild = this.Node.Children[currentIndex];
                this.currentBehavior.Setup(this.currentChild);
            }
        }
    }

    public class Selector : Task
    {
        protected Node currentChild;
        protected Behavior currentBehavior;
        protected int currentIndex;

        public Selector(Composite node)
           : base(node)
        {
        }

        public Composite Node { get { return (Composite)this.node; } }

        public override void OnInitialize()
        {
            this.currentIndex = this.Node.Children.Count - 1;
            this.currentChild = this.Node.Children.FirstOrDefault();
            this.currentBehavior.Setup(this.currentChild);
        }

        public override Status Update()
        {
            while (true)
            {
                var s = this.currentBehavior.Tick();
                if (s != Status.Failure)
                {
                    return s;
                }
                if (++currentIndex == this.Node.Children.Count)
                {
                    return Status.Failure;
                }
                this.currentChild = this.Node.Children[currentIndex];
                this.currentBehavior.Setup(this.currentChild);
            }
        }
    }
}
