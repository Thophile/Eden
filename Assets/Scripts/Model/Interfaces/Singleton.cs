using System;

namespace Assets.Scripts.Model
{
    public abstract class Singleton<T> where T : class
    {
        private static readonly Lazy<T> _instance = new Lazy<T>(() => CreateInstanceOfT());

        public static T Instance { get { return _instance.Value; } }

        private static T CreateInstanceOfT()
        {
            return Activator.CreateInstance(typeof(T), true) as T;
        }
    }
}