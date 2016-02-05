using UnityEngine;
using System.Collections.Generic;
using System;

namespace Path {

	class Edge<T>
	{
		public Node<T> node;
		public float cost;

		public Edge (Node<T> node, float cost)
		{
			this.node = node;
			this.cost = cost;
		}

	}

}
