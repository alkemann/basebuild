using UnityEngine;
using System.Collections.Generic;
using System;

namespace Path {

	abstract class GraphBase<T>
	{
		abstract public List<T> search (T from, T target);

		internal List<T> aStarSearch(
			Dictionary<T, Node<T>> nodes,
			T from,
			T target,
			Func<T, T, int> heuristic_cost_estimate
		)
		{
			Node<T> start = nodes [from];
			Node<T> goal = nodes [target];
			PriorityQueue<Node<T>> frontier = new PriorityQueue<Node<T>>();
			frontier.put (start, 0);

			Dictionary<Node<T>, Node<T>> came_from = new Dictionary<Node<T>, Node<T>> (); // Assume we must travel half nodes
			Dictionary<Node<T>, float> cost_so_far = new Dictionary<Node<T>, float>();    // nodes.Count / 2

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

			if (came_from [goal] == null) {
				Debug.Log ("No path found");
				return null;
			}

			List<T> path = new List<T> ();
			Node<T> iterator = goal;
			do {
				path.Add(iterator.data);
				iterator = came_from[iterator];
			} while (iterator != null);
			return path;
		}
	}

}
