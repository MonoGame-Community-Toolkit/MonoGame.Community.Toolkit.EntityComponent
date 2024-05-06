// Copyright (c) Christopher Whitley. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Community.Toolkit.EntityComponent;

/// <summary>
/// Represents an drawable component.
/// </summary>
public abstract class DrawableComponent : Component, IDrawableComponent
{
    private bool _visible = true;
    private int _drawOrder;

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public event EventHandler<EventArgs> VisibleChanged;

    /// <inheritdoc />
    public event EventHandler<EventArgs> DrawOrderChanged;

    /// <inheritdoc />
    public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime) { }

    /// <inheritdoc />
    public virtual void HandleGraphicsDeviceCreated(GraphicsDevice graphicsDevice) { }

    /// <inheritdoc />
    public virtual void HandleGraphicsDeviceReset(GraphicsDevice graphicsDevice) { }
}
