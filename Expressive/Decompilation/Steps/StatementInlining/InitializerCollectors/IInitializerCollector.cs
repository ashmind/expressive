using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps.StatementInlining.InitializerCollectors {
    public interface IInitializerCollector {
        bool MatchNew(Expression expression);
        Expression AttemptToCollect(Expression @new, int variableIndex, int elementIndex, IList<IElement> elements);
    }
}