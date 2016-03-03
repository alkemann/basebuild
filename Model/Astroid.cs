using UnityEngine;
using System.Collections.Generic;
using System;

public class Astroid
{
	public Tile tile { get; protected set; }

	public float resources_left { get; protected set; }
	Action<Astroid> cbOnChange;

	public Astroid (Tile tile, float resources = 100f)
	{
		resources_left = resources; // TODO random amount as starting value?
		this.tile = tile;
	}

	public float mine (float amount)
	{
		float mined = Mathf.Max(amount, resources_left);
		this.resources_left -= mined;
		return mined;
	}

	public void RegisterOnChange (Action<Astroid> cb)
	{
		cbOnChange += cb;
	}
}

