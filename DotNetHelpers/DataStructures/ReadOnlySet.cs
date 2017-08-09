﻿using System.Collections;
using System.Collections.Generic;


namespace HamstarHelpers.DotNetHelpers.DataStructures {
	public class ReadOnlySet<T> : ISet<T> {
		private ISet<T> MySet;


		public ReadOnlySet( ISet<T> myset ) {
			this.MySet = myset;
			new HashSet<T>();
		}

		public int Count { get { return this.MySet.Count; } }

		public bool IsReadOnly { get { return true; } }

		void ICollection<T>.Add( T item ) { return; }

		public void Clear() { }

		public bool Contains( T item ) { return this.MySet.Contains( item ); }

		public void CopyTo( T[] array, int array_idx ) { this.MySet.CopyTo( array, array_idx ); }

		public bool Remove( T item ) { return false; }


		public bool Add( T item ) { return false; }

		public void ExceptWith( IEnumerable<T> other ) { }

		public void IntersectWith( IEnumerable<T> other ) { }

		public bool IsProperSubsetOf( IEnumerable<T> other ) { return this.MySet.IsProperSubsetOf( other ); }

		public bool IsProperSupersetOf( IEnumerable<T> other ) { return this.MySet.IsProperSupersetOf( other ); }

		public bool IsSubsetOf( IEnumerable<T> other ) { return this.MySet.IsSubsetOf( other ); }

		public bool IsSupersetOf( IEnumerable<T> other ) { return this.MySet.IsSupersetOf( other ); }

		public bool Overlaps( IEnumerable<T> other ) { return this.MySet.Overlaps( other ); }

		public bool SetEquals( IEnumerable<T> other ) { return this.MySet.SetEquals( other ); }

		public void SymmetricExceptWith( IEnumerable<T> other ) { }

		public void UnionWith( IEnumerable<T> other ) { }
		
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return this.MySet.GetEnumerator();
		}

		public IEnumerator GetEnumerator() {
			return this.MySet.GetEnumerator();
		}
	}
}
