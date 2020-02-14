using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameUserDB
{
    public class Sequence
    {
        private string NewSequence { get; set; }

        public string Shuffle()
        {
            Random r = new Random();
            for (int i = 0; i < 5; i++)
            {
                NewSequence += r.Next(0, 2).ToString();
            }
            return NewSequence;
        }
    }
}
