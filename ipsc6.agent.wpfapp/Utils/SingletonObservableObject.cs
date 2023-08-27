using System;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ipsc6.agent.wpfapp.Utils
{
    public abstract class SingletonObservableObject<T> : ObservableObject where T : class
    {
        #region Members

        /// <summary>
        /// Static instance. Needs to use lambda expression
        /// to construct an instance (since constructor is private).
        /// </summary>
        private static readonly Lazy<T> instance = new(CreateInstanceOfT);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance of this singleton.
        /// </summary>
        public static T Instance => instance.Value;

        #endregion

        #region Methods

        /// <summary>
        /// Creates an instance of T via reflection since T's constructor is expected to be private.
        /// </summary>
        /// <returns></returns>
        private static T CreateInstanceOfT()
        {
            return Activator.CreateInstance(typeof(T), true) as T;
        }

        #endregion
    }
}
