using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Expressive {
    [Serializable]
    public class DecompilationException : Exception {
        public DecompilationException() {
        }

        public DecompilationException(string message) : base(message) {
        }

        public DecompilationException(string message, Exception inner) : base(message, inner) {
        }

        protected DecompilationException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}
