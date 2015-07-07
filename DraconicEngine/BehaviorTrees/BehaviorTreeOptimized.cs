using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.BehaviorTrees.bt3
{
    public enum Status
    {
        Invalid,
        Success,
        Failure,
        Running,
    }

    public abstract class Behavior
    {
        protected Status status = Status.Invalid;

        public Behavior()
        {

        }

        public Status Tick()
        {
            if (status == Status.Invalid)
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

        protected virtual void OnInitialize() { }

        protected abstract Status Update();

        protected virtual void OnTerminate(Status status) { }
    }

    public class BehaviorTree
    {

    }
}
