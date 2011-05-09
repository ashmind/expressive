using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Expressive.Pipeline {
    [Serializable]
    public class InterpretationException : Exception {
        public InterpretationException() {
        }

        public InterpretationException(string message) : base(message) {
        }

        public InterpretationException(string message, Exception inner) : base(message, inner) {
        }

        protected InterpretationException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}
