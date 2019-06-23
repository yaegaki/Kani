using System.Collections.Generic;
using dnlib.DotNet;
using Kani.Models;

namespace Kani.Decompile
{
    public static class DecompileFormatUtil
    {
        public static void AddTexts(TypeDef type, IList<TextRun> target)
        {
            AddTexts(type.ToTypeSig(), target);
            var isFirst = true;
            foreach (var param in type.GenericParameters)
            {
                if (isFirst)
                {
                    target.Add(new TextRun("<"));
                    isFirst = false;
                }
                else
                {
                    target.Add(new TextRun(", "));
                }

                target.Add(new TextRun(param.Name.String, "d-typegenericparameter"));
            }

            if (!isFirst)
            {
                target.Add(new TextRun(">"));
            }
        }

        public static void AddTexts(TypeSig type, IList<TextRun> target)
        {
            if (type.IsSZArray)
            {
                if (type.Next != null)
                {
                    AddTexts(type.Next, target);
                    target.Add(new TextRun("[]"));
                    return;
                }
            }
            else if (type.IsArray)
            {
                if (type.Next != null)
                {
                    AddTexts(type.Next, target);
                    target.Add(new TextRun("["));
                    var arraySig = type.ToArraySig();
                    for (var i = 1; i < arraySig.Rank; i++)
                    {
                        target.Add(new TextRun(","));
                    }
                    target.Add(new TextRun("]"));
                    return;
                }
            }

            string text;
            string cssClass;

            var keyword = TypeSigAsKeyword(type);
            if (!string.IsNullOrEmpty(keyword))
            {
                text = keyword;
                cssClass = "d-keyword";
            }
            else
            {
                text = type.TypeName;
                // Remove generic parameter count
                // ex) List`1 => List
                var generic = text.LastIndexOf('`');
                if (generic > 0)
                {
                    text = text.Substring(0, generic);
                }

                switch (type.ElementType)
                {
                    case ElementType.ValueType:
                        cssClass = "d-valuetype";
                        break;
                    case ElementType.Class:
                        var classSig = type.ToClassSig();
                        if (classSig.IsTypeDef)
                        {
                            var typeDef = classSig.TypeDef;
                            if (typeDef.IsInterface)
                            {
                                cssClass = "d-interface";
                            }
                            else if (typeDef.IsSealed)
                            {
                                cssClass = "d-sealedtype";
                            }
                            else
                            {
                                cssClass = "d-type";
                            }
                        }
                        else
                        {
                            cssClass = "d-type";
                        }
                        break;
                    case ElementType.GenericInst:
                        var genericInstSig = type.ToGenericInstSig();
                        if (genericInstSig.GenericType.IsTypeDef)
                        {
                            var genericTypeSig = genericInstSig.GenericType.TypeDef.ToTypeSig();
                            AddTexts(genericTypeSig, target);
                        }
                        else
                        {
                            target.Add(new TextRun(text, "d-type"));
                        }
                        target.Add(new TextRun("<"));
                        var _isFirst = true;
                        foreach (var genericArgType in genericInstSig.GenericArguments)
                        {
                            if (_isFirst)
                            {
                                _isFirst = false;
                            }
                            else
                            {
                                target.Add(new TextRun(", "));
                            }
                            AddTexts(genericArgType, target);
                        }
                        target.Add(new TextRun(">"));
                        return;
                    case ElementType.I:
                    case ElementType.U:
                        cssClass = "d-valuetype";
                        break;
                    case ElementType.Var:
                        cssClass = "d-typegenericparameter";
                        break;
                    case ElementType.MVar:
                        cssClass = "d-methodgenericparameter";
                        break;
                    default:
                        cssClass = string.Empty;
                        break;
                }
            }

            target.Add(new TextRun(text, cssClass));
        }

        private static string TypeSigAsKeyword(TypeSig type)
        {
            switch (type.ElementType)
            {
                case ElementType.Void: return "void";
                case ElementType.Boolean: return "bool";
                case ElementType.Char: return "char";
                case ElementType.I1: return "sbyte";
                case ElementType.U1: return "byte";
                case ElementType.I2: return "short";
                case ElementType.U2: return "ushort";
                case ElementType.I4: return "int";
                case ElementType.U4: return "uint";
                case ElementType.I8: return "long";
                case ElementType.U8: return "ulong";
                case ElementType.R4: return "float";
                case ElementType.R8: return "double";
                case ElementType.String: return "string";
                case ElementType.Object: return "object";
                case ElementType.ValueType:
                    if (type.FullName == "System.Decimal")
                    {
                        return "decimal";
                    }
                    return string.Empty;
                default: return string.Empty;
            }
        }
    }
}