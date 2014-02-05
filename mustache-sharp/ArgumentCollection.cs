﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Mustache
{
    /// <summary>
    /// Associates parameters to their argument values.
    /// </summary>
    internal sealed class ArgumentCollection
    {
        private readonly Dictionary<TagParameter, string> _argumentLookup;

        /// <summary>
        /// Initializes a new instance of an ArgumentCollection.
        /// </summary>
        public ArgumentCollection()
        {
            _argumentLookup = new Dictionary<TagParameter, string>();
        }

        /// <summary>
        /// Associates the given parameter to the key placeholder.
        /// </summary>
        /// <param name="parameter">The parameter to associate the key with.</param>
        /// <param name="key">The key placeholder used as the argument.</param>
        /// <remarks>If the key is null, the default value of the parameter will be used.</remarks>
        public void AddArgument(TagParameter parameter, string key)
        {
            _argumentLookup.Add(parameter, key);
        }

        /// <summary>
        /// Gets the key that will be used to find the substitute value.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        public string GetKey(TagParameter parameter)
        {
            string key;
            if (_argumentLookup.TryGetValue(parameter, out key))
            {
                return key;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Substitutes the key placeholders with their respective values.
        /// </summary>
        /// <param name="keyScope">The key/value pairs in the current lexical scope.</param>
        /// <param name="contextScope">The key/value pairs in current context.</param>
        /// <returns>A dictionary associating the parameter name to the associated value.</returns>
        public Dictionary<string, object> GetArguments(Scope keyScope, Scope contextScope)
        {
            Dictionary<string, object> arguments = new Dictionary<string,object>();
            foreach (KeyValuePair<TagParameter, string> pair in _argumentLookup)
            {
                object value;
                if (pair.Value == null)
                {
                    value = pair.Key.DefaultValue;
                }
                else if (pair.Value.StartsWith("@"))
                {
                    value = contextScope.Find(pair.Value.Substring(1));
                }
                else if (pair.Value.StartsWith("_")) {
                    value = pair.Value.Remove(0, 1);
                }
                else
                {
                    value = keyScope.Find(pair.Value);
                }
                arguments.Add(pair.Key.Name, value);
            }
            return arguments;
        }

        public Dictionary<string, object> GetArgumentKeyNames()
        {
            return _argumentLookup.ToDictionary(p => p.Key.Name, p => (object)p.Value);
        }
    }
}
