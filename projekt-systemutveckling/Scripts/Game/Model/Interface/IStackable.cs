using System;
using System.Collections.Generic;
using System.Linq;

namespace Goodot15.Scripts.Game.Model.Interface;

public interface IStackable
{
	/// <summary>
	///     Traverses the entire stack (Both Forward and Backwards) from the current instance.
	///     Gets the current entire stack collection
	/// </summary>
	public IReadOnlyCollection<IStackable> Stack
	{
		get
		{
			ICollection<IStackable> stackBackwards = [];
			ICollection<IStackable> stackForwards = [];


			var current = this;
			IStackable next;

			// Traverse backwards
			while (current != null)
			{
				next = current.NeighbourBelow;
				stackBackwards.Add(next);
				current = next;
			}

			// Traverse forwards
			current = this;
			while (current != null)
			{
				next = current.NeighbourAbove;
				stackForwards.Add(next);
				current = next;
			}

			List<IStackable> stack = [];
			stack.AddRange(stackBackwards
				.Reverse()); // Stack backwards needs to be reversed so the order of the cards are correct
			stack.AddRange(stackForwards);

			return stack.AsReadOnly();
		}
	}

	public IStackable NeighbourAbove { get; set; }
	public IStackable NeighbourBelow { get; set; }

	/// <summary>
	///     Gets the types this <code>IStackable</code> is capable of stacking to
	/// </summary>
	/// <returns></returns>
	public IReadOnlyCollection<Type> GetStackableTypes();

	protected void SetNeighbourAbove(IStackable card);
	protected void SetNeighbourBelow(IStackable card);

	/// <summary>
	///     Determines if this <code>IStackable</code> may stack with the other object <code>card</code>
	/// </summary>
	/// <param name="card">Other card to check if can stack</param>
	/// <returns>True if capable of stacking, false otherwise</returns>
	public bool CanStackWith(Card card)
	{
		var stackableTypes = GetStackableTypes();
		if (stackableTypes.Any(t => !t.IsAssignableFrom(typeof(IStackable))))
			throw new InvalidOperationException("The Stackable Types collection must all implement IStackable");

		return stackableTypes.Any(t => t.IsAssignableFrom(card.GetType()));
	}
}