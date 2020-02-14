using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameUserDB
{
    public class ButtonProp
    {
        public bool Disabled { get; set; }

        public ButtonProp(bool disabled = true)
        {
            Disabled = disabled;
        }
    }
}
