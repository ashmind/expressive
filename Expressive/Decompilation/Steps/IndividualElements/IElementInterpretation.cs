using Expressive.Elements;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public interface IElementInterpretation {
        void Initialize(DecompilationContext context);
        bool CanInterpret(IElement element);
        IElement Interpret(IElement element, IndividualDecompilationContext context);
    }
}