using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tofu.Logging.Formatters
{
	public abstract class LogFormatterObject : LogFormatter<object>
    {
        #region Enumerations

        // ******************************************************************
        // *																*
        // *					 Protected Member Variables				    *
        // *																*
        // ******************************************************************

        /// <summary>
        /// An enumeration that specifies all the supported ObjectMetaData types
        /// </summary>
        public enum ObjectMetaDataType
        {
            RootType,
            PropertyType,
            FieldType,
            EnumerableType,
            DuplicateType
        }

        #endregion

        #region ObjectMetaData Class

        // ******************************************************************
        // *																*
        // *			           ObjectMetaData Class                     *
        // *																*
        // ******************************************************************

        /// <summary>
        /// A class that caches metadata info about the class type of a given object
        /// </summary>
        public class ObjectMetaData
        {
            #region Constructors

            // ******************************************************************
            // *																*
            // *			               Constructors			                *
            // *																*
            // ******************************************************************

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="entryType">
            /// A ObjectMetaDataType enum value
            /// </param>
            private ObjectMetaData(ObjectMetaDataType entryType)
            {
                // Set members to default
                Type = entryType;
                ChildEntries = new List<ObjectMetaData>();
            }

            #endregion

            #region Public Methods

            // ******************************************************************
            // *																*
            // *			              Public Methods                        *
            // *																*
            // ******************************************************************

            /// <summary>
            /// Creates a new ObjectMetaData instance for the class type of the specified object
            /// </summary>
            /// <param name="obj">
            /// An object reference
            /// </param>
            /// <returns>
            /// A new ObjectMetaData instance
            /// </returns>
            public static ObjectMetaData CreateRootType(object obj)
            {
                // Create 'RootType' entry
                var entry = new ObjectMetaData(ObjectMetaDataType.RootType);

                try
                {
                    // Try to resolve value properties
                    entry.Value = obj;
                    entry.ValueType = obj != null ? obj.GetType() : null;
                    entry.ValueIsEnumerable = obj is IEnumerable;
                }
                catch (Exception ex)
                {
                    // Store exception that occured while resolving value
                    entry.ValueException = ex;
                }

                // Return reference to new entry
                return entry;
            }

            /// <summary>
            /// Creates a new ObjectMetaData instance for the class type of the specified object
            /// </summary>
            /// <param name="obj">
            /// An object reference
            /// </param>
            /// <returns>
            /// A new ObjectMetaData instance
            /// </returns>
            public static ObjectMetaData CreateDuplicateType(object obj)
            {
                // Create 'Duplicate' entry
                var entry = new ObjectMetaData(ObjectMetaDataType.DuplicateType);

                try
                {
                    // Try to resolve value properties
                    entry.Value = obj;
                    entry.ValueType = obj != null ? obj.GetType() : null;
                    entry.ValueIsEnumerable = obj is IEnumerable;
                }
                catch (Exception ex)
                {
                    // Store exception that occured while resolving value
                    entry.ValueException = ex;
                }

                // Return reference to new entry
                return entry;
            }

            /// <summary>
            /// Creates a new ObjectMetaData instance for the class type of the specified object
            /// </summary>
            /// <param name="obj">
            /// An object reference
            /// </param>
            /// <param name="property">
            /// A PropertyInfo instance that holds details about the public property
            /// </param>
            /// <returns>
            /// A new ObjectMetaData instance
            /// </returns>
            public static ObjectMetaData CreatePropertyType(object obj, PropertyInfo property)
            {
                // Create 'Property' entry
                var entry = new ObjectMetaData(ObjectMetaDataType.PropertyType);

                try
                {
                    // Try to resolve value properties
                    entry.Property = property;
                    entry.ValueType = property.PropertyType;
                    entry.ValueIsEnumerable = entry.Value is IEnumerable;
                    entry.Value = property.GetValue(obj,null);
                }
                catch (Exception ex)
                {
                    // Store exception that occured while resolving value
                    entry.ValueException = ex;
                }

                // Return reference to new entry
                return entry;
            }

            /// <summary>
            /// Creates a new ObjectMetaData for the specified type
            /// </summary>
            /// <param name="obj">
            /// An object reference
            /// </param>
            /// <param name="field">
            /// A FieldInfo instance that holds details about the public field
            /// </param>
            /// <returns>
            /// A new ObjectMetaData instance
            /// </returns>
            public static ObjectMetaData CreateFieldType(object obj, FieldInfo field)
            {
                // Create 'Property' entry
                var entry = new ObjectMetaData(ObjectMetaDataType.FieldType);

                try
                {
                    // Try to resolve value properties
                    entry.Field = field;
                    entry.ValueType = field.FieldType;
                    entry.ValueIsEnumerable = entry.Value is IEnumerable;
                    entry.Value = field.GetValue(obj);
                }
                catch (Exception ex)
                {
                    // Store exception that occured while resolving value
                    entry.ValueException = ex;
                }

                // Return reference to new entry
                return entry;
            }

            /// <summary>
            /// Creates a new ObjectMetaData for the specified type
            /// </summary>
            /// <param name="obj">
            /// An object reference
            /// </param>
            /// <param name="index">
            /// An integer that holds the actual index location within the enumerable
            /// </param>
            /// <returns>
            /// A new ObjectMetaData instance
            /// </returns>
            public static ObjectMetaData CreateEnumerableType(object obj, int index)
            {
                // Create 'Enumerable' entry
                var entry = new ObjectMetaData(ObjectMetaDataType.EnumerableType);

                try
                {
                    // Try to resolve value properties
                    entry.EnumerableIndex = index;
                    entry.Value = obj;
                    entry.ValueType = obj != null ? obj.GetType() : null;
                    entry.ValueIsEnumerable = obj is IEnumerable;
                }
                catch (Exception ex)
                {
                    // Store exception that occured while resolving value
                    entry.ValueException = ex;
                }

                // Return reference to new entry
                return entry;
            }

            #endregion

            #region Public Properties

            // ******************************************************************
            // *																*
            // *			             Public Properties			            *
            // *																*
            // ******************************************************************

            /// <summary>
            /// Gets a ObjectMetaDataType enum value
            /// </summary>
            public ObjectMetaDataType Type
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets an integer that holds the index location in case this entry was obtained
            /// from an IEnumerable collection
            /// </summary>
            public int EnumerableIndex
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets a PropertyInfo instance in case this entry was obtained as public property
            /// </summary>
            public PropertyInfo Property
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets a FieldInfo instance in case this entry was obtained as public field
            /// </summary>
            public FieldInfo Field
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets an object reference to the actual value that is associated with this entry.
            /// This is either the root object reference, the actual property value or the instance
            /// obtained by iterating an enumerable collection
            /// </summary>
            public object Value
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets a bool that indicates if the value reference implements IEnumerable or not
            /// </summary>
            public bool ValueIsEnumerable
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets a type that specifies the class type of the value
            /// </summary>
            public Type ValueType
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets an exception that was thrown when the value was resolved
            /// </summary>
            public Exception ValueException
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets a collection that holds the child entries that are associated with this entry
            /// </summary>
            public List<ObjectMetaData> ChildEntries
            {
                get;
                private set;
            }

            #endregion
        }

        #endregion

        #region Public Methods

        // ******************************************************************
        // *																*
        // *			              Public Methods                        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Decomposes an object into a metadata tree expression that describes the structure
        /// and contents of the specified object
        /// </summary>
        /// <param name="obj">
        /// An object that must be decomposed
        /// </param>
        /// <returns>
        /// An ObjectMetaData instance that holds the metadata structure and contents of the 
        /// specified object
        /// </returns>
        public static ObjectMetaData DecomposeObject(object obj)
        {
            // Create root entry
            var entry = ObjectMetaData.CreateRootType(obj);

            // Recurse down object's metadata structure
            DecomposeObjectRecurse(obj, entry, new List<object>());

            // Return root entry
            return entry;
        }

        #endregion

        #region Private Methods

        // ******************************************************************
        // *																*
        // *			             Private Methods                        *
        // *																*
        // ******************************************************************

        /// <summary>
        /// Recursively decomposes the specified object into a metadata instance
        /// </summary>
        /// <param name="obj">
        /// An object reference to the instance to decompose
        /// </param>
        /// <param name="parentEntry">
        /// A ObjectMetaData to the parent of the specified object
        /// </param>
        /// <param name="processedReferences">
        /// A collection of processed objects that is used to avoid infinite recursions
        /// </param>
        private static void DecomposeObjectRecurse(
            object obj,
            ObjectMetaData parentEntry,
            List<object> processedReferences)
        {
            // We do not recurse down certain value types
            if (obj == null ||
                obj is string ||
                obj is Type ||
                obj is IConvertible)
                return;

            // Are we recursing reference type?
            if (!(obj is ValueType))
            {
                // Avoid re-processing same objects
                if (processedReferences.Contains(obj))
                {
                    // Add to parent list
                    var entry = ObjectMetaData.CreateDuplicateType(obj);
                    parentEntry.ChildEntries.Add(entry);
                    return;
                }

                // Add object to list of processed ones
                processedReferences.Add(obj);
            }

            // Check if object is reference type
            var props = obj.GetType().GetProperties(
                BindingFlags.Instance |
                BindingFlags.GetProperty |
                BindingFlags.Public |
                BindingFlags.FlattenHierarchy);
            props = props.OrderBy(o => o.Name).ToArray();
            foreach (var prop in props)
            {
                // Skip accessor properties
                if (prop.GetIndexParameters().Length == 0)
                {
                    // Add to parent list
                    var entry = ObjectMetaData.CreatePropertyType(obj, prop);
                    parentEntry.ChildEntries.Add(entry);

                    // Recurse down
                    DecomposeObjectRecurse(entry.Value, entry, processedReferences);
                }
            }

            // Check if object is reference type
            var fields = obj.GetType().GetFields(
                BindingFlags.Instance |
                BindingFlags.GetProperty |
                BindingFlags.Public |
                BindingFlags.FlattenHierarchy);
            fields = fields.OrderBy(o => o.Name).ToArray();
            foreach (var field in fields)
            {
                // Add to parent list
                var entry = ObjectMetaData.CreateFieldType(obj, field);
                parentEntry.ChildEntries.Add(entry);

                // Recurse down
                DecomposeObjectRecurse(entry.Value, entry, processedReferences);
            }

            // Check if object is enumerable type
            var iEnumerable = obj as IEnumerable;
            if (iEnumerable != null)
            {
                // Iterate
                int i = 0;
                foreach (var o in iEnumerable)
                {
                    // Add to parent list
                    var entry = ObjectMetaData.CreateEnumerableType(o, i++);
                    parentEntry.ChildEntries.Add(entry);

                    // Recurse down
                    DecomposeObjectRecurse(entry.Value, entry, processedReferences);
                }
            }
        }

        #endregion
    }
}
