using System.Collections.Generic;
using UnityEngine;

public class Pathfinder 
{

	World world;
	List<Tile> traversable_graph;

	public Pathfinder(World world)
	{
		traversable_graph = new List<Tile> ();

		world.registerOnTileChanged (tileChanged); // listen to all tile changes
	}

	/**
	 * Be ready to add or remove tiles to possible traversable graph
	 */
	protected void tileChanged(Tile t)
	{
		if (t.type == Tile.TYPE.EMPTY || (t.Furniture != null && t.Furniture.type == Furniture.TYPE.WALL)) {
			// should NOT be in traversable graph
			if (traversable_graph.Contains (t))
				traversable_graph.Remove (t);
		} else {
			// should be in graph
			if (traversable_graph.Contains (t) == false)
				traversable_graph.Add(t);
		}
	}

	public List<Tile> findPath(Tile from, Tile to)
	{
		return astar_search (from, to);
	}



	protected List<Tile> astar_search (Tile start, Tile goal)
	{
		List<Tile> frontier = new List<Tile>(traversable_graph.Count);
		Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>(traversable_graph.Count / 2);
		Dictionary<Tile, float> cost_so_far = new Dictionary<Tile, float>(traversable_graph.Count / 2);

		frontier.Add(start);

		cameFrom[start] = start;
		cost_so_far[start] = 0;

		while (frontier.Count > 0) {
			Tile current = null;

			// This is instead of a priorty queue
			foreach (Tile possibleCurrent in frontier) {
				if (current == null ||  cost_so_far [possibleCurrent] < cost_so_far [current])
					current = possibleCurrent;
			}

			if (current == goal) {
				break; // we found the path
			}

			foreach (Tile next in current.getConnected()) {
				float newCost = cost_so_far[current] + next.costToEnterFrom(current.X, current.Y);
				if (!cost_so_far.ContainsKey(next) || newCost < cost_so_far[next]) {
					cost_so_far[next] = newCost;
					float priority = newCost + heuristic_cost_estimate(next, goal);
//					frontier.Add(next);
					cameFrom[next] = current;
				}
			}
		}
	}



	protected List<Tile> dijikstra_search (Tile source, Tile target)
	{
		List<Tile> frontier = new List<Tile>(traversable_graph.Count);
		Dictionary<Tile, float> cost_so_far = new Dictionary<Tile, float> (traversable_graph.Count);
		Dictionary<Tile, Tile> came_from = new Dictionary<Tile, Tile> (traversable_graph.Count);

		// Initialization
		foreach (Tile t in traversable_graph) {
			cost_so_far[t] = Mathf.Infinity; 	// Unknown distance from source to v
			came_from[t] = null; 				// Previous node in optimal path from source
			frontier.Add(t); 					// All nodes initially in unvisited nodes
		}

		cost_so_far[source] = 0;
		while (frontier.Count > 0) {
			Tile current = null;

			// This is instead of a priorty queue
			foreach (Tile possibleCurrent in frontier) {
				if (current == null ||  cost_so_far [possibleCurrent] < cost_so_far [current])
					current = possibleCurrent;
			}

			if (current == target) {
				break; // we found the path
			}

			foreach (Tile next in current.getConnected()) { // where v is still in unvisited.
				if (current.isPassable() == false && current != source ) continue;
				if (came_from.ContainsKey(next) == false) continue;
				float new_cost = cost_so_far[current] + next.costToEnterFrom(current.X, current.Y);
				if (new_cost < cost_so_far[next]) { // A shorter path to v has been found
					cost_so_far[next] = new_cost;
					came_from[next] = current;

					// add back next to frontier?
				}
			}

			frontier.Remove (current); // ie remove possibleCurrent from the frontier
		}

		if (came_from [target] == null) { // No path to target could be found
			Debug.Log ("No route found");
			return null;
		}
		List<Tile> path = new List<Tile> ();
		Tile curr = target;
		while (curr != null) {  // Construct the shortest path with a stack S
			path.Add (curr); 	// Push the vertex onto the stack
			curr = came_from [curr];	// Traverse from target to source
		}

		return path;
	}

	
	protected int heuristic_cost_estimate (Tile start, Tile goal)
	{
		return Mathf.Abs(start.X-goal.X) + Mathf.Abs(start.Y-goal.Y);
	}

}


