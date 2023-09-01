using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.CodeAnalysis;

namespace RPBNetGenerator
{
    public static class ReflectiveEnumerator
    {
        private static List<INamedTypeSymbol> _symbols = new List<INamedTypeSymbol>();
        public static IEnumerable<INamedTypeSymbol> GetEnumerableOfType(GeneratorExecutionContext context)
        {
            var rpbCommon = context.Compilation.SourceModule.ReferencedAssemblySymbols.FirstOrDefault((x =>
                x.Name == "SMG1Common"));
            _getAllSymbols(rpbCommon.GlobalNamespace);
            return _symbols.Where(x => x.BaseType?.Name == "RPBPacket").ToList(); ;
        }

        private static void _getAllSymbols(INamespaceSymbol namespaceSymbol)
        {
            _symbols.AddRange(namespaceSymbol.GetTypeMembers());
            foreach (var namespaceMember in namespaceSymbol.GetNamespaceMembers())
            {
                _getAllSymbols(namespaceMember);
            }
        }
    }
}