using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace AdvancedProperties.Analyzer;

[Generator]
public class PropertyContextCreator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is AdvancedPropertyRegisterReceiver receiver)
        {
            var compilation = context.Compilation;
            var ownersProperties = receiver.CandidateFields.ToLookup(x => x.TOwner);


            foreach (var ownerProperties in ownersProperties)
            {
                var semanticModel = compilation.GetSemanticModel(ownerProperties.Key.SyntaxTree);
                var generatedCode = GeneratePropertyContextesForType(ownerProperties.Key, ownerProperties, semanticModel);

                context.AddSource($"{ownerProperties.Key}.aprop.g.cs", SourceText.From(generatedCode, Encoding.UTF8));
            }
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
#if DEBUG
        /*if (!Debugger.IsAttached) Debugger.Launch();*/
#endif
        context.RegisterForSyntaxNotifications(() => new AdvancedPropertyRegisterReceiver());
    }

    private static string GeneratePropertyContextesForType(TypeSyntax type, IEnumerable<APropRegisterInfo> registerInfos,SemanticModel semanticModel)
    {
        var contextDeclations = new StringBuilder();
        var contextInitializations = new StringBuilder();

        var ownerTypeInfo = semanticModel.GetTypeInfo(type);

        foreach (var registerInfo in registerInfos)
        {
            contextDeclations.AppendLine(GeneratePropertyContext(registerInfo, semanticModel));

            contextInitializations.AppendLine($"{registerInfo.PropertyName}Context = new({registerInfo.PropertyName}, this);");
        }
        

        return $@"
#nullable enable
using System;
using AdvancedProperties;

namespace {ownerTypeInfo.Type!.ContainingNamespace};

partial class {type}
{{
    {contextDeclations}

    private void Init()
    {{
        {contextInitializations}
    }}
}}
";
    }

    private static string GeneratePropertyContext(APropRegisterInfo registerInfo, SemanticModel semanticModel)
    {
        var valueTypeInfo = semanticModel.GetTypeInfo(registerInfo.TValue);
        var fullNameValueType = valueTypeInfo.Type.ToString();

        if (registerInfo.TValue is NullableTypeSyntax)
        {
            fullNameValueType += '?';
        }

        return $@"
    protected AdvancedPropertyContext<{registerInfo.TOwner},{fullNameValueType}> {registerInfo.PropertyName}Context
    {{
        get; private set;
    }} = null!;

    public {fullNameValueType} Get{registerInfo.PropertyName}()
    {{
        return {registerInfo.PropertyName}Context.Value;
    }}

    public void Set{registerInfo.PropertyName}({fullNameValueType} value)
    {{
        {registerInfo.PropertyName}Context.SetValue(value);
    }}

";
    }
}
