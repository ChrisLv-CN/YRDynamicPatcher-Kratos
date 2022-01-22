using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp.Utilities
{

    // Enumerates the cell offsets using the cell spread logic.
    /*
		Enumeration starts at the center. Returned values are to be considered
		offsets, so the center is the origin.

		Offsets are returned in unspecified order, but all cells of CellSpread N
		are guaranteed to be returned before enumerating cells of N+1. This means
		that this class can extend or replace the original CellSpread table, as
		long as a single mechanism is used to complete a CellSpread distance.

		The algorithm does not generate any cell offset not in the requested
		CellSpread range.

		The following properties might change in the future:

		Currently, the implementation matches the output order of the original
		CellSpread table for values less than 11, except for the offsets 2 and 4
		(which are swapped here).

		The enumeration stops at Max not because of technical reasons, but because
		at some point is gets unfeasible to enumerate cell contents like this. A
		CellSpread of 256 contains 175789 cells. Input like 1000 to mean full map
		effect is a bad way to do something, and people should be punished for it.

		\author AlexB
	*/
    public class CellSpreadEnumerator : IEnumerable<CellStruct>, IEnumerator<CellStruct>
    {
        CellStruct current;
        uint spread;
        uint curspread;
        bool hasTwo;
        bool hadTwo;

        public const uint Max = 0x100u;

        public CellStruct Current => current;

        object IEnumerator.Current => Current;

        public void Dispose() { }

        public IEnumerator<CellStruct> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        public void Reset()
        {
            Reset(0);
        }

        public CellSpreadEnumerator(uint spread, uint start = 0u)
        {
            this.spread = spread;

            if (spread > Max)
            {
                this.spread = Max;
            }

            Reset(start);
        }

        public bool HasNext()
        {
            return curspread <= spread;
        }

        public void Reset(uint radius)
        {
            curspread = radius;
            current.X = (short)(radius != 0 ? -1 : 0);
            current.Y = (short)-radius;
            hasTwo = true;
            hadTwo = true;
        }

        public bool MoveNext()
        {
            // already done?
            if (!HasNext())
            {
                return false;
            }

            // center or top-right-most cell finishes this
            // round. move to the start of the next one.
            if ((current == default) || (current.X == 1 && current.Y == (short)curspread))
            {
                Reset(curspread + 1);
                return HasNext();
            }

            // finish this line
            if (hasTwo)
            {
                current.X++;

                // quick way to finish first and last line
                hasTwo = (current.X == 0);
                return true;
            }

            // if we are on the left side, mirror to the
            // right side and restore hasTwo.
            if (current.X < 0)
            {
                current.X = (short)-current.X;
                hasTwo = hadTwo;
                return true;
            }

            // move up to the next line
            current.Y++;

            // map upper part to lower part
            // this is reverted later
            bool changeSign = (current.Y > 0);
            if (changeSign)
            {
                current.Y = (short)-current.Y;
            }

            // for each line up, go two steps left
            current.X = (short)(((short)curspread + current.Y) * -2 - 1);

            int diff = current.X - current.Y;
            hasTwo = diff >= 0;
            hadTwo = hasTwo;

            if (diff == -1)
            {
                // "clip" the one
                current.X++;
            }
            else if (diff < 0)
            {
                // get from other direction
                current.X = (short)(-current.Y / 2 - (short)curspread);
            }

            // revert the sign change
            if (changeSign)
            {
                current.Y = (short)-current.Y;
            }

            return true;
        }
    }
}
