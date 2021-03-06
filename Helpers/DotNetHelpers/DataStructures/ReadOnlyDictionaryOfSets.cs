﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace HamstarHelpers.Helpers.DotNetHelpers.DataStructures {
	public class ReadOnlyDictionaryOfSets<K, V> : IReadOnlyDictionary<K, V> {
		private IDictionary<K, ISet<V>> Dict = new Dictionary<K, ISet<V>>();



		////////////////

		public ReadOnlyDictionaryOfSets( IDictionary<K, V> dict ) {
			foreach( var kv in dict ) {
				this.Dict[ kv.Key ] = new HashSet<V>() { kv.Value };
			}
		}

		public ReadOnlyDictionaryOfSets( IDictionary<K, ISet<V>> dict_copy ) {
			this.Dict = new Dictionary<K, ISet<V>>( dict_copy );
		}


		////////////////

		public V this[K key] {
			get {
				return this.Dict[key].First();
			}
		}

		public ISet<V> Get( K key ) {
			return this.Dict[key];
		}


		////////////////

		public int Count {
			get {
				return this.Dict.Count;
			}
		}

		public int CountAll {
			get {;
				return this.Dict.Select( kv => kv.Value.Count ).Sum();
			}
		}


		////////////////

		public IEnumerable<K> Keys {
			get {
				return this.Dict.Keys;
			}
		}

		IEnumerable<K> IReadOnlyDictionary<K, V>.Keys {
			get {
				return this.Dict.Keys;
			}
		}

		public IEnumerable<V> Values {
			get {
				return this.Dict.Values.Select( v => v.First() );
			}
		}

		IEnumerable<V> IReadOnlyDictionary<K, V>.Values {
			get {
				return this.Dict.Values.Select( v => v.First() );
			}
		}

		////////////////

		public bool ContainsKey( K key ) {
			return this.Dict.ContainsKey( key );
		}

		////////////////

		public bool TryGetValue( K key, out V value ) {
			ISet<V> value_set;
			bool tried = this.Dict.TryGetValue( key, out value_set );

			if( tried ) {
				value = value_set.First();
			} else {
				value = default( V );
			}

			return tried;
		}

		public bool TryGetValues( K key, out ISet<V> values ) {
			return this.Dict.TryGetValue( key, out values );
		}

		////////////////

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator() {
			return this.Dict.Select(
				kv => new KeyValuePair<K, V>( kv.Key, kv.Value.First() )
			).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return this.Dict.Select(
				kv => new KeyValuePair<K, V>( kv.Key, kv.Value.First() )
			).GetEnumerator();
		}
	}
}
