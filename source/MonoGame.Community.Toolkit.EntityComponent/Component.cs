// Copyright (c) Christopher Whitley. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace MonoGame.Community.Toolkit.EntityComponent;

/// <summary>
/// Represents an updateable component.
/// </summary>
public abstract class Component : IUpdateableComponent
{
    private bool _active = true;
    private int _updateOrder;

    /// <summary>
    /// Gets the entity this component is attached to.
    /// </summary>
    public Entity Entity { get; private set; }

    /// <inheritdoc />
    public bool Active
    {
        get => _active;
        set
        {
            if (_active == value) { return; }
            _active = value;
            ActiveChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <inheritdoc />
    public int UpdateOrder
    {
        get => _updateOrder;
        set
        {
            if (_updateOrder == value) { return; }
            _updateOrder = value;
            UpdateOrderChanged?.Invoke(this, EventArgs.Empty);

        }
    }

    /// <inheritdoc />
    public event EventHandler<EventArgs> ActiveChanged;

    /// <inheritdoc />
    public event EventHandler<EventArgs> UpdateOrderChanged;

    /// <inheritdoc />
    public virtual void Update(GameTime gameTime) { }

    /// <summary>
    /// Self removes this component from the entity it is attached to.
    /// </summary>
    public void RemoveSelf()
    {
        Debug.Assert(Entity is not null, $"Attempted to remove this component when it's not attached to an entity");
        Entity.Components.Remove(this);
    }

    //  Called by the component collection when when this component is added to an entity.
    internal void Added(Entity entity)
    {
        Debug.Assert(Entity is null, $"Attempted to attach component to an second entity");
        Entity = entity;
    }

    //  Called by the component collection when this component is removed from an entity.
    internal void Removed()
    {
        Entity = null;
    }
}
