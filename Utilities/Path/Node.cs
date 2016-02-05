using UnityEngine;
using System.Collections.Generic;
using System;

namespace Path {

	class Node<T>
	{
		public T data;
		public List<Edge<T>> edges;

		public Node(T data)
		{
			this.data = data;
			edges = new List<Edge<T>> (8);
		}
	}

}
