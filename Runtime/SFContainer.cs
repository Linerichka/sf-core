using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SFramework.Core.Runtime
{

    public sealed class SFContainer : ISFContainer
    {
        private static readonly Dictionary<Type, SFInjectableTypeInfo> InjectableTypes;
        private readonly Dictionary<Type, object> _dependencies = new();
        private readonly Dictionary<Type, List<Type>> _mapping = new();
        private readonly List<ISFService> _services = new();

        static SFContainer()
        {
            InjectableTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !internalAssemblyNames.Contains(a.GetName().Name))
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && typeof(ISFInjectable).IsAssignableFrom(type))
                .Select(type => new SFInjectableTypeInfo(ref type))
                .ToDictionary(typeInfo => typeInfo.Type, t => t);
        }


        public SFContainer(GameObject gameObject)
        {
            Register<ISFContainer, SFContainer>(this);
            Root = gameObject.transform;
        }

        public TService Register<TService, TImplementation>() 
        where TService : ISFRegistered
        where TImplementation : class, TService
        {
            if (_dependencies.ContainsKey(typeof(TService)))
            {
                if (SFDebug.IsDebug)
                {
                    SFDebug.Log("Object of this type already exists in the dependency container", LogType.Warning);
                }
            }

            object instance;

            var constructor = typeof(TImplementation).GetTypeInfo().DeclaredConstructors.FirstOrDefault();

            if (constructor != null)
            {
                var parameters = constructor.GetParameters();

                if (parameters.Length > 0)
                {
                    var parameterInstances = new object[parameters.Length];
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        if (!_dependencies.TryGetValue(parameters[i].ParameterType, out var obj))
                        {
                            if (SFDebug.IsDebug)
                            {
                                SFDebug.Log($"Service of type {parameters[i].ParameterType.FullName} is not registered. Returning NULL.", LogType.Error);
                            }

                            break;
                        }

                        parameterInstances[i] = obj;
                    }

                    instance = constructor.Invoke(parameterInstances);
                }
                else
                {
                    instance = Activator.CreateInstance(typeof(TImplementation), true);
                }
            }
            else
            {
                instance = Activator.CreateInstance(typeof(TImplementation), true);
            }

            return Register<TService, TImplementation>(instance);
        }

        public TService Register<TService, TImplementation>(object instance) 
            where TService : ISFRegistered
            where TImplementation : class, TService
        {
            if (_dependencies.ContainsKey(typeof(TService)))
            {
                if (SFDebug.IsDebug)
                {
                    SFDebug.Log("Object of this type already exists in the dependency container", LogType.Warning);
                }
            }

            if (SFDebug.IsDebug)
            {
                SFDebug.Log($"[Core] Bind: {typeof(TService).Name} to {typeof(TImplementation).Name}");
            }

            foreach (var subclassType in typeof(TService).GetInterfaces())
            {
                if (!_mapping.ContainsKey(subclassType))
                {
                    _mapping[subclassType] = new List<Type>();
                }

                _mapping[subclassType].Add(typeof(TService));
            }

            _dependencies[typeof(TService)] = instance ?? throw new ArgumentNullException(nameof(instance));

            if (typeof(ISFService).IsAssignableFrom(typeof(TService)))
            {
                _services.Add(instance as ISFService);
            }

            return (TService)instance;
        }

        public TService Register<TService>(object instance)
            where TService : ISFRegistered
        {
            if (instance is not TService) return default;

            if (_dependencies.ContainsKey(typeof(TService)))
            {
                if (SFDebug.IsDebug)
                {
                    SFDebug.Log("Object of this type already exists in the dependency container", LogType.Warning);
                }
            }

            if (SFDebug.IsDebug)
            {
                SFDebug.Log($"[Core] Bind: {typeof(TService).Name} to {instance.GetType().Name}");
            }

            foreach (var subclassType in typeof(TService).GetInterfaces())
            {
                if (!_mapping.ContainsKey(subclassType))
                {
                    _mapping[subclassType] = new List<Type>();
                }

                _mapping[subclassType].Add(typeof(TService));
            }

            _dependencies[typeof(TService)] = instance ?? throw new ArgumentNullException(nameof(instance));

            if (typeof(ISFService).IsAssignableFrom(typeof(TService)))
            {
                _services.Add(instance as ISFService);
            }

            return (TService)instance;
        }

        public void Register(Type type, object instance)
        {
            if (_dependencies.ContainsKey(type))
            {
                if (SFDebug.IsDebug)
                {
                    SFDebug.Log("Object of this type already exists in the dependency container", LogType.Warning);
                }
            }


            if (SFDebug.IsDebug)
            {
                SFDebug.Log($"[Core] Bind: {type.Name} to {instance.GetType().Name}");
            }

            foreach (var subclassType in type.GetInterfaces())
            {
                if (!_mapping.ContainsKey(subclassType))
                {
                    _mapping[subclassType] = new List<Type>();
                }

                _mapping[subclassType].Add(type);
            }

            _dependencies[type] = instance ?? throw new ArgumentNullException(nameof(instance));

            if (typeof(ISFService).IsAssignableFrom(type))
            {
                _services.Add(instance as ISFService);
            }
        }

        public Transform Root { get; private set; }

        public T Resolve<T>() where T : class
        {
            return Resolve(typeof(T)) as T;
        }

        public T[] ResolveMany<T>()
        {
            if (!_mapping.ContainsKey(typeof(T))) return Array.Empty<T>();

            var result = new T[_mapping[typeof(T)].Count];

            for (int i = 0; i < _mapping[typeof(T)].Count; i++)
            {
                var type = _mapping[typeof(T)][i];
                result[i] = (T)Resolve(type);
            }

            return result;
        }

        public object Resolve(Type type)
        {
            if (_dependencies.TryGetValue(type, out var resolve))
            {
                return resolve;
            }

            return _dependencies.FirstOrDefault(kvp => type.IsAssignableFrom(kvp.Key)).Value;
        }

        public object[] Bindings => _dependencies.Values.ToArray();

        public async UniTask InitServices(CancellationToken cancellationToken)
        {
            foreach (var service in _services)
            {
                await service.Init(cancellationToken);
            }
        }


        public IEnumerable<T> GetDependencies<T>() where T : class
        {
            var result = new List<T>();

            foreach (var dependency in _dependencies)
            {
                var typedDependency = dependency.Value as T;
                if (typedDependency != null)
                {
                    result.Add(typedDependency);
                }
            }

            return result;
        }

        /// <summary>
        /// Inject all dependencies in container
        /// </summary>
        internal void Inject()
        {
            foreach (var dependency in _dependencies.Values)
            {
                Inject(dependency);
            }
        }

        internal void Inject(GameObject gameObject, bool includeInactive = false)
        {
            foreach (var injectable in gameObject.GetComponentsInChildren<ISFInjectable>(includeInactive))
            {
                Inject(injectable);
            }
        }

        internal void Inject(object targetObject)
        {
            if (targetObject == null) throw new ArgumentNullException(nameof(targetObject));

            if (!InjectableTypes.TryGetValue(targetObject.GetType(), out var injectableType)) return;

            InjectFields(ref targetObject, ref injectableType.Fields);
            InjectProperties(ref targetObject, ref injectableType.Properties);
            InjectMethods(ref targetObject, ref injectableType.Methods, ref injectableType.ParametersByMethod);
        }


        private void InjectFields(ref object targetObject, ref FieldInfo[] injectableFields)
        {
            foreach (var fieldInfo in injectableFields)
            {
                var dependency = Resolve(fieldInfo.FieldType);
                fieldInfo.SetValue(targetObject, dependency);
            }
        }

        private void InjectProperties(ref object targetObject, ref PropertyInfo[] injectableProperties)
        {
            foreach (var propertyInfo in injectableProperties)
            {
                var dependency = Resolve(propertyInfo.PropertyType);
                propertyInfo.SetValue(targetObject, dependency);
            }
        }

        private void InjectMethods(ref object targetObject, ref MethodInfo[] methods,
            ref Dictionary<MethodInfo, ParameterInfo[]> parametersByMethod)
        {
            foreach (var methodInfo in methods)
            {
                if (parametersByMethod.TryGetValue(methodInfo, out var parameters))
                {
                    var dependencies = new object[parameters.Length];

                    for (var i = 0; i < parameters.Length; i++)
                    {
                        var parameter = parameters[i];
                        var dependency = Resolve(parameter.ParameterType);
                        dependencies[i] = dependency;
                    }

                    methodInfo.Invoke(targetObject, dependencies);
                }
                else
                {
                    methodInfo.Invoke(targetObject, null);
                }
            }
        }

        #region IgnorerAssembly
        private static readonly HashSet<string> internalAssemblyNames = new HashSet<string>()
        {
            "mscorlib",
            "System",
            "System.Core",
            "System.Security.Cryptography.Algorithms",
            "System.Net.Http",
            "System.Data",
            "System.Runtime.Serialization",
            "System.Xml.Linq",
            "System.Numerics",
            "System.Xml",
            "System.Configuration",
            "ExCSS.Unity",
            "Unity.Cecil",
            "Unity.CompilationPipeline.Common",
            "Unity.SerializationLogic",
            "Unity.TestTools.CodeCoverage.Editor",
            "Unity.ScriptableBuildPipeline.Editor",
            "Unity.Addressables.Editor",
            "Unity.ScriptableBuildPipeline",
            "Unity.CollabProxy.Editor",
            "Unity.Timeline.Editor",
            "Unity.PerformanceTesting.Tests.Runtime",
            "Unity.Settings.Editor",
            "Unity.PerformanceTesting",
            "Unity.PerformanceTesting.Editor",
            "Unity.Rider.Editor",
            "Unity.ResourceManager",
            "Unity.TestTools.CodeCoverage.Editor.OpenCover.Mono.Reflection",
            "Unity.PerformanceTesting.Tests.Editor",
            "Unity.TextMeshPro",
            "Unity.Timeline",
            "Unity.Addressables",
            "Unity.TestTools.CodeCoverage.Editor.OpenCover.Model",
            "Unity.VisualStudio.Editor",
            "Unity.TextMeshPro.Editor",
            "Unity.VSCode.Editor",
            "UnityEditor",
            "UnityEditor.UI",
            "UnityEditor.TestRunner",
            "UnityEditor.CacheServer",
            "UnityEditor.WindowsStandalone.Extensions",
            "UnityEditor.Graphs",
            "UnityEditor.UnityConnectModule",
            "UnityEditor.UIServiceModule",
            "UnityEditor.UIElementsSamplesModule",
            "UnityEditor.UIElementsModule",
            "UnityEditor.SceneTemplateModule",
            "UnityEditor.PackageManagerUIModule",
            "UnityEditor.GraphViewModule",
            "UnityEditor.CoreModule",
            "UnityEngine",
            "UnityEngine.UI",
            "UnityEngine.XRModule",
            "UnityEngine.WindModule",
            "UnityEngine.VirtualTexturingModule",
            "UnityEngine.TestRunner",
            "UnityEngine.VideoModule",
            "UnityEngine.VehiclesModule",
            "UnityEngine.VRModule",
            "UnityEngine.VFXModule",
            "UnityEngine.UnityWebRequestWWWModule",
            "UnityEngine.UnityWebRequestTextureModule",
            "UnityEngine.UnityWebRequestAudioModule",
            "UnityEngine.UnityWebRequestAssetBundleModule",
            "UnityEngine.UnityWebRequestModule",
            "UnityEngine.UnityTestProtocolModule",
            "UnityEngine.UnityCurlModule",
            "UnityEngine.UnityConnectModule",
            "UnityEngine.UnityAnalyticsModule",
            "UnityEngine.UmbraModule",
            "UnityEngine.UNETModule",
            "UnityEngine.UIElementsNativeModule",
            "UnityEngine.UIElementsModule",
            "UnityEngine.UIModule",
            "UnityEngine.TilemapModule",
            "UnityEngine.TextRenderingModule",
            "UnityEngine.TextCoreModule",
            "UnityEngine.TerrainPhysicsModule",
            "UnityEngine.TerrainModule",
            "UnityEngine.TLSModule",
            "UnityEngine.SubsystemsModule",
            "UnityEngine.SubstanceModule",
            "UnityEngine.StreamingModule",
            "UnityEngine.SpriteShapeModule",
            "UnityEngine.SpriteMaskModule",
            "UnityEngine.SharedInternalsModule",
            "UnityEngine.ScreenCaptureModule",
            "UnityEngine.RuntimeInitializeOnLoadManagerInitializerModule",
            "UnityEngine.ProfilerModule",
            "UnityEngine.Physics2DModule",
            "UnityEngine.PhysicsModule",
            "UnityEngine.PerformanceReportingModule",
            "UnityEngine.ParticleSystemModule",
            "UnityEngine.LocalizationModule",
            "UnityEngine.JSONSerializeModule",
            "UnityEngine.InputLegacyModule",
            "UnityEngine.InputModule",
            "UnityEngine.ImageConversionModule",
            "UnityEngine.IMGUIModule",
            "UnityEngine.HotReloadModule",
            "UnityEngine.GridModule",
            "UnityEngine.GameCenterModule",
            "UnityEngine.GIModule",
            "UnityEngine.DirectorModule",
            "UnityEngine.DSPGraphModule",
            "UnityEngine.CrashReportingModule",
            "UnityEngine.CoreModule",
            "UnityEngine.ClusterRendererModule",
            "UnityEngine.ClusterInputModule",
            "UnityEngine.ClothModule",
            "UnityEngine.AudioModule",
            "UnityEngine.AssetBundleModule",
            "UnityEngine.AnimationModule",
            "UnityEngine.AndroidJNIModule",
            "UnityEngine.AccessibilityModule",
            "UnityEngine.ARModule",
            "UnityEngine.AIModule",
            "SyntaxTree.VisualStudio.Unity.Bridge",
            "nunit.framework",
            "Newtonsoft.Json",
            "ReportGeneratorMerged",
            "Unrelated",
            "netstandard",
            "SyntaxTree.VisualStudio.Unity.Messaging"
        };
        #endregion

    }

}