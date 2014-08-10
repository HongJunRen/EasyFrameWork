﻿using System.Reflection;
using Easy.Extend;
using Easy.IOCAdapter;
using Easy.IOCAdapter.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easy
{
    public static class Loader
    {
        public static T CreateInstance<T>()
        {
            Type type = typeof(T);
            if ((type.IsInterface || type.IsAbstract))
            {
                if (Container.All.ContainsKey(type))
                {
                    var implType = Container.All[type].First();
                    return (T)implType.Instance;
                }
                else
                {
                    return default(T);
                    //throw new UnregisteredException(ty);
                }
            }
            else if (!type.IsInterface)
            {
                return (T)BuildInstance(type);
            }
            return default(T);
        }
        public static T CreateInstance<T>(string assemblyName, string fullClassName, bool dynamicLoad = false) where T : class
        {
            if (dynamicLoad)
            {
                assemblyName = string.Format("{0}.dll", assemblyName);
                Cache.StaticCache cache = new Cache.StaticCache();
                Type type = cache.Get(string.Format("{0}_{1}", assemblyName, fullClassName), m =>
                {
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (Assembly item in assemblies)
                    {
                        if (item.ManifestModule.Name.Equals(assemblyName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            return item.GetType(fullClassName);
                        }
                    }
                    return null;
                });
                T result = BuildInstance(type) as T;
                return result;
            }
            return System.Activator.CreateInstance(assemblyName, fullClassName).Unwrap() as T;
        }

        public static List<T> ResolveAll<T>() where T : class
        {
            var result = new List<T>();
            Type type = typeof(T);
            if (Container.All.ContainsKey(type))
            {
                Container.All[type].Each(m => result.Add((T)m.Instance));
            }
            else
            {
                AppDomain.CurrentDomain.GetAssemblies().Each(m => m.GetTypes().Each(t =>
                {
                    if (t.IsClass && type.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
                    {
                        Container.Register(type, t);
                        result.Add(BuildInstance(t) as T);
                    }
                }));
            }
            return result;
        }
        public static object CreateInstance(Type type)
        {
            if (Container.All.ContainsKey(type))
            {
                var implType = Container.All[type].First();
                return implType.Instance;
            }
            return BuildInstance(type);
        }

        public static object BuildInstance(Type type)
        {
            if (type.FullName == "System.String") return null;
            var constructors = type.GetConstructors();
            if (constructors.Length > 0)
            {
                var paras = constructors[0].GetParameters();
                if (paras.Length > 0)
                {
                    object[] paraObjects = new object[paras.Length];
                    for (int i = 0; i < paras.Length; i++)
                    {
                        paraObjects[i] = BuildInstance(GetType(paras[i].ParameterType));
                    }
                    return Activator.CreateInstance(type, paraObjects);
                }
                return Activator.CreateInstance(type);
            }
            return Activator.CreateInstance(type);
        }

        public static Type GetType<T>()
        {
            Type type = typeof(T);
            if (Container.All.ContainsKey(type))
            {
                return Container.All[type].First().TargeType;
            }
            return type;
        }
        public static Type GetType(Type type)
        {
            if (Container.All.ContainsKey(type))
            {
                return Container.All[type].First().TargeType;
            }
            return type;
        }
        public static Type GetType(string fullClassName)
        {
            Cache.StaticCache cache = new Cache.StaticCache();
            return cache.Get(fullClassName, m =>
            {
                Type type = null;
                AppDomain.CurrentDomain.GetAssemblies().Each(n => n.GetTypes().Each(t =>
                {
                    if (type == null && t.FullName == fullClassName)
                    {
                        type = t;
                    }
                }));
                return type;
            });
        }
    }
}
