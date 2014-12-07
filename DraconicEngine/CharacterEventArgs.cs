using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine
{
    public class CharacterEventArgs : EventArgs
    {
        public Loc Position { get { return mPos; } }
        public Character Character { get { return mCharacter; } }
        public int X { get { return mPos.X; } }
        public int Y { get { return mPos.Y; } }

        public CharacterEventArgs(Character character, Loc pos)
        {
            mCharacter = character;
            mPos = pos;
        }

        private Character mCharacter;
        private Loc mPos;
    }
}
