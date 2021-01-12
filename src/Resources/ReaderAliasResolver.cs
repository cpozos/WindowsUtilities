using System.Collections;
using System.Reflection;

namespace Resources
{
   public sealed class ReaderAliasResolver
   {
      private readonly Hashtable _cachedAliases;

      internal ReaderAliasResolver()
      {
         _cachedAliases = new Hashtable();
      }

      public AssemblyName ResolveAlias(string alias)
      {
         AssemblyName result = null;
         if (_cachedAliases != null)
         {
            result = (AssemblyName)_cachedAliases[alias];
         }

         return result;
      }

      public void PushAlias(string alias, AssemblyName name)
      {
         if (_cachedAliases != null && !string.IsNullOrEmpty(alias))
         {
            _cachedAliases[alias] = name;
         }
      }
   }
}
