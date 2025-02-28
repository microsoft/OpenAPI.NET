//TODO remove this if we ever remove the netstandard2.0 target
#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices {
   using System.ComponentModel;
   /// <summary>
   /// Reserved to be used by the compiler for tracking metadata.
   /// This class should not be used by developers in source code.
   /// </summary>
   [EditorBrowsable(EditorBrowsableState.Never)]
   internal static class IsExternalInit {
   }
}
#endif
