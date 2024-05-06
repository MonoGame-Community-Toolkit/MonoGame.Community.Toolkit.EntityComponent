// Copyright (c) Christopher Whitley. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Community.Toolkit.EntityComponent;

/// <summary>
/// Represents an entity that components can be added to.
/// </summary>
public abstract class Entity
{
    private bool _active;
    private bool _visible;
    private int _updateOrder;
    private int _drawOrder;

    /// <summary>
    /// Gets or Sets a value that indicates whether this entity is active.
    /// </summary>
    /// <remarks>
    /// Only active entities should be updated.
    /// </remarks>
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

    /// <summary>
    /// Gets or Sets a value that indicates whether this entity is visible.
    /// </summary>
    /// <remarks>
    /// Only visible entities should be rendered.
    /// </remarks>
    public bool Visible
    {
        get => _visible;
        set
        {
            if (_visible == value) { return; }
            _visible = value;
            VisibleChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets or Sets a value that indicates the order this entity is updated in comparison to other entities in the same
    /// entity collection.
    /// </summary>
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

    /// <summary>
    /// Gets or Sets a value that indicates the order this entity is rendered in comparison to other entities in the
    /// same entity colleciton.
    /// </summary>
    public int DrawOrder
    {
        get => _drawOrder;
        set
        {
            if (_drawOrder == value) { return; }
            _drawOrder = value;
            DrawOrderChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Event that is triggered when the Active property of this entity changes.
    /// </summary>
    public event EventHandler<EventArgs> ActiveChanged;

    /// <summary>
    /// Event that is triggered when the Visible property of this entity changes.
    /// </summary>
    public event EventHandler<EventArgs> VisibleChanged;

    /// <summary>
    /// Event that is triggered when the UpdateOrder property of this entity changes.
    /// </summary>
    public event EventHandler<EventArgs> UpdateOrderChanged;

    /// <summary>
    /// Event that is triggered when the DrawOrder property of this entity changes.
    /// </summary>
    public event EventHandler<EventArgs> DrawOrderChanged;

    /// <summary>
    /// Gets the component collection instance for this entity for adding or removing components.
    /// </summary>
    public ComponentCollection Components { get; }

    /// <summary>
    /// Initializes a new entity.
    /// </summary>
    /// <param name="active">Indicates whether the entity is active (Default = true).</param>
    /// <param name="updateOrder">
    /// Indicates the order in which this entity is updated compared to other entities in the same entity collection.
    /// (Default = 0)
    /// </param>
    /// <param name="visible">Indicates whether the entity is visible (Default = true).</param>
    /// <param name="drawOrder">
    /// Indicates the order in which this entity is rendered compared to other entities in the same entity collection
    /// (Default = 0).
    /// </param>
    protected Entity(bool active = true, int updateOrder = 0, bool visible = true, int drawOrder = 0)
    {
        Components = new ComponentCollection(this);
        _active = active;
        _visible = visible;
        _updateOrder = updateOrder;
        _drawOrder = drawOrder;
    }

    internal void BeforeUpdate(GameTime gameTime)
    {
        Components.Update(gameTime);
    }

    /// <summary>
    /// Updates this entity.
    /// </summary>
    /// <remarks>
    /// This is called after components attached to this entity have been updated.
    /// </remarks>
    /// <param name="gameTime">A snapshot of the timing values for the current update cycle.</param>
    public virtual void Update(GameTime gameTime) { }

    internal void BeforeDraw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        Components.Draw(spriteBatch, gameTime);
    }

    /// <summary>
    /// Renders this entity.
    /// </summary>
    /// <remarks>
    /// This is called after components attached to this entity has been rendered.
    /// </remarks>
    /// <param name="spriteBatch">the sprite bach used for rendering.</param>
    /// <param name="gameTime">A snapshot of the timing values for the current draw cycle.</param>
    public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime) { }

    /// <summary>
    /// Perform any logic necessary when a GraphicsDeviceReset event is triggered.
    /// </summary>
    /// <param name="graphicsDevice">The graphics device.</param>
    public virtual void HandleGraphicsReset(GraphicsDevice graphicsDevice) { }

    /// <summary>
    /// Perform any logic necessary when a GraphicsDeviceCreated event is triggered.
    /// </summary>
    /// <param name="graphicsDevice">The graphics device.</param>
    public virtual void HandleGraphicsCreated(GraphicsDebug graphicsDevice) { }
}
