using Expressive.Elements;

namespace Expressive.Pipelines.Steps.IndividualElements.Support {
    public interface IElementInterpretation {
        void Initialize(InterpretationContext context);
        bool CanInterpret(IElement element);
        IElement Interpret(IElement element, IndividualInterpretationContext context);
    }
}