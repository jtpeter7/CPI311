using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPI311.GameEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Assignment4
{
    public class Agent : GameObject
    {
        public AStarSearch search;
        public List<Vector3> path;
        private float speed = 5f; //moving speed
        private int gridSize = 20; //grid size
        private Random random;
        private TerrainRenderer Terrain;
        public ContentManager Content { get; }
        public Model model;
        public Model model2;

        private int count = 0;
        private int maxCount = 50;
        private int currentIndex = 0;


        public Agent(TerrainRenderer terrain, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {
            Terrain = terrain;
            this.Content = Content;
            path = null;

            //... Potentially need to add more here

            // *** Adding GameObject Elements *** 
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;

            rigidbody.Mass = 1;
            rigidbody.Transform.LocalScale = new Vector3(3.0f, 3.0f, 3.0f);
            Add<Rigidbody>(rigidbody);

            model = Content.Load<Model>("Box");
            (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = Color.Black.ToVector3();

            Renderer renderer = new Renderer(model, rigidbody.Transform, camera, Content, graphicsDevice, light, 1, null, 20f, null);
            Add<Renderer>(renderer);

            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            search = new AStarSearch(gridSize, gridSize);
            float gridW = Terrain.size.X / gridSize;
            float gridH = Terrain.size.Y / gridSize;
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Vector3 pos = new Vector3(gridW * j + gridW / 2 - terrain.size.X / 2, 0.0f, gridH * i + gridH / 2 - terrain.size.Y / 2f);
                    if (Terrain.GetAltitude(pos) > 1.0)
                        search.Nodes[i, j].Passable = false;
                }
            }

            //Sets first path node
            RandomPathFinding();
            rigidbody.Transform.LocalPosition = GetGridPosition(path[0]);

        }

       

        public override void Update()
        {
            if (count == maxCount)
            {
                count = 0;
                currentIndex++;
            }
            if (currentIndex >= path.Count - 1)
            {
                RandomPathFinding();
                currentIndex = 0;
            }
            else
            {
                this.Transform.LocalPosition = Vector3.Lerp(GetGridPosition(path[currentIndex]), GetGridPosition(path[currentIndex + 1]), (float)count / (float)maxCount);
                count++;
            }
        }

        public Vector3 GetGridPosition(Vector3 gridPos)
        {
            float gridW = Terrain.size.X / search.Cols;
            float gridH = Terrain.size.Y / search.Rows;
            return new Vector3(gridW * gridPos.X + gridW / 2 - Terrain.size.X / 2, 1, gridH * gridPos.Z + gridH / 2 - Terrain.size.Y / 2);
        }

        public void reset()
        {
            RandomPathFinding();
            this.Transform.LocalPosition = GetGridPosition(path[0]);
        }

        public void RandomPathFinding()
        {
            Random random = new Random();
            while (!(search.Start = search.Nodes[random.Next(search.Cols), random.Next(search.Rows)]).Passable) ;
            search.End = search.Nodes[search.Cols / 2, search.Rows / 2];
            search.Search();
            path = new List<Vector3>();
            AStarNode current = search.End;
            var count = 0;
            while (current != null)
            {
                path.Insert(0, current.Position);
                current = current.Parent;
            }

            count = 0;
            currentIndex = 0;
        }
    }
}