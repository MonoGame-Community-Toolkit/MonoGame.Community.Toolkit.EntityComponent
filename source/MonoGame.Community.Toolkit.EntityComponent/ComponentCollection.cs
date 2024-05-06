// Copyright (c) Christopher Whitley. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Community.Toolkit.EntityComponent;

/// <summary>
/// Represents a collection for adding, removing, updating, and rendering components for an entity.
/// </summary>
public class ComponentCollection
{
    private readonly Entity _entity;

    private readonly List<IUpdateableComponent> _updateables;
    private readonly List<IDrawableComponent> _drawables;
    private readonly List<Component> _components;

    private readonly HashSet<Component> _current;
    private readonly HashSet<Component> _adding;
    private readonly HashSet<Component> _removing;

    private bool _sortUpdateable;
    private bool _sortDrawables;

    internal ComponentCollection(Entity entity)
    {
        Debug.Assert(entity is not null, $"{nameof(entity)} parameter is null");
        _entity = entity;
        _updateables = new List<IUpdateableComponent>();
        _drawables = new List<IDrawableComponent>();
        _components = new List<Component>();
        _current = new HashSet<Component>();
        _adding = new HashSet<Component>();
        _removing = new HashSet<Component>();
    }

    /// <summary>
    /// Updates this collection then calls update on each IUpdatableComponent that is active in this collection.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current update cycle.</param>
    public void Update(GameTime gameTime)
    {
        //  Add any components that are queued to be added
        if (_adding.Count > 0)
        {
            foreach (Component component in _adding)
            {
                AddComponent(component);
            }

            _adding.Clear();
        }

        //  Remove any components that are queued to be removed.
        if (_removing.Count > 0)
        {
            foreach (Component component in _removing)
            {
                RemoveComponent(component);
            }

            _removing.Clear();
        }

        //  Sort the updateable collection if it needs it
        if (_sortUpdateable)
        {
            _updateables.Sort();
            _sortUpdateable = false;
        }

        //  Sort the drawable collection if it needs it
        if (_sortDrawables)
        {
            _drawables.Sort();
            _sortDrawables = false;
        }

        //  Finally call update on all components that are IUpdateableComponents and Active in order
        for (int i = 0; i < _updateables.Count; ++i)
        {
            if (_updateables[i].Active)
                _updateables[i].Update(gameTime);
        }
    }

    private void AddComponent(Component component)
    {
        //  Only continue if the component can be added to the component hash
        if (!_current.Add(component))
            return;

        //  If its updateable, add to the updateable collection and mark for sort
        if (component is IUpdateableComponent updateable)
        {
            _updateables.Add(updateable);
            updateable.UpdateOrderChanged += MarkUpdateablesUnsorted;
            _sortUpdateable = true;
        }

        //  If it's drawable, add to the drawable collection and mark for sort
        if (component is IDrawableComponent drawable)
        {
            _drawables.Add(drawable);
            drawable.DrawOrderChanged += MarkDrawablesUnsorted;
            _sortDrawables = true;
        }
    }

    private void RemoveComponent(Component component)
    {
        //  Only continue if the component is in the component hash and can be removed
        if (!_current.Remove(component))
            return;

        //  If it's an updatable, remove it from the updatable collection
        if (component is IUpdateableComponent updateable)
        {
            _updateables.Remove(updateable);
            updateable.UpdateOrderChanged -= MarkUpdateablesUnsorted;
        }

        //  If it's drawable, remove it from the drawable collection.
        if (component is IDrawableComponent drawable)
        {
            _drawables.Remove(drawable);
            drawable.VisibleChanged -= MarkDrawablesUnsorted;
        }

    }

    private void MarkDrawablesUnsorted(object sender, EventArgs e)
    {
        _sortDrawables = true;
    }

    private void MarkUpdateablesUnsorted(object sender, EventArgs e)
    {
        _sortUpdateable = true;
    }

    /// <summary>
    /// Draws each IDrawableComponent in this collection that is visible.
    /// </summary>
    /// <param name="spriteBatch">The sprite batched used for rendering.</param>
    /// <param name="gameTime">A snapshot of the timing values for the current draw cycle.</param>
    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        for (int i = 0; i < _drawables.Count; ++i)
        {
            if (_drawables[i].Visible)
                _drawables[i].Draw(spriteBatch, gameTime);
        }
    }

    /// <summary>
    /// Adds the given component to this collection.
    /// </summary>
    /// <param name="component">The component to add.</param>
    public void Add(Component component)
    {
        Debug.Assert(component is not null);
        _adding.Add(component);
    }

    /// <summary>
    /// Adds the given components to this collection.
    /// </summary>
    /// <param name="components">The components to add.</param>
    public void Add(params Component[] components)
    {
        Debug.Assert(components is not null);
        for (int i = 0; i < components.Length; i++)
        {
            Add(components[i]);
        }
    }

    /// <summary>
    /// Adds a range of components to this collection.
    /// </summary>
    /// <param name="components">An enumerable collection of components to add.</param>
    public void AddRange(IEnumerable<Component> components)
    {
        Debug.Assert(components is not null);
        foreach (Component component in components)
        {
            Add(component);
        }
    }

    /// <summary>
    /// Removes the given component from this collection.
    /// </summary>
    /// <param name="component">The component to remove.</param>
    public void Remove(Component component)
    {
        Debug.Assert(component is not null);
        _removing.Add(component);
    }

    /// <summary>
    /// Removes the given components from this collection.
    /// </summary>
    /// <param name="components">The components to remove.</param>
    public void Remove(params Component[] components)
    {
        Debug.Assert(components is not null);

        for (int i = 0; i < components.Length; i++)
        {
            Remove(components[i]);
        }
    }

    /// <summary>
    /// Removes a range fo components from this collection.
    /// </summary>
    /// <param name="components">An enumerable collection of component to remove.</param>
    public void RemoveRange(IEnumerable<Component> components)
    {
        Debug.Assert(components is not null);

        foreach (Component component in components)
        {
            Remove(component);
        }
    }

    /// <summary>
    /// Returns the total number of components in this collection.
    /// </summary>
    /// <returns>The total number of components in this collection.</returns>
    public int Count() => _components.Count;

    /// <summary>
    /// Returns the total number of components in this collection that are of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of component to count.</typeparam>
    /// <returns>The total number of components in this collection that are of type <typeparamref name="T"/>.</returns>
    public int Count<T>() where T : Component
    {
        int count = 0;

        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i] is T)
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>
    /// Returns the first component of type <typeparamref name="T"/> in this collection, if one is contained within
    /// this collection, or the default of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of component to find.</typeparam>
    /// <returns>
    /// The first component of the type <typeparamref name="T"/> in this collection, if one is contained within this
    /// this collection, or the default of type <typeparamref name="T"/>.
    /// </returns>
    public T FirstOrDefault<T>() where T : Component
    {
        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i] is T component)
            {
                return component;
            }
        }

        return default(T);
    }

    /// <summary>
    /// Returns a list of all components in this collection that are of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of components to find.</typeparam>
    /// <returns>A list of all components in this collection that are of type <typeparamref name="T"/>.</returns>
    public IList<T> FindAll<T>() where T : Component
    {
        List<T> found = new List<T>();

        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i] is T component)
            {
                found.Add(component);
            }
        }

        return found;
    }

    /// <summary>
    /// Returns this collection as an array.
    /// </summary>
    /// <returns>This collection as a new array.</returns>
    public Component[] ToArray() => _components.ToArray();

    internal void HandleGraphicsDeviceCreated(GraphicsDevice graphicsDevice)
    {
        for (int i = 0; i < _drawables.Count; ++i)
            _drawables[i].HandleGraphicsDeviceCreated(graphicsDevice);
    }

    internal void HandleGraphicsDeviceReset(GraphicsDevice graphicsDevice)
    {
        for (int i = 0; i < _drawables.Count; ++i)
            _drawables[i].HandleGraphicsDeviceReset(graphicsDevice);
    }
}
