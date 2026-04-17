// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "IDE0130:Namespace does not match folder structure", Justification = "<Pending>", Scope = "namespace", Target = "~N:Microsoft.OpenApi")]
[assembly: SuppressMessage("ApiDesign", "RS0026:Do not add multiple overloads with optional parameters", Justification = "Consistent with existing SerializeAsJsonAsync and SerializeAsYamlAsync overloads.", Scope = "member", Target = "~M:Microsoft.OpenApi.OpenApiSerializableExtensions.SerializeAsTomlAsync``1(``0,System.IO.Stream,Microsoft.OpenApi.OpenApiSpecVersion,System.Threading.CancellationToken)~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("ApiDesign", "RS0026:Do not add multiple overloads with optional parameters", Justification = "Consistent with existing SerializeAsJsonAsync and SerializeAsYamlAsync overloads.", Scope = "member", Target = "~M:Microsoft.OpenApi.OpenApiSerializableExtensions.SerializeAsTomlAsync``1(``0,Microsoft.OpenApi.OpenApiSpecVersion,System.Threading.CancellationToken)~System.Threading.Tasks.Task{System.String}")]
