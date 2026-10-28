using UnityEngine;

namespace Runtime.Extensions
{
    public static class ComponentSearchExtensions
    {
        public static T FindChild<T>(this Component parent, string objectName) where T : Component
        {
            if (!parent || string.IsNullOrEmpty(objectName)) return null;

            foreach (T component in parent.GetComponentsInChildren<T>(true))
            {
                if (component.name == objectName)
                    return component;
            }

            return null;
        }
    }
}
