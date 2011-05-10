using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Tests.TestClasses {
    public class ClassWithMagic {
        public static readonly int ManaRequiredForMagic = 1000;
        public int Mana { get; set; }
        public bool IsAllowedToDoMagic { get; set; }

        public bool CanDoMagic {
            get {
                return this.Mana > ManaRequiredForMagic
                    && this.IsAllowedToDoMagic;
            }
        }
    }
}
