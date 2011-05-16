using System;
using System.Collections.Generic;
using System.Linq;

using Expressive.Decompilation.Pipelines;

namespace Expressive {
    public static class ExpressiveEngine {
        static ExpressiveEngine() {
            ExpressiveEngine.GetDisassembler = () => new Disassembler();
            ExpressiveEngine.GetDecompiler = () => new Decompiler(ExpressiveEngine.GetDisassembler(), new DefaultPipeline());
            ExpressiveEngine.GetInliner = () => new Inliner(ExpressiveEngine.GetDecompiler());
        }

        public static Func<IDisassembler> GetDisassembler { get; set; }
        public static Func<IDecompiler> GetDecompiler { get; set; }
        public static Func<IInliner> GetInliner { get; set; }
    }
}
