using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TechnoDemo.Extensions
{
    public static class FGetExtension
    {
        private static T GetObject<T>(this IList<GameObject> @list) where T : class
        {
            if (@list == null) return null;
            
            Span<GameObject> objs = @list.ToArray();
            
            for (int i = 0, count = objs.Length; i < count; i++)
                if (objs[i].TryGetComponent(out T comp))
                    return comp;

            return null;
        }

        public static bool TryGetObject<T>(this IList<GameObject> @list, out T obj) where T : class
        {
            if (@list == null)
            {
                obj = null;
                return false;
            }
            
            obj = @list.GetObject<T>();

            return obj != null;
        }

        public static async UniTask<T> GetObjectAsync<T>(this IList<GameObject> @list) where T : class
        {
            if (@list == null) return null;
            
            T obj = null;
            await UniTask.WaitUntil(() => (obj = @list.GetObject<T>()) != null);

            return obj;
        }

        public static async UniTask<Tuple<bool, T>> TryGetObjectAsync<T>(this IList<GameObject> @list) where T : class
        {
            if (@list == null) return new Tuple<bool, T>(false, null);
            
            T obj = null;
            await UniTask.WaitUntil(() => (obj = @list.GetObject<T>()) != null);

            return new Tuple<bool, T>(obj != null, obj);
        }
    }
}