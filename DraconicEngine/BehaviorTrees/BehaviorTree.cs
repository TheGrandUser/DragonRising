using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.BehaviorTrees.bt1
{
    public enum Status
    {
        Invalid,
        Success,
        Failure,
        Running,
        Aborted,
    }

    public abstract class Behavior
    {
        Status status = Status.Invalid;

        public abstract Status Update();

        public virtual void OnInitialize() { }
        public virtual void OnTerminate(Status status) { }

        public Status Tick()
        {
            if (this.status != Status.Running)
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

        public void Reset()
        {
            this.status = Status.Invalid;
        }

        public void Abort()
        {
            OnTerminate(Status.Aborted);
            this.status = Status.Aborted;
        }

        public bool IsTerminated { get { return this.status == Status.Success || this.status == Status.Failure; } }

        public bool IsRunning { get { return this.status == Status.Running; } }

        public Status Status { get { return this.status; } }
    }

    public abstract class Decorator : Behavior
    {
        protected Behavior child;

        public Decorator(Behavior child)
        {
            this.child = child;
        }
    }

    public class Repeat : Decorator
    {
        int limit;
        int counter;

        public Repeat(Behavior child)
           : base(child)
        { }

        public int Count { get { return limit; } set { this.limit = value; } }

        public override void OnInitialize()
        {
            this.counter = 0;
        }

        public override Status Update()
        {
            while (true)
            {
                this.child.Tick();
                if (child.Status == Status.Running) break;
                if (child.Status == Status.Failure) return Status.Failure;
                if (++counter == limit) return Status.Success;
                child.Reset();
            }

            return Status.Invalid;
        }
    }

    public abstract class Composite : Behavior
    {
        public void AddChild(Behavior child) { children.Add(child); }
        public void RemoveChild(Behavior child) { children.Remove(child); }
        public void ClearChildren() { children.Clear(); }

        private List<Behavior> children = new List<Behavior>();

        protected ICollection<Behavior> Children { get { return children; } }
    }

    public class Sequence : Composite
    {
        public override void OnInitialize()
        {
            enumerator = UpdateHelper();
        }

        public override Status Update()
        {
            while (true)
            {
                if (enumerator.MoveNext())
                {
                    var s = enumerator.Current;

                    if (s != Status.Success)
                    {
                        return s;
                    }
                }
                else
                {
                    return Status.Success;
                }
            }
        }

        IEnumerator<Status> enumerator;

        IEnumerator<Status> UpdateHelper()
        {
            foreach (var child in Children)
            {
                Status s = child.Tick();
                while (s != Status.Success)
                {
                    yield return s;
                    s = child.Tick();
                }
            }
            yield return Status.Success;
        }
    }

    public class Selector : Composite
    {
        public override void OnInitialize()
        {
            current = this.Children.First();
            enumerator = UpdateHelper();
        }

        public override Status Update()
        {
            while (true)
            {
                if (enumerator.MoveNext())
                {
                    var s = enumerator.Current;

                    if (s != Status.Failure)
                    {
                        return s;
                    }
                }
                else
                {
                    return Status.Failure;
                }
            }
        }

        IEnumerator<Status> enumerator;

        protected Behavior current;

        protected virtual IEnumerator<Status> UpdateHelper()
        {
            foreach (var child in Children)
            {
                this.current = child;
                Status s = child.Tick();
                while (s != Status.Failure)
                {
                    yield return s;
                    s = child.Tick();
                }
            }
            yield return Status.Failure;
        }
    }

    public class Parallel : Composite
    {
        public enum Policy { RequireOne, RequireAll }

        protected Policy successPolicy;
        protected Policy failurePolicy;

        public Parallel(Policy forSuccess, Policy forFailure)
        {
            this.successPolicy = forSuccess;
            this.failurePolicy = forFailure;
        }

        public override Status Update()
        {
            int successCount = 0;
            int failureCount = 0;

            foreach (var child in Children)
            {
                if (!child.IsTerminated)
                {
                    child.Tick();
                }

                if (child.Status == Status.Success)
                {
                    ++successCount;
                    if (successPolicy == Policy.RequireOne)
                    {
                        return Status.Success;
                    }
                }

                if (child.Status == Status.Failure)
                {
                    ++failureCount;
                    if (failurePolicy == Policy.RequireOne)
                    {
                        return Status.Failure;
                    }
                }
            }

            if (failurePolicy == Policy.RequireAll && failureCount == Children.Count)
            {
                return Status.Failure;
            }
            if (successPolicy == Policy.RequireAll && successCount == Children.Count)
            {
                return Status.Failure;
            }

            return Status.Running;
        }

        public override void OnTerminate(Status status)
        {
            foreach (var child in this.Children)
            {
                if (child.IsRunning)
                {
                    child.Abort();
                }
            }
        }
    }

    public class ActiveSelector : Selector
    {
        public override void OnInitialize()
        {
            this.current = this.Children.Last();
        }

        public override Status Update()
        {
            var previous = this.current;

            base.OnInitialize();
            var result = base.Update();

            if (previous != this.Children.Last() && this.current != previous)
            {
                previous.OnTerminate(Status.Aborted);
            }

            return result;
        }
    }
}
