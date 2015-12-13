#r "..\DragonRising.Game\bin\Debug\DragonRising.Game.dll"
#r "..\DragonRising.Game\bin\Debug\LanguageExt.Core.dll"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonRising.Generators;
using LanguageExt;
using static LanguageExt.Prelude;

var r = new Random();
var speciesPath = @"..\DragonRising.Game\Data\DragonSpecies.json";


//var dragons = SpeciesGenerator.GenerateDragons(r, 10, speciesPath).ToList();