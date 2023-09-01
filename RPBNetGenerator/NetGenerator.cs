﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace RPBNetGenerator
{
    [Generator]
    public class NetGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
//#if DEBUG
//            if (!Debugger.IsAttached)
//            {
//                Debugger.Launch();
//            }

//#endif
            var packets = ReflectiveEnumerator.GetEnumerableOfType(context).ToList();

            _generatePacketRegister(packets, context);
            //_generateInitialize(context);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }
        private static void _generatePacketRegister(IEnumerable<INamedTypeSymbol> packets, GeneratorExecutionContext context)
        {
            var registerBuilder = new StringBuilder();
            var namespaces = new List<INamespaceSymbol>();
            foreach (var packet in packets)
            {
                if(!namespaces.Contains(packet.ContainingNamespace))
                    namespaces.Add(packet.ContainingNamespace);
                registerBuilder.AppendLine($"PacketParser.AddPacketFactory<{packet.Name}>();");
            }

            var nameSpaceBuilder = new StringBuilder();
            foreach (var namespaceSymbol in namespaces)
            {
                nameSpaceBuilder.AppendLine($"using {namespaceSymbol};");
            }
            var classString = $@"  
using RPBNet.NetworkBase.Server;
{nameSpaceBuilder}
namespace RPBNet.Packets
{{
internal static partial class PacketRegister
        {{
            static partial void _initialize()
            {{
                {registerBuilder}
            }}
        }}
}}";
            context.AddSource($"PacketRegister.g.cs", classString);
        }

//        private static void _generateInitialize(GeneratorExecutionContext context)
//        {
//            // Find the main method
//            var mainMethod = context.Compilation.GetEntryPoint(context.CancellationToken);

//            // Build up the source code
//            string source = $@"// <auto-generated/>
//using System;
//using RPBTestServer.Packets;

//namespace {mainMethod.ContainingNamespace.ToDisplayString()}
//{{
//    internal static partial class {mainMethod.ContainingType.Name}
//    {{
//        static partial void Initialize() =>
//            PacketRegister.Initialize();
//    }}
//}}
//";
//            var typeName = mainMethod.ContainingType.Name;

//            // Add the source code to the compilation
//            context.AddSource($"{typeName}.g.cs", source);
//        }
    }


}