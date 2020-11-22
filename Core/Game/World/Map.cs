﻿using System.Collections.Generic;
using System.Linq;
using QuantumCore.Core.Utils;
using QuantumCore.Game.World.Entities;
using Serilog;

namespace QuantumCore.Game.World
{
    public class Map
    {
        public const uint MapUnit = 25600;
        public string Name { get; private set; }
        public uint PositionX { get; private set; }
        public uint UnitX => PositionX / MapUnit;
        public uint PositionY { get; private set; }
        public uint UnitY => PositionY / MapUnit;
        public uint Width { get; private set; }
        public uint Height { get; private set; }

        private readonly List<Entity> _entities;
        private readonly QuadTree<Entity> _quadTree;
        
        public Map(string name, uint x, uint y, uint width, uint height)
        {
            Name = name;
            PositionX = x;
            PositionY = y;
            Width = width;
            Height = height;
            _entities = new List<Entity>();
            _quadTree = new QuadTree<Entity>((int) x, (int) y, (int) (width * MapUnit), (int) (height * MapUnit), 20);
        }

        public void Initialize()
        {
            Log.Debug($"Load map '{Name}' at {PositionX}x{PositionY} (size {Width}x{Height})");
        }

        public void Update(double elapsedTime)
        {
            foreach (var entity in _entities)
            {
                entity.Update(elapsedTime);
            }
        }
        
        public bool SpawnEntity(Entity entity)
        {
            if (!_quadTree.Insert(entity)) return false;
            
            // Add this entity to all entities nearby
            var nearby = new List<Entity>();
            _quadTree.QueryAround(nearby, entity.PositionX, entity.PositionY, Entity.ViewDistance);
            foreach (var e in nearby.Where(e => e != entity))
            {
                e.AddNearbyEntity(entity);
            }

            _entities.Add(entity);
            entity.Map = this;
            return true;

        }
    }
}