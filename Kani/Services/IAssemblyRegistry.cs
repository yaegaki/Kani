using System;
using System.Threading;
using System.Threading.Tasks;
using dnlib.DotNet;
using Kani.Decompile;

namespace Kani.Services
{
    public interface IAssemblyRegistry
    {
        event Action<AssemblyDef> OnAssemblyRegistered;

        Task<AssemblyDef> RegisterAsync(string assemblyUrl, CancellationToken cancellationToken);
        Task<TypeDef> ResolveAsync(TypeRef typeRef, CancellationToken cancellationToken);
        Task<MethodDef> ResolveAsync(MethodSpec methodSpec, CancellationToken cancellationToken);
        Task<MethodOrFieldDef> ResolveAsync(MemberRef methodSpec, CancellationToken cancellationToken);
    }
}
