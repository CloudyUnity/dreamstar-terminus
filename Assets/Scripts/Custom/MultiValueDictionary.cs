#region COPYRIGHT
//////////////////////////////////////////////////////////////////////
// Algorithmia is (c) 2018 Solutions Design. All rights reserved.
// https://github.com/SolutionsDesign/Algorithmia
//////////////////////////////////////////////////////////////////////
// COPYRIGHTS:
// Copyright (c) 2018 Solutions Design. All rights reserved.
// 
// The Algorithmia library sourcecode and its accompanying tools, tests and support code
// are released under the following license: (BSD2)
// ----------------------------------------------------------------------
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met: 
//
// 1) Redistributions of source code must retain the above copyright notice, this list of 
//    conditions and the following disclaimer. 
// 2) Redistributions in binary form must reproduce the above copyright notice, this list of 
//    conditions and the following disclaimer in the documentation and/or other materials 
//    provided with the distribution. 
// 
// THIS SOFTWARE IS PROVIDED BY SOLUTIONS DESIGN ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, 
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL SOLUTIONS DESIGN OR CONTRIBUTORS BE LIABLE FOR 
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
// NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR 
// BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
// USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
//
// The views and conclusions contained in the software and documentation are those of the authors 
// and should not be interpreted as representing official policies, either expressed or implied, 
// of Solutions Design. 
//
//////////////////////////////////////////////////////////////////////
// Contributers to the code:
//		- Frans  Bouma [FB]
//////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using SD.Tools.Algorithmia.UtilityClasses;
using System.Runtime.Serialization;

namespace SD.Tools.Algorithmia.GeneralDataStructures
{
    #region DESCRIPTION
    /// <summary>
    /// Extension to the normal Dictionary. This class can store more than one value for every key. It keeps a HashSet for every Key value.
    /// Calling Add with the same Key and multiple values will store each value under the same Key in the Dictionary. Obtaining the values
    /// for a Key will return the HashSet with the Values of the Key. It can also merge with other instances of MultiValueDictionary, as long
    /// as the TKey and TValue types are equal.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    #endregion

    [Serializable]
	public class MultiValueDictionary<TKey, TValue> : Dictionary<TKey, HashSet<TValue>>
	{
		IEqualityComparer<TValue> _valueComparer;

		public MultiValueDictionary() : this(null)
		{
			// Default
		}

		public MultiValueDictionary(IEqualityComparer<TValue> valueComparer) : base()
		{
			_valueComparer = valueComparer;
		}

		public MultiValueDictionary(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer) : base(keyComparer)
		{
			_valueComparer = valueComparer;
		}

		protected MultiValueDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			try
			{
				_valueComparer = info.GetValue("_valueComparer", typeof(IEqualityComparer<TValue>)) as IEqualityComparer<TValue>;
			}
			catch
			{
				// ignore. Versioning issue -> data doesn't contain the comparer. 
			}
		}

		public MultiValueDictionary<TKey, TValue> Clone() => MemberwiseClone() as MultiValueDictionary<TKey, TValue>;

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("_valueComparer", _valueComparer);
		}

		public void Add(TKey key, TValue value)
		{
			ArgumentVerifier.CantBeNull(key, "key");

			HashSet<TValue> container;

			if(!TryGetValue(key, out container))
			{
				container = new HashSet<TValue>(_valueComparer);
				Add(key, container);
			}

			container.Add(value);
		}

		public void AddRange(TKey key, IEnumerable<TValue> values)
		{
			if (values == null)
				return;

			foreach(TValue value in values)
			{
				Add(key, value);
			}
		}

		public bool ContainsValue(TKey key, TValue value)
		{
			ArgumentVerifier.CantBeNull(key, "key");

			HashSet<TValue> values;

			if(TryGetValue(key, out values))
			{
				return values.Contains(value);
			}

			return false;
		}

		public void Remove(TKey key, TValue value)
		{
			ArgumentVerifier.CantBeNull(key, "key");

			HashSet<TValue> container;

			if (!TryGetValue(key, out container))
				return;

			container.Remove(value);

			if (container.Count <= 0)
				Remove(key);
		}

		public void Merge(MultiValueDictionary<TKey, TValue> toMergeWith)
		{ 
			if(toMergeWith == null)
				return;

			foreach (KeyValuePair<TKey, HashSet<TValue>> pair in toMergeWith)
			{
				foreach(TValue value in pair.Value)
				{
					Add(pair.Key, value);
				}
			}
		}

		public HashSet<TValue> GetValues(TKey key, bool returnEmptySet)
		{
			HashSet<TValue> resultHashSet;

			if (TryGetValue(key, out resultHashSet))
				return resultHashSet;

			if (returnEmptySet)
				return new HashSet<TValue>(_valueComparer);

			return null;
		}
	}
}
