using System.Collections.Generic;
using System.ComponentModel;

namespace UISystem
{
    public class DefaultValueTemplate
    {
        Dictionary<string, object> mDefaultValueDictionary = new Dictionary<string, object>();

        public bool IsEqualDefaultValue(object instance, string propertyName)
        {
            var property = TypeDescriptor.GetProperties(instance)[propertyName];
            if (property == null)
                return false;

            //var defValue = mDefaultValueDictionary[propertyName];
            object defValue;
            if (!mDefaultValueDictionary.TryGetValue(propertyName, out defValue))
                return false;

            //if (property.PropertyType != defValue.GetType())
            //    return false;

            return object.Equals(defValue, property.GetValue(instance));

            //AttributeCollection attributes = property.Attributes;
            //UIEditor_DefaultValue defAtt = attributes[typeof(UIEditor_DefaultValue)] as UIEditor_DefaultValue;
            //if (defAtt != null)
            //{
            //    if (property.PropertyType != value.GetType())
            //        return false;

            //    return object.Equals(value, defAtt.Value);
            //}

            //return false;
        }

        public void RegisterDefaultValue(string propertyName, object defaultValue)
        {
            mDefaultValueDictionary[propertyName] = defaultValue;
        }

        public object GetDefaultValue(string propertyName)
        {
            return mDefaultValueDictionary[propertyName];
        }
    }
}
