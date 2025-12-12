using System;
using System.Collections.Generic;

namespace Hawky.Loading
{
    public static class LoadingControllerFactory
    {
        private static Dictionary<string, ILoadingControllerCreator> _dictCreator;
        private static bool _init;
        public static ILoadingController CreateLoading(string loadingControllerCreatorId = null)
        {
            Init();

            if (string.IsNullOrEmpty(loadingControllerCreatorId))
            {
                loadingControllerCreatorId = LoadingControllerCreatorId.DEFAULT;
            }

            if (_dictCreator.TryGetValue(loadingControllerCreatorId, out var creator))
            {
                return creator.CreateLoading();
            }

            return null;
        }

        public static void AddOrOverrideCreator(ILoadingControllerCreator loadingControllerCreator)
        {
            Init();

            _dictCreator[loadingControllerCreator.GetId()] = loadingControllerCreator;
        }

        public static void Init()
        {
            if (_init)
            {
                return;
            }

            _init = true;
            _dictCreator = new Dictionary<string, ILoadingControllerCreator>();

            var allType = TypeUtilities.FindAllDerivedTypesInDomain<IAutoLoadingControllerCreator>();

            foreach (var type in allType)
            {
                var instance = Activator.CreateInstance(type) as IAutoLoadingControllerCreator;

                _dictCreator.Add(instance.GetId(), instance);
            }
        }
    }
}
