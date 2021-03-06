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

	class Node<T>
	{
		public T data;
		public Edge<T>[] edges;

		public Node(T data)
		{
			this.data = data;
		}
	}

	abstract class Graphs<T>
	{
		abstract public Stack<T> search (T from, T target);

		internal Stack<T> aStarSearch(
			Dictionary<T, Node<T>> nodes,
			T from,
			T target,
			Func<T, T, int> heuristic_cost_estimate
		)
		{
			if (nodes.ContainsKey(from) == false) {
				Debug.LogError("From node missing from nodes available to path!");
				Debug.Log(from);
			}
			if (nodes.ContainsKey(target) == false) {
				Debug.LogError("Target node missing from nodes available to path!");
				Debug.Log(target);
			}
			Node<T> start = nodes [from];
			Node<T> goal = nodes [target];
			PriorityQueue<Node<T>> frontier = new PriorityQueue<Node<T>>();
			frontier.put (start, 0);

			Dictionary<Node<T>, Node<T>> came_from = new Dictionary<Node<T>, Node<T>> (nodes.Count / 2); // Assume we must travel half nodes
			Dictionary<Node<T>, float> cost_so_far = new Dictionary<Node<T>, float>(nodes.Count / 2);

			came_from [start] = null;
			cost_so_far [start] = 0;

			while (frontier.Count > 0) {
				Node<T> current = frontier.get ();

				if (current == goal)
					break; // found our goal

				foreach (Edge<T> edge in current.edges) {
					Node<T> next = edge.node;
					float new_cost = cost_so_far [current] + edge.cost;
					if (cost_so_far.ContainsKey (next) == false || new_cost < cost_so_far [next]) {
						cost_so_far [next] = new_cost;
						int priority = (int)new_cost + heuristic_cost_estimate(next.data, target);
						frontier.put (next, priority);
						came_from [next] = current;
					}
				}
			}

			if (came_from.ContainsKey(goal) == false || came_from [goal] == null) {
				return null;
			}

			Stack<T> path = new Stack<T> ();
			Node<T> iterator = goal;
			do {
				path.Push(iterator.data);
				iterator = came_from[iterator];
			} while (iterator != null);
			return path;
		}
	}

}
