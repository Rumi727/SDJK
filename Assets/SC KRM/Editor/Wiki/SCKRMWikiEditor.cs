using SCKRM.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace SCKRM.Editor
{
    public class SCKRMWikiEditor : EditorWindow
    {
        string selectedAssemblyName = "SC-KRM";
        string selectedNameSpace = "SCKRM";
        string selectedGithubWikiSite = "https://github.com/SimsimhanChobo/SC-KRM-1.0/wiki";
        const string unityWikiSite = "https://docs.unity3d.com/ScriptReference/";
        void OnGUI()
        {
            selectedAssemblyName = EditorGUILayout.TextField("Assembly name to create wiki", selectedAssemblyName);
            selectedNameSpace = EditorGUILayout.TextField("Namespace to create wiki", selectedNameSpace);
            selectedGithubWikiSite = EditorGUILayout.TextField("Github Wiki site to create wiki", selectedGithubWikiSite);

            if (GUILayout.Button("Create a wiki"))
                Initialize();
        }

        void Initialize()
        {
            string path = EditorUtility.OpenFolderPanel("위키 파일을 저장할 폴더 선택", "", "");
            if (string.IsNullOrWhiteSpace(path))
                return;

            if (EditorUtility.DisplayDialog("파일 삭제", $"{selectedNameSpace}. 으로 시작하는 모든 파일을 제거하시갰습니까?", "예", "아니요"))
            {
                string[] files = Directory.GetFiles(path, selectedNameSpace + "*.md");
                for (int i = 0; i < files.Length; i++)
                    File.Delete(files[i]);
            }

            apiList.Clear();
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int assemblysIndex = 0; assemblysIndex < assemblys.Length; assemblysIndex++)
            {
                Assembly assembly = assemblys[assemblysIndex];
                string assemblyName = assembly.GetName().Name;

                if (assemblyName == selectedAssemblyName)
                {
                    Type[] types = assemblys[assemblysIndex].GetTypes();
                    for (int typesIndex = 0; typesIndex < types.Length; typesIndex++)
                    {
                        Type type = types[typesIndex];
                        if (type.IsPublic && type.Namespace != null)
                        {
                            string[] nameSpaces = type.Namespace.Split(".");
                            if (type.Namespace == selectedNameSpace || nameSpaces[0] == selectedNameSpace)
                                WikiTextCreate(assembly, type, path);
                        }
                    }

                    fastString.Clear();
                    string nameSpace = "";
                    int tabSize = 0;
                    for (int i = 0; i < apiList.Count; i++)
                    {
                        Type api = apiList[i];
                        if (nameSpace != api.Namespace)
                        {
                            nameSpace = api.Namespace;

                            string[] split = api.Namespace.Split(".");
                            tabSize = split.Length * 2;
                            fastString.Append("\n" + Tab(tabSize - 2) + "* " + split[split.Length - 1]);
                        }

                        fastString.Append("\n" + Tab(tabSize) + "* " + TypeLinkCreate(assembly, api));
                    }

                    File.WriteAllText(PathTool.Combine(path, "_Sidebar.md"), fastString.ToString());
                    return;
                }
            }
        }

        string Tab(int tabSize)
        {
            string tab = "";
            for (int i = 0; i < tabSize; i++)
                tab += " ";

            return tab;
        }

        readonly List<Type> apiList = new List<Type>();
        FastString fastString = new FastString();
        void WikiTextCreate(Assembly assembly, Type type, string path)
        {
            if (IsIgnore(type))
                return;

            fastString.Clear();

            {
                string[] nameSplit = type.Name.Split("`");
                fastString.Append($"# {nameSplit[0]}");
                if (IsObsolete(type, out string description))
                    fastString.Append($" (사용되지 않음 '{description}')");
            }

            fastString.Append($"\n네임스페이스 - {type.Namespace}  ");
            fastString.Append($"\n엑세스 한정자 - {GetAccessModifier(type)}  ");
            fastString.Append($"\n타입 - {GetTypeName(type)}  ");
            fastString.Append($"\n[식별자 이름](https://docs.microsoft.com/ko-kr/dotnet/csharp/fundamentals/coding-style/identifier-names) - {TypeLinkCreate(assembly, type)}  ");

            if (type.BaseType != null)
                fastString.Append($"\n[상속](https://docs.microsoft.com/ko-kr/dotnet/csharp/fundamentals/object-oriented/inheritance) - {TypeLinkCreate(assembly, type.BaseType)}  ");

            {
                string genericConstraint = "";
                Type[] generics = type.GetGenericArguments();
                for (int i = 0; i < generics.Length; i++)
                {
                    Type generic = generics[i];
                    string text = "where " + generic.Name + " : ";
                    bool isGenericConstraints = false;

                    string parameter = ListGenericParameterAttributes(generic);
                    if (parameter != "")
                        isGenericConstraints = true;

                    text += parameter;

                    Type[] constraints = generic.GetGenericParameterConstraints();
                    for (int j = 0; j < constraints.Length; j++)
                    {
                        Type constraint = constraints[j];
                        if (constraint == typeof(ValueType))
                            continue;

                        if (j == 0 && parameter == "")
                            text += TypeLinkCreate(assembly, constraint);
                        else
                            text += ", " + TypeLinkCreate(assembly, constraint);

                        isGenericConstraints = true;
                    }

                    if (isGenericConstraints)
                        genericConstraint += text + " ";
                }

                if (genericConstraint != "")
                    fastString.Append($"\n[제네릭 제약 조건](https://docs.microsoft.com/ko-kr/dotnet/csharp/fundamentals/types/generics) - {genericConstraint}  ");
            }

            fastString.Append("\n\n## 설명");
            fastString.Append("\n" + GetDescription(type));

            {
                fastString.Append("\n\n## 프로퍼티");

                PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                int removeCount = 0;
                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    PropertyInfo propertyInfo = propertyInfos[i];
                    string accessModifterText = GetAccessModifier(propertyInfo, null, null, out PropertyEventMethod.AccessModifier accessModifter);
                    if (accessModifter == PropertyEventMethod.AccessModifier.Private || IsIgnore(propertyInfo) || IsInheritance(type, propertyInfo))
                    {
                        removeCount++;
                        continue;
                    }

                    fastString.Append($"\n### {propertyInfo.Name}");
                    if (IsObsolete(propertyInfo, out string description))
                        fastString.Append($" (사용되지 않음 '{description}')  ");
                    else
                        fastString.Append("  ");

                    fastString.Append($"\n엑세스 한정자 - {accessModifterText}  ");
                    fastString.Append($"\n타입 - {TypeLinkCreate(assembly, propertyInfo.PropertyType)}  ");

                    if (propertyInfo.GetMethod != null)
                        fastString.Append($"\nget 접근자 - {GetAccessModifier(propertyInfo.GetMethod)}  ");
                    if (propertyInfo.SetMethod != null)
                        fastString.Append($"\nset 접근자 - {GetAccessModifier(propertyInfo.SetMethod)}  ");

                    fastString.Append("\n\n" + GetDescription(propertyInfo));

                    if (i != propertyInfos.Length - 1)
                        fastString.Append("\n\n");
                }

                if (propertyInfos.Length == removeCount)
                    fastString.Append("\n없음  ");
            }

            {
                fastString.Append("\n\n## 필드");

                FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                int removeCount = 0;
                for (int i = 0; i < fieldInfos.Length; i++)
                {
                    FieldInfo fieldInfo = fieldInfos[i];
                    if (fieldInfo.IsPrivate || IsIgnore(fieldInfo) || IsInheritance(type, fieldInfo))
                    {
                        removeCount++;
                        continue;
                    }

                    fastString.Append($"\n### {fieldInfo.Name}");
                    if (IsObsolete(fieldInfo, out string description))
                        fastString.Append($" (사용되지 않음 '{description}')  ");
                    else
                        fastString.Append("  ");

                    fastString.Append($"\n엑세스 한정자 - {GetAccessModifier(null, fieldInfo, null, out _)}  ");
                    fastString.Append($"\n타입 - {TypeLinkCreate(assembly, fieldInfo.FieldType)}  ");

                    fastString.Append("\n\n" + GetDescription(fieldInfo));

                    if (i != fieldInfos.Length - 1)
                        fastString.Append("\n\n");
                }

                if (fieldInfos.Length == removeCount)
                    fastString.Append("\n없음  ");
            }

            {
                fastString.Append("\n\n## 이벤트");

                EventInfo[] eventInfos = type.GetEvents(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                int removeCount = 0;
                for (int i = 0; i < eventInfos.Length; i++)
                {
                    EventInfo eventInfo = eventInfos[i];
                    string accessModifterText = GetAccessModifier(null, null, eventInfo, out PropertyEventMethod.AccessModifier accessModifter);
                    if (accessModifter == PropertyEventMethod.AccessModifier.Private || IsIgnore(eventInfo) || IsInheritance(type, eventInfo))
                    {
                        removeCount++;
                        continue;
                    }

                    fastString.Append($"\n### {eventInfo.Name}");
                    if (IsObsolete(eventInfo, out string description))
                        fastString.Append($" (사용되지 않음 '{description}')  ");
                    else
                        fastString.Append("  ");

                    fastString.Append($"\n엑세스 한정자 - {accessModifterText}  ");
                    fastString.Append($"\n타입 - {TypeLinkCreate(assembly, eventInfo.EventHandlerType)}  ");

                    if (eventInfo.AddMethod != null)
                        fastString.Append($"\nadd 접근자 - {GetAccessModifier(eventInfo.AddMethod)}  ");
                    if (eventInfo.RemoveMethod != null)
                        fastString.Append($"\nremove 접근자 - {GetAccessModifier(eventInfo.RemoveMethod)}  ");

                    fastString.Append("\n\n" + GetDescription(eventInfo));

                    if (i != eventInfos.Length - 1)
                        fastString.Append("\n\n");
                }

                if (eventInfos.Length == removeCount)
                    fastString.Append("\n없음  ");
            }

            {
                fastString.Append("\n\n## 메소드");

                MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                int removeCount = 0;
                for (int i = 0; i < methodInfos.Length; i++)
                {
                    MethodInfo methodInfo = methodInfos[i];
                    if (methodInfo.IsPrivate || methodInfo.IsConstructor || methodInfo.IsSpecialName || IsIgnore(methodInfo) || IsInheritance(type, methodInfo))
                    {
                        removeCount++;
                        continue;
                    }

                    fastString.Append($"\n### {methodInfo.Name}");
                    if (IsObsolete(methodInfo, out string description))
                        fastString.Append($" (사용되지 않음 '{description}')  ");
                    else
                        fastString.Append("  ");

                    fastString.Append($"\n엑세스 한정자 - {GetAccessModifier(methodInfo)}  ");
                    fastString.Append($"\n반환 타입 - {TypeLinkCreate(assembly, methodInfo.ReturnType)}  ");

                    fastString.Append("\n\n" + GetDescription(methodInfo));

                    if (i != methodInfos.Length - 1)
                        fastString.Append("\n\n");
                }

                if (methodInfos.Length == removeCount)
                    fastString.Append("\n없음  ");
            }

            File.WriteAllText(PathTool.Combine(path, type.Namespace + "." + type.Name + ".md"), fastString.ToString());
            apiList.Add(type);
        }

        string GetAccessModifier(Type type)
        {
            string accessModifier = "[public](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/public)";
            if (type.IsAbstract && type.IsSealed)
                accessModifier += " [static](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/static)";
            else if (type.IsAbstract)
                accessModifier += " [abstract](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/abstract)";
            else if (type.IsSealed)
                accessModifier += " [sealed](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/sealed)";

            return accessModifier;
        }

        string GetAccessModifier(PropertyInfo propertyInfo, FieldInfo fieldInfo, EventInfo eventInfo, out PropertyEventMethod.AccessModifier accessModifier)
        {
            string accessModifierText = "";

            if (propertyInfo != null)
            {
                accessModifier = propertyInfo.Accessmodifier();

                if (accessModifier == PropertyEventMethod.AccessModifier.Private)
                    accessModifierText = "[private](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/private)";
                else if (accessModifier == PropertyEventMethod.AccessModifier.Public)
                    accessModifierText = "[public](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/public)";
                else if (accessModifier == PropertyEventMethod.AccessModifier.Internal)
                    accessModifierText = "[internal](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/internal)";
                else if (accessModifier == PropertyEventMethod.AccessModifier.Protected)
                    accessModifierText = "[protected](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/protected)";
                else if (accessModifier == PropertyEventMethod.AccessModifier.ProtectedInternal)
                    accessModifierText = "[protected](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/protected) [internal](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/internal)";
                else if (accessModifier == PropertyEventMethod.AccessModifier.PrivateProtected)
                    accessModifierText = "[private](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/private) [protected](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/protected)";

                if ((propertyInfo.GetMethod != null && propertyInfo.GetMethod.IsStatic) || (propertyInfo.SetMethod != null && propertyInfo.SetMethod.IsStatic))
                    accessModifierText += " [static](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/static)";

                return accessModifierText;
            }
            else if (eventInfo != null)
            {
                accessModifier = eventInfo.Accessmodifier();

                if (accessModifier == PropertyEventMethod.AccessModifier.Private)
                    accessModifierText = "[private](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/private)";
                else if (accessModifier == PropertyEventMethod.AccessModifier.Public)
                    accessModifierText = "[public](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/public)";
                else if (accessModifier == PropertyEventMethod.AccessModifier.Internal)
                    accessModifierText = "[internal](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/internal)";
                else if (accessModifier == PropertyEventMethod.AccessModifier.Protected)
                    accessModifierText = "[protected](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/protected)";
                else if (accessModifier == PropertyEventMethod.AccessModifier.ProtectedInternal)
                    accessModifierText = "[protected](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/protected) [internal](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/internal)";
                else if (accessModifier == PropertyEventMethod.AccessModifier.PrivateProtected)
                    accessModifierText = "[private](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/private) [protected](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/protected)";

                if ((eventInfo.GetAddMethod() != null && eventInfo.GetAddMethod().IsStatic) || (eventInfo.GetRemoveMethod() != null && eventInfo.GetRemoveMethod().IsStatic))
                    accessModifierText += " [static](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/static)";

                return accessModifierText;
            }
            else if (fieldInfo != null)
            {
                accessModifier = PropertyEventMethod.AccessModifier.Private;

                if (fieldInfo.IsPrivate)
                    accessModifierText = "[private](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/private)";
                else if (fieldInfo.IsPublic)
                {
                    accessModifier = PropertyEventMethod.AccessModifier.Public;
                    accessModifierText = "[public](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/public)";
                }
                else if (fieldInfo.IsAssembly)
                {
                    accessModifier = PropertyEventMethod.AccessModifier.Internal;
                    accessModifierText = "[internal](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/internal)";
                }
                else if (fieldInfo.IsFamily)
                {
                    accessModifier = PropertyEventMethod.AccessModifier.Protected;
                    accessModifierText = "[protected](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/protected)";
                }
                else if (fieldInfo.IsFamilyOrAssembly)
                {
                    accessModifier = PropertyEventMethod.AccessModifier.ProtectedInternal;
                    accessModifierText = "[protected](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/protected) [internal](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/internal)";
                }
                else if (fieldInfo.IsFamilyAndAssembly)
                {
                    accessModifier = PropertyEventMethod.AccessModifier.PrivateProtected;
                    accessModifierText = "[private](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/private) [protected](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/protected)";
                }

                if (fieldInfo.IsLiteral && !fieldInfo.IsInitOnly)
                    accessModifierText += " [const](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/const)";
                else if (fieldInfo.IsStatic)
                    accessModifierText += " [static](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/static)";
                else if (fieldInfo.IsInitOnly)
                    accessModifierText += " [readonly](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/readonly)";

                return accessModifierText;
            }

            throw new NullReferenceException();
        }

        string GetAccessModifier(MethodInfo methodInfo)
        {
            string accessModifier = "";
            if (methodInfo.IsPrivate)
                accessModifier = "[private](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/private)";
            else if (methodInfo.IsPublic)
                accessModifier = "[public](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/public)";
            else if (methodInfo.IsAssembly)
                accessModifier = "[internal](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/internal)";
            else if (methodInfo.IsFamily)
                accessModifier = "[protected](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/protected)";
            else if (methodInfo.IsFamilyOrAssembly)
                accessModifier = "[protected](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/protected) [internal](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/internal)";
            else if (methodInfo.IsFamilyAndAssembly)
                accessModifier = "[private](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/private) [protected](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/protected)";

            if (methodInfo.IsStatic)
                accessModifier += " [static](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/static)";
            else if (methodInfo.IsAbstract)
                accessModifier += " [abstract](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/abstract)";
            else if (methodInfo.IsVirtual)
                accessModifier += " [virtual](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/virtual)";
            else if (IsOverride(methodInfo))
                accessModifier += " [override](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/override)";

            return accessModifier;
        }

        string GetTypeName(Type type)
        {
            if (type.IsClass)
                return "[class](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/class)";
            else if (type.IsValueType)
                return "[struct](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/builtin-types/struct)";
            else if (type.IsInterface)
                return "[interface](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/interface)";
            else if (type.IsEnum)
                return "[enum](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/enum)";
            else
                return "알 수 없음";
        }

        string TypeLinkCreate(Assembly assembly, Type type)
        {
            string fullName = type.Namespace + "." + type.Name;
            string firstNameSpace = "";
            if (type.Namespace != null)
                firstNameSpace = type.Namespace.Split(".")[0];

            if (type.Assembly == assembly)
                return $"[{GetTypeGenericsName(type, true)}]({PathTool.Combine(selectedGithubWikiSite, fullName).Replace("[]", "")})";
            else if (firstNameSpace == "UnityEngine" || firstNameSpace == "UnityEditor")
                return $"[{GetTypeGenericsName(type, true)}]({unityWikiSite + fullName.Replace("`", "_").Replace("[]", "").Substring(firstNameSpace.Length + 1)}.html)";
            else if (firstNameSpace == "System")
                return $"[{GetTypeGenericsName(type, true)}](https://docs.microsoft.com/ko-kr/dotnet/api/{fullName.Replace("`", "-").Replace("[]", "")}?view=netframework-4.8)";
            else if (type.IsGenericParameter)
                return $"[{GetTypeGenericsName(type, true)}](https://docs.microsoft.com/ko-kr/dotnet/csharp/fundamentals/types/generics)";
            else
                return GetTypeGenericsName(type);
        }

        string GetTypeGenericsName(Type type, bool replace = false)
        {
            string genericsName;
            string[] nameSplit = type.Name.Split("`");
            Type[] generics = type.GetGenericArguments();
            if (generics.Length > 0)
            {
                genericsName = ($"{nameSplit[0]}<");

                for (int genericsIndex = 0; genericsIndex < generics.Length; genericsIndex++)
                {
                    if (genericsIndex == generics.Length - 1)
                        genericsName += $"{generics[genericsIndex].Name}>";
                    else
                        genericsName += $"{generics[genericsIndex].Name}, ";
                }
            }
            else
                genericsName = nameSplit[0];

            if (replace)
                return genericsName.Replace("<", "\\<").Replace(">", "\\>");
            else
                return genericsName;
        }

        string ListGenericParameterAttributes(Type type)
        {
            string retval = "";
            GenericParameterAttributes attributes = type.GenericParameterAttributes;
            GenericParameterAttributes constraints = attributes & GenericParameterAttributes.SpecialConstraintMask;

            if (constraints != GenericParameterAttributes.None)
            {
                if ((constraints & GenericParameterAttributes.ReferenceTypeConstraint) != 0)
                {
                    retval += "[class](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/class)";

                    if ((constraints & GenericParameterAttributes.DefaultConstructorConstraint) != 0)
                        retval += ", new()";
                }
                else if ((constraints & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0)
                    retval += "[struct](https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/builtin-types/struct)";
            }

            return retval;
        }

        static bool IsOverride(MethodInfo methodInfo) => methodInfo.GetBaseDefinition().DeclaringType != methodInfo.DeclaringType;

        bool IsInheritance(Type type, PropertyInfo propertyInfo)
        {
            if (type.BaseType != null)
                return type.BaseType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Any(x => x.Name == propertyInfo.Name);

            return false;
        }

        bool IsInheritance(Type type, FieldInfo fieldInfo)
        {
            if (type.BaseType != null)
                return type.BaseType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Any(x => x.Name == fieldInfo.Name);

            return false;
        }

        bool IsInheritance(Type type, EventInfo eventInfo)
        {
            if (type.BaseType != null)
                return type.BaseType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Any(x => x.Name == eventInfo.Name);

            return false;
        }

        bool IsInheritance(Type type, MethodInfo methodInfo)
        {
            if (type.BaseType != null)
                return type.BaseType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Any(x => x.Name == methodInfo.Name);

            return false;
        }

        bool IsObsolete(Type type, out string description)
        {
            Attribute[] obsoleteAttributes = Attribute.GetCustomAttributes(type, typeof(ObsoleteAttribute));
            if (obsoleteAttributes.Length <= 0)
            {
                description = "";
                return false;
            }

            description = ((ObsoleteAttribute)obsoleteAttributes[0]).Message;
            return true;
        }

        bool IsObsolete(PropertyInfo propertyInfo, out string description)
        {
            Attribute[] obsoleteAttributes = Attribute.GetCustomAttributes(propertyInfo, typeof(ObsoleteAttribute));
            if (obsoleteAttributes.Length <= 0)
            {
                description = "";
                return false;
            }

            description = ((ObsoleteAttribute)obsoleteAttributes[0]).Message;
            return true;
        }

        bool IsObsolete(FieldInfo fieldInfo, out string description)
        {
            Attribute[] obsoleteAttributes = Attribute.GetCustomAttributes(fieldInfo, typeof(ObsoleteAttribute));
            if (obsoleteAttributes.Length <= 0)
            {
                description = "";
                return false;
            }

            description = ((ObsoleteAttribute)obsoleteAttributes[0]).Message;
            return true;
        }

        bool IsObsolete(EventInfo eventInfo, out string description)
        {
            Attribute[] obsoleteAttributes = Attribute.GetCustomAttributes(eventInfo, typeof(ObsoleteAttribute));
            if (obsoleteAttributes.Length <= 0)
            {
                description = "";
                return false;
            }

            description = ((ObsoleteAttribute)obsoleteAttributes[0]).Message;
            return true;
        }

        bool IsObsolete(MethodInfo methodInfo, out string description)
        {
            Attribute[] obsoleteAttributes = Attribute.GetCustomAttributes(methodInfo, typeof(ObsoleteAttribute));
            if (obsoleteAttributes.Length <= 0)
            {
                description = "";
                return false;
            }

            description = ((ObsoleteAttribute)obsoleteAttributes[0]).Message;
            return true;
        }

        string GetDescription(Type type)
        {
            Attribute[] descriptionAttribute = Attribute.GetCustomAttributes(type, typeof(WikiDescriptionAttribute));
            if (descriptionAttribute.Length > 0 && !string.IsNullOrEmpty(((WikiDescriptionAttribute)descriptionAttribute[0]).description))
                return ((WikiDescriptionAttribute)descriptionAttribute[0]).description;

            return "없음";
        }

        string GetDescription(PropertyInfo propertyInfo)
        {
            Attribute[] descriptionAttribute = Attribute.GetCustomAttributes(propertyInfo, typeof(WikiDescriptionAttribute));
            if (descriptionAttribute.Length > 0 && !string.IsNullOrEmpty(((WikiDescriptionAttribute)descriptionAttribute[0]).description))
                return ((WikiDescriptionAttribute)descriptionAttribute[0]).description;

            return "설명 없음";
        }

        string GetDescription(FieldInfo fieldInfo)
        {
            Attribute[] descriptionAttribute = Attribute.GetCustomAttributes(fieldInfo, typeof(WikiDescriptionAttribute));
            if (descriptionAttribute.Length > 0 && !string.IsNullOrEmpty(((WikiDescriptionAttribute)descriptionAttribute[0]).description))
                return ((WikiDescriptionAttribute)descriptionAttribute[0]).description;

            return "설명 없음";
        }

        string GetDescription(EventInfo eventInfo)
        {
            Attribute[] descriptionAttribute = Attribute.GetCustomAttributes(eventInfo, typeof(WikiDescriptionAttribute));
            if (descriptionAttribute.Length > 0 && !string.IsNullOrEmpty(((WikiDescriptionAttribute)descriptionAttribute[0]).description))
                return ((WikiDescriptionAttribute)descriptionAttribute[0]).description;

            return "설명 없음";
        }

        string GetDescription(MethodInfo methodInfo)
        {
            Attribute[] descriptionAttribute = Attribute.GetCustomAttributes(methodInfo, typeof(WikiDescriptionAttribute));
            if (descriptionAttribute.Length > 0 && !string.IsNullOrEmpty(((WikiDescriptionAttribute)descriptionAttribute[0]).description))
                return ((WikiDescriptionAttribute)descriptionAttribute[0]).description;

            return "설명 없음";
        }

        bool IsIgnore(Type type) => Attribute.GetCustomAttributes(type, typeof(WikiIgnoreAttribute)).Length > 0;
        bool IsIgnore(PropertyInfo propertyInfo) => Attribute.GetCustomAttributes(propertyInfo, typeof(WikiIgnoreAttribute)).Length > 0;
        bool IsIgnore(FieldInfo fieldInfo) => Attribute.GetCustomAttributes(fieldInfo, typeof(WikiIgnoreAttribute)).Length > 0;
        bool IsIgnore(EventInfo eventInfo) => Attribute.GetCustomAttributes(eventInfo, typeof(WikiIgnoreAttribute)).Length > 0;
        bool IsIgnore(MethodInfo methodInfo) => Attribute.GetCustomAttributes(methodInfo, typeof(WikiIgnoreAttribute)).Length > 0;
    }

    public static class PropertyEventMethod
    {
        public static readonly List<AccessModifier> AccessModifiers = new List<AccessModifier>
        {
            AccessModifier.Private,
            AccessModifier.Protected,
            AccessModifier.ProtectedInternal,
            AccessModifier.Internal,
            AccessModifier.Public
        };


        public static AccessModifier Accessmodifier(this PropertyInfo propertyInfo)
        {
            if (propertyInfo.SetMethod == null)
                return propertyInfo.GetMethod.Accessmodifier();
            if (propertyInfo.GetMethod == null)
                return propertyInfo.SetMethod.Accessmodifier();
            var max = Math.Max(AccessModifiers.IndexOf(propertyInfo.GetMethod.Accessmodifier()),
                AccessModifiers.IndexOf(propertyInfo.SetMethod.Accessmodifier()));
            return AccessModifiers[max];
        }

        public static AccessModifier Accessmodifier(this EventInfo eventInfo)
        {
            if (eventInfo.AddMethod == null)
                return eventInfo.AddMethod.Accessmodifier();
            if (eventInfo.RemoveMethod == null)
                return eventInfo.AddMethod.Accessmodifier();
            var max = Math.Max(AccessModifiers.IndexOf(eventInfo.AddMethod.Accessmodifier()),
                AccessModifiers.IndexOf(eventInfo.RemoveMethod.Accessmodifier()));
            return AccessModifiers[max];
        }

        public static AccessModifier Accessmodifier(this MethodInfo methodInfo)
        {
            if (methodInfo.IsPrivate)
                return AccessModifier.Private;
            if (methodInfo.IsFamily)
                return AccessModifier.Protected;
            if (methodInfo.IsFamilyOrAssembly)
                return AccessModifier.ProtectedInternal;
            if (methodInfo.IsFamilyAndAssembly)
                return AccessModifier.PrivateProtected;
            if (methodInfo.IsAssembly)
                return AccessModifier.Internal;
            if (methodInfo.IsPublic)
                return AccessModifier.Public;
            throw new ArgumentException("Did not find access modifier", "methodInfo");
        }

        public enum AccessModifier
        {
            Private,
            Protected,
            ProtectedInternal,
            PrivateProtected,
            Internal,
            Public
        }
    }
}