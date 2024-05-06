// Copyright (c) Christopher Whitley. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Community.Toolkit.EntityComponent;

/// <summary>
/// Represents a component that can be drawn.
/// </summary>
public interface IDrawableComponent
{
    /// <summary>
    /// Gets or Sets a value that indicates whether this component is visible.
    /// </summary>
    /// <remarks>
    /// Only visible components should be rendered.
    /// </remarks>
    bool Visible { get; set; }

    /// <summary>
    /// Gets or Sets the order in which this component is rendered compared to other components in the same component
    /// collection.
    /// </summary>
    /// <remarks>
    /// Components are rendered in the order of lowest draw order value to highest draw order value.
    /// </remarks>
    int DrawOrder { get; set; }

    /// <summary>
    /// Event that is triggered when the Visible property is changed.
    /// </summary>
    event EventHandler<EventArgs> VisibleChanged;

    /// <summary>
    /// Event that is triggered when the DrawOrder property is changed.
    /// </summary>
    event EventHandler<EventArgs> DrawOrderChanged;

    /// <summary>
    /// Renders this component.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch used to render.</param>
    /// <param name="gameTime">A snapshot of the timing values for the current draw cycle.</param>
    void Draw(SpriteBatch spriteBatch, GameTime gameTime);

    /// <summary>
    /// Perform any logic necessary when a GraphicsDeviceReset event is triggered.
    /// </summary>
    /// <param name="graphicsDevice">The graphics device.</param>
    void HandleGraphicsDeviceReset(GraphicsDevice graphicsDevice);

    /// <summary>
    /// Perform any logic necessary when a GraphicsDeviceCreated event is triggered.
    /// </summary>
    /// <param name="graphicsDevice">The graphics device.</param>
    void HandleGraphicsDeviceCreated(GraphicsDevice graphicsDevice);
}
