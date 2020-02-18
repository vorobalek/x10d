namespace System
{
    /// <summary>
    /// Типизированный объект.
    /// </summary>
    public class TypedObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypedObject"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="data">The data.</param>
        public TypedObject(Type type, object data)
        {
            Type = type;
            Object = data;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public Type Type { get; }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public object Object { get; }
    }
}
