// Copyright (c) Christopher Whitley. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Community.Toolkit.EntityComponent;

/// <summary>
/// Represents a collection for adding, removing, updating, and rendering entities.
/// </summary>
public class EntityCollection
{
    private readonly List<Entity> _updateables;
    private readonly List<Entity> _drawables;
    private readonly List<Entity> _entities;

    private readonly HashSet<Entity> _current;
    private readonly HashSet<Entity> _adding;
    private readonly HashSet<Entity> _removing;

    private bool _sortUpdateable;
    private bool _sortDrawables;

    /// <summary>
    /// Initialize a new instance of the <see cref="EntityCollection"/> class.
    /// </summary>
    public EntityCollection()
    {
        _updateables = new List<Entity>();
        _drawables = new List<Entity>();
        _entities = new List<Entity>();
        _current = new HashSet<Entity>();
        _adding = new HashSet<Entity>();
        _removing = new HashSet<Entity>();
    }

    /// <summary>
    /// Updates the internal state of this <see cref="EntityCollection"/> as necessary if elements have been recently added or
    /// removed, then updates all <see cref="Entity"/> elements in this <see cref="EntityCollection"/>.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current game update cycle.</param>
    public void Update(GameTime gameTime)
    {
        //  Add any entities that are queued to be added
        if (_adding.Count > 0)
        {
            foreach (Entity entity in _adding)
            {
                AddEntity(entity);
            }

            _adding.Clear();
        }

        //  Remove any entities that are queued to be removed
        if (_removing.Count > 0)
        {
            foreach (Entity entity in _removing)
            {
                RemoveEntity(entity);
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

        //  Finally call update on all entities
        for (int i = 0; i < _updateables.Count; i++)
        {
            if (_updateables[i].Active)
            {
                _updateables[i].BeforeUpdate(gameTime);
                _updateables[i].Update(gameTime);
            }
        }
    }

    private void AddEntity(Entity entity)
    {
        //  Only continue if the entity can be added to the entity hash
        if (!_current.Add(entity))
            return;

        _updateables.Add(entity);
        entity.UpdateOrderChanged += MarkUpdateablesUnsorted;
        _sortUpdateable = true;

        _drawables.Add(entity);
        entity.DrawOrderChanged += MarkDrawablesUnsorted;
        _sortDrawables = true;
    }

    private void RemoveEntity(Entity entity)
    {
        //  Only continue if the entity is in the component hash
        if (!_current.Remove(entity))
            return;

        _updateables.Remove(entity);
        entity.UpdateOrderChanged -= MarkUpdateablesUnsorted;

        _drawables.Remove(entity);
        entity.DrawOrderChanged -= MarkDrawablesUnsorted;
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
    /// Draws each entity that is visible in this entity collection
    /// </summary>
    /// <param name="spriteBatch">The sprite batched used for rendering.</param>
    /// <param name="gameTime">A snapshot of the timing values for the current draw cycle.</param>
    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        for (int i = 0; i < _drawables.Count; i++)
        {
            if (_drawables[i].Visible)
                _drawables[i].Draw(spriteBatch, gameTime);
        }
    }

    /// <summary>
    /// Adds the given entity to this collection.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    public void Add(Entity entity)
    {
        Debug.Assert(entity is not null);
        _adding.Add(entity);
    }

    /// <summary>
    /// Adds the given entities to this collection.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    public void Add(params Entity[] entities)
    {
        Debug.Assert(entities is not null);
        for (int i = 0; i < entities.Length; i++)
        {
            Add(entities[i]);
        }
    }

    /// <summary>
    /// Adds a range of entities to this collection.
    /// </summary>
    /// <param name="entities">An enumerable collection of entities to add.</param>
    public void AddRange(IEnumerable<Entity> entities)
    {
        Debug.Assert(entities is not null);
        foreach (Entity entity in entities)
        {
            Add(entity);
        }
    }

    /// <summary>
    /// Removes the given entity to this collection.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    public void Remove(Entity entity)
    {
        Debug.Assert(entity is not null);
        _removing.Add(entity);
    }

    /// <summary>
    /// Removes the given entities to this collection.
    /// </summary>
    /// <param name="entities">The entities to remove.</param>
    public void Remove(params Entity[] entities)
    {
        Debug.Assert(entities is not null);

        for (int i = 0; i < entities.Length; i++)
        {
            Remove(entities[i]);
        }
    }

    /// <summary>
    /// Removes a range of entities to this collection.
    /// </summary>
    /// <param name="entities">An enumerable collection of entities to remove.</param>
    public void RemoveRange(IEnumerable<Entity> entities)
    {
        Debug.Assert(entities is not null);

        foreach (Entity entity in entities)
        {
            Remove(entity);
        }
    }

    /// <summary>
    /// Returns the total number of entities in this collection.
    /// </summary>
    /// <returns>The total number of entities in this collection.</returns>
    public int Count() => _entities.Count;

    /// <summary>
    /// Returns the total number of entities in this collection that are of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of entities to count.</typeparam>
    /// <returns>The total number of entities in this collection that are of type <typeparamref name="T"/>.</returns>
    public int Count<T>() where T : Entity
    {
        int count = 0;

        for (int i = 0; i < _entities.Count; i++)
        {
            if (_entities[i] is T entity)
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>
    /// Returns the first entity of type <typeparamref name="T"/> in this collection, if one is contained within
    /// this collection, or the default of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of entity to find.</typeparam>
    /// <returns>
    /// The first entity of the type <typeparamref name="T"/> in this collection, if one is contained within this
    /// this collection, or the default of type <typeparamref name="T"/>.
    /// </returns>
    public T FirstOrDefault<T>() where T : Entity
    {
        for (int i = 0; i < _entities.Count; i++)
        {
            if (_entities[i] is T entity)
            {
                return entity;
            }
        }

        return default;
    }

    /// <summary>
    /// Returns a list of all entities in this collection that are of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of entities to find.</typeparam>
    /// <returns>A list of all entities in this collection that are of type <typeparamref name="T"/>.</returns>
    public IList<T> FindAll<T>() where T : Entity
    {
        List<T> found = new List<T>();

        for (int i = 0; i < _entities.Count; i++)
        {
            if (_entities[i] is T entity)
            {
                found.Add(entity);
            }
        }

        return found;
    }

    /// <summary>
    /// Returns this collection as an array.
    /// </summary>
    /// <returns>This collection as a new array.</returns>
    public Entity[] ToArray() => _entities.ToArray();
}
