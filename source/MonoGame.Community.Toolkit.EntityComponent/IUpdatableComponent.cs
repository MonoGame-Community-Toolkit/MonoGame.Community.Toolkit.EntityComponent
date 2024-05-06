// Copyright (c) Christopher Whitley. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.Xna.Framework;

namespace MonoGame.Community.Toolkit.EntityComponent;

/// <summary>
/// Represents a component that can be updated.
/// </summary>
public interface IUpdateableComponent
{
    /// <summary>
    /// Gets or Sets a value that indicates whether this component is active.
    /// </summary>
    /// <remarks>
    /// Only active components should be updated.
    /// </remarks>
    bool Active { get; set; }

    /// <summary>
    /// Gets or Sets the order in which this component is updated compared to other components in the same component
    /// collection.
    /// </summary>
    /// <remarks>
    /// Components are updated in the order of lowest update order value to highest update order value.
    /// </remarks>
    int UpdateOrder { get; set; }

    /// <summary>
    /// Event that is triggered when the Active property is changed.
    /// </summary>
    event EventHandler<EventArgs> ActiveChanged;

    /// <summary>
    /// Event that is triggered when the UpdateOrder property is changed.
    /// </summary>
    event EventHandler<EventArgs> UpdateOrderChanged;

    /// <summary>
    /// Updates the component.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current update cycle.</param>
    void Update(GameTime gameTime);
}
