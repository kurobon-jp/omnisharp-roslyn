using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Roslyn.CSharp.Services.Refactoring.V2
{
    public class CodeActionOperator
    {
        public async Task<ImmutableArray<CodeActionOperation>> GetOperationsAsync(CodeAction codeAction, CancellationToken cancellationToken)
        {
            if (codeAction is CodeActionWithOptions codeActionWithOptions)
            {
                var options = GetOptions(codeActionWithOptions, cancellationToken);
                if (options != null)
                {
                    var operations = await codeActionWithOptions.GetOperationsAsync(options, cancellationToken);
                    return operations.ToImmutableArray();
                }
            }
            return await codeAction.GetOperationsAsync(cancellationToken);
        }

        protected virtual object GetOptions(CodeActionWithOptions codeActionWithOptions, CancellationToken cancellationToken)
        {
            return null;
        }
    }

    public class GenerateOverrideCodeActionOperator : CodeActionOperator
    {
        private IDictionary<string, object> Params { get; }

        public GenerateOverrideCodeActionOperator(IDictionary<string, object> @params)
        {
            Params = @params;
        }

        protected override object GetOptions(CodeActionWithOptions codeActionWithOptions, CancellationToken cancellationToken)
        {
            var options = codeActionWithOptions.GetOptions(cancellationToken);
            var members = options.GetType().GetField("Members");
            var symbols = members.GetValue(options) as IEnumerable<ISymbol>;
            members.SetValue(options, FilterSymbol(symbols).ToImmutableArray());
            return options;
        }

        private IEnumerable<ISymbol> FilterSymbol(IEnumerable<ISymbol> symbols)
        {
            if (Params != null)
            {
                return symbols.Where(x => GetDisplayText(x).Equals(Params["target"]));
            }
            return symbols;
        }

        private string GetDisplayText(ISymbol symbol)
        {
            string text = string.Empty;
            switch (symbol.Kind)
            {
                case SymbolKind.Method:
                    var method = (IMethodSymbol)symbol;
                    var parameters = string.Join(", ", method.Parameters.Select(x => $"{x.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} {x.Name}"));
                    var typeParameters = string.Join(", ", method.TypeParameters);
                    if (!string.IsNullOrEmpty(typeParameters))
                    {
                        typeParameters = $"<{typeParameters}>";
                    }
                    text = $"{method.Name}{typeParameters}({parameters})";
                    break;
                case SymbolKind.Property:
                    var property = (IPropertySymbol)symbol;
                    text = property.Name;
                    break;
            }
            return text;
        }
    }
}
