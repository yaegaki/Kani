using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using dnlib.DotNet;
using Kani.Decompile;

namespace Kani.Services
{
    public class AssemlbyRegistry : IAssemblyRegistry
    {
        private HttpClient http;
        private Dictionary<string, AssemblyDef> assemblyDict = new Dictionary<string, AssemblyDef>();

        public event Action<AssemblyDef> OnAssemblyRegistered;

        public AssemlbyRegistry(HttpClient http)
            => this.http = http;
        
        public async Task<AssemblyDef> RegisterAsync(string assemblyUrl, CancellationToken cancellationToken)
        {
            var res = await this.http.GetAsync(assemblyUrl, cancellationToken);
            var bin = await res.Content.ReadAsByteArrayAsync();
            cancellationToken.ThrowIfCancellationRequested();
            var assemblyDef = AssemblyDef.Load(bin);
            if (!assemblyDict.ContainsKey(assemblyDef.FullName))
            {
                this.assemblyDict[assemblyDef.FullName] = assemblyDef;
                this.OnAssemblyRegistered?.Invoke(assemblyDef);
            }
            return assemblyDef;
        }


        public async Task<TypeDef> ResolveAsync(TypeRef typeRef, CancellationToken cancellationToken)
        {
            var assemblyDef = await Resolve(typeRef.DefinitionAssembly, cancellationToken);
            return assemblyDef.Find(typeRef);
        }

        public async Task<TypeDef> ResolveAsync(TypeSpec typeSpec, CancellationToken cancellationToken)
        {
            var assemblyDef = await Resolve(typeSpec.DefinitionAssembly, cancellationToken);
            var fullName = typeSpec.FullName;

            // remove generic type parameter
            // ex) System.EmptyArray`1<T> => System.EmptyArray`1
            var generic = fullName.LastIndexOf('<');
            if (generic > 0)
            {
                fullName = fullName.Substring(0, generic);
            }

            return assemblyDef.Find(fullName, false);
        }

        public async Task<TypeDef> ResolveAsync(ITypeDefOrRef typeDefOrRef, CancellationToken cancellationToken)
        {
            if (typeDefOrRef is TypeRef typeRef)
            {
                return await ResolveAsync(typeRef, cancellationToken);
            }
            else if (typeDefOrRef is TypeSpec typeSpec)
            {
                System.Console.WriteLine("isTypeSpec");
                return await ResolveAsync(typeSpec, cancellationToken);
            }
            else if (typeDefOrRef is TypeDef typeDef)
            {
                return typeDef;
            }

            return null;
        }

        public async Task<MethodDef> ResolveAsync(MethodSpec methodSpec, CancellationToken cancellationToken)
        {
            var typeDef = await ResolveAsync(methodSpec.DeclaringType, cancellationToken);
            return typeDef?.FindMethod(methodSpec.Method.Name, methodSpec.Method.MethodSig);
        }

        public async Task<MethodOrFieldDef> ResolveAsync(MemberRef memberRef, CancellationToken cancellationToken)
        {
            var typeDef = await ResolveAsync(memberRef.DeclaringType, cancellationToken);
            if (typeDef == null)
            {
                return null;
            }

            if (memberRef.IsFieldRef)
            {
                var fieldDef = typeDef.FindField(memberRef.Name, memberRef.FieldSig);
                if (fieldDef == null)
                {
                    return null;
                }
                return new MethodOrFieldDef(fieldDef);
            }
            else
            {
                var methodDef = typeDef.FindMethod(memberRef.Name, memberRef.MethodSig);
                if (methodDef == null)
                {
                    return null;
                }
                return new MethodOrFieldDef(methodDef);
            }
        }

        public async Task<AssemblyDef> Resolve(IAssembly assemblyRef, CancellationToken cancellationToken)
        {
            AssemblyDef assemblyDef;
            if (!this.assemblyDict.TryGetValue(assemblyRef.FullName, out assemblyDef))
            {
                try
                {
                    var url = $"/_framework/_bin/{assemblyRef.Name}.dll";
                    assemblyDef = await RegisterAsync(url, cancellationToken);
                }
                catch (Exception e)
                {
                    if (e is OperationCanceledException) throw;
                    return null;
                }
            }

            return assemblyDef;
        }
    }
}
