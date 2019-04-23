using System;
using System.Collections;
namespace Mono.CompilerServices.SymbolWriter
{
	internal class NamespaceInfo
	{
		public string Name = null;
		public int NamespaceID = 0;
		public ArrayList UsingClauses = new ArrayList();
	}
}
