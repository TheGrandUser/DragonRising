﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;

namespace DraconicEngine.Generators
{
   public interface IMapGenerator
   {
      Loc MakeMap(Scene scene);
   }
}