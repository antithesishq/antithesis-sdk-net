namespace System.Runtime.CompilerServices;

// This polyfill allows us to use "record" Types in this .NET Standard 2.0 project.
// We've marked the class as internal so that it doesn't collide with the real implementation elsewhere.
internal static class IsExternalInit { }