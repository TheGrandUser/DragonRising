using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tinkering.JBT
{
   abstract class TickException : Exception
   {
      public TickException() { }
      public TickException(string message) : base(message) { }
   }

   class IllegalReturnStatusException : TickException
   {
      public IllegalReturnStatusException() { }
      public IllegalReturnStatusException(string message) : base(message) { }
   }

   class NotTickableException : TickException
   {
      public NotTickableException() { }
      public NotTickableException(string message) : base(message) { }
   }

   class SpawnException : Exception
   {
      public SpawnException() { }
      public SpawnException(string message) : base(message) { }
   }
}
